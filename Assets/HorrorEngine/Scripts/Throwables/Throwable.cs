using UnityEngine;

namespace HorrorEngine
{
    public class Throwable : MonoBehaviour
    {
        private Rigidbody m_Rigidbody;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        public void Throw(Vector3 velocity)
        {
            m_Rigidbody.isKinematic = false;
            m_Rigidbody.linearVelocity = velocity;
        }

        
    }
}
