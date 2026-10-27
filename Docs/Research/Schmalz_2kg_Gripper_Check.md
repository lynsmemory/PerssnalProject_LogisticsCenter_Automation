# 앞서 제시한 Schmalz 제품의 2kg 박스 적합성

확인일 2026-10-27. 대상 박스는 SRS 초안의 360×280×340mm, 2kg이다.

## 판단

앞서 외형과 CAD 후보로 제시한 ROB-SET ECBPMi FAIRINO를 기본 흡착컵 그대로 2kg 박스용으로 확정하지 않는다. 소형 SLG 102x75.3 FSGA 4도 이번 하중에는 부족하다.

| 대상 | 제조사 수치 | 판단 |
|---|---|---|
| ROB-SET ECBPMi FAIRINO, 10.03.01.00987 | 진공 발생기 ECBPMi, 최대 흡입 유량 1.6L/min, 최대 진공도 60%. 교체용 컵은 SPB1 30과 SFF 20으로 안내 | 로봇 장착용 세트라는 사실만으로 2kg 골판지 박스 취급이 보장되지 않음 |
| SPB1 30 ED-65, 10.01.06.04530 | -600mbar에서 이론 흡착력 20.7N. 건조하고 매끈하며 평평한 표면, 안전계수 미포함 | 2kg 무게 약 19.6N과 차이가 작음. 가속도와 안전계수, 누설을 고려한 여유 부족 |
| SLG 102x75.3 FSGA 4, 10.01.10.14041 | -0.6bar에서 최대 이론 유지하중 1.88kg, 안전계수 미포함 | 2kg보다 작아서 제외 |

소형 SLG의 컵은 문서상 FSG 20이다. 앞서 계산에 사용한 지름 50mm 컵 4개와 같은 제품이 아니다. 진공 발생기의 취급하중 안내, 실제 흡착컵의 힘, 박스 표면 누설은 각각 구분해야 한다.

## 이전 경로 검사의 의미

FR5 경로 검사는 별도로 정한 125mm 길이의 단순 툴과 190×140mm 헤드를 사용했다. Schmalz 제품의 실제 조립 CAD를 넣은 검사가 아니며 진공 유지, 골판지 누설, 손목 하중 및 가속도 시험도 수행하지 않았다. 따라서 경로 검사를 통과했다는 이유로 해당 Schmalz 제품을 사용할 수 있다고 판단하지 않는다.

2kg 골판지 박스용 흡착 장치는 더 큰 컵을 여러 개 사용하는 구성과 박스의 누설을 감당할 진공원으로 다시 선정해야 한다. 실제 모델을 정하면 길이와 폭, 질량에 따라 기존 충돌 경로도 다시 확인한다. 현재 SRS는 Schmalz 특정 제품을 채택한 상태가 아니다.

## 제조사 자료

## FR5 장착용 재선정 후보

2026-10-27 추가 조사. 우선 검토할 구성은 ROB-SET ECBPi FAIRINO(10.03.01.00988)에 VEE 4xSPB1/SPF 218.3(10.01.36.00269)와 SPB1 40 ED-65 G1/4-AG(10.01.06.03498) 흡착컵 4개를 사용하는 것이다. VEE 4컵 헤드는 세트의 선택 액세서리로 안내된다. 기본 세트에 이 헤드가 포함된다고 단정하지 않는다. 컵 중복 구매 여부도 구성표 확인이 필요하다.

제조사의 현재 표기는 Fairino Europe FR3-30이며, 이전 지역별 제품 안내에는 FR3, FR5, FR10이 명시되어 있다. 세트에는 장착 플랜지 FLAN-PL, VEE-QCM, 진공 발생기 ECBPi, VEE 헤드, ASK-B 케이블이 포함된다. 호환 안내는 유럽 표준 모델 기준이므로 사용하는 FR5의 플랜지 도면과 전기 연결은 대조해야 한다.

SPB1 40의 이론 흡착력은 컵당 35.9N이다. -600mbar에서 건조하고 매끈하며 평평한 표면을 기준으로 하며 안전계수는 포함하지 않는다. 4컵이 모두 같은 진공을 유지하고 하중을 나눈다고 가정하면 합계 143.6N이다. 박스 2kg의 정지 무게는 19.62N이며, 설계 검토용 안전계수 2를 적용한 필요 힘은 39.24N이다. 상향 가속도 2m/s²를 가정하면 필요 힘은 2×2×(9.81+2)=47.24N이다. 이 조건에서 계산상 힘에는 여유가 있다. 이 수치를 골판지의 실제 허용 하중으로 사용하지 않는다. 테이프 경계, 접힌 부분, 골판지 누설, 컵 일부 미접촉과 편심 하중은 별도 확인 대상이다.

ECBPi는 최대 흡입 유량 12L/min이며 제조사는 밀폐 표면과 약한 다공성 표면용으로 안내한다. 박스에서 -600mbar가 유지되어야 위 계산을 사용할 수 있다. ECBPi 12 본체 질량은 775g이다. 플랜지와 헤드, 컵과 케이블을 포함한 최종 조립 질량은 확인되지 않았으므로 FR5의 잔여 가반하중을 확정하지 않는다. 박스와 최종 툴의 총질량, 무게중심과 관성까지 확인해야 한다.

세트 도면의 높이 H는 188.8mm, 지름 D는 151.5mm이다. 선택한 4컵 헤드의 최종 조립 길이와 TCP 위치는 조립 CAD로 확인해야 한다. 이전 125mm 단순 툴보다 길어질 수 있으므로 기존 3단 적재 경로 검증 결과를 그대로 적용하지 않는다. 현재 SRS의 툴 치수나 로봇 설치 높이는 이번 조사만으로 변경하지 않았다.

공식 제품 페이지에 TraceParts CAD 다운로드가 제공된다. CAD 다운로드 경로의 존재를 확인했으며 STEP 파일 자체는 아직 확보하지 않았다.

- [ROB-SET ECBPi FAIRINO와 CAD 진입점](https://www.schmalz.com/en/products/automation-743270/vacuum-grippers-746238/gripping-systems-and-end-of-arm-tooling-eoat-746240/handling-sets-312463/handling-sets-ecbpi-308297/10.03.01.00988)
- [세트 도면과 호환 액세서리 데이터시트](https://media.schmalz.com/MAM_Library/Dokumente/Datenblatt_Artikel/1/100/10030100988/e1b7676725ca_Datasheet_Article_10.03.01.00988_en-EN.pdf)
- [FR5가 명시된 지역별 제품 안내](https://www.schmalz.com/pl-pl/produkty/technologia-podcisnienia-w-automatyce-301607/komponenty-podcisnienia-301608/generatory-podcisnienia-307617/elektryczne-generatory-podcisnienia-738973/elektryczne-generatory-podcisnienia-montowane-na-koncu-ramienia-739374/zestawy-do-obslugi-ecbpi-308297/10.03.01.00988)
- [SPB1 40 흡착력과 적용 조건](https://media.schmalz.com/MAM_Library/Dokumente/Datenblatt_Artikel/1/100/10010603498/89c023f16298_Datasheet_Article_10.01.06.03498_en-EN.pdf)
- [ECBPi 본체 질량과 전기 사양](https://www.schmalz.com/en-de/products/automation-743270/vacuum-generators-307617/electric-vacuum-generators-738973/electric-vacuum-generators-end-of-arm-739374/vacuum-generators-ecbpi-308294/10.03.01.00314)
- [ECBPi 적용 가능한 표면](https://media.schmalz.com/MAM_Library/Dokumente/Datenblatt_Produktfamilie/0_/054/05463/6016e9fe8923_Datasheet_Vacuum%20Generators%20ECBPi_en-EN.pdf)

## 이전 후보의 제조사 자료

- [ROB-SET ECBPMi FAIRINO](https://www.schmalz.com/de/produkte/automation-743270/vakuum-greifer-746238/greifsysteme-und-end-of-arm-toolings-eoat-746240/handling-sets-312463/handling-sets-ecbpmi-308307/10.03.01.00987)
- [SPB1 30 ED-65 흡착력과 조건](https://www.schmalz.com/de/produkte/automation-743270/vakuum-greifer-746238/vakuum-sauggreifer-301609/balgsauggreifer-rund-302817/balgsauggreifer-spb1-15-falten-304445/10.01.06.04530)
- [SLG 기술자료 3쪽](https://media.schmalz.com/MAM_Library/Dokumente/Bedienungsanleitung/30/3030/303001/30300103693/cd5faa4e7ef3_BAL_30.30.01.03693_en-EN.pdf)

## 케이블을 제외한 질량 합산 확인

툴 질량은 구성품 질량을 합산해 구한다. 이번 조립은 ECBPi 본체, FR5 장착 플랜지와 체결부, VEE-QCM, 벨로즈 4컵 헤드다. 4컵 헤드의 전체 질량을 사용하면 흡착컵을 다시 더하지 않는다. 기존 기본 1컵 툴은 제거했으므로 합계에 포함하지 않는다.

공식 자료로 확인된 값은 ECBPi 본체 775g과 SPB1 40 컵 21.2g×4=84.8g이다. 이 두 항목의 소계는 859.8g이다. 박스 2kg을 더하면 확인된 항목의 소계는 2859.8g이지만, 이것을 최종 탑재 질량으로 쓰지 않는다. 여기에는 헤드 몸체, FR5 장착 플랜지, 연결 모듈과 필요한 체결부가 빠져 있다.

4컵 헤드 10.01.36.00269의 공식 데이터시트는 직접 내려받아 확인했으나 질량 항목이 없다. 받은 STEP와 TXT에도 질량과 재질 밀도 항목이 없다. CAD 체적에 임의 재질 밀도를 곱하지 않았다. 따라서 합산 방법은 확정했지만 전체 질량은 아직 수치로 확정하지 않았다.

- [ECBPi 본체 775g 공식 데이터시트](https://media.schmalz.com/MAM_Library/Dokumente/Datenblatt_Artikel/1/100/10030100314/bf3c0b7678f4_Datasheet_Article_10.03.01.00314_en-EN.pdf)
- [4컵 헤드 공식 데이터시트](https://media.schmalz.com/MAM_Library/Dokumente/Datenblatt_Artikel/1/100/10013600269/2b9885202e3d_Datasheet_Article_10.01.36.00269_en-EN.pdf)
