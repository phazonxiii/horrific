using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorEngine
{
    public class SceneTransitionPreMessage : BaseMessage
    {

    }
    public class SceneTransitionPostMessage : BaseMessage
    {

    }

    public class SceneTransitionEndMessage : BaseMessage
    {

    }

    public class SceneTransitionController : SingletonBehaviour<SceneTransitionController>
    {
        private UIFade m_UIFade;
        [SerializeField] float m_FadeOutDuration = 1f;
        [SerializeField] float m_FadeInDuration = 1f;


        // --------------------------------------------------------------------

        private void Start()
        {
            m_UIFade = UIManager.Get<UIFade>();
        }


        // --------------------------------------------------------------------

        public void Trigger(SceneTransition transition)
        {
            StartCoroutine(StartTransitionRoutine(transition));
        }

        // --------------------------------------------------------------------

        private IEnumerator StartTransitionRoutine(SceneTransition transition)
        {
            PauseController.Instance.Pause(this);

            // Fade Out
            if (transition.FadeOut)
                yield return m_UIFade.Fade(0f, 1f, m_FadeOutDuration);

            yield return transition.StartSceneTransition();

            PauseController.Instance.Resume(this);

            // Fade In
            if (transition.FadeIn)
                yield return m_UIFade.Fade(1f, 0f, m_FadeInDuration);

        }
    }
}