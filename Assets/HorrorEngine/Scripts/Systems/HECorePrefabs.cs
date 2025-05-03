using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorEngine
{
    [CreateAssetMenu(menuName = "Horror Engine/Core Prefabs")]
    public class HECorePrefabs : ScriptableObject
    {
        [SerializeField] private HECorePrefabs m_Prototype;

        [SerializeField] private GameObject m_GameManager;
        [SerializeField] private GameObject m_CameraSystem;

        [Tooltip("These prefabs will be instantiated too. Custom prefabs defined in the prototype HECorePrefabs will also be instantiated")]
        [SerializeField] private GameObject[] m_CustomObjects;

        [Header("UI")]
        [SerializeField] private GameObject m_Inventory;
        [SerializeField] private GameObject m_Document;
        [SerializeField] private GameObject m_DocumentList;
        [SerializeField] private GameObject m_Dialog;
        [SerializeField] private GameObject m_Choices;
        [SerializeField] private GameObject m_SaveGame;
        [SerializeField] private GameObject m_Item;
        [SerializeField] private GameObject m_ItemContainer;
        [SerializeField] private GameObject m_ExamineItem;
        [SerializeField] private GameObject m_ExamineItemRenderer;
        [SerializeField] private GameObject m_Pause;
        [SerializeField] private GameObject m_Map;
        [SerializeField] private GameObject m_MapRenderer;
        [SerializeField] private GameObject m_GameOver;
        [SerializeField] private GameObject m_CinematicPlayer;
        [SerializeField] private GameObject m_InteractionPrompt;
        [SerializeField] private GameObject m_Fade;
        [SerializeField] private GameObject m_LetterBox;
        [SerializeField] private GameObject m_Ranking;

        [Tooltip("These prefabs will be instantiated too. Custom prefabs defined in the prototype HECorePrefabs will also be instantiated")]
        [SerializeField] private GameObject[] m_CustomUI;

        private Dictionary<string, List<GameObject>> m_MappedPrefabs;

        // --------------------------------------------------------------------

        public Dictionary<string, List<GameObject>> GetMappedPrefabs()
        {
            return m_MappedPrefabs;
        }

        // --------------------------------------------------------------------

        private void OnEnable()
        {
            m_MappedPrefabs = new Dictionary<string, List<GameObject>>();

            List<GameObject> rootGO = new List<GameObject>();
            Add(rootGO, m_GameManager, m_Prototype?.m_GameManager);
            Add(rootGO, m_CameraSystem, m_Prototype?.m_CameraSystem);

            AddCustomObjects(rootGO);

            List<GameObject> uiGO = new List<GameObject>();
            Add(uiGO, m_Inventory, m_Prototype?.m_Inventory);
            Add(uiGO, m_Document, m_Prototype?.m_Document);
            Add(uiGO, m_DocumentList, m_Prototype?.m_DocumentList);
            Add(uiGO, m_Dialog, m_Prototype?.m_Dialog);
            Add(uiGO, m_Choices, m_Prototype?.m_Choices);
            Add(uiGO, m_SaveGame, m_Prototype?.m_SaveGame);
            Add(uiGO, m_Item, m_Prototype?.m_Item);
            Add(uiGO, m_ItemContainer, m_Prototype?.m_ItemContainer);
            Add(uiGO, m_ExamineItemRenderer, m_Prototype?.m_ExamineItemRenderer); // This need to be instantiated before the ExamineItem
            Add(uiGO, m_ExamineItem, m_Prototype?.m_ExamineItem);
            Add(uiGO, m_Pause, m_Prototype?.m_Pause);
            Add(uiGO, m_Map, m_Prototype?.m_Map);
            Add(uiGO, m_MapRenderer, m_Prototype?.m_MapRenderer);
            Add(uiGO, m_GameOver, m_Prototype?.m_GameOver);
            Add(uiGO, m_CinematicPlayer, m_Prototype?.m_CinematicPlayer);
            Add(uiGO, m_InteractionPrompt, m_Prototype?.m_InteractionPrompt);
            Add(uiGO, m_Fade, m_Prototype?.m_Fade);
            Add(uiGO, m_LetterBox, m_Prototype?.m_LetterBox);
            Add(uiGO, m_Ranking, m_Prototype?.m_Ranking);

            AddCustomUI(uiGO);

            m_MappedPrefabs.Add("", rootGO);
            m_MappedPrefabs.Add("UI", uiGO);
        }

        // --------------------------------------------------------------------

        private void AddCustomObjects(List<GameObject> list)
        {
            if (m_CustomObjects != null)
                AddArray(list, m_CustomObjects);

            m_Prototype?.AddCustomObjects(list);
        }

        // --------------------------------------------------------------------

        private void AddCustomUI(List<GameObject> list)
        {
            AddArray(list, m_CustomUI);
            m_Prototype?.AddCustomUI(list);
        }

        // --------------------------------------------------------------------

        private void AddArray(List<GameObject> list, GameObject[] go)
        {
            if (go != null)
            {
                foreach (var entry in go)
                {
                    Add(list, entry, null);
                }
            }
        }

        // --------------------------------------------------------------------

        private void Add(List<GameObject> list, GameObject go, GameObject fallback)
        {
            GameObject gameObject = go != null ? go : fallback;
            if (gameObject)
                list.Add(gameObject);
        }
    }

}
