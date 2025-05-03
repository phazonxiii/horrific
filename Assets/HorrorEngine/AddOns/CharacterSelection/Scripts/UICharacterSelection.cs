using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HorrorEngine
{
    [Serializable]
    public class CharacterSelectionEntry
    {
        public CharacterData Character;
        public GameObject IDPrefab;
        public Sprite Portrait;
    }

    public class UICharacterSelection : MonoBehaviour
    {
        [SerializeField] SceneReference m_StartScene;
        [SerializeField] CharacterSelectionEntry[] m_Characters;

        [Space()]
        [SerializeField] RectTransform m_IDCenterPos;
        [SerializeField] RectTransform m_IDLeftPos;
        [SerializeField] RectTransform m_IDRightPos;
        [SerializeField] Image m_Portrait;
        [SerializeField] GameObject m_NextIndicator;
        [SerializeField] GameObject m_PrevIndicator;
        [SerializeField] float m_TransitionTime = 0.5f;

        private int m_CurrentIndex;
        private IUIInput m_UIInput;
        private bool m_CanSelect;

        private GameObject[] IDs;

        // --------------------------------------------------------------------

        void Awake()
        {
            int index = 0;
            IDs = new GameObject[m_Characters.Length];
            foreach (var character in m_Characters)
            {
                var instance = Instantiate(character.IDPrefab, m_IDCenterPos.parent);
                IDs[index] = instance;

                instance.transform.position = index == 0 ? m_IDCenterPos.position : m_IDRightPos.position;
                instance.transform.localScale = index == 0 ? m_IDCenterPos.localScale : m_IDRightPos.localScale;
                instance.transform.localRotation = index == 0 ? m_IDCenterPos.localRotation : m_IDRightPos.localRotation;

                ++index;
            }

            m_CanSelect = true;
            SetSelected(0);
        }

        // --------------------------------------------------------------------

        private void Start()
        {
            m_UIInput = UIManager.Instance.GetComponent<IUIInput>();
        }

        // --------------------------------------------------------------------

        void Update()
        {
            Vector2 axis = m_UIInput.GetPrimaryAxis();
            if (axis.magnitude > 0.5f && m_CanSelect)
            {
                if (axis.x > 0)
                    MoveRight();
                else
                    MoveLeft();
            }

            if (m_UIInput.IsConfirmDown())
            {
                PlayerPrefs.SetString(GameManager.k_StartCharacterPlayerPrefs, m_Characters[m_CurrentIndex].Character.UniqueId);

                SceneManager.LoadScene(m_StartScene.Name);
            }
        }

        // --------------------------------------------------------------------

        private void MoveRight()
        {
            if (m_CurrentIndex == IDs.Length - 1)
                return;

            m_CanSelect = false;
            StartCoroutine(MoveID(IDs[m_CurrentIndex], m_IDLeftPos));
            StartCoroutine(MoveID(IDs[m_CurrentIndex+1], m_IDCenterPos));

            SetSelected(m_CurrentIndex + 1);
        }

        // --------------------------------------------------------------------

        private void MoveLeft()
        {
            if (m_CurrentIndex == 0)
                return;

            m_CanSelect = false;
            StartCoroutine(MoveID(IDs[m_CurrentIndex], m_IDRightPos));
            StartCoroutine(MoveID(IDs[m_CurrentIndex - 1], m_IDCenterPos));

            SetSelected(m_CurrentIndex - 1);
        }

        // --------------------------------------------------------------------

        private void SetSelected(int selected)
        {
            m_CurrentIndex = selected;
            m_Portrait.sprite = m_Characters[selected].Portrait;

            m_PrevIndicator.SetActive(m_CurrentIndex > 0);
            m_NextIndicator.SetActive(m_CurrentIndex < IDs.Length - 1);
        }

        // --------------------------------------------------------------------

        IEnumerator MoveID(GameObject ID, Transform ToPos)
        {
            float t = 0;
            float time = 0;
            
            Vector3 initPos = ID.transform.position;
            Vector3 initScale = ID.transform.localScale;
            Quaternion initRotation = ID.transform.localRotation;

            while (t < 1)
            {
                time += Time.deltaTime;
                t = time / m_TransitionTime;
                if (t > 1f) t = 1;
                
                ID.transform.position = Vector3.Lerp(initPos, ToPos.position, t);
                ID.transform.localScale = Vector3.Lerp(initScale, ToPos.localScale, t);
                ID.transform.localRotation = Quaternion.Slerp(initRotation, ToPos.localRotation, t);

                yield return null;
            }

            m_CanSelect = true;
        }

    }
}