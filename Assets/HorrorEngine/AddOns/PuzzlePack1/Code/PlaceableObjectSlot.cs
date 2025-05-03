using UnityEngine.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace HorrorEngine
{
    public class PlaceableObjectSlot : MonoBehaviour
    {
        public UnityEvent OnSelected;
        [FormerlySerializedAs("OnSwapped")]
        public UnityEvent OnPlaced;

    }
}
