using System;
using UnityEngine;

namespace Logistics
{
    // Swing-arm (flipper) diverter at one lane mouth of the main loop (FR-07).
    // The component sits on the arm hinge: local +Z is the loop flow, local +X points across the belt toward the lane.
    // Retracted, the arm lies along the far-side guide and the belt section under the arm runs with the loop.
    // When a box whose read result matches laneId approaches and the lane accepts boxes, the arm swings across
    // the belt and, once out, the section drives along the arm toward the lane, so the box is carried off the belt
    // edge with the arm as the fence. (With the loop belt alone, boxes stuck against a frictionless arm instead of
    // sliding along it.) Both return once the box has left the belt. The main loop never stops; a box that is not
    // diverted passes and circulates.
    public class LaneDiverter : MonoBehaviour
    {
        [SerializeField] private string laneId = "L01";
        [Tooltip("Kinematic arm body; its pivot is the hinge and the arm extends along its local +Z.")]
        [SerializeField] private Rigidbody arm;
        [Tooltip("Kinematic belt section under the arm sweep; its colliders are the belt there.")]
        [SerializeField] private Rigidbody section;
        [SerializeField, Min(0f)] private float speed = 0.5f;
        [SerializeField, Min(0.1f)] private float armLength = 1.4f;
        [SerializeField, Range(5f, 80f)] private float extendedAngle = 35f;
        [SerializeField, Min(0.05f)] private float swingTime = 0.3f;
        [Tooltip("Belt centre line and half width in this component's local X (m).")]
        [SerializeField] private float beltCentreX = 0.4f;
        [SerializeField, Min(0.1f)] private float beltHalfWidth = 0.437f;
        [Tooltip("Local Z range upstream of the hinge in which a matching box fires the arm (m).")]
        [SerializeField] private Vector2 triggerWindow = new Vector2(-1.0f, -0.25f);
        [Tooltip("A box closer than this behind the candidate would ride the section and be pushed or struck by the arm, so the candidate is let past to circulate (m).")]
        [SerializeField, Min(0f)] private float followerClearance = 1.5f;
        [Tooltip("Box centre beyond the belt edge at which the hand-off counts as done (m).")]
        [SerializeField, Min(0f)] private float handOffMargin = 0.1f;
        [SerializeField, Min(0.1f)] private float divertTimeout = 4f;
        [SerializeField] private bool running = true;

        private float angle;
        private float elapsed;
        private Rigidbody current;
        private readonly Collider[] hits = new Collider[16];
        private readonly float[] bodies = new float[16];

        /// <summary>Set by the lane monitor; when false matching boxes pass and keep circulating.</summary>
        public bool AcceptsBoxes { get; set; } = true;
        public string LaneId => laneId;
        public bool IsDiverting => current != null;
        public bool IsArmRetracted => angle <= 0.01f;
        public bool IsRunning => running;

        /// <summary>The arm started swinging out for this box.</summary>
        public event Action<Rigidbody> DivertStarted;
        /// <summary>The box left the belt into the lane; raised once per box.</summary>
        public event Action<Rigidbody> Diverted;
        /// <summary>The box passed the arm or the hand-off timed out; the arm retracts.</summary>
        public event Action<Rigidbody> DivertFailed;

        private float Reach => armLength * Mathf.Cos(extendedAngle * Mathf.Deg2Rad);

        private void Awake()
        {
            foreach (var body in new[] { arm, section })
                if (body != null)
                {
                    body.isKinematic = true;
                    body.useGravity = false;
                }
        }

        private void FixedUpdate()
        {
            if (!running || arm == null) return;
            float dt = Time.fixedDeltaTime;
            if (current != null) Track(dt);
            else TryStart();

            float target = current != null ? extendedAngle : 0f;
            angle = Mathf.MoveTowards(angle, target, extendedAngle / swingTime * dt);
            arm.MovePosition(transform.position);
            arm.MoveRotation(transform.rotation * Quaternion.Euler(0f, angle, 0f));

            if (section != null)
            {
                // Kinematic belt: step back, then MovePosition forward so contacts get the belt velocity
                // while the body itself stays in place.
                bool transfer = current != null && angle >= extendedAngle - 0.01f;
                Vector3 local = transfer ? Quaternion.Euler(0f, extendedAngle, 0f) * Vector3.forward : Vector3.forward;
                Vector3 v = transform.TransformDirection(local) * speed;
                Vector3 p = section.position;
                section.position = p - v * dt;
                section.MovePosition(p);
            }
        }

        private void TryStart()
        {
            if (!AcceptsBoxes) return;
            // Region over the belt from the follower clearance behind the trigger window to past the arm tip.
            // The retracted arm lies along the belt for its full length, so its tip sweeps that far downstream.
            float zMin = triggerWindow.x - followerClearance, zMax = armLength + 0.3f;
            Vector3 centre = transform.TransformPoint(new Vector3(beltCentreX, 0.25f, (zMin + zMax) * 0.5f));
            int n = Physics.OverlapBoxNonAlloc(centre, new Vector3(beltHalfWidth, 0.25f, (zMax - zMin) * 0.5f), hits, transform.rotation, ~0, QueryTriggerInteraction.Ignore);
            Rigidbody candidate = null;
            bool blocked = false;
            int others = 0;
            for (int i = 0; i < n; i++)
            {
                var body = hits[i].attachedRigidbody;
                if (body == null || body.isKinematic || body == candidate) continue;
                Vector3 p = transform.InverseTransformPoint(body.position);
                var routing = body.GetComponent<BoxRouting>();
                bool matches = routing != null && routing.ReadLaneId == laneId;
                if (matches && candidate == null && p.z >= triggerWindow.x && p.z <= triggerWindow.y) candidate = body;
                else if (others < bodies.Length) bodies[others++] = p.z;
            }
            if (candidate == null) return;
            float candidateZ = transform.InverseTransformPoint(candidate.position).z;
            for (int i = 0; i < others; i++)
            {
                // Ahead of the candidate in the sweep, or close behind it: either would be pushed into the lane too.
                if (bodies[i] > candidateZ && bodies[i] > triggerWindow.y - 0.3f) blocked = true;
                if (bodies[i] <= candidateZ && candidateZ - bodies[i] < followerClearance) blocked = true;
            }
            if (blocked) return;
            current = candidate;
            elapsed = 0f;
            DivertStarted?.Invoke(candidate);
        }

        private void Track(float dt)
        {
            elapsed += dt;
            Vector3 p = transform.InverseTransformPoint(current.position);
            if (p.x > beltCentreX + beltHalfWidth + handOffMargin)
            {
                var done = current;
                current = null;
                Diverted?.Invoke(done);
            }
            else if (p.z > Reach + 0.4f || elapsed > divertTimeout)
            {
                var failed = current;
                current = null;
                Debug.LogWarning($"[LaneDiverter {laneId}] divert failed for {failed.name}", this);
                DivertFailed?.Invoke(failed);
            }
        }

        public void Play() => running = true;
        public void Stop() => running = false;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1f, 0.6f, 0.1f, 0.8f);
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0f, extendedAngle, 0f) * Vector3.forward * armLength);
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.6f);
            Gizmos.DrawWireCube(new Vector3(beltCentreX, 0.1f, (triggerWindow.x + triggerWindow.y) * 0.5f),
                new Vector3(beltHalfWidth * 2f, 0.2f, triggerWindow.y - triggerWindow.x));
        }
#endif
    }
}
