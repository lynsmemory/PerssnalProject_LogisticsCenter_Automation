using System;
using UnityEngine;

namespace Logistics
{
    // Swing-arm (flipper) diverter at one lane mouth of the main loop (FR-07).
    // The hinge is on the lane-side guide. Folded, the arm closes the lane opening;
    // extended, it acts as an oblique fence. In passive mode the belt always moves
    // straight and contact with that fence redirects the selected box into the lane.
    // flowSign and laneSideSign describe the hinge's local coordinate convention.
    public class LaneDiverter : MonoBehaviour
    {
        [Tooltip("Destination lane served by this diverter (was the string laneId L01/L02/L03).")]
        [SerializeField] private DestinationLane lane = DestinationLane.Lane1;
        [Tooltip("Kinematic arm body; its pivot is the hinge and the arm extends along its local +Z.")]
        [SerializeField] private Rigidbody arm;
        [Tooltip("Kinematic belt section under the arm sweep; its colliders are the belt there.")]
        [SerializeField] private Rigidbody section;
        [Tooltip("Keep the belt moving straight; the angled arm alone deflects the box.")]
        [SerializeField] private bool passiveDeflection = true;
        [Tooltip("Main flow along local Z: +1 or -1.")]
        [SerializeField] private float flowSign = 1f;
        [Tooltip("Lane side along local X: +1 or -1.")]
        [SerializeField] private float laneSideSign = 1f;
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
        [Tooltip("While not diverting, the section steers boxes this many degrees away from the open lane mouth (toward the far guide), " +
                 "so small boxes riding the lane-side edge do not slip into the lane undiverted. Tried at 10 deg on 10/29: worse, left at 0; the mouth guard handles this.")]
        [SerializeField, Range(0f, 30f)] private float passSteerAngle = 0f;
        [Tooltip("Kinematic guard wall across the lane mouth in the side guide. It stays up so passing boxes cannot slip into the lane " +
                 "by themselves (180 mm boxes did, 1-2 per 600 s on 10/29), and drops below the lane floor only while this diverter hands a box off.")]
        [SerializeField] private Rigidbody mouthGuard;
        [Tooltip("How far the guard drops when open (m, world down).")]
        [SerializeField, Min(0f)] private float guardDrop = 0.2f;
        [SerializeField, Min(0.05f)] private float guardMoveTime = 0.2f;
        [Tooltip("Time the guard stays down after the hand-off so the box's trailing edge is clear before it rises (s).")]
        [SerializeField, Min(0f)] private float guardCloseDelay = 0.6f;
        [SerializeField] private bool running = true;

        private float angle;
        private float elapsed;
        private float guardOpen;          // 0 up (closed), 1 down (open)
        private float sinceHandOff = float.MaxValue;
        private Vector3 guardClosedPosition;
        private Rigidbody current;
        private readonly Collider[] hits = new Collider[128];
        private readonly float[] bodies = new float[128];
        private bool overlapCapacityWarned;

        /// <summary>Set by the lane monitor; when false matching boxes pass and keep circulating.</summary>
        public bool AcceptsBoxes { get; set; } = true;
        public DestinationLane Lane => lane;
        /// <summary>Belt speed scale applied by MainConveyorController (production speed).</summary>
        public float SpeedMultiplier { get; set; } = 1f;
        public bool IsDiverting => current != null;
        public bool IsArmRetracted => angle <= 0.01f;
        public bool IsGuardClosed => mouthGuard != null ? guardOpen <= 0.01f : IsArmRetracted;
        public bool IsRunning => running;

        /// <summary>The arm started swinging out for this box.</summary>
        public event Action<Rigidbody> DivertStarted;
        /// <summary>The box left the belt into the lane; raised once per box.</summary>
        public event Action<Rigidbody> Diverted;
        /// <summary>The box passed the arm or the hand-off timed out; the arm retracts.</summary>
        public event Action<Rigidbody> DivertFailed;
        /// <summary>A box read for this lane went past the arm without being diverted (lane full or spacing); it circulates.</summary>
        public event Action<Rigidbody> BoxPassed;

        // Matching boxes seen in the trigger window, watched until they are diverted or go past the arm tip.
        private readonly System.Collections.Generic.HashSet<Rigidbody> watched = new System.Collections.Generic.HashSet<Rigidbody>();
        private readonly System.Collections.Generic.List<Rigidbody> passedNow = new System.Collections.Generic.List<Rigidbody>();

        private float FlowSign => flowSign < 0f ? -1f : 1f;
        private float LaneSideSign => laneSideSign < 0f ? -1f : 1f;
        private float Reach => Mathf.Max(0f, armLength * Mathf.Cos(extendedAngle * Mathf.Deg2Rad) * FlowSign);
        private float LaneDistance(Vector3 localPosition) => (localPosition.x - (beltCentreX + LaneSideSign * beltHalfWidth)) * LaneSideSign;

        private void Awake()
        {
            foreach (var body in new[] { arm, section, mouthGuard })
                if (body != null)
                {
                    body.isKinematic = true;
                    body.useGravity = false;
                }
            if (mouthGuard != null) guardClosedPosition = mouthGuard.position;
        }

        private void FixedUpdate()
        {
            if (!running || arm == null) return;
            float dt = Time.fixedDeltaTime;
            WatchPassing();
            if (current != null) Track(dt);
            else TryStart();

            float target = current != null ? extendedAngle : 0f;
            angle = Mathf.MoveTowards(angle, target, extendedAngle / swingTime * dt);
            arm.MovePosition(transform.position);
            arm.MoveRotation(transform.rotation * Quaternion.Euler(0f, angle, 0f));

            if (mouthGuard != null)
            {
                // Down from the moment a divert starts until guardCloseDelay after the box has left the belt.
                sinceHandOff = current != null ? 0f : sinceHandOff + dt;
                float guardTarget = current != null || sinceHandOff < guardCloseDelay ? 1f : 0f;
                // Never rise into a box sitting over the mouth: a rising kinematic wall launches it off the line (seen 10/29).
                if (guardTarget < guardOpen && GuardSpaceOccupied()) guardTarget = guardOpen;
                guardOpen = Mathf.MoveTowards(guardOpen, guardTarget, dt / guardMoveTime);
                mouthGuard.MovePosition(guardClosedPosition + Vector3.down * guardDrop * guardOpen);
            }

            if (section != null)
            {
                // Kinematic belt: step back, then MovePosition forward so contacts get the belt velocity
                // while the body itself stays in place.
                bool transfer = current != null && angle >= extendedAngle - 0.01f;
                Vector3 local = Vector3.forward * FlowSign;
                if (!passiveDeflection)
                    local = (transfer ? Quaternion.Euler(0f, extendedAngle, 0f) : Quaternion.Euler(0f, -passSteerAngle, 0f)) * local;
                Vector3 v = transform.TransformDirection(local) * speed * SpeedMultiplier;
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
            float zMin = triggerWindow.x - followerClearance, zMax = Mathf.Max(0f, armLength * FlowSign) + 0.3f;
            Vector3 centre = transform.TransformPoint(new Vector3(beltCentreX, 0.25f, (zMin + zMax) * 0.5f * FlowSign));
            int n = Physics.OverlapBoxNonAlloc(centre, new Vector3(beltHalfWidth, 0.25f, (zMax - zMin) * 0.5f), hits, transform.rotation, ~0, QueryTriggerInteraction.Ignore);
            // A truncated query cannot prove that the swept region is clear.
            if (n == hits.Length)
            {
                if (!overlapCapacityWarned)
                {
                    overlapCapacityWarned = true;
                    Debug.LogWarning($"[LaneDiverter {lane}] clearance query full; divert deferred", this);
                }
                return;
            }
            Rigidbody candidate = null;
            bool blocked = false;
            int others = 0;
            for (int i = 0; i < n; i++)
            {
                var body = hits[i].attachedRigidbody;
                if (body == null || body.isKinematic || body == candidate) continue;
                Vector3 p = transform.InverseTransformPoint(body.position);
                var box = body.GetComponent<LogisticsBox>();
                bool matches = box != null && box.HasReadResult && box.ReadLane == lane;
                float progress = p.z * FlowSign;
                if (matches && candidate == null && progress >= triggerWindow.x && progress <= triggerWindow.y) candidate = body;
                else if (others < bodies.Length) bodies[others++] = progress;
            }
            if (candidate == null) return;
            float candidateZ = transform.InverseTransformPoint(candidate.position).z * FlowSign;
            for (int i = 0; i < others; i++)
            {
                // Ahead of the candidate in the sweep, or close behind it: either would be pushed into the lane too.
                // Small boxes can both fit inside the trigger window. Do not leave an
                // unchecked interval between the candidate and the end of that window.
                if (bodies[i] > candidateZ) blocked = true;
                if (bodies[i] <= candidateZ && candidateZ - bodies[i] < followerClearance) blocked = true;
            }
            if (blocked) return;
            if (passiveDeflection && HasCloseUpstreamBody(candidate, candidateZ)) return;
            current = candidate;
            elapsed = 0f;
            DivertStarted?.Invoke(candidate);
        }

        // Near the north turn, following boxes are still outside the straight belt's
        // X range. Include that curved approach before reserving an arm cycle.
        private bool HasCloseUpstreamBody(Rigidbody candidate, float candidateProgress)
        {
            int n = Physics.OverlapSphereNonAlloc(candidate.position, followerClearance, hits, ~0, QueryTriggerInteraction.Ignore);
            if (n == hits.Length) return true;
            float distanceSquared = followerClearance * followerClearance;
            for (int i = 0; i < n; i++)
            {
                var body = hits[i].attachedRigidbody;
                if (body == null || body == candidate || body.isKinematic || body.GetComponent<LogisticsBox>() == null) continue;
                Vector3 p = transform.InverseTransformPoint(body.position);
                if (p.z * FlowSign >= candidateProgress || LaneDistance(p) > 0.15f) continue;
                if ((body.position - candidate.position).sqrMagnitude < distanceSquared) return true;
            }
            return false;
        }

        // True when a loose body is inside the space the raised guard occupies (plus 2 cm).
        private bool GuardSpaceOccupied()
        {
            var guardCollider = mouthGuard.GetComponent<BoxCollider>();
            if (guardCollider == null) return false;
            Transform t = mouthGuard.transform;
            Vector3 centre = guardClosedPosition + t.rotation * Vector3.Scale(guardCollider.center, t.lossyScale);
            Vector3 half = Vector3.Scale(guardCollider.size, t.lossyScale) * 0.5f + Vector3.one * 0.02f;
            int n = Physics.OverlapBoxNonAlloc(centre, half, hits, t.rotation, ~0, QueryTriggerInteraction.Ignore);
            if (n == hits.Length) return true;
            for (int i = 0; i < n; i++)
            {
                var body = hits[i].attachedRigidbody;
                if (body != null && !body.isKinematic) return true;
            }
            return false;
        }

        // Watches matching boxes from the trigger window on; one that ends up past the retracted arm tip while still on
        // the belt was not diverted and is reported once as passed (the lane monitor counts it as circulating).
        private void WatchPassing()
        {
            Vector3 centre = transform.TransformPoint(new Vector3(beltCentreX, 0.25f, (triggerWindow.x + triggerWindow.y) * 0.5f * FlowSign));
            int n = Physics.OverlapBoxNonAlloc(centre, new Vector3(beltHalfWidth, 0.25f, (triggerWindow.y - triggerWindow.x) * 0.5f), hits, transform.rotation, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < n; i++)
            {
                var body = hits[i].attachedRigidbody;
                if (body == null || body.isKinematic) continue;
                var box = body.GetComponent<LogisticsBox>();
                if (box != null && box.HasReadResult && box.ReadLane == lane) watched.Add(body);
            }

            passedNow.Clear();
            watched.RemoveWhere(b => b == null);
            foreach (var body in watched)
            {
                if (body == current) continue;
                Vector3 p = transform.InverseTransformPoint(body.position);
                if (LaneDistance(p) > 0f) passedNow.Add(body);
                else if (p.z * FlowSign > Mathf.Max(0f, armLength * FlowSign) + 0.3f) passedNow.Add(body);
            }
            foreach (var body in passedNow)
            {
                watched.Remove(body);
                Vector3 p = transform.InverseTransformPoint(body.position);
                if (LaneDistance(p) <= 0f) BoxPassed?.Invoke(body);
            }
        }

        private void Track(float dt)
        {
            elapsed += dt;
            Vector3 p = transform.InverseTransformPoint(current.position);
            if (LaneDistance(p) > handOffMargin)
            {
                var done = current;
                current = null;
                Diverted?.Invoke(done);
            }
            else if (p.z * FlowSign > Reach + 0.4f || elapsed > divertTimeout)
            {
                var failed = current;
                current = null;
                Debug.LogWarning($"[LaneDiverter {lane}] divert failed for {failed.name}", this);
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
            Gizmos.DrawWireCube(new Vector3(beltCentreX, 0.1f, (triggerWindow.x + triggerWindow.y) * 0.5f * FlowSign),
                new Vector3(beltHalfWidth * 2f, 0.2f, triggerWindow.y - triggerWindow.x));
        }
#endif
    }
}
