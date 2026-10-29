using System;
using System.Collections;
using UnityEngine;

namespace Logistics
{
    // 레인 하나와 전담 설비를 묶고, 중간 스토퍼 방출 → 픽업 정렬 → (로봇 적재) 순서를 진행한다. 레인당 1개. (FR-13, FR-19, FR-20, FR-22)
    // 지금은 레인 설비(집계·스토퍼·픽업)까지 연결한다. FR5 적재(PalletizingSequence)·파레트·AMR·도크는 해당 클래스 작성 후 연결한다.
    // 로봇 연결 전 시험용으로 simulatedPickSeconds > 0이면 정렬된 박스를 그 시간 뒤 공정에서 빼서(로봇이 집어 간 것으로) 다음 박스를 받는다.
    public class LaneProcessController : MonoBehaviour
    {
        #region 1. 연결과 설정
        [Tooltip("레인 번호")]
        [SerializeField] private DestinationLane lane = DestinationLane.Lane1;

        [Tooltip("레인 대기 수")]
        [SerializeField] private LaneOccupancyMonitor occupancyMonitor;

        [Tooltip("중간 스토퍼")]
        [SerializeField] private MiddleStopper middleStopper;

        [Tooltip("픽업 구역")]
        [SerializeField] private PickupZoneAligner pickupZone;

        [Tooltip("박스 정리 (로봇 대체 시험에서 집어 간 박스 제거)")]
        [SerializeField] private BoxSpawner boxSpawner;

        [Tooltip("로봇 연결 전 시험용: 정렬 후 이 시간(초)이 지나면 박스를 집어 간 것으로 처리. 0이면 사용 안 함")]
        [SerializeField, Min(0f)] private float simulatedPickSeconds = 0f;

        [Tooltip("방출 조건 확인 주기 (초)")]
        [SerializeField, Min(0.05f)] private float pollInterval = 0.1f;
        #endregion

        #region 2. 상태
        private Coroutine loop;
        private Rigidbody inTransit;     // 중간 스토퍼를 지나 픽업으로 내려가는 박스
        private float releasedAt;
        private bool transitWarned;

        [Tooltip("방출한 박스가 이 시간(초) 안에 픽업에 도착하지 않으면 경고 (다음 방출은 계속 막음)")]
        [SerializeField, Min(1f)] private float transitTimeout = 30f;

        // 방출 후 픽업 도착 대기 중
        public bool HasBoxInTransit => inTransit != null;

        public DestinationLane Lane => lane;
        public LaneOccupancyMonitor OccupancyMonitor => occupancyMonitor;
        public MiddleStopper MiddleStopper => middleStopper;
        public PickupZoneAligner PickupZone => pickupZone;
        public bool IsRunning => loop != null;
        // 이 레인에서 처리(픽업 완료)한 박스 수
        public int ProcessedCount { get; private set; }

        // 박스 1개 처리 완료 (기록용)
        public event Action<DestinationLane> BoxProcessed;
        #endregion

        #region 3. 운전
        private void OnEnable()
        {
            if (middleStopper != null) middleStopper.Released += OnStopperReleased;
            if (pickupZone != null) pickupZone.BoxAligned += OnBoxAligned;
        }

        private void OnDisable()
        {
            if (middleStopper != null) middleStopper.Released -= OnStopperReleased;
            if (pickupZone != null) pickupZone.BoxAligned -= OnBoxAligned;
        }

        private void OnStopperReleased(Rigidbody body)
        {
            inTransit = body;
            releasedAt = Time.time;
            transitWarned = false;
        }

        private void OnBoxAligned(LogisticsBox box)
        {
            inTransit = null;
        }

        // 설정 적용 후 레인 운전 시작
        public void Begin(SimulationSettings settings)
        {
            if (loop == null)
            {
                loop = StartCoroutine(RunLaneLoop());
            }
        }

        // 일시정지·재개는 전체 시간 정지로 처리하므로 별도 동작 없음 (LogisticsSimulationController 참조)
        public void Pause() { }
        public void Resume() { }

        // 방출 → 적재 반복 코루틴. 픽업 구역이 비고 FR5가 취급 중이 아닐 때만 방출
        private IEnumerator RunLaneLoop()
        {
            var wait = new WaitForSeconds(pollInterval);
            while (true)
            {
                //1. 픽업 구역이 비고 앞서 방출한 박스가 픽업에 도착했으면 중간 스토퍼에서 1개 방출
                //   (방출한 박스가 경사 구간을 내려가는 동안 다음 박스를 내보내지 않는다: 중간 1·픽업 1 제한)
                if (inTransit != null && Time.time - releasedAt > transitTimeout && !transitWarned)
                {
                    transitWarned = true;
                    Debug.LogWarning($"[LaneProcessController {lane}] 방출한 {inTransit.name}가 {transitTimeout:F0}초 안에 픽업에 도착하지 않음", this);
                }
                if (inTransit == null && pickupZone != null && pickupZone.IsEmpty && middleStopper != null && !middleStopper.IsBusy && middleStopper.HasWaitingBox)
                {
                    middleStopper.ReleaseOne();
                }

                //2. 정렬된 박스: 로봇 연결 전 시험이면 일정 시간 뒤 집어 간 것으로 처리
                if (pickupZone != null && pickupZone.CurrentBox != null && simulatedPickSeconds > 0f)
                {
                    LogisticsBox box = pickupZone.CurrentBox;
                    // 대기 중에 처리 시간이 바뀌어도 반영되도록 매 프레임 확인한다
                    float waited = 0f;
                    while (waited < simulatedPickSeconds && pickupZone.CurrentBox == box)
                    {
                        waited += Time.deltaTime;
                        yield return null;
                    }
                    if (pickupZone.CurrentBox == box && box != null)
                    {
                        pickupZone.ClearZone();
                        if (boxSpawner != null) boxSpawner.Despawn(box);
                        else Destroy(box.gameObject);
                        OnBoxProcessed();
                    }
                }
                yield return wait;
            }
        }

        private void OnBoxProcessed()
        {
            ProcessedCount++;
            BoxProcessed?.Invoke(lane);
        }

        // 초기화 버튼: 레인 순서 진행 중지, 레인 설비 초기화 호출
        public void ResetToInitialState()
        {
            if (loop != null)
            {
                StopCoroutine(loop);
                loop = null;
            }
            if (occupancyMonitor != null) occupancyMonitor.ResetToInitialState();
            if (middleStopper != null) middleStopper.ResetToInitialState();
            if (pickupZone != null) pickupZone.ResetToInitialState();
            inTransit = null;
            ProcessedCount = 0;
        }
        #endregion
    }
}
