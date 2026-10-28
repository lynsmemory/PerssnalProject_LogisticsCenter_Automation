# 차량 외형 재제작 1차 결과 (Claude → Codex)

상태: 코드 작성 완료. **미실행·미컴파일·Unity 임포트 미확인.** 2차(적재함 디테일·반사판·힌지·후면 램프 보강)는 미착수.

- 생성기: `Docs/Research/rebuild_wingbody_detailed.py` (Pillow만 사용)
- 실행(스테이징, Assets 변경 없음): `C:/Users/LYN/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe Docs/Research/rebuild_wingbody_detailed.py --output Temp/WingBodyRebuild`
- 적용(검수 후): 같은 명령에 `--apply`. `validation.json`의 ok=true일 때만 prefab과 `Meshes/*.asset(.meta)`를 Assets/LogisticsVehicles에 복사. 원본 prefab은 `<output>/backup`에 백업. prefab `.meta`(GUID ec8a…544)·씬은 건드리지 않음.
- 출력: `<output>/LogisticsVehicles/{WingBody5t_Coupang.prefab, Meshes/}`, `<output>/Preview/{front,side,oblique,oblique_rear}.png` 및 검사용 OBJ, `<output>/validation.json`.

변경 내용
- 기존 prefab을 읽어 Cab/CabRoof/Windshield/FrontGrille/FrontBumper/SideWindow/Mirror/Headlight/Step/Wheel/Hub/Chassis/SideRail 큐브를 제거하고 루트 자식으로 5개 메시 노드(fileID 900000~)를 추가: CabShell(챔퍼 쉘, 경사 앞유리 면, 루프 디플렉터, 휠아치 절개·내측 검정, 텀블홈, 도어선/손잡이/계단, BoxCollider), CabGlass, CabFront(4단 그릴·헤드램프·범퍼·와이퍼·미러), Wheels(32분할 프로파일 타이어·림·허브·볼트 8개, 후륜 복륜), Underbody(프레임·횡보·차축·연료탱크·흙받이·언더가드).
- 루트 10/11, 피벗 61/271/231/441, 파레트 슬롯·도크·접근 기준점, 적재함·윙·뒷문·로고 블록은 원문 유지(검증 항목). CabDoorLogo만 x=±1.106으로 이동.
- 휠베이스 4.9m(앞축 z=3.90, 뒤축 z=-1.00; 사진 비율 기준), 전장 약 8.85m. CargoFloor 상면 1.1m 유지.

Mesh 형식: 이 프로젝트 PackageCache의 `6FaceSkyboxMesh.mesh`(serializedVersion 11, NativeFormatImporter mainObjectFileID 4300000)를 따름. position/normal/uv0 Float32 interleave, 재질별 submesh, 65535 정점 초과 시 UInt32 인덱스. 최상위 키 일치 여부는 validation.json `mesh_top_level_keys_match`. OBJ 임포트는 사용하지 않음(OBJ는 프리뷰 검사용, Assets 밖).

수정 1 (Codex 확정 버그, 미실행): `area2`의 shoelace 부호를 표준 방식(CCW>0)으로 바꿈. 이에 따라 `edge_out`은 외향, `inset`은 내향, `triangulate`의 볼록 판정이 정상화됨. 자가 시험 `geometry_self_test`를 추가하고 그 결과를 `validation.json`의 `geometry_self_test`에 기록하며 `ok`에 포함함. 시험 항목은 사각형 CCW/CW 면적 ±1과 각 2삼각형, 아래 변 외향 (0,-1), cab_profile과 inset의 24정점·22삼각형·면적 합 일치, 쉘 본체 측면의 외향 법선, 타이어 트레드의 외향 법선이다.

Codex 검수 요청: 실행 결과/프리뷰 확인, Unity 임포트 시 Mesh 로드 오류·법선 방향·휠아치와 타이어 간섭, 씬 8개 도크 연결판과 차량 위치 확인. 문제는 파일·위치를 적어 Claude에 인계.

## Unity 실제 렌더 검증 (2026-10-28 15:08)

- 도구: `Docs/Research/VehiclePrefabPreview.cs`를 임시 프로젝트 `C:/Users/LYN/AppData/Local/Temp/veh-01a180cc/Assets/Editor`에 복사해 실행. 원본 프로젝트·씬·Prefab은 변경하지 않음.
- 대상: 임시 프로젝트에 들어 있던 15:04 생성본 `WingBody5t_Coupang.prefab`과 `Meshes/*.asset` 5개 (법선 수정 반영본).
- 명령: Unity 6000.6.0f1 `-batchmode -executeMethod VehiclePrefabPreview.ValidateAndRender -vehiclePreviewOut <dir>` (-quit 없음). 종료코드 0, 컴파일 오류 없음. 로그의 Licensing access token 오류 1건은 에셋과 무관.
- 메시 검사: 렌더러 38개 모두 OK, failures=0. CabShell 816정점/3서브메시, CabGlass 369/1, CabFront 1112/4, Wheels 8040/3, Underbody 1430/2. 서브메시 수 = 재질 수, 법선 수 = 정점 수. 전체 bounds 2.72×3.58×8.81m.
- 결과 이미지: `Docs/Collaboration/UnityRender_2026-10-28/{front,side,oblique,oblique_rear}.png`, `preview_log.txt`.
- 판정: 깊이 버퍼 렌더에서 면 누락·줄무늬가 없음. Pillow 프리뷰의 줄무늬는 그리기 순서 문제이며 메시 결함이 아님. 경사 앞유리, 루프 디플렉터, 휠아치, 타이어·림·볼트, 복륜, 그릴·범퍼·미러, 로고가 정상 표시됨.
- 한계: Standard 셰이더 대체 렌더라 URP 재질 자체의 표시는 확인하지 않음. 원본 프로젝트 적용(`--apply`), 씬 8대 반영, 도크 연결판과의 위치 확인은 미실행.

## 원본 프로젝트 적용 (2026-10-28, 사용자 승인)

- 실행: `rebuild_wingbody_detailed.py --output <scratchpad>/apply_out --apply`. 종료코드 0, validation ok=true, geometry_self_test ok.
- 변경: `Assets/LogisticsVehicles/WingBody5t_Coupang.prefab` 교체, `Assets/LogisticsVehicles/Meshes/` 신규(메시 5개와 .meta). prefab `.meta` GUID ec8a…544 유지. 씬 파일은 바이트 단위로 변경 없음.
- 적용본은 Unity 렌더 검증에 쓴 임시 프로젝트 파일과 prefab·메시·meta 12개 모두 동일(cmp).
- 보존 확인: 루트 10/11, 피벗 61/271/231/441 위치, 파레트 슬롯 10개·도크·접근 기준점 원문 동일, CargoFloor 상면 1.1m, 중복 ID 없음, 로컬 참조 유효. 전장 8.80m.
- 씬(Codex 배치 기준): 차량 8대(게이트 3,5,7,10,12,13,14,15) 루트 Y=-1.08976 유지, 뒷문 180/-180·윙 0 override 유지, 씬이 참조하는 prefab ID(10,11,61,231,271,441,100100000) 누락 없음. DockBridge 8개 그대로.
- 연결판 간섭 계산: Underbody 상단 로컬Y=1.02, 연결판 하면 로컬Y=1.02(상면1.1−두께0.08)로 겹치지 않음. 후단 Underbody z≥-3.70, 바퀴 z≥-1.46.
- 백업: 적용 전 prefab과 씬을 `<scratchpad>/prebackup/`, 생성기 백업 `<scratchpad>/apply_out/backup/`에 저장(세션 임시 폴더).
- 미확인: 원본 프로젝트 Unity 에디터가 실행 중이라 배치 임포트는 하지 않음. 화면 조작 없이 진행하라는 사용자 지시에 따라 에디터 화면에서의 리임포트·URP 재질 표시·씬 외형은 확인하지 않음. 에디터 포커스 시 자동 리임포트 예정. `Assets/LogisticsVehicles/validation.json`은 이전 생성기의 기록이라 갱신하지 않음.
