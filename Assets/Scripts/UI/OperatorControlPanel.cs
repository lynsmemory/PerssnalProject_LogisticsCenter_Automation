using System.Text;
using UnityEngine;

namespace Logistics
{
    // 운전 조작·설정 선택·뷰 선택을 받아 해당 클래스 함수를 호출하고, 설정 L·N과 레인 상태·정체 알람을 표시한다. (FR-02, FR-18, FR-19, FR-21, FR-23)
    // OnStart 등 공개 함수는 uGUI Button OnClick에 연결한다. UI 에셋을 연결하기 전에는 drawFallbackGui로 화면 왼쪽 위에 기본 패널을 그린다.
    // 버튼은 운전 상태에 맞을 때만 누를 수 있다 (정지: 시작·설정 선택 / 운전: 일시정지 / 일시정지: 재개 / 초기화는 항상).
    public class OperatorControlPanel : MonoBehaviour
    {
        #region 1. 연결과 설정
        [Tooltip("운전 명령 대상")]
        [SerializeField] private LogisticsSimulationController simulation;

        [Tooltip("뷰 전환")]
        [SerializeField] private CameraViewSwitcher cameraSwitcher;

        [Tooltip("UI 에셋 연결 전 기본 화면 (IMGUI)")]
        [SerializeField] private bool drawFallbackGui = true;
        #endregion

        #region 2. 상태
        private readonly StringBuilder text = new StringBuilder();
        public string DisplayText => text.ToString();
        #endregion

        #region 3. 조작
        // 운전 버튼
        public void OnStart() { if (simulation != null) simulation.StartSimulation(); }
        public void OnPause() { if (simulation != null) simulation.PauseSimulation(); }
        public void OnResume() { if (simulation != null) simulation.ResumeSimulation(); }
        public void OnReset() { if (simulation != null) simulation.ResetSimulation(); }

        // 생산 속도
        public void OnSpeedChanged(float multiplier) { if (simulation != null) simulation.SetSpeedMultiplier(multiplier); }

        // 설정 선택 (UI 버튼용 번호: 0 기본, 1 시연)
        public void OnPresetSelected(int preset) { OnPresetSelected((SettingsPreset)preset); }
        public void OnPresetSelected(SettingsPreset preset) { if (simulation != null) simulation.SelectPreset(preset); }

        // 뷰 선택 (UI 버튼용 번호: 0 전체, 1 분류, 2 FR5, 3 도크)
        public void OnViewSelected(int view) { if (cameraSwitcher != null) cameraSwitcher.SwitchToIndex(view); }
        #endregion

        #region 4. 표시
        private void Update()
        {
            RefreshDisplay();
        }

        // 표시값·정체 알람 갱신
        public void RefreshDisplay()
        {
            text.Clear();
            if (simulation == null)
            {
                return;
            }
            SimulationSettings settings = simulation.ActiveSettings;
            text.AppendLine($"운전 상태: {StateName(simulation.State)}   속도 x{simulation.SpeedMultiplier:F1}");
            if (settings != null)
            {
                text.AppendLine($"설정: {(simulation.ActivePreset == SettingsPreset.Default ? "기본" : "시연")}  L={settings.PalletLayerCount}  N={settings.TruckPalletCapacity}  (층당 {SimulationSettings.BoxesPerLayer}개)");
            }
            if (simulation.BoxSpawner != null)
            {
                text.AppendLine($"투입 {simulation.BoxSpawner.SpawnedCount}개");
            }
            if (simulation.BarcodeScanner != null)
            {
                text.AppendLine($"판독 {simulation.BarcodeScanner.ReadCount}개  실패 {simulation.BarcodeScanner.FailCount}개");
            }
            foreach (LaneProcessController lane in simulation.LaneProcesses)
            {
                if (lane == null || lane.OccupancyMonitor == null)
                {
                    continue;
                }
                LaneOccupancyMonitor m = lane.OccupancyMonitor;
                bool waiting = lane.MiddleStopper != null && lane.MiddleStopper.HasWaitingBox;
                bool pickup = lane.PickupZone != null && !lane.PickupZone.IsEmpty;
                text.AppendLine($"레인{(int)lane.Lane + 1}: 순환 {m.CirculatingCount}  스토퍼 {(waiting ? 1 : 0)}  픽업 {(pickup ? 1 : 0)}  처리 {lane.ProcessedCount}{(m.IsCongested ? "  [정체]" : "")}");
            }
        }

        private static string StateName(SimulationRunState state)
        {
            switch (state)
            {
                case SimulationRunState.Running: return "운전";
                case SimulationRunState.Paused: return "일시정지";
                default: return "정지";
            }
        }

        private void OnGUI()
        {
            if (!drawFallbackGui || simulation == null)
            {
                return;
            }
            GUILayout.BeginArea(new Rect(10, 10, 380, 330), GUI.skin.box);
            SimulationRunState state = simulation.State;

            GUILayout.BeginHorizontal();
            GUI.enabled = state == SimulationRunState.Stopped;
            if (GUILayout.Button("시작")) OnStart();
            GUI.enabled = state == SimulationRunState.Running;
            if (GUILayout.Button("일시정지")) OnPause();
            GUI.enabled = state == SimulationRunState.Paused;
            if (GUILayout.Button("재개")) OnResume();
            GUI.enabled = true;
            if (GUILayout.Button("초기화")) OnReset();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = state == SimulationRunState.Stopped;
            if (GUILayout.Button("기본 설정")) OnPresetSelected(SettingsPreset.Default);
            if (GUILayout.Button("시연 설정")) OnPresetSelected(SettingsPreset.Demonstration);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            SimulationSettings settings = simulation.ActiveSettings;
            if (settings != null)
            {
                Vector2 range = settings.SpeedMultiplierRange;
                float speed = GUILayout.HorizontalSlider(simulation.SpeedMultiplier, range.x, range.y);
                if (!Mathf.Approximately(speed, simulation.SpeedMultiplier)) OnSpeedChanged(speed);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("전체")) OnViewSelected(0);
            if (GUILayout.Button("분류")) OnViewSelected(1);
            if (GUILayout.Button("FR5")) OnViewSelected(2);
            if (GUILayout.Button("도크")) OnViewSelected(3);
            GUILayout.EndHorizontal();

            GUILayout.Label(text.ToString());
            GUILayout.EndArea();
        }
        #endregion
    }
}
