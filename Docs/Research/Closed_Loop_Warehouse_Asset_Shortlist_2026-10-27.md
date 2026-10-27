# 타원형 순환·3레인 물류센터 에셋 추가 조사

확인일: 2026-10-27. SRS 또는 에셋 설치 결과가 아닌 후보 조사다.

## 국내 제품·쿠팡 공급 사례 추가 확인

분류 레인 하강 구간은 사용자 지시에 따라 **무동력 경사 롤러 컨베이어**를 사용한다. 구동 벨트 컨베이어나 판형 슈트를 최종 대체품으로 제안하지 않는다. 메인 순환 라인의 구동과 레인 중력 이송을 구분한다.

| 국내 업체·제품군 | 쿠팡 관련 확인 근거 | 이번 프로젝트의 활용 |
|---|---|---|
| TXR로보틱스(구 태성시스템) — PIVOTLE 휠소터·FULFILIO 틸트트레이 | [2026년 사업보고서](https://kind.krx.co.kr/external/2026/03/19/000322/20260319001651/11011.htm)에 쿠팡 광주3FC·경기광주5FC·광주2FC·북천안2FC 소터 사업 및 일부 캠프의 틸트트레이 운영을 기재. [유진기업 공식 소식](https://eugenecorp.co.kr/media/news_view.php?index=1&page=50)도 태성시스템의 쿠팡 고객 관계와 제품군 안내 | 분기 장치·스캐너·물류 설비 외형 참고. 공시의 구체 사업과 모든 제품 모델을 일대일 대응시키지는 않음. 틸트트레이는 현재 계획의 고정 롤러 레인과 다른 기구이므로 자동 채택하지 않음 |
| 대화콘베어 — 자바라·사다리형 롤러·롤러카페트 | [업체의 쿠팡 납품 사례](https://www.dhconveyor.com/review/?bmode=view&idx=166272256)는 알루미늄 자바라 제품, 롤러폭 600mm, 길이 5m/7.5m를 명시. 검색 색인으로 확인했으며 본문 직접 요청은 접근 제한. [공식 경기지사](https://idaehwa.co.kr/)는 무구동 물류 이송 설치와 롤러 제품 안내 | 무동력 레인의 프레임·롤러·높이 조절 다리·가이드 외형에 유력한 국내 참고. 쿠팡 납품 확인 제품은 자바라형이므로 다른 고정 롤러 제품까지 쿠팡 사용품이라고 단정하지 않음 |
| 가치소프트 — 스캐너·소터·싱귤레이터 | [업체 연혁](https://www.gachisoft.com/company/history)에 쿠팡 고양·시흥·창원 관련 공급 내역 안내. 검색 색인 확인, 본문 직접 요청은 서버 오류 | QR 판독 구간·정렬 장치의 인접 참고. 경사 롤러 제조사나 URDF 공급사로 제시하지 않음 |

위 자료는 실제 장비와 납품 사례에 대한 자료다. 해당 업체의 Unity용 무료 에셋, 공개 URDF 또는 자유롭게 사용할 수 있는 전체 STEP/FBX 패키지는 이번 조사에서 확인하지 못했다. 외형 참고 자료와 가져다 쓸 수 있는 3D 파일은 구분하며, 상세 CAD 제공과 사용 조건은 업체 확인이 필요하다. 업체에 연락하거나 자료 제공을 요청하지 않았다.

중력 레인은 경사·박스 바닥 길이·질량·롤러 간격·구름 저항·속도 제한·스토퍼를 함께 검토한다. 기존의 구동 벨트 속도 부여 방식으로 중력 롤러를 구현했다고 표현하지 않는다. 단순 미끄럼 마찰만 사용하면 실제 롤러 이송과 달라질 수 있어 물리 모델을 구분한다.

## 외형 선정 기준 변경: 사실적인 산업 설비

사용자 선호에 따라 저면수 단순 색상 스타일보다 실제 창고·컨베이어·로봇의 형상과 PBR 재질 표현을 우선한다. Parcel Depot과 Kenney는 앞선 구조 검토·프로토타입 후보로 유지하되 최종 외형의 우선 후보에서는 내린다. 사실적인 외형을 위해 반드시 HDRP로 바꿔야 하는 것은 아니며, 현재 URP를 우선 유지한다. 패키지의 홍보 이미지와 Unity 6.6 URP 임포트 결과가 같다고 보장하지 않는다.

| 대상 | 우선 검토 | 조건 |
|---|---|---|
| 무료 창고 | [Unity Warehouse](https://assetstore.unity.com/packages/3d/environments/industrial/unity-warehouse-276394) | 공식 포토리얼 데모. 원본 HDRP라 URP 재질·조명 조정 필요. 기존 Cell_Factory 예제의 환경 자산도 먼저 대조 |
| 유료 URP 창고 대안 | [Modular Warehouse AW](https://assetstore.unity.com/packages/3d/environments/industrial/modular-warehouse-aw-310207) | 조사 시점 $20, PBR, URP/HDRP 제공. 홍보 스크린샷은 HDRP임을 명시. 사용자가 구매를 승인한 것은 아님 |
| 유료 창고 소품 대안 | [Warehouse Props Vol. 1](https://assetstore.unity.com/packages/3d/props/warehouse-props-vol-1-258744) | 조사 시점 $39.99, URP 지원 표시. [제작자 페이지](https://www.cyberneticwalrus.com/our-assets/)는 62개 메시·71개 프리팹 안내. 상세 컨베이어 구성은 미검증 |
| 실제 곡선 컨베이어 | [mk KGF-P 2040](https://www.mk-group.com/en/products/conveyor-technology/belt-conveyors/kgf-p-2040.html) | 실제 제조사의 90°/180° 곡선 제품, CAD ZIP 제공. 타원형 끝단의 형상 참고·변환 후보. CAD 파일 형식·재사용 약관 확인 및 Unity용 메시 경량화 필요 |
| 실제 모듈러 곡선 컨베이어 | [mk KMF-P 2040](https://www.mk-group.com/en/products/conveyor-technology/modular-belt-conveyors/kmf-p-2040.html) | L/S/U 경로와 CAD 제공. 체인/모듈러 벨트 외형 선택지이며 요청한 고무 벨트와 동일한 외형은 아님 |
| 로봇암 | FAIRINO 공식 FR5 URDF·STL | 실제 기구 형상 기반. 재질 보강과 흡착 장치 추가, 사용권 확인 필요 |

제조사 CAD가 다운로드 가능하다는 사실을 무료 게임 에셋 또는 자유 재배포 권한과 동일시하지 않는다. 상세 형상은 CAD를 참고하되 단순화된 외형을 별도 모델링하는 선택지도 있다. 컨베이어·슬로프의 외형과 물류 이송·중력 동작은 분리해서 구현한다.

로컬 `/home/lyn/Unity/Examples/Cell_Factory-main`에는 벽·바닥·천장·랙·박스·파레트·차량 FBX와 환경 프리팹이 존재하며 Unity 버전도 현재 프로젝트와 같은 6000.6.0f1이다. 따라서 새 자산 도입 전에 기존 환경의 외형·재질 상태와 취득 권한을 확인하는 것이 우선이다. 원 출처 또는 현재 재질 품질은 파일명만으로 확정하지 않았다.

시각 구성 목표는 산업용 금속 프레임·고무 벨트·측면 가이드·센서·안전 펜스·바닥 라인·도크·조명을 일관되게 구성하는 것이다. 나사·모터 같은 장식 상세는 필요한 수준으로 제한하고, 충돌체는 외형보다 단순하게 둔다. ML/QR 관측 구역은 외형 연출과 함께 고정 조명·노출·판독 선명도를 별도로 검증한다.

## 조사 대상

타원형 메인 컨베이어 상시 순환 → QR 이미지 판독 → 3개 분류 레인·중력 슬로프 → 레인별 FR5와 파레트 → AMR 지게차 운반·차량 상차. 포화 레인 목적 박스는 재순환하고, QR 라벨이 없는 박스는 총 2회 미검출 후 포장작업대로 반송해 종료한다. ML은 슬로프 끝 박스의 위치·회전 인식에 사용한다.

## 결론

요청한 구조와 공정 제어까지 모두 갖춘 Unity 완성형 무료 패키지는 이번 조사에서 확인하지 못했다. **형상이 가장 가까운 모듈 팩과 FR5 URDF를 조합하고, 공정 제어를 별도로 구현하는 방식**이 현실적이다.

URDF는 이 프로젝트에서 주로 로봇의 링크·관절·기구 구조를 가져오는 용도로 사용한다. 창고 전체의 외형과 QR·분기·재순환·ML·배차 기능은 별도 씬과 스크립트로 구성해야 한다.

## 우선 후보

| 후보 | 비용·라이선스 | 확보할 수 있는 부분 | 추가 작업과 확인 사항 |
|---|---|---|---|
| [Parcel Depot and Sorting Hall](https://3dassets.dev/packs/parcel-depot-and-sorting-hall) | 무료, CC0. GLB 53개 | 직선·90도 곡선 컨베이어, 분기 암, 중력 롤러, 슈트, 스캐너 아치, 창고·도크·지게차 소품 | **구조상 가장 가까운 외형 후보.** AI 생성 자산. 제작자 설명상 콜라이더·이송·내비게이션·게임 로직은 사용자가 추가. 곡선과 직선 조합으로 타원형 레이아웃을 구성할 수 있으나 이는 조립 제안이며 검증된 해당 레이아웃은 아님 |
| [Unity Robotics Warehouse](https://github.com/Unity-Technologies/Robotics-Warehouse) | 무료, Apache 2.0 | URP/HDRP용 로봇 시뮬레이션 창고 환경 | 현재 URP 환경의 대안. 구버전 패키지 의존성 정리와 Unity 6.6 검증 필요. 요청한 분류·슬로프·FR5 공정이 완성되어 있는 것은 아님 |
| [Unity Warehouse](https://assetstore.unity.com/packages/3d/environments/industrial/unity-warehouse-276394) | 무료, Standard Unity Asset Store EULA | 포토리얼 창고·랙·물류 환경 | 시각 품질 우선 후보. 원본은 HDRP용이며 현재 URP와 직접 호환되지 않음. 재질·조명 재구성 또는 렌더 파이프라인 선택 필요 |
| [Kenney Factory Kit](https://kenney.nl/assets/factory-kit) | 무료, CC0 | 컨베이어·공장 소품, 단순화된 스타일 | 기능 프로토타입용 대안. 현재 배포본의 곡선·슈트 개별 구성과 포맷은 아카이브 확인 전이므로 보장하지 않음 |
| [FAIRINO 공식 모델](https://github.com/FAIR-INNOVATION/frcobot_ros2/tree/main/fairino_description) | 공개 다운로드, description 라이선스 선언 미완성 | FR5 URDF·STL, 링크·관절·질량·제한 | FR5 3대를 구성할 기구 모델 후보. 라이선스 확인, 정확한 기종 선택, 플랜지/TCP·흡착 툴 추가, 충돌·IK·경로 검증 필요 |
| [forklift_robot](https://github.com/renan028/forklift_robot) | 공개 다운로드, description package.xml Apache 2.0 선언 | 단순 지게차 URDF·DAE, 포크 승강 관절 | 기구 구조 참고용. 메시별 권리 확인과 URDF 보정 필요. Unity에서 자동으로 AMR 주행·LiDAR·포크 제어가 작동하는 것은 아님 |
| [Warehouse Pack Free](https://assetstore.unity.com/packages/3d/environments/warehouse-pack-free-low-poly-warehouse-essentials-407032) | 무료, Standard Unity Asset Store EULA, URP | 포장작업대, 박스, 파레트, 랙, 도크 문·범퍼 | 반송 수거 지점 외형 후보. 저면수 스타일. 제공 파레트를 지정한 1,100×1,100×150mm 플라스틱 제품과 동일시하지 않음 |

### Parcel Depot 팩이 특히 가까운 이유

공식 페이지에서 2m/4m 직선, 중심선 반경 1m의 90도 곡선, 측면 배출용 분기 암, 중력 롤러와 슈트를 확인했다. 직선과 곡선의 벨트 높이는 0.76m로 안내되어 있다. 슈트는 우편물 수거 자루 프레임으로 이어지는 형태이므로, 로봇 픽업 지점으로 쓰려면 끝단을 수정해야 한다.

일부 모델의 롤러·분기 암·문·포크에는 노드 애니메이션이 있지만, 박스를 실제로 운반하거나 분류하는 제어 기능은 아니다. GLB 임포트 도구 또는 FBX 변환이 필요할 수 있고, 조립 간격·메시 품질·콜라이더·성능은 아직 Unity에서 확인하지 않았다.

## 운반 기능 후보와 유료 대안

- [realvirtual.io Starter](https://realvirtual.io/en/demo/): 공식 무료 Starter에서 드라이브·센서·운반 기본 프레임워크 제공을 안내한다. [TransportSurface](https://doc.realvirtual.io/components-and-scripts/motion/transportsurface)는 물리 이송·직선/회전 이동의 기능 후보다. 전용 곡선 컨베이어 생성기나 고급 경로/IK 기능이 무료 Starter에 모두 포함된다고 단정하지 않는다. Simulation/Professional은 별도 유료 제품이므로 판본을 구분한다.
- [Conveyor Systems — Belts, Rollers, Turntables and Sorters](https://heyheythere.itch.io/conveyor-systems): 유료, Unity 6·Built-in/URP, Rigidbody 운반·센서·푸셔·분류 예제를 안내한다. 타원형 곡선과 요청한 전체 공정은 확인하지 못했다. AI 보조 제작 고지가 있으며, 자산 원본 재배포 금지 조건을 안내한다. 이번 조사에서는 구매·설치하지 않았다.
- [Warehouse Props Vol. 1](https://assetstore.unity.com/packages/3d/props/warehouse-props-vol-1-258744): 유료, 표준 Asset Store EULA, Built-in/URP/HDRP 호환 표시. 사실적인 창고 소품 외형 대안이다. 현재 원본 페이지에서 상세 설명을 가져오지 못했으므로 외부 재배포 사이트의 컨베이어 세부 목록을 확정 근거로 사용하지 않았다.

## 공정 구성이 가까운 참고 데모

[RobotecAI ROSCon2023Demo](https://github.com/RobotecAI/ROSCon2023Demo)는 컨베이어 공급·여러 로봇암의 파레타이징·AMR 파레트 운반을 연결한다. 작은 데모 레벨도 컨베이어 4개와 로봇암 4개로 구성된다고 안내한다.

다만 Unity가 아닌 O3DE용이고, 로봇암은 FR5 대신 UR20, 운반체는 OTTO 계열을 모사한 AMR이다. 박스 위치는 정답 상태값으로 읽는 방식이라 현재 계획한 ML 인식 검증을 제공하지 않는다. 요청한 타원 순환·QR 미부착 반송·5톤 차량 상차가 완성되어 있다고 판단하지 않는다.

[라이선스 파일](https://github.com/RobotecAI/ROSCon2023Demo/blob/main/LICENSE.txt)은 프로젝트 코드와 3D 자산을 구분하며, 자산에 CC BY-NC 4.0을 명시한다. 전체가 상업용으로 자유로운 Apache/MIT 에셋 팩이라고 제시하지 않는다. 현재 프로젝트에는 배치·공정 참고 자료로만 추천한다.

## 현재 프로젝트에 적용할 조합

1. 외형 검증 우선: Parcel Depot의 직선·곡선·분기·슈트를 임시 씬에서 확인.
2. FR5는 공식 URDF 후보를 사용하고, 베이스 높이 900mm·흡착 툴·파레트와 픽업 위치를 별도 검증.
3. 1,100×1,100×150mm 플라스틱 파레트와 높이 200mm 박스는 지정 치수로 준비. 오류 박스는 같은 박스에서 QR 라벨만 생략.
4. AMR 지게차는 외형과 별도로 지도 기반 주행·장애물 대응·포크·상차를 구성.
5. 타원형 순환, 3개 독립 레인 제어, 재순환 수 제한, QR 판독, 미검출 이력·포장작업대 반송, ML 인식과 IK 연결은 직접 구현.

초기 목록 작성 당시에는 페이지·저장소·로컬 파일만 확인했으며 다운로드·임포트는 하지 않았다. 이후 실제 파일 확보 결과는 아래 추가 조사에 기록했다. 기존 예제의 realvirtual 컨베이어·흡착 프리팹도 재사용 후보로 유지하되 사용 판본과 권리를 확인한다. SRS는 작성하지 않았다.

## 추가 조사: 실제 CAD·URDF 확보

실제 원본을 요구한 후 Syskomp 제조사 CAD ZIP(95,460,921 bytes, STEP 39개)과 FAIRINO FR5 URDF 및 연결 STL 7개를 내려받았다. 자세한 구성·검증·권리 상태는 [CAD_References/README.md](CAD_References/README.md)에 기록했다.

실사 외형의 컨베이어 기반은 현재 확보한 제조사 STEP를 우선 검토한다. 다만 ZIP의 직선·곡선은 구동/구역제어형 모델이므로 중력 레인은 구동부 제외·롤러 배치·경사·스토퍼 설계가 필요하다. 국내 업체/쿠팡 특정 제품의 CAD 또는 무동력 완제품 STEP를 확보했다고 주장하지 않는다. Unity 임포트와 SRS 작성은 아직 하지 않았다.
