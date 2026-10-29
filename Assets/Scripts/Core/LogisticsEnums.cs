namespace Logistics
{
    // 박스 종류. 새 박스는 값을 추가한다. (FR-03, 박스 타입 추가 대비)
    // Standard: 현재 적용 박스 180×140×170mm, 2kg (SRS·Design의 360×280×340mm에서 사용자 승인으로 변경됨)
    public enum BoxType
    {
        Standard
    }

    // 목적 레인. 바코드 L01·L02·L03과 대응한다. (FR-07)
    public enum DestinationLane
    {
        Lane1,
        Lane2,
        Lane3
    }

    // 기본(L=3, N=10)·시연(L=2, N=3) 설정 선택 (FR-23)
    public enum SettingsPreset
    {
        Default,
        Demonstration
    }

    // 전체 운전 상태 (FR-21)
    public enum SimulationRunState
    {
        Stopped,
        Running,
        Paused
    }

    // 카메라 뷰. CameraViewSwitcher의 카메라 배열 순서와 같다. (FR-02)
    public enum CameraView
    {
        Overview,
        Sorting,
        Palletizing,
        Dock
    }

    // FR5 설비 상태 (화면 표시) (FR-09, FR-18)
    public enum PalletizingState
    {
        Idle,
        Picking,
        Placing,
        Returning
    }

    // AMR 설비 상태 (화면 표시) (FR-13~15, FR-18)
    public enum ForkliftState
    {
        Idle,
        MovingToPallet,
        Lifting,
        MovingToDock,
        Loading,
        Returning,
        StoppedByObstacle
    }
}
