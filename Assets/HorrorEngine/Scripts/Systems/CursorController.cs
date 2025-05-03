using UnityEngine;
using System;

namespace HorrorEngine
{
    [Serializable]
    public class CursorState
    {
        public CursorLockMode Mode;
        public bool Visible;
    }

    public class CursorController : SingletonBehaviour<CursorController>
    {
        [SerializeField] bool m_StartInUI;
        [SerializeField] CursorState m_InUIState;
        [SerializeField] CursorState m_OutOfUIState;

        private int m_InUICount;

        protected override void Awake()
        {
            base.Awake();

            m_InUICount = m_StartInUI ? 0 : 1;
            SetInUI(m_StartInUI);
        }

        public void SetInUI(bool inUI)
        {
            if (!inUI)
            {
                --m_InUICount;
                Debug.Assert(m_InUICount >= 0, "Cursor InUI count went negative. This shouldn't happen. Ssomething calling SetInUI multiple times with the same value");

                if (m_InUICount == 0)
                {
                    Cursor.lockState = m_OutOfUIState.Mode;
                    Cursor.visible = m_OutOfUIState.Visible;
                }
            }
            else
            {
                Cursor.lockState = m_InUIState.Mode;
                Cursor.visible = m_InUIState.Visible;
                ++m_InUICount;
            }
        }


    }
}
