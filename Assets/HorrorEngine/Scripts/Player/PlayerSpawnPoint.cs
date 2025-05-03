using UnityEngine;

namespace HorrorEngine
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        public CharacterData Character;

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 up = transform.position + Vector3.up * 2;
            Gizmos.DrawLine(transform.position, up);
            Gizmos.DrawLine(up, up - Vector3.up * 0.5f + transform.forward);
            Gizmos.DrawLine(up - Vector3.up * 0.5f + transform.forward, up - Vector3.up);
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

#endif
    }
}