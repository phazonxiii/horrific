using System;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorEngine
{
    public class InteractionChangedMessage : BaseMessage
    {
        public Interactive Interactive;
    }

    public abstract class InteractionDetector : MonoBehaviour, IDeactivateWithActor
    {
        protected List<Interactive> m_Interactives = new List<Interactive>();

        private InteractionChangedMessage m_InteractionChangedMsg = new InteractionChangedMessage();
        private Action<OnDisableNotifier> m_OnInteractiveDisabled;

        // --------------------------------------------------------------------

        public List<Interactive> DetectedInteractives => m_Interactives;

        // --------------------------------------------------------------------

        public Interactive FocusedInteractive
        {
            get
            {
                if (m_Interactives.Count == 0)
                    return null;
                Interactive i = m_Interactives[m_Interactives.Count - 1];
                return i.isActiveAndEnabled ? i : null;
            }
        }

        // --------------------------------------------------------------------

        protected virtual void Awake()
        {
            m_OnInteractiveDisabled = OnInteractiveDisabled;
        }

        // --------------------------------------------------------------------

        protected void AddInteractive(Interactive interactive)
        {
            if (!m_Interactives.Contains(interactive))
                m_Interactives.Add(interactive);
            OnDisableNotifier disableNotif = interactive.GetComponent<OnDisableNotifier>();
            disableNotif.AddCallback(m_OnInteractiveDisabled);
            m_InteractionChangedMsg.Interactive = interactive;
            MessageBuffer<InteractionChangedMessage>.Dispatch(m_InteractionChangedMsg);
        }

        // --------------------------------------------------------------------

        protected void RemoveInteractive(GameObject obj)
        {
            Interactive interactive = obj.GetComponent<Interactive>();
            if (interactive)
            {
                RemoveInteractive(interactive);
            }
        }

        protected void RemoveInteractive(Interactive interactive)
        {
            bool isFocused = interactive == FocusedInteractive;
            if (m_Interactives.Remove(interactive) && isFocused)
            {
                if (m_Interactives.Count > 0)
                {
                    interactive = m_Interactives[m_Interactives.Count - 1];
                    m_InteractionChangedMsg.Interactive = interactive;
                }
                else
                {
                    m_InteractionChangedMsg.Interactive = null;
                }

                MessageBuffer<InteractionChangedMessage>.Dispatch(m_InteractionChangedMsg);
            }
        }

        // --------------------------------------------------------------------

        private void OnInteractiveDisabled(OnDisableNotifier notifier)
        {
            notifier.RemoveCallback(m_OnInteractiveDisabled);
            RemoveInteractive(notifier.gameObject);
        }
        // --------------------------------------------------------------------

        protected virtual void OnDisable()
        {
            m_Interactives.Clear();

            m_InteractionChangedMsg.Interactive = null;
            MessageBuffer<InteractionChangedMessage>.Dispatch(m_InteractionChangedMsg);
        }
    }
}