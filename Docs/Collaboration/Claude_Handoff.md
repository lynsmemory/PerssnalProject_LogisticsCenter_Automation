# Claude와 Codex 작업 인계

## 사용자 결정 및 Codex 처리 — 2026-10-28 후속

투입점은 현재 컨베이어 최상단 끝 `EntryLine/UpperEntryStart`로 확정. FR5 셀 3곳·카메라 최종 지정은 10/29, SRS 규격 물리 재시험은 신규 에셋 적용 후로 연기한다. 앞선 시험 박스 수정 요청은 당장 실행하지 않는다. 사용자는 Claude의 오늘 작업을 종료하고 파레트는 Codex가 직접 만들도록 지시했다. Claude를 추가 호출하지 않았다.

Codex가 `Assets/LogisticsVehicles/Pallet_1100x1100x150.prefab`과 목재 재질 3종을 만들고 별도 Unity 프로젝트에서 임포트·4방향 렌더를 확인했다. 셀 위치 미정으로 씬 배치는 하지 않았다. 상세는 `Pallet_Prefab_Result_2026-10-28.md` 참조. WBS 실적 기준일과 20·24·25·26·27행 배치 완료, 12행 신규 파레트 완료일을 반영했다. 21·22행은 경계 표시 미완료, 23·36행은 최종 지정 대기로 완료일을 비워뒀다. 직접 영향이 있던 P20·P21 참조 수식을 수정해 재계산했다.

## Codex 검수 추가 — 2026-10-28

사용자 요청으로 최신 작업·오늘 WBS를 대조했다. 상세 결과는 `Codex_Audit_2026-10-28.md` 참조. 이번 검수는 파일·기존 로그·렌더 검토이며 새 Unity 실행은 하지 않았다.

- 수정 요청(기록만, CLI 전달·수정 실행 전): `Assets/Editor/ConveyorLayoutBuilder.cs:778,780`의 시험 박스는 0.35×0.34×0.48m·3kg으로, SRS의 360×280×340mm·2kg과 다름. 축 대응을 명시해 수정하고 기존 6초·10초 간격 물리 시험을 다시 검증해야 함. 원본 씬 Build를 자동 재실행하지 말 것. 코드 수정은 Claude 담당.
- 오늘 잔여: 투입점 확정·투입/분류 구역 경계 표시, FR5 셀 3곳 공간/통로 검증, 카메라 가림·프레임 조정, 경사 레인의 롤러 외형 미완성 상태 명시, 검증 후 WBS 갱신.
- FR5 URDF·STL 7개 다운로드 완료는 재확인함. 현재 Assets의 FR5 임포트·Prefab은 확인되지 않음. WBS 13행 Import 완료와 원본 다운로드를 혼동하지 말 것.
- 카메라 전환·스토퍼·바코드·AMR 등 후속 일정의 미구현을 오늘 누락으로 보고하지 않음. 사용자 지정 차량 8대를 임의 정리하지 않음.

## 연결 상태

2026-10-28 사용자 지정: Codex가 작업 지시·결과 수신·검수·수정 요청을 담당하고 사용자에게 최종 보고한다. 화면 조작 대신 Claude Code CLI를 사용한다. 사용자 승인으로 공식 설치 프로그램의 stable 버전 2.1.308을 설치했고 `C:\Users\LYN\.local\bin\claude.exe --version`으로 실행을 확인했다. 사용자 로그인 후 auth status의 loggedIn=true를 확인했다. Read 도구만 허용한 CLI 요청에서 AGENTS.md와 CLAUDE.md를 읽도록 지시해 종료 코드 0, is_error=false, CLAUDE_LINK_OK 응답을 받았다. 명령 전달과 결과 수신까지 연동 확인 완료다. 이 경로가 PATH에 없어도 절대 경로로 실행할 수 있다.

검증 세션: `081e8297-7652-48c6-b2f0-0fede64cb555`. 응답 기록: `Claude_Link_Check.json`. 검증 호출은 safe-mode, strict-mcp-config, tools=Read, allowedTools=Read, permission-mode=dontAsk로 실행했다. 스크립트 편집·명령 실행 도구를 Claude에 제공하지 않았다. 이 세션은 연동 확인용이며 구현 승인 시 작업 범위와 도구 권한을 새로 지정한다.

2026-10-28 갱신: 연동 확인 후 사용자가 Claude 데스크톱에서 직접 지시해 차량 적용, 컨베이어·레인·플리퍼 분기, 카메라 배치, 에셋 스크립트 정리를 진행했다(아래 "2026-10-28 Claude 작업 종합" 참조). 작업 001 중 공정 데이터 구조(SimulationSettings, LaneContext, LaneMonitor, PalletState, LogisticsController)는 아직 작성하지 않았다. 지속 감시나 백그라운드 자동 작성 작업은 시작하지 않았다.

공유 프로젝트 경로: `C:\Users\LYN\Desktop\Unity\PerssnalProject_LogisticsCenter_Automation`

## 담당

| 담당 | 작업 |
|---|---|
| Claude | 모든 스크립트 설계·작성·수정, 코드 수정 결과 보고 |
| Codex | 요구사항·코드 검수, Unity 씬·Prefab·재질·에셋 작업, 실행 검증 |

상태는 요청 준비 → Claude 작업 중 → 검수 요청 → Codex 검수 중 → 수정 요청 또는 검수 완료로 기록한다. 상대방의 실제 착수·완료를 확인하지 않고 상태를 바꾸지 않는다. 자동 폴링이나 지속 감시는 아직 설정하지 않았다.

## 공통 기준

- WBS: Docs/WBS_LogisticsCenter.xlsx.
- SRS: Docs/SRS_LogisticsCenter_v1.1.docx.
- 기획표: Docs/LYN_프로젝트_한장기획표.pptx.
- 클래스·오늘 작업 분석: Docs/Research/Development_Tasks_2026-10-28.md. 여기에 적은 클래스는 검토 제안이며 필수 확정 구조가 아니다.
- 참고 코드: 상위 폴더 Cell_Factory-main/Assets/Scripts/Factory. Niryo·손가락 그리퍼·조립 공정 전용 조건을 FR5·진공흡착·파레타이징에 맞게 변경한다.
- FR5 공식 원본: Docs/Research/CAD_References/FR5/fairino_description/urdf/fairino5_v6.urdf와 meshes/fairino5_v6의 STL 7개. 파일 검사 완료, Unity 반영·동작 시험은 미확인.
- CNN은 11/4 통합 시험 후 선택 확장. 기본은 Code 128 이미지 판독과 스토퍼·가이드 정렬이다.

## 요청 준비 001 클래스 분류와 초기 스크립트

담당: Claude. 상태: 부분 진행(2026-10-28). 사용자 직접 지시로 BoxRouting(판독 결과)·LaneDiverter(분기)·BrakeRollerZone(레인 속도 제어)만 작성. 아래 2~5항의 나머지 클래스는 미작성.

1. 분석 문서와 SRS를 대조해 클래스별 책임과 초기 작성 범위를 정리한다.
2. 우선 설정 L/N, 박스 추적/판독 상태, 3개 레인의 전담 참조, 레인 집계·정체 상태, 파레트 6슬롯 상태와 전체 운전 명령 경계를 설계한다.
3. 사용자의 오늘 목표에 맞춰 필요한 스크립트를 작성한다. 구체적 구현 전 기존 코드와 자산을 확인하고 불필요한 범용 프레임워크는 추가하지 않는다.
4. 신규 스크립트의 컴포넌트 부착 위치, Inspector 필드, 호출 순서, 공개 API, 기대 신호를 Codex에게 알려 준다.
5. 미정 투입 간격·생산속도·재시도·초기화·기록 규칙을 확정 요구사항처럼 하드코딩하지 않는다.

인수 조건: 코드가 SRS에 추적되고, 판독값을 박스 메타데이터로 대체하지 않으며, 3→2 유지→1 해제 정체 규칙과 중복 없는 박스/파레트 집계 경계가 드러난다. 코드 컴파일과 씬 시험은 실제 실행한 결과만 기록한다.

## 기존 제작 코드와 프리팹 현황

역할 분담 이전 Codex가 만든 코드도 이후 수정은 Claude가 담당한다.

- Assets/Editor/LogisticsVehiclePrefabBuilder.cs.
- Docs/Research/build_vehicle_prefabs.py.
- Assets/LogisticsVehicles/WingBody5t_Coupang.prefab.
- Assets/LogisticsVehicles/RobotForklift.prefab.
- Assets/LogisticsVehicles/README.md와 validation.json.

기본 형상 차량 프리팹은 파일 직접 생성 방식으로 작성했다. 로고는 사진 참고 글꼴 근사본이다. Unity 배치 실행이 초기 로딩에서 종료되어 Editor 임포트·외형·충돌은 검증하지 못했다. 로컬 참조와 파레트 10자리 경계만 검사했다. 차량 주행·상차·윙 개폐 제어 코드는 아직 없다. 이 상태를 완성된 작동 프리팹으로 간주하지 않는다.

## Claude 결과 기록 양식

- 요청 번호와 상태:
- 변경 파일:
- 구현한 FR:
- 공개 API와 Inspector 연결:
- 실행한 검사와 결과:
- Codex에 필요한 씬 작업:
- 남은 문제:

## Codex 검수 기록 양식

- 요청 번호와 상태:
- 검토한 파일과 버전/시점:
- 문제 위치와 재현 조건:
- 기대 동작과 실제 동작:
- 관련 FR/TC:
- Claude 수정 요청:
- Unity 작업 및 실행 결과:

## 컨베이어·레인 씬 배치 (2026-10-28, Claude, 사용자 직접 지시)

- 요청 번호와 상태: 사용자 지시 "컨베이어 먼저 설치". 도크는 사용자 지정 게이트 12·13·14. 사용자 지시로 화면 조작 없이 배치 모드로 작업. 배치·물리 시험 완료, Codex 검수 전.
- 변경 파일: `Assets/Editor/ConveyorLayoutBuilder.cs`(신규, 배치 생성기), `Assets/Editor/SceneLayoutScan.cs`(신규, 씬 조회 전용·저장 안 함), `Assets/LogisticsLayout/LaneRollerLowFriction.physicMaterial`(신규), `Assets/Scenes/LogisticCenter_Automation.unity`(루트 `ConveyorSystem` 추가). 기존 오브젝트는 수정하지 않음. 변경 전 씬 백업: `Docs/Collaboration/ConveyorLayout_2026-10-28/LogisticCenter_Automation_before_conveyor.unity.bak`.
- 사용 에셋: `Conveyor Works/Prefabs/Conveyor/Rotor`의 직선·짧은 직선·90도 턴·V90 분기, `ConveyorLegs`. 구동은 기존 `RoboLacyTech` LinearConveyor/CurvedConveyor.
- 배치(월드 m, 바닥 Y=0): 메인 순환 루프 `(=====)` 벨트 상면 1.25m(사용자 요청으로 높임, `Lift`=0.75). 서쪽 직선 x=-18.6(흐름 -Z), 동쪽 직선 x=-16.94(흐름 +Z), z -3.0~14.6, 양 끝 180도 턴(반지름 0.83). 반시계 방향. 투입 라인은 동쪽에서 z=3.0으로 합류(5.3m). 레인 1/2/3은 서쪽 직선에서 z=-0.36/8.6/13.0으로 서쪽 분기, 각 5m·4도 하향, 끝 상면 약 0.88m, 픽업점 x≈-24.17. 레인1→게이트12, 레인2→게이트13, 레인3→게이트14. 기둥(-14.2,7.2) 비간섭.
- 기준점: `ConveyorSystem/Markers/BoxSpawnPoint`, `BarcodeReadPoint`(동쪽 직선 z=8), `LaneN_Gate_XX/DiverterPoint`, `PickupPoint`.
- 공개 API·Inspector: `Tools/Logistics/Build Conveyor Layout` 메뉴 또는 `-executeMethod ConveyorLayoutBuilder.Build|Simulate|Render [-layoutOut <dir>]`. Build는 기존 `ConveyorSystem`을 지우고 다시 만든다. 치수·높이·레인 위치·속도(0.5m/s)·경사·마찰은 파일 상단 상수.
- 구현 시 확인한 사항: (1) LinearConveyor/CurvedConveyor는 축 반대 방향으로 물체를 움직인다. 직선은 Backward, 턴은 Forward로 설정해 실제 흐름을 맞춤. (2) CurvedConveyor는 피벗 기준 회전이라 턴 피벗을 호 중심에 둔다. (3) V90 분기 메쉬 상면이 직선보다 3.6cm 높고 분기부를 옆으로 밀 수 없어, 메쉬 충돌체를 끄고 직선 높이의 보이지 않는 구동면(Band, 합류부 Branch)을 둠. (4) 레인 롤러 조각 사이 이음매에서 박스가 걸려(PhysX 내부 모서리) 레인마다 단일 경사면 충돌체 사용. (5) 창고 바닥 메쉬에 충돌체가 없어 `ConveyorSystem/FloorCollider` 추가. (6) 루프·레인·투입에 보이지 않는 측면 가이드(간격 0.8m, 높이 0.12m), 레인 끝에 임시 고정 엔드스톱.
- 실행한 검사와 결과(배치 모드 Physics.Simulate, 0.35×0.34×0.48m·3kg 시험 박스, 씬 저장 안 함): 투입 박스가 합류 후 루프를 약 72초에 한 바퀴 돌며 3개 분기점을 지나 계속 순환(분기 동작 없음). 레인 1~3 시작점에 놓은 박스가 중력으로 내려와 엔드스톱 앞 x=-24.28에 정지. 컴파일 오류 없음. 결과: `Docs/Collaboration/ConveyorLayout_2026-10-28/conveyor_simulate.txt`, 렌더 `layout_top/oblique/lanes.png`.
- 미검증·남은 문제: Unity 에디터 Play 모드 확인은 하지 않음. 분기 장치의 레인 방출 동작(LaneDiverter)과 바코드 판독은 공정 스크립트 보류로 미구현. 레인 끝 도달 속도 약 0.8m/s로 엔드스톱 충돌이 강함 → 중간 스토퍼·실제 픽업 스토퍼 작업 시 마찰/경사 재조정 필요. 측면 가이드는 충돌체만 있고 외형 없음. 픽업점 주변 FR5·파레트·AMR 통로 공간은 확보만 하고 간섭 검증 안 함.
- Codex에 필요한 작업: 에디터에서 씬을 열어 외형·높이·도크 동선 검수. 수정이 필요하면 생성기 상수 변경을 Claude에 요청(씬 직접 수정 시 Build 재실행으로 덮어써짐).

## 레인 분기 장치 스크립트 (2026-10-28, Claude, 사용자 지시 "Write the diverter script")

- 상태: 사용자 지시로 공정 스크립트 보류 중 분기 장치만 착수. 코드 작성·컴파일·배치 모드 물리 시험 완료. Play 모드·Codex 검수 전.
- 변경 파일: 신규 `Assets/Scripts/Logistics/Box/BoxRouting.cs`, `Assets/Scripts/Logistics/Sorting/LaneDiverter.cs`, `Assets/LogisticsLayout/GuideNoFriction.physicMaterial`. 수정 `Assets/Editor/ConveyorLayoutBuilder.cs`(분기점 생성, 가이드 마찰 0, 합류부 유도판, 분기 시험), 씬 `ConveyorSystem` 재생성.
- 구현 FR: FR-07 목적 레인 분기(판독 결과와 레인 수용 여부로 분기, 메인 계속 운전). FR-19의 "수용 불가 시 통과·순환" 경계.
- 동작: 분기점마다 길이 0.9m 이송 구간(`Diverter_LaneN/Transfer`)이 평소 메인 흐름 방향으로 구동. `BoxRouting.ReadLaneId`가 자기 `laneId`와 같고 `AcceptsBoxes`가 true인 박스의 중심이 구간 중앙 ±0.12m에 오면 구간만 레인 방향(-X)으로 바꿔 밀어 넣고, 박스 전체가 레인 시작점을 넘으면 메인 방향으로 복귀. 4초 안에 못 넘기면 `TransferFailed` 후 복귀.
- 공개 API: `BoxRouting.SetReadResult(string)/ClearReadResult()/ReadLaneId/HasReadResult` — 바코드 판독기만 기록(설정 목적지 복사 금지). `LaneDiverter.LaneId`, `AcceptsBoxes`(레인 모니터가 설정, 기본 true), `IsTransferring`, `IsRunning`, `Play()/Stop()`, 이벤트 `TransferStarted`, `Diverted`(박스당 1회), `TransferFailed`.
- Inspector: `LaneDiverter`는 Rigidbody(kinematic)+BoxCollider와 같은 오브젝트. laneId(L01/L02/L03), speed 0.5, transferDirection(local +X), alignTolerance, exitDistance, transferTimeout. 생성기가 자동 설정.
- 시험(배치 모드 Physics.Simulate 170초, 시험 코드가 판독기 대신 판독값을 직접 기록, 결과 `Docs/Collaboration/ConveyorLayout_2026-10-28/diverter_simulate.txt`): L03·L01·L02 박스가 각자 레인으로 분기 후 엔드스톱에 도달. 판독값 없는 박스는 계속 순환. 레인2가 첫 분기 후 거부(AcceptsBoxes=false)하는 동안 두 번째 L02 박스는 통과·순환하고, t=100초 재허용 후 다음 바퀴(158.5초)에 분기되어 레인2 앞 박스 뒤에 정지. 이송 실패 0건.
- 수정하며 발견한 점: 가이드 기본 마찰(0.6)에 박스가 붙어 잠들어 레인 중간에 정지 → 가이드 마찰 0. 박스 뒤끝이 이송 구간에 남은 채 복귀하면 끌림 → 복귀 기준을 박스 전체 통과로. 합류부 가이드 끝에 박스가 걸림 → 사선 유도판 추가.
- 남은 문제: 투입 박스와 순환 박스가 합류부에서 동시에 만나면 충돌 가능(합류 간격 판단은 BoxSpawner/투입 허용 작업에서 필요). 이송 구간 길이 0.9m 안에 박스 2개가 동시에 있으면 뒤 박스도 밀릴 수 있어 최소 박스 간격 기준 필요. 레인 집계(순환 3/중간 1/픽업 1)와 `AcceptsBoxes` 자동 설정은 LaneMonitor 미작성. 바코드 판독기 미작성.

## 컨베이어 재배치 v2: 공장 중앙 루프 + 플리퍼 분기 (2026-10-28, Claude, 사용자 지시)

- 상태: 사용자가 에디터에서 옮긴 배치(15:57 저장본, 백업 `ConveyorLayout_2026-10-28/LogisticCenter_Automation_user_edit_1557.unity.bak`)를 기준으로 생성기를 다시 작성해 `ConveyorSystem`을 재생성. 배치 모드 물리 시험 통과. Play 모드·Codex 검수 전.
- 사용자 결정: 메인 루프 상면 3.00m(4.20m/2.5m 검토 후), 레인은 바닥 높이 픽업까지, 스토퍼는 메인 근처, 플리퍼 분기, 생성 위치는 추후 지정, 높이는 딱 떨어지는 값.
- 변경 파일: `Assets/Editor/ConveyorLayoutBuilder.cs`(전면 재작성), `Assets/Editor/SceneLayoutScan.cs`(조회·렌더 추가, 저장 안 함), `Assets/Scripts/Logistics/Sorting/LaneDiverter.cs`(플리퍼로 재작성), 신규 `Assets/Scripts/Logistics/Lane/BrakeRollerZone.cs`, 신규 에셋 `Assets/LogisticsLayout/{GuideNoFriction.physicMaterial, LaneSurface.asset, LaneGuideDownstream.asset, LaneGuideUpstream.asset}`, 씬 `ConveyorSystem`.
- 배치(월드 m): 루프 서 x=-9.26(흐름 -Z) / 동 x=2.97(+Z) / 남 z=-18.53(+X) / 북 z=0.73(-X), 턴 중심 z=-17.70·-0.10, 반시계. 벨트 상면 3.00. 투입: 위층 착지 구간 상면 5.00(+X) → 20° 동력 하강 → 동쪽 직선 안쪽 합류(z=-11.64). 바코드 판독점 동쪽 z=-5.0. 레인 1/2/3 = z -15.06/-6.10/-1.70(사용자 배치 위치), 서쪽으로 분기. 레인 단면 상면 3.00(벨트) → 2.99(이송 단차 10mm) → 4° → 2.90(중간 스토퍼 지점) → 15° → 1.00 → 4° → 0.90(픽업·엔드스톱), 레인 끝 x≈-19.65.
- 플리퍼(`LaneN_Flipper`, `LaneDiverter`): 동쪽 가이드 위 힌지(레인 중심 +0.30m 상류), 암 1.46m·55°·0.3초. 해당 판독값 박스가 힌지 상류 1.0~0.25m에 오고 레인이 수용 가능하면 펼침, 펼친 동안 암 아래 벨트 구간(`Section`)이 암 방향(대각선)으로 구동해 박스를 레인으로 보내고 박스가 벨트 가장자리를 0.1m 넘으면 복귀. 앞쪽(접힌 암 길이 범위)이나 1.5m 이내 뒤에 다른 박스가 있으면 분기하지 않고 통과·순환. 공개 API: `LaneId`, `AcceptsBoxes`, `IsDiverting`, `IsArmRetracted`, `IsRunning`, `Play()/Stop()`, 이벤트 `DivertStarted`, `Diverted`, `DivertFailed`.
- `BrakeRollerZone`: 레인 15°·픽업 구간의 속도 제어 롤러. 무동력 유지, 내리막 속도 0.6m/s 초과분만 제거.
- 시험(배치 모드 Physics.Simulate 300초, 판독기 대신 시험 코드가 판독값 기록, 결과 `ConveyorLayout_2026-10-28/v2_centre_flipper/simulate_gap6s.txt`, `simulate_gap10s.txt`): 투입 간격 6초(3m)·10초(5m) 모두 L03·L01·L02×2 박스가 각자 레인 분기 후 픽업(0.90m)에 정지, 레인 최고 속도 0.66~0.69m/s. 판독값 없는 박스 계속 순환. 레인2 거부 중 두 번째 L02 통과·순환, 재허용 후 분기. 분기 소요 약 3.2초, 실패 0건. 렌더 `loop_top/oblique/lanes.png`.
- 수정하며 확인한 물리 문제: (1) 패키지 컨베이어 구동으로는 마찰 0 암에 박스가 붙어 미끄러지지 않음 → 암 아래 구간 대각선 구동. (2) 박스 간 충돌체 꺾임부·이음매 단면이 접촉 여유 안에 들어와 정지 → 레인 바닥·측벽을 연속 메시로, 꺾임부 반지름 1m 곡선, 측벽 바닥 50mm 띄움, 측벽 단면. (3) 합류부 +X/+Z 경계 정체 → 분기 쪽 대각선 구동, 유도판 제거, 개구부 하류 0.9m. (4) 접힌 암 끝 옆 박스가 펼침에 맞아 튕김 → 차단 범위를 암 전체 길이로.
- 남은 문제·필요 사항: 투입 간격 관리 필요(박스 간격 1.5m 미만 또는 접힌 암 범위 안에서 겹치면 분기 생략·순환, 6초 간격에서 정상). 생성 위치 미지정(`EntryLine/UpperEntryStart` 표시만). 위층 바닥 구조물 없음(착지 구간 5.00m가 다리로만 지지). 바코드 판독기·중간 스토퍼·픽업 스토퍼·레인 집계 미작성. FR5·파레트·AMR 공간(레인 끝 서쪽 약 3.6m, 레인 사이)과 도크 12·13·14까지 AMR 경로 미검증. 측면 가이드는 충돌체만 있고 외형 없음. Play 모드 미확인.

## 컨베이어 v3: 측면 합류 + 레인을 도크 앞으로 (2026-10-28, Claude, 사용자 지시)

- 상태: 생성기 수정 후 `ConveyorSystem` 재생성, 배치 모드 물리 시험 통과. Play 모드·Codex 검수 전. 변경 전 씬 백업 `ConveyorLayout_2026-10-28/v3_side_merge_docks/LogisticCenter_Automation_before_v3.unity.bak`.
- 합류부: V90 T자 제거(사용자: 한 바퀴 돌면 투입 쪽으로 되돌아가는 모양). 동쪽 직선은 직선 롤러로 연속, 투입은 레인처럼 안쪽에서 옆으로 붙음. 투입 끝 0.45m 평탄 구간(상면 3.01m, 10mm 이송 단차, +X 구동) → 메인 위 합류 구간 `EntryMergeSection`(z -12.08~-10.74, 45° 대각선 구동). 순환 박스는 동쪽 가이드를 따라 통과. 경사로 상단 5.00m → 3.01m.
- 루프 확장: 서쪽 직선 x=-9.26 → -11.50, 북쪽 끝 z=-0.10 → 13.80(북쪽 직선 z=14.63, 계단 플랫폼 z≥15.6 회피). 기둥 (-14.2,7.2), (-1.0,6.1)과 비간섭. 동쪽 x=2.97, 남쪽 z=-18.53, 상면 3.00 유지.
- 레인 위치: 레인1 z=-0.36(게이트12 선상), 레인2 z=9.64(게이트13 선상), 레인3 z=12.00(게이트14 남쪽). 픽업 끝 x≈-21.9 — 게이트12·13 도크 안쪽 끝(x=-29.17)까지 약 7.3m, 게이트14(x=-21.66, z=16.73)까지 북쪽 약 4.7m를 FR5·파레트·AMR 공간으로 남김. 레인 단면 높이는 v2와 동일.
- 시험(450초, 투입 간격 6초·10초): L03·L01·L02 첫 바퀴 분기 후 각 도크 앞 픽업(0.90m) 도착, 최고 0.67~0.70m/s. 판독값 없는 박스 순환. 레인2 거부 중 두 번째 L02 통과, 재허용 후 다음 바퀴(약 320초)에 분기. 결과 `v3_side_merge_docks/simulate_gap6s.txt`, `simulate_gap10s.txt`, 평면 렌더 `loop_top.png`. 한 바퀴 약 190초.
- 남은 문제: 합류 구간에서 투입 박스와 순환 박스가 동시에 만나면 충돌 가능(투입 허용 판단 필요). 사선 렌더 카메라 위치는 새 루프 크기에 맞게 미조정. 그 외 v2 남은 문제 동일.

## 카메라 4대 배치 (2026-10-28, Claude, 사용자 지시 / WBS 36행)

- 변경: 신규 `Assets/Editor/ViewCameraBuilder.cs`(메뉴 `Tools/Logistics/Build View Cameras`, 배치 `-executeMethod ViewCameraBuilder.Build|Render -viewOut <dir>`), 씬 루트 `ViewCameras` 추가.
- 카메라(기존 Main Camera 설정 복사, 천장 8.83m 아래): `Cam_Overview` (10, 8.3, 10)→(-12, 1.5, -2) FOV 65 / `Cam_Sorting` (-4, 6.5, 4)→(-11.5, 3, 6) FOV 55, 서쪽 직선·플리퍼 3개 / `Cam_FR5` (-12.5, 5.5, 2.5)→(-21, 1.2, 6) FOV 65, 레인 끝·FR5 자리·도크(기둥 (-14.2,7.2) 회피) / `Cam_Dock` (-15, 6.5, -6)→(-26, 1, 9) FOV 60, 게이트 12·13·14.
- 4대 모두 비활성 상태로 저장(현재 Main Camera 화면 유지). 전환은 TC-02 작업에서 스크립트로 연결. 시점 렌더 `Docs/Collaboration/ViewCameras_2026-10-28/Cam_*.png`.
- 남은 문제: 전체 뷰는 기둥에 일부 가림, 에디터에서 미세 조정 필요. 레인 롤러 외형은 내일 다른 에셋으로 교체 예정(현재 Conveyor Works Rotor는 줄무늬 벨트 셰이더이며 원통 롤러 모델 없음).

## 창고 에셋 스크립트 제거 (2026-10-28, Claude, 사용자 지시)

- 사용자: 아직 작업하지 않은 기존 스크립트는 없애도 됨. 범위 확인 결과 창고 에셋 스크립트만 제거(`RoboLacyTech` 컨베이어 구동과 `Logistics` 스크립트는 유지).
- 씬: 루트 `DistributionCenterScripts`(CameraSwitcher·RandomAgentMovement·GOTranslator·AudioMuter·UI_Toggle·GPUDetection)와 `ImmersiveTraining`(오토바이 조립 교육 단계·타이머·안내 UI·CameraRT) 삭제, `cam/Main Camera`의 PlaySoundOnTrainingStepComplete 컴포넌트 제거.
- 파일: `Assets/Scripts/DistributionCenter`, `Assets/Scripts/ImmersiveTraining`(스크립트 41개, .meta 포함)를 프로젝트 루트 `_RemovedScripts_2026-10-28/`로 이동(Unity 미인식, 복구 가능). 정리 도구 `AssetScriptCleanup.cs`와 변경 전 씬 백업도 같은 폴더.
- 확인: 컴파일 오류 없음, 씬 로드 정상(루트: EventSystem, 조명, cam, env, 반사·조명, 차량, 연결판, ConveyorSystem, ViewCameras), 컨베이어 시험(6초 간격) 결과 이전과 동일.
- 영향: 메인 씬에서 쓰지 않는 교육용 프리팹(`Prefabs/env/immersiveTraining_screen_prefab`, `tablet_prefab`, `moto_bot`, `moto_training_parts/*`, `UI/InstructionMenu`)과 `SampleScene`·`project_prefabs` 씬은 누락 스크립트 표시가 생김. 필요 시 해당 프리팹 정리 여부 결정.


## 창고 에셋 교육용 프리팹·씬 정리 (2026-10-28, Claude, 사용자 지시 "프리팹도 정리")

- 이동(프로젝트 루트 `_RemovedScripts_2026-10-28/Assets/...`, 목록 `moved_assets.txt`): 제거한 스크립트를 쓰던 프리팹 21개(`Prefabs/UI/InstructionMenu`, `UI/ModalCanvas`, `env/immersiveTraining_screen_prefab`, `env/tablet_prefab`, `moto_bot/moto_prefab`, `moto_training_parts/*` 16개)와 이를 끼워 쓰던 `moto_bot/moto_bot_forassembly`, `moto_bot/assembly_instruction Variant`, 씬 `Scenes/urp_scene_with_training.unity`, `Scenes/project_prefabs.unity`.
- 유지(중요): `Assets/Scenes/urp_scene_with_training/` 폴더(LightingData·라이트맵 약 1.2GB)는 메인 씬의 구운 조명(`m_LightingDataAsset`)이라 이동하지 않음. 폴더 이름이 교육용 씬과 같지만 삭제·이동 금지. 한 번 옮겼다가 참조를 발견해 즉시 원위치함.
- 유지: `Prefabs/env`의 다른 프리팹(메인 씬 벽 등), `GhostParts`, `moto_part`, 나머지 UI 프리팹, `SampleScene`(참조 없음).
- 빌드 설정: `ProjectSettings/EditorBuildSettings.asset`의 유일한 빌드 씬을 `urp_scene_with_training.unity` → `LogisticCenter_Automation.unity`로 교체(백업 `_RemovedScripts_2026-10-28/EditorBuildSettings.asset.bak`).
- 확인: 남은 Assets 안에 이동한 파일을 참조하는 프리팹·씬·에셋 없음(GUID 검사). 컴파일 오류 없음. 메인 씬 카메라 렌더에서 조명 정상.

## 2026-10-28 Claude 작업 종합 (Codex 인계)

사용자 지시로 Claude가 Claude 데스크톱에서 직접 작업했고, Codex로 다시 넘긴다. 아래는 위 개별 기록의 요약이며 상세 수치·시험은 각 절과 `Docs/Collaboration/ConveyorLayout_2026-10-28/`, `ViewCameras_2026-10-28/`에 있다. 화면 조작 없이 Unity 배치 모드로만 실행했고 에디터 Play 모드 확인은 하지 않았다.

### 1. 완료 상태

| 항목 | 상태 | 근거 |
|---|---|---|
| 쿠팡 5톤 윙바디 새 외형 | 원본 프리팹 적용, 씬 8대 반영 | 임시 프로젝트 Unity 렌더, GUID·피벗·적재함 1.1m·씬 참조 검사 |
| 메인 순환 컨베이어(상면 3.00m) | 씬 배치, 순환 확인 | 배치 모드 물리 시험 |
| 위층 투입(5.00m) → 20° 하강 → 측면 합류 | 씬 배치, 합류 확인 | 물리 시험 |
| 플리퍼 분기 3개(L01/L02/L03) | 스크립트·배치, 분기 확인 | 물리 시험(6·10초 간격) |
| 경사 레인 3개 → 도크 12·13·14 앞 픽업 0.90m | 씬 배치, 도착 확인(0.67~0.70m/s) | 물리 시험 |
| 카메라 4대(전체·분류·FR5·도크) | 씬 배치(비활성) | 시점 렌더 |
| 창고 에셋 스크립트·교육용 프리팹·씬 정리 | 백업 폴더로 이동 | 컴파일·참조 검사 |
| WBS 기록, CLAUDE.md 상태 문구 | 미갱신 | CLAUDE.md 3행은 여전히 "구현 보류" — 사용자/Codex가 갱신 필요 |

### 2. 씬 구성 (`Assets/Scenes/LogisticCenter_Automation.unity`, 루트)

EventSystem, Directional Light, Global Volume, cam(Main Camera), env(창고), reflectionProbe, extraBakedLights, DockedWingBodyTrucks(8대), DockGapBridges(8개), **ConveyorSystem**(MainLoop, Guides, EntryLine, Markers, Lane1_Gate_12, Lane2_Gate_13, Lane3_Gate_14, FloorCollider), **ViewCameras**(4대, 비활성).

좌표 요약(월드 m): 루프 서 x=-11.50 / 동 x=2.97 / 남 z=-18.53 / 북 z=14.63, 상면 3.00, 반시계 0.5m/s. 투입 합류 z=-11.64(동쪽 직선 안쪽). 바코드 판독점 동쪽 z=-5.0. 레인 z=-0.36/9.64/12.00, 높이 3.00 → 2.99(이송 단차) → 2.90(중간 스토퍼 지점) → 15° → 1.00 → 0.90(픽업, x≈-21.9). 기준점: `EntryLine/UpperEntryStart`(생성 위치 미정, 사용자 추후 지정), `Markers/BarcodeReadPoint`, 레인별 `DiverterPoint`·`MiddleStopperPoint`·`PickupPoint`.

### 3. 작성·변경한 스크립트

- 런타임 `Assets/Scripts/Logistics/`: `Box/BoxRouting.cs`(판독 결과만 보관), `Sorting/LaneDiverter.cs`(플리퍼 분기, `AcceptsBoxes`, 이벤트 `DivertStarted/Diverted/DivertFailed`), `Lane/BrakeRollerZone.cs`(무동력 레인 속도 상한 0.6m/s).
- 에디터 `Assets/Editor/`: `ConveyorLayoutBuilder.cs`(Build/Simulate/Probe, 모든 배치 수치가 상단 상수), `ViewCameraBuilder.cs`(Build/Render), `SceneLayoutScan.cs`(조회·렌더 전용, 씬 저장 안 함). 기존 `LogisticsVehiclePrefabBuilder.cs` 미변경.
- 에셋 `Assets/LogisticsLayout/`: `LaneRollerLowFriction.physicMaterial`(0.03), `GuideNoFriction.physicMaterial`(0), `LaneSurface.asset`, `LaneGuideDownstream.asset`, `LaneGuideUpstream.asset`(레인 바닥·측벽 메시).
- 차량: `Docs/Research/rebuild_wingbody_detailed.py`(--apply 실행), `Docs/Research/VehiclePrefabPreview.cs`(임시 프로젝트 렌더 도구).
- 사용: `RoboLacyTech` LinearConveyor/CurvedConveyor(패키지 원본 미수정). 축 반대 방향으로 움직이는 특성 때문에 직선은 Backward, 턴은 Forward로 설정.

### 4. Codex 주의 사항

1. `ConveyorSystem`은 생성기가 지우고 다시 만든다. 씬에서 직접 옮기면 다음 Build에서 덮어써지므로 수치 변경은 `ConveyorLayoutBuilder` 상수로 하거나 Claude에 요청. (`ViewCameras`도 `ViewCameraBuilder.Build`가 재생성.)
2. 측면 가이드·레인 바닥·벨트 구동면은 보이지 않는 충돌체다. 보이는 롤러 프리팹(Conveyor Works Rotor)은 충돌체를 끈 외형일 뿐이다. 레인 롤러 외형은 내일 다른 에셋으로 교체 예정(Rotor는 줄무늬 벨트 셰이더, 원통 롤러 모델 없음).
3. 플리퍼는 앞쪽(접힌 암 1.46m 범위) 또는 1.5m 이내 뒤에 다른 박스가 있으면 분기하지 않고 순환시킨다. 투입 간격 3m(6초) 이상이면 정상.
4. `Assets/Scenes/urp_scene_with_training/` 폴더는 메인 씬 조명 데이터 — 이동·삭제 금지.
5. 배치 모드 실행 시 원본 프로젝트가 에디터에 열려 있으면 안 된다(잠금 충돌).

### 5. 남은 작업

- 미작성 스크립트: BoxSpawner(투입 간격·합류 대기), 바코드 판독기(Code 128 → `BoxRouting.SetReadResult`), 중간 스토퍼·픽업 스토퍼(현재 레인 끝은 고정 엔드스톱), LaneMonitor(순환 3/중간 1/픽업 1 집계·정체, `AcceptsBoxes` 설정), 카메라 전환(TC-02), SimulationSettings·LaneContext·PalletState·LogisticsController, FR5·AMR·상차.
- 씬·에셋: 레인 롤러 외형 교체, 위층 구조물(투입 착지 5.00m가 다리로만 지지), FR5 URDF 임포트, 파레트·AMR 배치, 대기 차량 3대 외 나머지 5대 정리 여부(SRS는 도크 3·대기 3), 측면 가이드 외형.
- 확인: 에디터 Play 모드 동작, URP 화면의 차량 재질, 전체 카메라 미세 조정(기둥 가림), 합류부에서 투입·순환 박스 동시 도착 충돌.
