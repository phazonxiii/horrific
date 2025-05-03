using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HorrorEngine
{
    [Serializable]
    public struct WeathervaneSaveData
    {
        public bool IsRotating;
        public bool HasReachedCardinalDirection;
        public float Speed;
        public float EulerAngleY;
    }

    
    public class WeatherVaneController : MonoBehaviour, ISavableObjectStateExtra
    {
        [SerializeField] float m_AnimatorSpeed;
        [SerializeField] float m_MinimumAnimationSpeed;        
        [SerializeField] Transform m_RotationObject;
        [SerializeField] TransformReferences m_TransformReferences;

        //[SerializeField] CardinalDirectionDataSet m_CardinalDirectionData;

        [Space]
        [FormerlySerializedAs("m_RotationStarted")]
        [SerializeField] UnityEvent m_OnRotationStarted;
        [FormerlySerializedAs("m_RotationEnded")]
        [SerializeField] UnityEvent<string> m_OnRotationEnded;

        private TransformRefEntry m_ClosestRefEntry;
        private Animator m_Animator;
        private bool m_HasReachedCardinalDirection = false;
        private bool m_IsRotating = true;

        // --------------------------------------------------------------------

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
            enabled = false;
        }

        // --------------------------------------------------------------------

        void Update()
        {
            if (m_IsRotating)
            {
                m_AnimatorSpeed = Mathf.MoveTowards(m_AnimatorSpeed, 1f, Time.unscaledDeltaTime);
            }
            else if (!m_HasReachedCardinalDirection)
            {
                m_ClosestRefEntry = m_TransformReferences.GetClosestRotation(m_RotationObject.rotation, out float angleDiff);
                const float threshold = 1f;
                if (angleDiff < threshold)
                {
                    m_AnimatorSpeed = 0;
                    m_Animator.speed = m_AnimatorSpeed;
                    
                    m_OnRotationEnded?.Invoke(m_ClosestRefEntry.ID);
                    m_Animator.enabled = false;
                    m_HasReachedCardinalDirection = true;

                    enabled = false;
                }
                else
                {
                    m_AnimatorSpeed = Mathf.MoveTowards(m_AnimatorSpeed, m_MinimumAnimationSpeed, Time.unscaledDeltaTime);
                }
            }

            m_Animator.speed = m_AnimatorSpeed;
        }

        // --------------------------------------------------------------------

        public void SetRotation()
        {
            enabled = true;

            if (m_IsRotating)
            {
                StopRotation();
                return;
            }
            else
            {
                StartRotation();
            }
        }

        // --------------------------------------------------------------------

        private void StartRotation()
        {
            m_IsRotating = true;
            m_HasReachedCardinalDirection = false;
            m_Animator.enabled = true;
            m_OnRotationStarted?.Invoke();
        }

        // --------------------------------------------------------------------

        private void StopRotation()
        {
            m_IsRotating = false;
        }

        //------------------------------------------------------
        // ISavable implementation
        //------------------------------------------------------

        public string GetSavableData()
        {
            WeathervaneSaveData serializedData = new WeathervaneSaveData
            {
                IsRotating = m_IsRotating,
                HasReachedCardinalDirection = m_HasReachedCardinalDirection,
                Speed = m_Animator.speed,
                EulerAngleY = m_RotationObject.rotation.eulerAngles.y
            };
            return JsonUtility.ToJson(serializedData);
        }

        // --------------------------------------------------------------------

        public void SetFromSavedData(string savedData)
        {
            WeathervaneSaveData serializedData = JsonUtility.FromJson<WeathervaneSaveData>(savedData);
            m_HasReachedCardinalDirection = serializedData.HasReachedCardinalDirection;
            m_IsRotating = serializedData.IsRotating;
            if (serializedData.Speed < m_MinimumAnimationSpeed)
            {
                m_Animator.enabled = false;
            }
            m_Animator.speed = serializedData.Speed;
            Quaternion targetRotation = Quaternion.Euler(0f, serializedData.EulerAngleY, 0f);
            m_RotationObject.rotation = targetRotation;
        }
    }
}
