# 물류센터 자동화 디지털 트윈 사전 조사

조사일: 2026-10-27 (한국시간)

추가 계산: 사용자가 제안한 FR5 베이스 높이 1.1m, 1,100 × 1,100 × 150mm 플라스틱 파레트, 흡착 툴 길이와 배치 간격을 반영한 결과는 [FR5 파레트 배치 타당성 검토](FR5_Pallet_Layout_Feasibility_2026-10-27.md)에 정리했다. 계산 조건에서 FR5 한 대의 고정 배치로 전체 파레트에 높이 2m까지 적재하는 구성은 높이와 수평 도달거리 모두 부족하다. 아래의 초기 검토보다 이 추가 계산을 우선 참고한다.

이 문서는 공정 이해와 URDF·무료 에셋 후보 조사 결과다. SRS, 확정 설계, 구현 결과가 아니다. 에셋은 제작자 페이지·저장소·일부 원본 파일을 확인했으며, 다운로드 후 Unity 임포트·실행·성능 검증은 수행하지 않았다. 품질 평가는 공개 설명·사양에 따른 후보 평가다.

## 1. 현재 작업 환경과 참고 자료

- 현재 프로젝트: `/home/lyn/Unity/LogisticsCenter_Automation`
- Unity: `6000.6.0f1`, URP: `17.6.0`, AI Navigation: `2.0.14`.
- 기본 SampleScene 상태이며, 물류 공정 스크립트와 FR5 모델은 아직 없다. URDF Importer·ROS 연결 패키지도 현재 manifest에는 없다.
- 사용자가 언급한 상위 Example 폴더는 실제로 `/home/lyn/Unity/Examples`다.
- 사용자 추가 안내에 따라 `/home/lyn/Unity/Examples/TeamProject_IFEA-main/Docs/SRS_IFEA.md`를 찾아 전체 내용을 검토했다. 같은 폴더에 `SRS_IFEA.docx`도 있다. 이번 검토는 Markdown 원본을 기준으로 했다.
- 문서 후보 `/home/lyn/Unity/Examples/Cell_Factory-main/셀조립공정.pdf`는 79쪽의 셀 조립 공정 교육 자료다. 텍스트를 추출해 공정 구조·컨베이어·제어·기록 관련 내용을 검토하고 구조 도식을 시각 확인했다. 물류센터 SRS로 취급하지 않았다.

예제에서 참고할 수 있는 것은 주문/공정 제어, 로봇 동작, 이송, UI, 기록의 역할 분리와 타임아웃·재시도·알람 처리다. 이번 물류센터는 여러 박스와 도크·로봇·AGV가 동시에 움직이므로 단일 셀의 순차 코루틴을 그대로 확대하기보다 설비별 상태와 작업 배정을 분리하는 방향이 적합하다. 이는 조사 단계의 제안이다.

### 확인한 SRS 참고 형식

`SRS_IFEA.md`는 실내 마감 비용·에너지 분석 프로젝트의 SRS v1.0(2026-10-06)이다. 물류 공정 자체의 요구사항이 아니라 향후 문서 구성과 상세 수준을 참고할 자료로 이해했다.

- 문서 구성: 개정 이력, 서론, 제품 개요, 기능 요구사항, 외부 인터페이스, 비기능 요구사항, 데이터 요구사항, 검증, 변경 관리, 검토 및 승인, 부록.
- 식별 체계: 기능 `FR`, 비기능 `NFR`, 화면 `UI`, 테스트 `TC`와 추적 매트릭스.
- 상세 기술 항목: 목적, 사용자, 사전 조건, 입력, 처리 순서, 출력, 예외 처리, 인수 기준, 검증 방법, 우선순위, 관련 화면.
- 작성 원칙: 요구사항의 단일 의미, 측정 가능한 조건과 기준, Must/Should/Could 우선순위.
- 원본의 개발 기간 6일, Windows 전용, 평균 30FPS, 외부 하드웨어 미연동 조건은 IFEA 프로젝트 조건이다. 이를 물류센터 프로젝트 조건으로 가져오지 않는다.
- 향후 문서화 시 참고 원본의 여러 FR을 묶은 상세 표보다는, 동시 동작과 예외 상황이 많은 물류 공정에 맞춰 기능별 인수 기준을 구분하는 것이 적합하다. 원본에서 표현과 우선순위가 일부 어긋나는 항목도 그대로 복제하지 않는다.

이 절은 참고 문서 검토 결과이며 물류센터 SRS 초안을 작성한 것은 아니다.

### 로컬 재사용 후보

아래 경로는 모두 `/home/lyn/Unity/Examples` 아래에 있다. 존재를 확인했으며 현재 프로젝트로 복사하지 않았다.

| 구분 | 확인한 파일 | 활용과 제한 |
|---|---|---|
| 컨베이어 로직 | `Cell_Factory-main/Assets/Scripts/Factory/Cell/ConveyorLine.cs` | 공급·정지·배출·푸셔·벨트 UV 표현 참고. 현재 예제는 이송 중 벨트 표현이 흐르는 구조이므로 이번 상시 운전 조건에 맞게 변경 필요 |
| 공정 제어 | `Cell_Factory-main/Assets/Scripts/Factory/Cell/CellController.cs` | 재시도·타임아웃·알람·운전 상태·생산성 기록 참고 |
| 로봇 IK | `Factory_ML_VLA-main/Assets/Scripts/Robot/ArmIK.cs` | ArticulationBody 기반 제어 참고. Niryo 관절명과 그리퍼에 의존하므로 FR5에 바로 적용 불가 |
| 흡착 그리퍼 | `Cell_Factory-main/Assets/realvirtual/3DPrefabs/SuctionGripper.prefab` | 외형 후보. 기능 스크립트·판본·사용권은 추가 확인 필요 |
| 컨베이어 | `Cell_Factory-main/Assets/realvirtual/3DPrefabs/ConveyorBig.prefab`, `ConveyorSmall.prefab` | 외형·모듈 배치 후보. 같은 이유로 출처/사용권 확인 필요 |
| 파레트·물류 | `Cell_Factory-main/Assets/Fbx/pallet.fbx`, `pallet_box_pack.fbx` | 적재 외형 후보. 원본 치수·라이선스 확인 필요 |
| 차량·주변 소품 | `Cell_Factory-main/Assets/Fbx/truck_trailer.fbx`, `shelf_forklift.fbx` | 이름만으로 5톤 차량이나 구동 가능한 지게차라고 판단할 수 없음. 메시 구조 확인 필요 |

로컬 예제에 파일이 있다는 사실만으로 무료 재배포 권한이 확보되는 것은 아니다. 특히 realvirtual 에셋은 실제 사용 판본과 취득 내역을 확인해야 한다.

## 2. 이해한 공정과 구현상 검토 지점

```text
컨베이어 상시 운전 → 박스 생성/투입 → 바코드 또는 색상 판별 → 목적 분류 레인 결정
→ 해당 분기에서 대각선 슬로프로 진입 → 중력 이송 → 픽업 위치 도착
→ FR5 진공 흡착 → 크기에 맞는 위치에 파레타이징
→ 적재 높이 제한 도달 → AGV 지게차 호출/파레트 교체
→ LiDAR로 주변을 감지하며 운반 → 출하 도크의 차량에 파레트 순차 상차
→ 차량 만재 판정 → 출발 표현 후 차량 및 탑재 화물 제거
```

여기서 컨베이어 분류 출구와 차량 접안 위치가 모두 '도크'로 불릴 수 있다. 조사에서는 앞쪽을 분류 레인, 뒤쪽을 출하 도크로 구분했다. AGV가 운반·상차하는 단위는 적재된 파레트로 해석했으며, 확정 요구사항은 아니다.

| 공정 | 조사 결과와 필요한 요소 |
|---|---|
| 상시 컨베이어 | 벨트 애니메이션과 박스 이송을 분리해야 한다. 정상 운전 중 벨트 상시 회전과 투입 제어를 별도로 표현한다. 박스 간격·추월 방지·분기 앞 대기가 필요하다 |
| 바코드/색상 | 박스 속성값을 읽는 가상 센서와 카메라 영상에서 실제 판독하는 센서는 구현 범위가 다르다. 바코드 미검출·오분류·중복 판독 처리도 달라진다 |
| 대각선 분기 | 수평 벨트 위 박스를 옆으로 보내는 푸셔·팝업 휠·게이트 등의 장치가 필요하다. 슬로프는 진입한 뒤의 중력 이송을 담당한다 |
| 중력 슬로프 | 진행 방향으로 높이차가 있어야 한다. 단순 미끄럼 모델에서는 경사와 마찰에 따라 출발·속도가 달라진다. 롤러 슬로프와 판형 슈트는 저항 특성이 다르다. 끝단 스토퍼와 박스 간 충돌 처리 필요 |
| FR5 흡착 | 픽업 위치 정렬, TCP, 흡착면 접촉, 흡착 성공/실패, 운반 중 유지, 놓기 동작이 필요하다. 외형 URDF만으로 흡착·IK·충돌 회피가 제공되지는 않는다 |
| 크기별 적재 | 박스 길이·폭·높이·질량, 허용 회전, 받침 면적, 적재 안정성에 따른 배치가 필요하다. 단순 좌표 격자와 혼합 크기 최적 적재는 난이도가 크게 다르다 |
| 2m 제한 | 파레트 포함 높이인지 화물만의 높이인지 미정이다. 다음 박스를 올리면 초과하는 경우도 만재로 처리할 수 있어야 한다 |
| 파레트 교체 | AGV가 가져간 뒤 빈 파레트를 누가 공급하는지 필요하다. 그동안 슬로프 버퍼·투입 제한·다른 레인 전환 중 어떤 방식을 쓰는지 정해야 한다 |
| AGV 지게차 | 경로 탐색 외에 조향·회전반경·후진·포크 삽입·승강·인양 확인이 필요하다. NavMesh 이동만으로 지게차 동작이 완성되지 않는다 |
| LiDAR | 센서 외형과 거리/포인트클라우드 생성 기능은 별개다. 감지 결과가 감속·정지·재계획에 연결되어야 한다. 낮은 장애물·돌출 화물·탑재 파레트의 자기 가림을 검토한다 |
| 차량 상차 | 도크 레벨러/경사판 또는 바닥 높이 정합, 차량 문 높이·폭, 적재함 내부 치수, 진입/후진 탈출 공간이 필요하다 |
| 차량 만재/교체 | 파레트 배치 공간과 총 화물 질량을 함께 검토한다. 만재 후 추가 AGV 진입을 막고 차량 출발 전에 작업 완료를 확인한다. 제거 전 출하 이력을 남기고 다음 차량 도착 정책을 정한다 |

### FR5와 2m 적재의 적합성

제조사 FR5 페이지는 정격 가반하중 5kg, 도달거리 922mm, 6축을 명시한다. 박스뿐 아니라 흡착 장치·플랜지 등의 질량과 무게중심도 함께 검토해야 한다. [FAIRINO FR5 사양](https://www.fairino.com/FR/4.html)

922mm는 바닥 기준 최대 적재 높이라는 뜻이 아니다. 베이스 위치, 박스 흡착 자세, 파레트의 가장 먼 모서리, 툴 길이와 간섭에 따라 실제 작업 범위가 달라진다. 따라서 FR5를 바닥에 고정한 배치로 2m 전체 층을 적재할 수 있다고 가정하면 안 된다.

검토할 대안은 로봇 승강축, 파레트를 층별로 내리는 리프트, 작업 높이 조정과 파레트 회전 장치, 더 긴 도달거리의 로봇이다. FR5를 유지한다면 승강축 또는 파레트 리프트를 포함한 작업 범위 검증이 우선이다. 받침대만 높이는 방식은 아래층 접근 문제까지 확인해야 한다.

## 3. 무료 URDF·에셋 후보

추천 표시는 현 프로젝트에 대한 적합성이다. '무료'는 모델/패키지 취득 비용을 뜻하며 기능 개발 비용이 없다는 의미가 아니다. 계정 로그인이나 CAD 제공 약관 동의가 필요한 후보도 있다.

### 로봇·흡착·이동체·센서

| 후보 / 원본 링크 | 확인한 제공 형태·라이선스 | 용도·품질 평가 | 추천 및 추가 작업 |
|---|---|---|---|
| [FAIRINO 공식 frcobot_ros2](https://github.com/FAIR-INNOVATION/frcobot_ros2/tree/main/fairino_description) | URDF + STL 메시. `fairino5_v6.urdf` 및 `meshes/fairino5_v6` 확인. 모델 패키지 라이선스 선언은 `TODO: License declaration` | FR5의 실제 링크·관절·질량/관성·제한값 기반 후보. 실기 형상 재현에 가장 직접적 | **FR5 1순위, 사용권 확인 조건부.** 정확한 하드웨어 세대 선택, TCP/툴 추가, 충돌 메시 단순화, IK/경로 구현 필요. `FR5WML`을 표준 FR5와 혼용하지 않음 |
| [Schmalz ROB-SET ECBPMi FAIRINO](https://www.schmalz.com/en/products/automation-743270/vacuum-grippers-746238/gripping-systems-and-end-of-arm-tooling-eoat-746240/handling-sets-312463/handling-sets-ecbpmi-308307/10.03.01.00987) | 제조사 CAD 제공 UI, 여러 포맷 다운로드 안내. CC0/오픈소스 권한은 확인되지 않음 | Fairino FR3-30용으로 표시된 흡착 장치. 실물 플랜지·컵 외형 참고에 적합 | **흡착 외형 1순위, CAD 약관 확인 조건부.** 실제 다운로드·파일 포맷·계정 조건은 미검증. 종이박스 흡착 성능과 크기 적합성은 별도. Unity용 메시 변환과 기능 구현 필요 |
| [renan028/forklift_robot](https://github.com/renan028/forklift_robot) | URDF·DAE·Webots PROTO. description `package.xml`은 Apache License 2.0 선언, 루트 LICENSE 전문은 확인 못함 | 포크 승강 관절을 가진 단순 지게차. 시각 고품질보다는 기구 구조 참고용 | **기능 참고 후보.** 원본에 `basse_link` 오타와 회전 관절 limit 누락 등이 있어 보정 필요. Gazebo/Webots 센서는 Unity에서 자동 작동하지 않음. 메시별 권리도 재확인 |
| [Nebula AMR Forklift](https://github.com/furkankarsli/amr_forklift) | Xacro·메시·ROS2/Gazebo·Nav2 관련 구성. `package.xml` MIT 선언, 독립 LICENSE 전문은 확인 못함 | 자율 지게차의 LiDAR·SLAM·주행 구조 참고. 실물 상용 지게차 고정밀 모델은 아님 | **ROS2 연동 참고 후보.** Xacro 전개, Gazebo 플러그인 대체, 모델별 권리 확인 필요 |
| [Forklift / Arsen Ismailov](https://sketchfab.com/3d-models/forklift-bdb03db7036e436286f4e2fd34c02a89) | 무료 다운로드 표시, CC Attribution, 약 11.1k 삼각형. Blender 모델링·Substance Painter 텍스처 설명 | 비교적 가벼운 PBR 지게차 외형 후보 | **AGV 외형 우선 후보.** 출처 표기 필요. URDF 없음. 포크/마스트/바퀴 분리 여부와 파일 포맷은 다운로드 후 확인. 무인화 외형 수정·LiDAR 추가 필요 |
| [Forklift / tasraven](https://sketchfab.com/3d-models/forklift-77a9000b8b584a16b739810fde725037) | 무료 다운로드 표시, CC Attribution, 약 34.4k 삼각형, 4K/2K/1K 텍스처 세트 설명 | 디테일 있는 지게차 시각 후보 | **고화질 대안.** 출처 표기 필요. 상표와 외형 처리 검토. URDF·자율주행·포크 제어는 별도 |
| [Innerscene Electric counterbalance forklift](https://www.innerscene.com/tools/library/3d-parts/electric-counterbalance-forklift-7acc0547) | 원작 CC0, GLB 약 690KB. 마스트·캐리지·포크 등 분리 설명 | 무상표 실척 시각화 모델. 명목 치수이며 성능 인증 CAD는 아님 | **CC0 대안.** 원본 설명상 가동부 분리가 장점. 품질·피벗은 임포트 후 확인. URDF·구동 로직 추가 필요 |
| [UnitySensors](https://github.com/Field-Robotics-Japan/UnitySensors) | Apache 2.0. LiDAR·RGB/RGBD·IMU 등, Unity/ROS 패키지와 샘플. README는 Unity 2022.3 이상 안내 | 센서 외형과 가상 측정 기능을 함께 확보할 후보. Velodyne·Livox 계열 포함 | **LiDAR 기능 1순위.** URP 이행 문서 제공. Unity 6.6 검증, 감지 레이어·장착 위치·해상도·주기 설정 필요. 회피 제어는 별도 |

FR5 라이선스 미확정의 근거는 공식 [모델 패키지 메타데이터](https://github.com/FAIR-INNOVATION/frcobot_ros2/blob/main/fairino_description/package.xml)다. 공개 저장소 전체를 특정 오픈소스 라이선스로 단정하지 않는다.

### 물류센터·컨베이어·박스·파레트·차량

| 후보 / 원본 링크 | 확인한 제공 형태·라이선스 | 용도·품질 평가 | 추천 및 추가 작업 |
|---|---|---|---|
| [Unity Warehouse / Unity Technologies Japan](https://assetstore.unity.com/packages/3d/environments/industrial/unity-warehouse-276394) | 무료 Asset Store 프로젝트, Standard Unity Asset Store EULA. 약 936.8MB, Unity 2022.3.16 기준 HDRP, URP 비호환 표시 | 포토리얼 물류창고 데모. 선반·화물·운반 장비 등 환경 구성 | **시각 품질 우선 1순위.** 현재 URP에 즉시 호환되지 않음. HDRP 채택 또는 메시 선별 후 URP 재질/조명 재구성 필요. 전체 물류 공정 자동화가 제공된다는 의미는 아님 |
| [Unity Robotics Warehouse](https://github.com/Unity-Technologies/Robotics-Warehouse) | Apache 2.0, URP/HDRP 프로젝트 및 패키지 제공 | 로봇 시뮬레이션용 창고 환경. URP를 유지할 때 유력 | **현 프로젝트 환경 기반 1순위.** 구버전 Perception preview·Post Processing 의존성 확인. 프로젝트 전체 병합보다 필요한 자산 선별 검토. Unity 6.6 호환 미검증 |
| [Kenney Factory Kit](https://kenney.nl/assets/factory-kit) | CC0, 3D, 140개 파일, 애니메이션·색 변형 표시 | 컨베이어·산업 소품 모듈 후보. 단순화된 스타일 | **공정 프로토타입에 추천.** 포토리얼 수준 후보와 구분. 현재 배포본의 상세 메시/포맷 구성은 미검증. 분기·슬로프 기능은 별도 제작 |
| [realvirtual.io Starter](https://realvirtual.io/en/demo/) | 공식 무료 Starter 안내. Unity 6.3 LTS 기준 컨베이어·드라이브·센서·기본 프레임워크 제공 | 디지털 트윈 운반/센서 기능을 빠르게 구성할 후보 | **기능 검토 우선.** 실제 Asset Store 약관·판본 확인 필요. 무료 Starter에 전문판의 모든 PLC 인터페이스·기능이 포함되는 것은 아님. 로컬 예제 판본과 동일 여부 미확인 |
| [Poly Haven Cardboard Box 01](https://polyhaven.com/a/cardboard_box_01) | CC0, PBR, 약 17k 삼각형·4K 모델 표시 | 낡은 상자 디테일이 있는 고품질 소품 | **근접 시각화에 추천.** 다량 박스에는 LOD/저면수 버전과 단순 BoxCollider 권장. 규격별 물류 상자는 치수 기반 별도 메시+재질 구성이 더 적합할 수 있음 |
| [Poly Haven Modular Factory Facade](https://polyhaven.com/a/modular_factory_facade) | CC0, 모듈식 공장 외벽 모델 | 산업 건물 외관 보강 후보 | **환경 보강 대안.** 물류창고 내부·출하 도크 완성 세트는 아님. 필요한 구조와 문 크기는 별도 구성 |
| [Warehouse Pack Free — Low-Poly Warehouse Essentials](https://assetstore.unity.com/packages/3d/environments/warehouse-pack-free-low-poly-warehouse-essentials-407032) | 무료, Standard Unity Asset Store EULA, Unity 6000.3 이상 URP. 15종 소품 안내 | 파레트·랙·도크 문/범퍼·박스·바닥 라인. 단순 색상 스타일 | **파레트/도크 프로토타입 대안.** 포토리얼 후보는 아님. 게시 설명 내 제작 방식 표기가 일관되지 않고 AI 사용 고지도 있어 패키지 품질 확인 필요 |
| [Kenney Car Kit](https://kenney.nl/assets/car-kit) | CC0 차량 팩 | 단순 차량 외형 후보 | **출발/교체 흐름 시제품용.** 현재 팩의 개별 트럭 구성은 다운로드 확인 필요. 국내 5톤 탑차의 치수·하중을 보증하지 않음 |
| [Innerscene 7.5 t class box truck](https://www.innerscene.com/tools/library/3d-parts/7-5-t-class-box-truck-with-roll-up-door-12baea66) | 원작 CC0, GLB 약 5.8MB, 무상표 탑차 | 적재함과 롤업 문이 있는 차량 외형 대안 | **임시 차량 후보.** 7.5t class를 국내 5톤 적재량 차량과 동일시하면 안 됨. 적재함 내부 치수·문 피벗·바닥은 검증/수정 필요. 페이지의 전체 치수 표기도 서로 달라 실척 검증 필요 |

Poly Haven 자산의 CC0 제공 정책은 [공식 라이선스 페이지](https://polyhaven.com/license)에서 확인했다. Asset Store 에셋은 CC0와 달리 제품별 EULA와 사용 조건에 따른다.

### Unity 연결 도구

| 후보 | 역할 | 판단 |
|---|---|---|
| [Unity URDF Importer](https://github.com/Unity-Technologies/URDF-Importer) | URDF를 Unity 로봇 구조로 임포트, Apache 2.0 | 로컬 예제 manifest에는 v0.5.2를 사용 중. 현재 프로젝트에는 아직 없음. FR5 모델의 단위·축·관절 제한·충돌 형상 확인 필요 |
| [ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector) | Unity와 ROS 메시지 연결, Apache 2.0 | 실기/ROS2 연동 또는 센서 데이터 외부 송수신 시 검토. Unity 단독 공정 시뮬레이션에 항상 필수인 것은 아님 |

### 우선 채택에서 제외하거나 보류한 항목

- **NVIDIA Isaac Sim / SimReady 자산:** 고품질 창고 후보이지만 코드 라이선스와 추가 모델·텍스처 라이선스가 다르다. 이를 Unity에 자유롭게 변환·배포할 수 있는 무료 에셋으로 제시하지 않는다. 별도 데이터셋에 명시된 허용 라이선스가 있으면 개별 검토 가능하다. [공식 라이선스 FAQ](https://docs.isaacsim.omniverse.nvidia.com/6.0.0/common/license-faq.html), [추가 자산 약관](https://www.nvidia.com/en-us/agreements/enterprise-software/isaac-sim-additional-software-and-materials-license/)
- **Sketchfab Hyundai Mighty:** 무료·CC Attribution 표시가 있지만 게시자가 원작이 아니라고 명시한다. 출처 권한과 실제 차급이 불명확하므로 우선 추천하지 않는다. [해당 모델](https://sketchfab.com/3d-models/hyundai-mighty-e1e2309aad024531b4d41184f5edc898)
- **정확한 국내 5톤 탑차:** 이번 조사에서 출처 권한·차급·사용 가능한 적재함까지 명확한 무료 고품질 모델은 확보하지 못했다. 특정 차종을 고르면 제원에 맞춘 적재함 제작과 합법적인 외형 모델 확보가 남는다.
- **완성된 분류기+중력 슬로프:** 무료 모듈 후보는 있지만, 요청한 각도·폭·높이·처리량에 맞는 완성품과 작동 로직을 함께 확인하지 못했다. 치수 기반 제작이 현실적이다.
- **SLAMTEC 드라이버:** 실제 센서 연결용 드라이버만으로 Unity LiDAR 시뮬레이션이나 센서 3D 모델이 확보되는 것은 아니므로 센서 에셋 목록에 넣지 않았다.

## 4. 추천 조합과 남는 제작 범위

현재 프로젝트를 유지하는 출발점으로는 **Robotics Warehouse URP + 공식 FR5 모델 + Schmalz CAD 또는 기존 흡착 외형 + UnitySensors + 별도 AGV 지게차 제어** 조합을 우선 검토한다. FR5와 CAD는 사용권 확인이 남는다. 화면 품질을 최우선으로 정하면 Unity Warehouse HDRP를 기반으로 바꾸는 선택지를 비교할 수 있다. 렌더 파이프라인 전환은 이번 조사에서 실행하지 않았다.

무료 자산으로 확보할 수 있는 부분과 직접 구현할 부분을 구분하면 다음과 같다.

| 확보 가능성이 높은 부분 | 별도 제작·개발이 남는 부분 |
|---|---|
| 창고·선반·기본 물류 소품 | 실제 물류센터 레이아웃과 설비 치수 |
| FR5 기구 모델 | IK, 흡착, 접근/퇴피 경로, 작업 범위 검증 |
| 컨베이어 외형 | 상시 구동, 박스 간격, 분기 타이밍, 버퍼/정체 처리 |
| 흡착 장치 CAD | 박스별 흡착 가능 판정, 성공/실패, 질량 제한 |
| 지게차 외형·URDF 참고 | 조향·포크·파레트 인양·AGV 배차·정밀 접안 |
| LiDAR 패키지 | 장애물 감속/정지/재계획과 물류 제어 연계 |
| 상자·파레트·차량 대체 외형 | 혼합 크기 적재, 2m 제한, 총중량, 상차 순서, 출하 기록 |

Unity 임포트 단계에서는 1 Unity unit = 1m, ROS/Unity 좌표 변환, 관절 축과 제한, TCP, 질량/관성, 단순화한 충돌체, URP 재질, 메시의 가동부 분리와 피벗을 확인해야 한다. STL은 CAD 형상을 확보해도 PBR 텍스처 품질이 따라오는 형식은 아니다. GLB도 별도 임포트 경로나 FBX 변환이 필요할 수 있다.

대량 박스는 이송·슬로프에서 동적 충돌을 사용하고, 적재 완료 후에는 안정화·물리 단순화를 검토하는 것이 적합하다. 벨트에서 슬로프, 흡착, 파레트, AGV, 트럭으로 넘어갈 때 각 박스의 제어 주체와 소속을 명확하게 바꿔 중복 이동·유실·중복 집계를 방지해야 한다.

## 5. SRS 전에 확인이 남은 내용

아래는 결정이 필요한 조사 항목이며 요구사항으로 확정하지 않았다.

1. 참고 SRS 원본 경로는 사용자 안내로 확인 완료. 물류센터 SRS 작성 시작은 별도 지시 대기.
2. 실장비 연동까지 포함하는지, 우선 Unity 내부 시뮬레이션인지. 연동한다면 관절/센서/설비 신호의 데이터 원천과 제어 방향.
3. 컨베이어 분류 레인 수, 출하 도크 수, FR5·AGV 수량과 공유 방식.
4. 박스 규격·질량·색상·바코드 종류와 발생 빈도, 목표 처리량.
5. 가상 속성 판독과 실제 영상 판독 중 목표 수준.
6. FR5 정확한 기종/세대와 설치 위치, 2m 적재를 지원할 승강 장치 여부.
7. 파레트 규격, 2m에 파레트 포함 여부, 혼합 적재와 상부 지지/회전 허용 조건.
8. 파레트 만재 후 빈 파레트 공급과 상시 컨베이어의 정체 해소 방식.
9. 슬로프 형식·경사·재질·끝단 스토퍼, 분기 장치 형식.
10. AGV 지게차 형식과 주행 모델, LiDAR 종류와 측정 수준, 장애물 반응 범위.
11. 5톤 차량 차종·적재함 치수·도크 높이, 파레트 단위 상차 여부, 만재 기준과 차량 교체 정책.
12. URP 유지 또는 HDRP 채택, 목표 PC 사양·동시 박스 수·성능 목표.

## 6. 이번 조사에서 수행한 범위

- 로컬 프로젝트/예제의 설정, 관련 코드와 에셋 존재 확인.
- 사용자 지정 `TeamProject_IFEA-main/Docs/SRS_IFEA.md` 전체 내용 검토와 문서 형식 파악.
- 문서 후보의 텍스트 검토와 공정 구조 페이지 시각 확인.
- 제작자·공식 저장소·Asset Store에서 후보의 공개 여부, 제공 형식, 명시된 라이선스와 렌더 파이프라인 확인.
- FR5 원본 URDF, 지게차 원본 URDF, 관련 package.xml 및 주요 오픈소스 라이선스 확인.
- 이 조사 보고서만 작성. SRS 작성, Unity 코드·씬 변경, 패키지 설치, 에셋 구매/계정 등록, 실제 에셋 임포트는 하지 않음.

참고 SRS 원본 확인은 완료했다. 개별 자산 사용권의 미확정 부분과 Unity 6.6 실제 동작·품질 검증은 남아 있다. 물류센터 SRS 작성은 시작하지 않았다.
