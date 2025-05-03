using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HorrorEngine
{
    public class RotatableObject : MonoBehaviour, ISavableObjectStateExtra
    {
        [FormerlySerializedAs("ObjectToSet")]
        [SerializeField] public Transform m_ObjectToSet;
        [SerializeField] public TransformReferences m_TransformReferences;
        [SerializeField] float m_RotationSpeed;

        [Space]
        [FormerlySerializedAs("m_OnRotationStart")]
        public UnityEvent OnRotationStart;
        [FormerlySerializedAs("m_OnRotationEnd")]
        public UnityEvent OnRotationEnd;

        private TransformRefEntry m_TransformRef;

        public string TransformRefID { get { return m_TransformRef.ID; } }

        // --------------------------------------------------------------------

        private void Start()
        {
            m_TransformRef = m_TransformReferences.GetClosestRotation(m_ObjectToSet.rotation, out float angleDiff);
        }

        // --------------------------------------------------------------------

        public void SnapTo(string directionID)
        {
            m_TransformRef = m_TransformReferences.Get(directionID);
            SnapTo(m_TransformRef);
        }

        // --------------------------------------------------------------------

        public void SnapTo(TransformRefEntry transformRef)
        {
            m_ObjectToSet.rotation = m_TransformRef.ReferenceTransform.rotation;
        }

        // --------------------------------------------------------------------

        public void RotateTo(string directionID)
        {
            float fromAngle = m_TransformRef.ReferenceTransform.rotation.eulerAngles.y;
            float toAngle = m_TransformReferences.Get(directionID).ReferenceTransform.rotation.eulerAngles.y;
            float diff = toAngle - fromAngle;
            if (diff > 180f) 
                diff = 360f - diff;
            StartCoroutine(RotateObject(diff));
        }

        // --------------------------------------------------------------------

        public void SnapToClosesDirection()
        {
            SnapTo(m_TransformReferences.GetClosestRotation(transform.rotation, out float angleDiff));
        }

        // --------------------------------------------------------------------

        public void StartRotation(float rotationAmount)
        {
            StartCoroutine(RotateObject(rotationAmount));
        }

        // --------------------------------------------------------------------

        public IEnumerator RotateObject(float rotationAmount)
        {
            float startRotation = m_ObjectToSet.eulerAngles.y;
            float endRotation = startRotation + rotationAmount;

            OnRotationStart?.Invoke();

            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / Mathf.Abs(rotationAmount) * m_RotationSpeed;
                float currentRotation = Mathf.Lerp(startRotation, endRotation, t);
                m_ObjectToSet.rotation = Quaternion.Euler(0f, currentRotation, 0f);
                yield return null;
            }

            m_TransformRef = m_TransformReferences.GetClosestRotation(m_ObjectToSet.rotation, out float angleDiff);
            OnRotationEnd?.Invoke();
        }

        //------------------------------------------------------
        // ISavable implementation
        //------------------------------------------------------

        public string GetSavableData()
        {
            return m_TransformRef.ID;
        }

        // --------------------------------------------------------------------

        public void SetFromSavedData(string savedData)
        {
            SnapTo(savedData);
        }
    }
}
