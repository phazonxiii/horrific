using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace HorrorEngine
{
    public class UIPause : MonoBehaviour
    {
        [SerializeField] private GameObject m_MainContent;
        [SerializeField] private GameObject m_PauseHint;
        [SerializeField] private AudioClip m_ShowClip;
        [SerializeField] private SceneReference m_QuitScene;
        [SerializeField] private GameObject m_DefaultSelection;

        public UnityEvent OnHide;

        private IUIInput m_Input;

        // --------------------------------------------------------------------

        private void Awake()
        {
            m_Input = GetComponentInParent<IUIInput>();
        }

        // --------------------------------------------------------------------

        void Start()
        {
            m_MainContent.SetActive(false);
        }

        // --------------------------------------------------------------------

        void Update()
        {
            bool isPlaying = GameManager.Instance.IsPlaying;

            if (m_PauseHint)
                m_PauseHint.SetActive(isPlaying);

            if (m_DefaultSelection.activeInHierarchy)
                EventSystemUtils.SelectDefaultOnLostFocus(m_DefaultSelection);
        }

        // --------------------------------------------------------------------

        public void Show()
        {
            PauseController.Instance.Pause(this);
            m_MainContent.SetActive(true);
            UIManager.Get<UIAudio>().Play(m_ShowClip);
            CursorController.Instance.SetInUI(true);
        }

        // --------------------------------------------------------------------

        public void Hide()
        {
            PauseController.Instance.Resume(this);
            CursorController.Instance.SetInUI(false);
            m_MainContent.SetActive(false);
            OnHide?.Invoke();
        }

        // --------------------------------------------------------------------

        public void QuitGame()
        {
            Hide();

            SceneManager.LoadScene(m_QuitScene.Name);
        }
    }
}