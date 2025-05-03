using UnityEngine;
using System.Collections.Generic;

namespace HorrorEngine
{
    public class PlayerMovementAlternate : MonoBehaviour, IPlayerMovementSettings
    {
        [SerializeField] AnimationCurve m_RotationSpeedOverAngle = AnimationCurve.Linear(0, 0, 360, 1384);
        [SerializeField] float m_MinInputMovementThreshold = 0.5f;
        [SerializeField] float m_MinInputRotationThreshold = 0.15f;
        //[SerializeField] float m_MinAngleRotationThreshold = 10f;
        [SerializeField] float m_InputUnlockAngleThreshold = 15f;
        [SerializeField] float m_AimingRotationSpeed = 180f;

        [SerializeField] List<ActorState> m_TankRotationStates;

        private Behaviour m_LockedCam;
        private Behaviour m_LastRefreshedCam;
        private Vector2 m_LockedInput;
        private ActorStateController m_StateController;

        private void Awake()
        {
            m_StateController = GetComponent<ActorStateController>();
        }

        public PlayerMovementType GetMovementType()
        {
            return PlayerMovementType.Alternate;
        }

        public float GetFwdRate(PlayerMovement movement)
        {
            Vector3 moveAxis = movement.InputAxis;
            if (moveAxis.magnitude > 1f)
                moveAxis.Normalize();

            Vector3 movementDir = CalculateDirFromCamera(movement.transform.position, moveAxis);
            Debug.DrawLine(movement.transform.position, movement.transform.position + movementDir, Color.magenta);
            return Vector3.Dot(movement.transform.forward, movementDir);
        }

        public float GetRightRate(PlayerMovement movement)
        {
            return 0f;
        }

        public void GetRotation(PlayerMovement movement, out float sign, out float rate)
        {
            if (m_TankRotationStates.Contains((ActorState)m_StateController.CurrentState))
            {
                if (Mathf.Abs(movement.InputAxis.x) < m_MinInputRotationThreshold)
                {
                    sign = 0f;
                    rate = 0f;
                    return;
                }

                sign = movement.InputAxis.x;
                rate = m_AimingRotationSpeed;
                return;
            }
            else
            {
                sign = 0;
                rate = 0;

                if (movement.InputAxis.magnitude < m_MinInputRotationThreshold)
                    return;

                Vector3 movementDir = CalculateDirFromCamera(movement.transform.position, movement.InputAxis);
                float signedAngle = Vector3.SignedAngle(movement.transform.forward, movementDir, Vector3.up);
                //if (Mathf.Abs(signedAngle) > m_MinAngleRotationThreshold)
                {
                    sign = Mathf.Sign(signedAngle);
                    rate = m_RotationSpeedOverAngle.Evaluate(Mathf.Abs(signedAngle));
                }
            }
        }

        private Vector3 CalculateDirFromCamera(Vector3 playerPos, Vector2 input)
        {
            // Ensure locked input is some direction for angle calculation
            if (m_LockedInput == Vector2.zero)
                m_LockedInput = Vector2.up;

            bool inDifferentCam = false;
            var cam = CameraSystem.Instance.ActiveCamera;
            // Refresh locked input when entering a different camera to prevent an early change
            if (cam && cam != m_LastRefreshedCam)
            {
                m_LockedInput = input;
                m_LastRefreshedCam = cam;
                inDifferentCam = true;
            }

            // Update cam if the stick when the input stops or changes too much
            float angle = Vector2.Angle(input, m_LockedInput);
            if (input.sqrMagnitude < m_MinInputMovementThreshold || angle > m_InputUnlockAngleThreshold)
            {
                m_LockedCam = m_LastRefreshedCam;
                m_LockedInput = input;
                inDifferentCam = false;
            }

            // No movement if input is not enough
            if (input.magnitude < m_MinInputMovementThreshold)
                return Vector3.zero;

            Transform inputCam = m_LockedCam ? m_LockedCam.transform : cam?.transform;
            if (inputCam)
            {
                Vector3 fwd = transform.position - inputCam.position;
                fwd.y = 0;
                fwd.Normalize();
                Vector3 right = -Vector3.Cross(fwd, Vector3.up);
                right.Normalize();

                Debug.DrawLine(transform.position, transform.position + fwd, Color.blue);
                Debug.DrawLine(transform.position, transform.position + right, Color.red);

                // Use locked input when in different cam to avoid incorrect small rotation
                Vector2 fixedInput = inDifferentCam ? m_LockedInput : input;
                Vector3 movementDir = right * fixedInput.x + fwd * fixedInput.y;

                return movementDir;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}