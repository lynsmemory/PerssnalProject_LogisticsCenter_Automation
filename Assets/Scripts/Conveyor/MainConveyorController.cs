using System.Collections.Generic;
using RoboLacyTech.Conveyors;
using UnityEngine;

namespace Logistics
{
    // 순환 컨베이어를 운전 중 상시 구동한다. 레인 정체로는 멈추지 않는다. (FR-05, FR-21)
    // loopRoot(MainLoop)와 extraRoots(EntryLine 등) 아래의 LinearConveyor·CurvedConveyor와 분기 장치를 모아
    // 시작·정지·속도 배율을 한꺼번에 적용한다. 각 컨베이어의 원래 속도·방향은 처음 수집할 때 저장한다.
    public class MainConveyorController : MonoBehaviour
    {
        #region 1. 연결과 설정
        [Tooltip("MainLoop 루트 (컨베이어 수집)")]
        [SerializeField] private Transform loopRoot;

        [Tooltip("같이 구동할 다른 컨베이어 루트 (투입 라인 EntryLine)")]
        [SerializeField] private Transform[] extraRoots = new Transform[0];

        [Tooltip("기준 속도 배율 1에서의 벨트 속도 (m/s). 0이면 각 컨베이어에 저장된 속도를 그대로 쓴다")]
        [SerializeField, Min(0f)] private float baseSpeed = 0f;
        #endregion

        #region 2. 상태
        private struct Drive
        {
            public LinearConveyor linear;
            public CurvedConveyor curved;
            public float signedSpeed;
        }

        private readonly List<Drive> drives = new List<Drive>();
        private LaneDiverter[] diverters = new LaneDiverter[0];

        public bool IsRunning { get; private set; }
        public float SpeedMultiplier { get; private set; } = 1f;
        public int ConveyorCount => drives.Count;
        #endregion

        #region 3. 수집
        private void Awake()
        {
            Collect();
        }

        // 운전 시작 전에는 멈춰 둔다. 패키지 기본값은 시작하자마자 구동하고, 그 초기화가 각 컨베이어 Awake에서 일어나므로 Start에서 끈다.
        // 이미 운전 명령을 받았으면(같은 프레임 Start 순서) 건드리지 않는다.
        private void Start()
        {
            if (!IsRunning)
            {
                Stop();
            }
        }

        private void Collect()
        {
            drives.Clear();
            var roots = new List<Transform> { loopRoot };
            roots.AddRange(extraRoots);
            var diverterList = new List<LaneDiverter>();
            foreach (Transform root in roots)
            {
                if (root == null)
                {
                    continue;
                }
                foreach (LinearConveyor c in root.GetComponentsInChildren<LinearConveyor>(true))
                {
                    drives.Add(new Drive { linear = c, signedSpeed = Signed(c.Motor) });
                }
                foreach (CurvedConveyor c in root.GetComponentsInChildren<CurvedConveyor>(true))
                {
                    drives.Add(new Drive { curved = c, signedSpeed = Signed(c.Motor) });
                }
            }

            // 분기 장치는 레인 그룹(LaneN_Gate) 아래에 있으므로 루트의 부모(ConveyorSystem) 기준으로 찾는다
            Transform system = loopRoot != null ? loopRoot.parent : null;
            if (system != null)
            {
                diverterList.AddRange(system.GetComponentsInChildren<LaneDiverter>(true));
            }
            diverters = diverterList.ToArray();
        }

        private float Signed(ConveyorMotor motor)
        {
            float speed = baseSpeed > 0f ? baseSpeed : motor.BaseSpeed;
            return motor.Direction == ConveyorDirection.Backward ? -speed : speed;
        }
        #endregion

        #region 4. 운전
        // 구동 (일시정지 후 재개 포함)
        public void Run()
        {
            ApplySpeed();
            foreach (Drive d in drives)
            {
                if (d.linear != null) d.linear.Play();
                if (d.curved != null) d.curved.Play();
            }
            foreach (LaneDiverter diverter in diverters)
            {
                diverter.Play();
            }
            IsRunning = true;
        }

        // 정지 (일시정지·초기화 전용)
        public void Stop()
        {
            foreach (Drive d in drives)
            {
                if (d.linear != null) d.linear.Stop();
                if (d.curved != null) d.curved.Stop();
            }
            foreach (LaneDiverter diverter in diverters)
            {
                diverter.Stop();
            }
            IsRunning = false;
        }

        // 속도 배율 적용
        public void SetSpeedMultiplier(float multiplier)
        {
            SpeedMultiplier = Mathf.Max(0.01f, multiplier);
            ApplySpeed();
        }

        private void ApplySpeed()
        {
            foreach (Drive d in drives)
            {
                float speed = d.signedSpeed * SpeedMultiplier;
                if (d.linear != null) d.linear.SetSpeed(speed);
                if (d.curved != null) d.curved.SetSpeed(speed);
            }
            foreach (LaneDiverter diverter in diverters)
            {
                diverter.SpeedMultiplier = SpeedMultiplier;
            }
        }

        // 초기화 버튼: 정지, 속도 배율 1
        public void ResetToInitialState()
        {
            Stop();
            SetSpeedMultiplier(1f);
        }
        #endregion
    }
}
