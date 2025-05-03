using System;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HorrorEngine
{
    [Serializable]
    public class TransformRefEntry
    {
        public string ID;
        public Transform ReferenceTransform;
    }

    public class TransformReferences : MonoBehaviour
    {
        [FormerlySerializedAs("References")]
        [SerializeField] TransformRefEntry[] m_References;
        [SerializeField] float m_GizmoSize = 1f;


        // --------------------------------------------------------------------

        public TransformRefEntry Get(string dirID)
        {
            foreach (TransformRefEntry entry in m_References)
            {
                if (entry.ID == dirID)
                    return entry;
            }

            Debug.LogError("TransformRef couldn't be found");
            return null;
        }

        // --------------------------------------------------------------------

        public TransformRefEntry GetClosestRotation(Quaternion rotation, out float angleDiff)
        {
            TransformRefEntry selected = null;
            float minAngle = 360f;
            foreach (TransformRefEntry entry in m_References)
            {
                float difference = Quaternion.Angle(rotation, entry.ReferenceTransform.rotation);
                if (difference < minAngle)
                {
                    selected = entry;
                    minAngle = difference;
                }
            }

            angleDiff = minAngle;
            return selected;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach(var entry in m_References)
            {
                if (entry != null && entry.ReferenceTransform)
                {
                    Gizmos.DrawLine(entry.ReferenceTransform.position, entry.ReferenceTransform.position + entry.ReferenceTransform.forward * m_GizmoSize);
                    Gizmos.DrawLine(entry.ReferenceTransform.position, entry.ReferenceTransform.position + entry.ReferenceTransform.up * m_GizmoSize);
                    Handles.Label(entry.ReferenceTransform.position + entry.ReferenceTransform.forward + Vector3.up * 0.1f, entry.ID);
                }
            }
        }
#endif
    }
}