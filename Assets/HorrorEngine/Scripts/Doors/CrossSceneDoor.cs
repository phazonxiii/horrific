using System.Collections;
using UnityEngine;

namespace HorrorEngine
{
    [RequireComponent(typeof(SceneTransition))]
    public class CrossSceneDoor : DoorBase
    {
        [SerializeField] private string m_DoorUniqueId;
        [SerializeField] private string m_ExitDoorUniqueId;
        
        private DoorLock m_Lock;
        private SceneTransition m_SceneTransition;

        // --------------------------------------------------------------------

        private void Awake()
        {
            m_SceneTransition = GetComponent<SceneTransition>();
            m_Lock = GetComponent<DoorLock>();
        }

        // --------------------------------------------------------------------

        public override bool IsLocked()
        {
            return (m_Lock && m_Lock.IsLocked);
        }

        // --------------------------------------------------------------------

        public override void Use(IInteractor interactor)
        {
            if (m_Lock && m_Lock.IsLocked)
            {
                m_Lock.OnTryToUnlock(out bool open);
                if (!open)
                {
                    OnLocked?.Invoke();
                    return;
                }
            }

            OnOpened?.Invoke();
            MonoBehaviour interactorMB = (MonoBehaviour)interactor;
            m_Interactor = interactorMB.transform;
            DoorTransitionController.Instance.Trigger(this, interactorMB.gameObject, TransitionRoutine);
        }

        // --------------------------------------------------------------------

        private IEnumerator TransitionRoutine()
        {            
            yield return m_SceneTransition.StartSceneTransition();

            bool doorFound = false;
            CrossSceneDoor[] doors = FindObjectsByType<CrossSceneDoor>(FindObjectsSortMode.None);
            foreach(var door in doors)
            {
                if (door.m_DoorUniqueId == m_ExitDoorUniqueId)
                {
                    TeleportInteractor(door.ExitPoint);
                    doorFound = true;
                    break;
                }
            }

            Debug.Assert(doorFound, $"CrossScene door exit with Id: {m_ExitDoorUniqueId} not found from {doors.Length} candidates");

            yield return Yielders.UnscaledTime(1.0f);
        }

    }
}