using UnityEngine;

namespace HorrorEngine
{
    public class PlaceableObject : MonoBehaviour
    {
        [SerializeField] private Vector3 m_Offset;
        
        public void Place(PlaceableObjectSlot slot)
        {
            transform.SetParent(slot.transform);
            transform.SetLocalPositionAndRotation(m_Offset, Quaternion.identity);

            slot.OnPlaced?.Invoke();
        }
    }
}