# 다른 PC에서 프로젝트 열기

1. Git과 Git LFS를 설치하고 `git lfs install`을 실행한다. GitHub 비공개 저장소에 접근 가능한 계정으로 인증한다.
2. 저장소를 clone하거나, 기존 복사본에서 작업 브랜치를 fetch/checkout한다. 오늘 작업 브랜치는 `feature/LYN-scene-2026-10-28`이다. 기존 작업이 있으면 먼저 보존한다.
3. 체크아웃한 브랜치에서 `git lfs pull`을 실행한다. `git lfs status`와 에셋 실제 파일을 확인한다. 대용량 에셋이 LFS 포인터 텍스트로 남아 있으면 Unity를 열기 전에 다운로드 문제를 해결한다.
4. Unity Hub에서 Unity **6000.6.0f1**로 프로젝트 루트를 연다. 패키지 복원과 최초 임포트에는 시간이 필요하다. Library·Temp·Logs·UserSettings는 새 PC에서 재생성한다.
5. `Assets/Scenes/LogisticCenter_Automation.unity`를 연다. `Assets/Scenes/urp_scene_with_training/` 폴더는 이 씬의 구운 조명 데이터이므로 폴더 이름만 보고 삭제하거나 이동하지 않는다.
6. AGENTS.md → Docs/Collaboration/Claude_Handoff.md → Pallet_Prefab_Result_2026-10-28.md → WBS/SRS 순서로 확인한다. WBS와 SRS가 일정·기능의 기준이다.

## Claude 연결

프로젝트 파일을 Git으로 옮겨도 CLI 설치·로그인·실행 중인 프로세스·기존 세션은 이전되지 않는다. 새 PC에서 Claude Code CLI와 로그인 상태를 별도로 확인해야 한다. 이전 PC의 `C:/Users/LYN/.local/bin/claude.exe` 경로를 그대로 가정하지 않는다. 로그인 정보나 토큰을 저장소에 넣지 않는다. 사용자 화면 조작은 로그인·권한 단계에 필요한 경우에만 요청한다.

스크립트는 Claude, 씬·Prefab·재질·검수는 Codex가 담당한다. 10/28 파레트는 사용자가 Codex 직접 제작을 별도 지시한 결과이며 일반 스크립트 역할 변경이 아니다. 직접 전달이 확인되지 않은 인계 문서를 연동 완료로 보고하지 않는다.

## 경로와 재실행 주의

- 문서의 이전 PC 절대 경로는 새 프로젝트 루트에 맞춰 해석한다. Temp·Codex 캐시·렌더용 임시 Unity 프로젝트는 옮겨지지 않는다.
- 상위 폴더의 `Cell_Factory-main`은 이 저장소 밖의 별도 참고 프로젝트다. 필요하면 새 PC에서 별도로 준비하며, 없을 때 있다고 가정하지 않는다.
- FR5 공식 URDF와 STL 7개는 `Docs/Research/CAD_References/FR5/fairino_description/`에 있다. 원본 확보와 Unity 임포트 완료를 구분한다.
- `ConveyorLayoutBuilder.Build`는 ConveyorSystem을 삭제 후 재생성한다. `ViewCameraBuilder.Build`도 ViewCameras를 재생성한다. 확인 없이 Build를 실행해 사용자 배치를 덮어쓰지 않는다.
- 5톤 차량은 사용자 지정 게이트 3·5·7·10·12·13·14·15의 8대다. 공정 도크가 3곳이라는 이유로 나머지를 임의 삭제하지 않는다.
- `Pallet_1100x1100x150.prefab`은 완성 에셋으로 저장되었지만 아직 셀에 배치하지 않았다. 인양·적재 시험은 미실행이다.
