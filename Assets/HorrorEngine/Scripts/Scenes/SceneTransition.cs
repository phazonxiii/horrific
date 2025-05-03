using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HorrorEngine
{
   
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] public bool FadeOut = true;
        [SerializeField] public bool FadeIn = true;
        [SerializeField] private SceneReference m_LoadScene;
        [SerializeField] private LoadSceneMode m_LoadMode;
        [SerializeField] private SceneReference m_UnloadScene;

        private CharacterData m_SpawnCharacter;

        // --------------------------------------------------------------------

        public void TriggerTransition(CharacterData character)
        {
            m_SpawnCharacter = character;
            SceneTransitionController.Instance.Trigger(this);
        }

        // --------------------------------------------------------------------

        public IEnumerator StartSceneTransition()
        {
            MessageBuffer<SceneTransitionPreMessage>.Dispatch();

            AsyncOperation asyncUnLoad = null;
            AsyncOperation asyncLoad = null;

            if (m_LoadScene && !m_LoadScene.IsLoaded())
            {
                asyncLoad = SceneManager.LoadSceneAsync(m_LoadScene.Name, m_LoadMode);
            }

            while ((asyncLoad != null && !asyncLoad.isDone))
            {
                yield return null;
            }

            if (m_UnloadScene && m_UnloadScene.IsLoaded())
            {
                asyncUnLoad = SceneManager.UnloadSceneAsync(m_UnloadScene.Name);
            }

            while ((asyncUnLoad != null && !asyncUnLoad.isDone))
            {
                yield return null;
            }

            // This needs to happen before player teleportation or the object state will override the player transform
            MessageBuffer<SceneTransitionPostMessage>.Dispatch();

            PlayerSpawnPoint spawnPoint = FindFirstObjectByType<PlayerSpawnPoint>();
            Debug.Assert(spawnPoint, "Default player spawn point not found");
            GameManager.Instance.SwitchCharacter(m_SpawnCharacter, spawnPoint.transform);
            m_SpawnCharacter = null;

        }

    }
}