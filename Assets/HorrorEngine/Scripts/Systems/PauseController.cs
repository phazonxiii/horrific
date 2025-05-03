using System.Collections.Generic;
using UnityEngine;

namespace HorrorEngine
{
    public class GamePausedMessage : BaseMessage
    {
        public static GamePausedMessage Default = new GamePausedMessage();
    }

    public class GameUnpausedMessage : BaseMessage
    {
        public static GameUnpausedMessage Default = new GameUnpausedMessage();
    }

    public class PauseController : SingletonBehaviourDontDestroy<PauseController>
    {
        private List<Object> m_PauseContext = new List<Object>();

        public bool IsPaused => m_PauseContext.Count > 0;

        public Object Context => m_PauseContext.Count > 0 ? m_PauseContext[m_PauseContext.Count - 1] : null;


        // --------------------------------------------------------------------

        public void Pause(Object context)
        {
            m_PauseContext.Remove(context);
            m_PauseContext.Add(context);

            if (m_PauseContext.Count == 1)
                MessageBuffer<GamePausedMessage>.Dispatch(GamePausedMessage.Default);

            Time.timeScale = 0f;
        }

        // --------------------------------------------------------------------

        public void Resume(Object context)
        {
            if (m_PauseContext.Remove(context))
            {
                if (m_PauseContext.Count == 0)
                {
                    MessageBuffer<GameUnpausedMessage>.Dispatch(GameUnpausedMessage.Default);
                    Time.timeScale = 1f;
                }
            }
        }

        // --------------------------------------------------------------------

        private void OnDestroy()
        {
            m_PauseContext.Clear();
            Time.timeScale = 1f;
        }

#if UNITY_EDITOR
        public void Update()
        {
            if (Input.GetKey(KeyCode.Numlock))
            {
                Debug.Break();
            }
        }
#endif
    }

    
}