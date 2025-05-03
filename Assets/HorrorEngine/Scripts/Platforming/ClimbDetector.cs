using System;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorEngine
{
    public class ClimbDetector : MonoBehaviour
    {
        private Action<OnDisableNotifier> m_OnClimbableDisabled;
        private List<Climbable> m_Climbables = new List<Climbable>();

        public Climbable Climbable => m_Climbables.Count > 0 ?  m_Climbables[0] : null;
        
        // --------------------------------------------------------------------

        private void Awake()
        {
            m_OnClimbableDisabled = OnClimbableDisabled;
        }

        // --------------------------------------------------------------------

        private void OnDisable()
        {
            foreach(var climbable in m_Climbables)
            {
                var disableNotif = climbable.GetComponent<OnDisableNotifier>();
                if (disableNotif)
                {
                    disableNotif.RemoveCallback(m_OnClimbableDisabled);
                }
            }

            m_Climbables.Clear();
        }

        // --------------------------------------------------------------------

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Climbable newClimbable) && !m_Climbables.Contains(newClimbable))
            {
                float distToOther = Vector3.Distance(other.transform.position, transform.position);
                int i = 0;
                foreach(var climbable in m_Climbables)
                {
                    if (distToOther < Vector3.Distance(climbable.transform.position, transform.position)) 
                    {
                        break;
                    }

                    ++i;
                }

                m_Climbables.Insert(i, newClimbable);

                enabled = true;

                var disableNotif = other.GetComponent<OnDisableNotifier>();
                if (disableNotif)
                {
                    disableNotif.AddCallback(m_OnClimbableDisabled);
                }
            }
        }

        // --------------------------------------------------------------------

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Climbable climbable) && m_Climbables.Contains(climbable))
            {
                RemoveClimbable(climbable);
            }
        }

        private void RemoveClimbable(Climbable climbable)
        {
            var disableNotif = climbable.GetComponent<OnDisableNotifier>();
            if (disableNotif)
            {
                disableNotif.RemoveCallback(m_OnClimbableDisabled);
            }

            m_Climbables.Remove(climbable);
        }

        // --------------------------------------------------------------------

        private void OnClimbableDisabled(OnDisableNotifier notifier)
        {
            if (notifier.TryGetComponent(out Climbable climbable))
                RemoveClimbable(climbable);
        }
    }
}
