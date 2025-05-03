using System;
using UnityEngine;
using UnityEngine.Events;

namespace HorrorEngine
{
    [Serializable]
#if GAME_2D
public class OnTriggerAction : UnityEvent<Collider2D> { }
#else
    public class OnTriggerAction : UnityEvent<Collider> { }
#endif

    public class ColliderObserver : MonoBehaviour
    {
        public OnTriggerAction TriggerEnter;
        public OnTriggerAction TriggerExit;

        private Action<OnDisableNotifier> m_OnColliderDisabled;
        private ColliderObserverFilter[] m_Filters;

        private void Awake()
        {
            m_Filters = GetComponents<ColliderObserverFilter>();
            m_OnColliderDisabled = OnColliderDisabled;
        }

#if GAME_2D
    private void OnTriggerEnter2D(Collider2D other)
#else
        private void OnTriggerEnter(Collider other)
#endif
        {
            if (!PassesFilter(other))
                return;

            other.GetComponentInParent<OnDisableNotifier>().AddCallback(m_OnColliderDisabled);
            TriggerEnter?.Invoke(other);
        }
#if GAME_2D
    private void OnTriggerExit2D(Collider2D other)
#else
        private void OnTriggerExit(Collider other)
#endif
        {
            if (!PassesFilter(other))
                return;

            other.GetComponentInParent<OnDisableNotifier>().RemoveCallback(m_OnColliderDisabled);
            TriggerExit?.Invoke(other);
        }

        private bool PassesFilter(Collider other)
        {
            if (m_Filters.Length == 0)
                return true;

            foreach (var filter in m_Filters)
            {
                if (filter.Passes(other))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnColliderDisabled(OnDisableNotifier notifier)
        {
            notifier.RemoveCallback(m_OnColliderDisabled);
#if GAME_2D
        TriggerExit?.Invoke(notifier.GetComponent<Collider2D>());
#else
            TriggerExit?.Invoke(notifier.GetComponent<Collider>());
#endif

        }
    }
}