using Unity.Cinemachine;
using UnityEngine;

namespace HorrorEngine
{
    public class CameraSystem : SingletonBehaviour<CameraSystem>
    {
        public static readonly int CamPreviewOverride = -1;

        private CinemachineBrain m_Brain;
        private Camera m_MainCamera;
        
        public CinemachineCamera ActiveCamera => m_Brain.ActiveVirtualCamera as CinemachineCamera;
        public Camera MainCamera => m_MainCamera;

        // --------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            m_Brain = GetComponentInChildren<CinemachineBrain>();
            m_Brain.ReleaseCameraOverride(CamPreviewOverride);

            m_MainCamera = m_Brain.GetComponent<Camera>();
        }

        // --------------------------------------------------------------------

        void OnDestroy()
        {
            CameraStack.Instance.ClearAllCameras();
        }
    }
}