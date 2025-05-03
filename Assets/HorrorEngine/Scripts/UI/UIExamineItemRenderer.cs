using UnityEngine;

namespace HorrorEngine
{
    public class UIExamineItemRenderer : MonoBehaviour
    {
        public Transform PreviewObject;
        public Camera Camera;
        public InteractionRaycastDetector InteractionDetector;

        [SerializeField] private float m_RenderTextureScale = 1f;
        [SerializeField] private FilterMode m_RenderTextureFilterMode = FilterMode.Bilinear;

        private RenderTexture m_Texture;

        public RenderTexture Texture => m_Texture;
     

        private void OnEnable()
        {
            UpdateTexture();
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdateTexture();
        }
#endif

        private void UpdateTexture()
        {
            int w = (int)(Screen.width * m_RenderTextureScale);
            int h = (int)(Screen.height * m_RenderTextureScale);
            if (!m_Texture || m_Texture.width != w || m_Texture.height != h)
            {
                m_Texture?.Release();
                m_Texture = new RenderTexture(w, h, 16);
                m_Texture.filterMode = m_RenderTextureFilterMode;

                Camera.targetTexture = m_Texture;
            }
        }

        private void OnDestroy()
        {
            m_Texture?.Release();
        }
    }
}
