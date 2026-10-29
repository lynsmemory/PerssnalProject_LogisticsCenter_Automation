using UnityEngine;

namespace Logistics
{
    // FR5 팔의 관절을 움직인다. (FR-10)
    // 지금은 시작·초기화 때 홈 자세로 즉시 배치하는 부분만 있다. IK 계산(SolveDownwardTool)과 이동(MoveTo)은 이후 작성한다.
    // 로봇 루트(fairino5_v6_robot)에 레인마다 1개 붙인다.
    [DisallowMultipleComponent]
    public class RobotArmInverseKinematics : MonoBehaviour
    {
        #region 1. 연결과 설정
        [Tooltip("URDF 관절 j1~j6 순서의 링크 (shoulder_link, upperarm_link, forearm_link, wrist1_link, wrist2_link, wrist3_link)")]
        [SerializeField] private ArticulationBody[] joints = new ArticulationBody[6];

        [Tooltip("홈 자세 관절 각도 (도, URDF 부호). 팔을 세우고 툴이 아래를 향하는 자세")]
        [SerializeField] private float[] homeJointAngles = { 0f, -90f, -90f, -90f, 90f, 0f };
        #endregion

        #region 2. 시작과 초기화
        // 시작 때 홈 자세로 즉시 배치한다. 0도 자세는 팔이 수평으로 뻗어 레인과 겹치므로 그 자세에서 움직이게 두지 않는다.
        private void Start()
        {
            ResetToInitialState();
        }

        // 초기화 버튼: 관절을 홈 자세로 즉시 옮기고 구동 목표도 같은 각도로 맞춘다.
        public void ResetToInitialState()
        {
            for (int i = 0; i < joints.Length && i < homeJointAngles.Length; i++)
            {
                ArticulationBody joint = joints[i];
                if (joint == null)
                {
                    continue;
                }

                //1. 관절 각도와 속도를 바로 바꾼다 (라디안)
                joint.jointPosition = new ArticulationReducedSpace(homeJointAngles[i] * Mathf.Deg2Rad);
                joint.jointVelocity = new ArticulationReducedSpace(0f);

                //2. 구동 목표를 같은 각도로 맞춰 그 자리를 유지한다 (도)
                ArticulationDrive drive = joint.xDrive;
                drive.target = homeJointAngles[i];
                joint.xDrive = drive;
            }
        }
        #endregion
    }
}
