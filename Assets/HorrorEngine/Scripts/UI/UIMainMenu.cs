using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HorrorEngine
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject m_MainScreen;
        [SerializeField] private GameObject m_LoadSlotsScreen;
        [SerializeField] private Button m_DefaultButton;
        [SerializeField] private Button m_LoadButton;
        [SerializeField] private AudioClip m_CloseSlotsClip;
        [SerializeField] private SceneReference m_StartScene;

        private IUIInput m_Input;
        private UISaveGameList m_SlotList;

        // --------------------------------------------------------------------

        private void Awake()
        {
            m_Input = GetComponentInParent<IUIInput>();

            m_SlotList = m_LoadSlotsScreen.GetComponentInChildren<UISaveGameList>();
            m_SlotList.OnSubmit.AddListener(OnLoadSlotSubmit);
        }

        // --------------------------------------------------------------------

        private void OnLoadSlotSubmit(GameObject selected)
        {
            int slotIndex = m_SlotList.GetSelectedIndex();
            
            SaveDataManager<GameSaveData> saveMgr = SaveDataManager<GameSaveData>.Instance;
            bool slotExists = saveMgr.SlotExists(slotIndex);

            if (slotExists)
            {
                gameObject.SetActive(false);
                GameSaveUtils.LoadSlot(slotIndex);
            }
            else
            {
                CloseSlotsScreen();
            }
        }

        // --------------------------------------------------------------------

        private void Start()
        {
            int lastSavedSlot = GameSaveUtils.GetLastSavedSlot();
            m_LoadButton.gameObject.SetActive(lastSavedSlot >= 0);
            m_LoadSlotsScreen.SetActive(false);
        }

        // --------------------------------------------------------------------

        private void OnEnable()
        {
            CursorController.Instance.SetInUI(true);
        }

        // --------------------------------------------------------------------

        private void OnDisable()
        {
            //CursorController.Instance.SetInUI(false);
        }

        // --------------------------------------------------------------------

        private void Update()
        {
            if (m_LoadSlotsScreen.activeSelf)
            {
                if (m_Input.IsConfirmDown())
                {
                    OnLoadSlotSubmit(m_SlotList.GetSelected());
                }
                else if (m_Input.IsCancelDown())
                {
                    CloseSlotsScreen();
                }
            }

            EventSystemUtils.SelectDefaultOnLostFocus(m_DefaultButton.gameObject);
        }

        // --------------------------------------------------------------------

        private void CloseSlotsScreen()
        {
            m_MainScreen.SetActive(true);
            m_LoadSlotsScreen.SetActive(false);
           
            if (m_CloseSlotsClip)
                UIManager.Get<UIAudio>().Play(m_CloseSlotsClip);
        }

        // --------------------------------------------------------------------

        public void NewGame()
        {
            gameObject.SetActive(false);

            ObjectStateManager.Instance.ClearStates();

            SceneManager.LoadScene(m_StartScene.Name);
        }

        // --------------------------------------------------------------------

        public void LoadGame()
        {
            m_Input.Flush(); // Prevents selecting the first slot immediately

            m_MainScreen.SetActive(false);

            m_LoadSlotsScreen.gameObject.SetActive(true);
            m_SlotList.FillSlotsInfo();
        }

        // --------------------------------------------------------------------

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExecuteMenuItem("Edit/Play");
#else
            Application.Quit();
#endif
        }
    }

}