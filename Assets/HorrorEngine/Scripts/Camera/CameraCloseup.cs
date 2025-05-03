using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace HorrorEngine
{
    public abstract class Closeup : MonoBehaviour
    {
        [SerializeField] bool m_PauseGame = true;
        [SerializeField] float m_FadeOutDuration = 0.5f;
        [SerializeField] float m_FadeInDuration = 0.5f;
        
        public abstract IEnumerator ActivationRoutine();
        public abstract IEnumerator DectivationRoutine();

        private UIFade m_UIFade;
        private IUIInput m_UIInput;
        
        private Coroutine m_ShowCoroutine;
        private Coroutine m_HideCoroutine;

        // --------------------------------------------------------------------

        protected virtual void Start()
        {
            m_UIFade = UIManager.Get<UIFade>();
            m_UIInput = UIManager.Instance.GetComponent<IUIInput>();
            enabled = false;
        }

        // --------------------------------------------------------------------

        public void Activate()
        {
            if (m_HideCoroutine != null)
                StopCoroutine(m_HideCoroutine);

            m_ShowCoroutine = StartCoroutine(StartCloseupRoutine(ActivationRoutine));
        }

        // --------------------------------------------------------------------

        public void Deactivate()
        {
            Deactivate(0);
        }

        // --------------------------------------------------------------------

        public void Deactivate(float delay)
        {
            if (m_ShowCoroutine != null)
                StopCoroutine(m_ShowCoroutine);

            if (gameObject.activeInHierarchy)
                m_HideCoroutine = StartCoroutine(EndCloseupRoutine(DectivationRoutine, delay));
        }


        // --------------------------------------------------------------------

        private IEnumerator StartCloseupRoutine(Func<IEnumerator> activationRoutine)
        {
            if (m_PauseGame)
                PauseController.Instance.Pause(this);

            // Fade Out
            yield return m_UIFade.Fade(0f, 1f, m_FadeOutDuration);

            yield return StartCoroutine(activationRoutine?.Invoke());

            // Fade In
            yield return m_UIFade.Fade(1f, 0f, m_FadeInDuration);

        }

        // --------------------------------------------------------------------

        private IEnumerator EndCloseupRoutine(Func<IEnumerator> deactivationRoutine, float delay)
        {
            enabled = false; // Deactivate early to prevent double dismiss input

            if (delay > 0)
                yield return Yielders.UnscaledTime(delay);

            // Fade Out
            yield return m_UIFade.Fade(0f, 1f, m_FadeOutDuration);

            yield return StartCoroutine(deactivationRoutine?.Invoke());

            // Fade In
            yield return m_UIFade.Fade(1f, 0f, m_FadeInDuration);

            if (m_PauseGame)
                PauseController.Instance.Resume(this);

        }
    }

    public class CameraCloseup : Closeup
    {
        [SerializeField] bool m_HidePlayer = false;
        [SerializeField] CinemachineCamera m_Camera;

        // --------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            m_Camera.gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------

        public override IEnumerator ActivationRoutine()
        {
            if (m_HidePlayer)
            {
                GameManager.Instance.Player.SetVisible(false);
            }
            m_Camera.gameObject.SetActive(true);
            yield return null;
        }

        // --------------------------------------------------------------------

        public override IEnumerator DectivationRoutine()
        {
            if (m_HidePlayer)
            {
                GameManager.Instance.Player.SetVisible(true);
            }
            m_Camera.gameObject.SetActive(false);
            yield return null;
        }
    }
}