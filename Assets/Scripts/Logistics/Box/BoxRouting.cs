using UnityEngine;

namespace Logistics
{
    // Holds the destination lane decoded from the box's Code 128 image (FR-06).
    // Only the barcode reader writes it; a configured destination label must never be copied here.
    [DisallowMultipleComponent]
    public class BoxRouting : MonoBehaviour
    {
        [SerializeField] private string readLaneId;

        /// <summary>Decoded lane value (L01, L02, L03), or empty until read.</summary>
        public string ReadLaneId => readLaneId;
        public bool HasReadResult => !string.IsNullOrEmpty(readLaneId);

        public void SetReadResult(string laneId) => readLaneId = laneId;
        public void ClearReadResult() => readLaneId = null;
    }
}
