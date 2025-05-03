using UnityEngine;
using Unity.Cinemachine;

namespace PreRenderBackgrounds
{
    public class MimicCamera : MonoBehaviour
    {
        public Camera Camera;

        private CinemachineCamera m_VirtualCam;

        private void OnValidate()
        {
            if (Camera)
                Mimic(Camera);
        }
        private void Awake()
        {
            if (Camera)
                Mimic(Camera);
        }

        private void OnEnable()
        {
            if (Camera)
                Mimic(Camera);
        }

        private void Mimic(Camera cam)
        {
            if (!m_VirtualCam)
                m_VirtualCam = GetComponentInChildren<CinemachineCamera>(true);

            if (m_VirtualCam)
            {
                m_VirtualCam.transform.position = cam.transform.position;
                m_VirtualCam.transform.rotation = cam.transform.rotation;
                LensSettings lens = LensSettings.FromCamera(cam);
                
                if (cam.usePhysicalProperties)
                    lens.ModeOverride = LensSettings.OverrideModes.Physical;

                m_VirtualCam.Lens = lens;
            }
        }
    }
}
