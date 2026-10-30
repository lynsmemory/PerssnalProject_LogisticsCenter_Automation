using System;
using UnityEngine;

namespace Logistics
{
    // 파레트 1개의 사전 지정 적재 위치(층당 6자리)와 적재 수를 관리한다. (FR-10, FR-11, FR-12)
    // 자리 순서는 layerSlots 배열 순서이며, 한 층 6자리를 모두 채워야 다음 층 자리를 준다. 6×L개를 놓으면 완료다.
    // 적재 수는 PalletizingSequence가 놓은 위치를 확인한 뒤 ConfirmPlaced를 부를 때만 늘어난다(실패한 자리는 세지 않는다).
    // 레인마다 파레트 적재 기준점(RobotCells/LaneN_RobotCell_*/PalletLoadPoint)에 1개 붙인다.
    // 빈 파레트 재생성(RespawnEmptyPallet, FR-20)은 AMR 반출 구현(WBS 57·72) 때 함께 작성한다.
    [DisallowMultipleComponent]
    public class PalletLoadState : MonoBehaviour
    {
        #region 1. 연결과 설정
        [Tooltip("한 층 6자리 지정 위치 (1단, 파레트 윗면). 위치 = 박스 바닥 중심, 회전 = 박스 방향. 배열 순서가 적재 순서")]
        [SerializeField] private Transform[] layerSlots = new Transform[SimulationSettings.BoxesPerLayer];

        [Tooltip("층 높이 (m). 박스 높이 0.17")]
        [SerializeField, Min(0.01f)] private float layerHeight = 0.17f;

        [Tooltip("적재 층 수 L. 운전 시작 때 설정값(SimulationSettings)으로 바뀐다")]
        [SerializeField, Min(1)] private int layerCount = 3;
        #endregion

        #region 2. 상태
        public int PlacedCount { get; private set; }                                  // 적재 완료 수
        public int CurrentLayer => PlacedCount / SimulationSettings.BoxesPerLayer;      // 지금 채우는 층 (0부터)
        public int LayerCount => layerCount;
        public int Capacity => SimulationSettings.BoxesPerLayer * layerCount;          // 6 × L
        public bool IsComplete => PlacedCount >= Capacity;
        public float LayerHeight => layerHeight;

        // L층을 모두 쌓았을 때 맨 위 박스 윗면 높이 (월드 Y). 운반 높이 계산용
        public float StackTopHeight => transform.position.y + layerHeight * layerCount;

        public event Action BoxPlaced;   // 박스 1개 적재 완료 (기록용)
        public event Action Completed;   // 6×L 완료 (1회)
        #endregion

        #region 3. 적재
        // 적재 층 수 L 적용. 적재 중에는 바꾸지 않는다
        public void Configure(int layers)
        {
            if (PlacedCount > 0)
            {
                Debug.LogWarning($"[PalletLoadState] {name}: 적재 중({PlacedCount}개)에는 층 수를 바꾸지 않음", this);
                return;
            }
            layerCount = Mathf.Max(1, layers);
        }

        // 다음 적재 자리 (박스 바닥 중심 위치와 박스 방향). 완료됐거나 자리 지정이 없으면 false
        public bool GetNextSlotPose(out Pose pose)
        {
            pose = default;
            if (IsComplete)
            {
                return false;
            }
            Transform slot = layerSlots[PlacedCount % SimulationSettings.BoxesPerLayer];
            if (slot == null)
            {
                return false;
            }
            pose = new Pose(slot.position + Vector3.up * (layerHeight * CurrentLayer), slot.rotation);
            return true;
        }

        // 놓은 위치 확인이 끝난 박스 1개를 센다. 6×L이 되면 Completed를 한 번 알린다
        public void ConfirmPlaced()
        {
            if (IsComplete)
            {
                return;
            }
            PlacedCount++;
            BoxPlaced?.Invoke();
            if (IsComplete)
            {
                Completed?.Invoke();
            }
        }

        // 초기화 버튼: 적재 수 0, 층 0 (쌓인 박스 제거는 BoxSpawner가 한다)
        public void ResetToInitialState()
        {
            PlacedCount = 0;
        }
        #endregion
    }
}
