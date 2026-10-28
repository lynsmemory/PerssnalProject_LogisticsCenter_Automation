using UnityEngine;

namespace Logistics
{
    // Speed-controller rollers on a gravity lane section (FR-08). The lane stays unpowered: boxes start and run
    // by gravity, and this only removes downhill speed above maxSpeed, like brake rollers on a steep chute.
    // Sits on the section's surface BoxCollider; local +Z is downhill, local +Y the surface normal.
    [RequireComponent(typeof(BoxCollider))]
    public class BrakeRollerZone : MonoBehaviour
    {
        [SerializeField, Min(0.05f)] private float maxSpeed = 0.6f;

        private BoxCollider surface;
        private readonly Collider[] hits = new Collider[16];

        public float MaxSpeed => maxSpeed;

        private void Awake() => surface = GetComponent<BoxCollider>();

        private void FixedUpdate()
        {
            Vector3 size = surface.size;
            Vector3 centre = transform.TransformPoint(surface.center + new Vector3(0f, size.y * 0.5f + 0.25f, 0f));
            int n = Physics.OverlapBoxNonAlloc(centre, new Vector3(size.x * 0.5f, 0.25f, size.z * 0.5f), hits, transform.rotation, ~0, QueryTriggerInteraction.Ignore);
            Vector3 downhill = transform.forward;
            for (int i = 0; i < n; i++)
            {
                var body = hits[i].attachedRigidbody;
                if (body == null || body.isKinematic) continue;
                float along = Vector3.Dot(body.linearVelocity, downhill);
                if (along > maxSpeed) body.linearVelocity -= downhill * (along - maxSpeed);
            }
        }
    }
}
