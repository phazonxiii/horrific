using UnityEngine;

namespace HorrorEngine
{
    public class Aimable : MonoBehaviour
    {
        [Tooltip("Use this to indicate multiple points to be used for visibility checks")]
        public Vector3[] VisibilityTracePoints = { Vector3.zero };

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            foreach(Vector3 v in VisibilityTracePoints)
            {
                Gizmos.DrawWireSphere(transform.TransformPoint(v), 0.125f);
            }
        }
    }
}
