using Unity.Cinemachine;
using UnityEngine;

namespace PreRenderBackgrounds
{
    [RequireComponent(typeof(Camera))]
    public class PreRenderedBackground : MonoBehaviour
    {
        private static readonly int k_ColorTexHash = Shader.PropertyToID("_ColorTex");
        private static readonly int k_DepthTexHash = Shader.PropertyToID("_DepthTex");

        public string Name = "UnnamedCamera";
        [SerializeField] private PreRenderSettings m_Settings;
        
        public Texture2D ColorTexture;
        public RenderTexture DepthRenderTexture;

        [SerializeField] bool m_RenderToColor;
        [SerializeField] bool m_RenderOnEnable;
        [SerializeField] Material[] m_RuntimeEffects;

        private Camera m_Camera;
        private bool m_Rendered;
        private Material m_BgMaterialInstance;
        private RenderTexture m_ColorRT;
        private CinemachineCamera m_VirtualCam;

        // --------------------------------------------------------------------

        public Vector2Int Resolution => m_Settings ? m_Settings.Resolution : Vector2Int.zero;

        // --------------------------------------------------------------------

        private void OnValidate()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }


        // --------------------------------------------------------------------

        private void Awake()
        {
            m_VirtualCam = GetComponentInParent<CinemachineCamera>();

            m_Camera = GetComponent<Camera>();
            m_Camera.enabled = false;

            m_BgMaterialInstance = new Material(m_Settings.PreRenderedBgMaterial);
            m_BgMaterialInstance.name = "Background (Instance)";

            m_ColorRT = new RenderTexture(ColorTexture.width, ColorTexture.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(ColorTexture, m_ColorRT);

            m_BgMaterialInstance.SetTexture(k_ColorTexHash, m_ColorRT);
            m_BgMaterialInstance.SetTexture(k_DepthTexHash, DepthRenderTexture);

            GameObject preRdBg = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var meshRnd = preRdBg.GetComponent<MeshRenderer>();
            meshRnd.material = m_BgMaterialInstance;
            //meshRnd.allowOcclusionWhenDynamic = false;

            preRdBg.transform.SetParent(transform);
            preRdBg.transform.localPosition = Vector3.zero;
            preRdBg.transform.localRotation = Quaternion.identity;

            preRdBg.GetComponent<Collider>().enabled = false;

            Debug.Assert(ColorTexture.width == m_Settings.Resolution.x && ColorTexture.height == m_Settings.Resolution.y, "Color texture resolution is different to settings resolution", gameObject);
            Debug.Assert(DepthRenderTexture.width == m_Settings.Resolution.x && DepthRenderTexture.height == m_Settings.Resolution.y, "Depth texture resolution is different to settings resolution", gameObject);
        }

        // --------------------------------------------------------------------

        private void OnEnable()
        {
            Camera mainCam = Camera.main;
            if (mainCam)
                Camera.main.depthTextureMode = DepthTextureMode.Depth;

            if (!m_Rendered || m_RenderOnEnable)
                Render();
        }

        // --------------------------------------------------------------------

        public void Render(Camera cam = null)
        {
            if (!m_VirtualCam)
                m_VirtualCam = GetComponentInParent<CinemachineCamera>();

            if (Application.isPlaying) 
            {
                Debug.Assert(m_VirtualCam, "PreRenderedBackground needs to be placed as a direct child of a virtual camera");
            }

            if (cam)
                m_Camera = cam;

            if (!m_Camera)
                m_Camera = gameObject.AddComponent<Camera>();

            int width = DepthRenderTexture.width;
            int height = DepthRenderTexture.height;

            

            MimicCamera mimicCam = GetComponentInParent<MimicCamera>();

            if (!mimicCam)
            {
                m_Camera.transform.position = m_VirtualCam.transform.position;
                m_Camera.transform.rotation = m_VirtualCam.transform.rotation;

                var lens = m_VirtualCam.Lens;
                m_Camera.nearClipPlane = lens.NearClipPlane;
                m_Camera.farClipPlane = lens.FarClipPlane;
                m_Camera.usePhysicalProperties = lens.IsPhysicalCamera;
                m_Camera.sensorSize = lens.PhysicalProperties.SensorSize;
                m_Camera.lensShift = lens.PhysicalProperties.LensShift;
                m_Camera.gateFit = lens.PhysicalProperties.GateFit;
                m_Camera.orthographic = lens.Orthographic;
                m_Camera.orthographicSize = lens.OrthographicSize;
                m_Camera.fieldOfView = lens.FieldOfView;
            }
             else
             {
                 m_Camera.CopyFrom(mimicCam.Camera);
             }

            m_Camera.cameraType = CameraType.Game;
            m_Camera.enabled = false;
            m_Camera.clearFlags = CameraClearFlags.Skybox;
            m_Camera.forceIntoRenderTexture = true;
            m_Camera.renderingPath = RenderingPath.Forward;
            m_Camera.useOcclusionCulling = true;
            m_Camera.cullingMask = m_Settings.IncludedLayers;
            m_Camera.scene = gameObject.scene;

            m_Camera.aspect = width / Mathf.Max(height, Mathf.Epsilon);
            m_Camera.depthTextureMode = DepthTextureMode.Depth;

            RenderTexture colorText = RenderTexture.GetTemporary(width, height, 24);
            m_Camera.SetTargetBuffers(colorText.colorBuffer, DepthRenderTexture.depthBuffer);
            m_Camera.Render();

            if (m_RenderToColor)
            {
                ToTexture2D(colorText, ColorTexture);
            }

            if (Application.isPlaying)
                m_Rendered = true;
        }

        // --------------------------------------------------------------------

        void ToTexture2D(RenderTexture rTex, Texture2D tex)
        {
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();
        }

        // --------------------------------------------------------------------

        private void Update()
        {
            if (m_RuntimeEffects.Length > 0)
            {
                var rt1 = RenderTexture.GetTemporary(ColorTexture.width, ColorTexture.height, 16);
                var rt2 = RenderTexture.GetTemporary(ColorTexture.width, ColorTexture.height, 16);

                Graphics.Blit(ColorTexture, rt1);
                
                var front = rt1;
                var back = rt2;
                foreach (var effect in m_RuntimeEffects)
                {
                    Graphics.Blit(front, back, effect);

                    // Swap buffers
                    var currentFront = front;
                    front = back;
                    back = currentFront;
                }

                Graphics.Blit(front, m_ColorRT);

                m_BgMaterialInstance.SetTexture(k_ColorTexHash, m_ColorRT);

                RenderTexture.ReleaseTemporary(rt1);
                RenderTexture.ReleaseTemporary(rt2);
            }   
        }
    }
}