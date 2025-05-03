#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

using UnityEngine;

namespace HorrorEngine
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ObjectUniqueId : MonoBehaviour
    {
        [SerializeField] private string m_Id;
        [Tooltip("This indicates the Id doesn't need to be different from the prefab since there won't be more instances")]
        public bool IsUniqueInstance;

        public string Id => m_Id;

        // --------------------------------------------------------------------

        private void Awake()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(Id) && PrefabStageUtility.GetCurrentPrefabStage() == null)
                RegenerateId();
#endif
        }

        // --------------------------------------------------------------------

        [ContextMenu("Regenerate Id")]
        public void RegenerateId()
        {
            m_Id = IdUtils.GenerateId();
        }

        // --------------------------------------------------------------------

        public void SetId(string id)
        {
            m_Id = id;
        }
    }
}