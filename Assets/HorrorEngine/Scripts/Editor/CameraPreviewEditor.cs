using UnityEngine;
using UnityEditor;
using Unity.Cinemachine;

namespace HorrorEngine
{
    [CustomEditor(typeof(CameraPreview))]
    public class CameraPreviewEditor : Editor
    {
        private Vector2Int m_Aspect = new Vector2Int(16, 9);
        private CinemachineCamera m_VirtualCam;
        private float m_Size = 250;
        
        private Camera m_Camera;

        // --------------------------------------------------------------------

        public void OnEnable()
        {
            if (!Application.isPlaying)
            {
                var preview = target as CameraPreview;
                m_VirtualCam = preview.GetComponent<CinemachineCamera>();
                float aspect = m_VirtualCam.Lens.Aspect;
                
                var camGO = EditorUtility.CreateGameObjectWithHideFlags("Preview Scene Camera", HideFlags.HideAndDontSave, typeof(Camera));
                m_Camera = camGO.GetComponent<Camera>();
                m_Camera.cameraType = CameraType.Preview;
                m_Camera.enabled = false;
                m_Camera.clearFlags = CameraClearFlags.Skybox;
                m_Camera.forceIntoRenderTexture = true;
                m_Camera.renderingPath = RenderingPath.Forward;
                m_Camera.useOcclusionCulling = false;
                m_Camera.scene = preview.gameObject.scene;

                EditorApplication.playModeStateChanged += OnPlayModeChanged;
            }
        }

        // --------------------------------------------------------------------

        private void Render()
        {
            
            float aspect = (float)Mathf.Max(m_Aspect.x, 1) / Mathf.Max(m_Aspect.y, 1);
            m_Camera.targetTexture = RenderTexture.GetTemporary((int)(m_Size * aspect), (int)m_Size, 16);
            m_Camera.transform.position = m_VirtualCam.transform.position;
            m_Camera.transform.rotation = m_VirtualCam.transform.rotation;
            m_Camera.fieldOfView = m_VirtualCam.Lens.FieldOfView;
            m_Camera.farClipPlane = m_VirtualCam.Lens.FarClipPlane;
            m_Camera.nearClipPlane = m_VirtualCam.Lens.NearClipPlane;

            m_Camera.Render();
            
        }

        // --------------------------------------------------------------------

        private void OnDisable()
        {
            ReleaseCamera();
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        // --------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                m_Aspect = EditorGUILayout.Vector2IntField("Aspect", m_Aspect);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box("", GUILayout.Height(m_Size));
                EditorGUILayout.EndHorizontal();
                Rect scale = GUILayoutUtility.GetLastRect();
                if (m_Camera)
                {
                    Render();
                    EditorGUI.DrawTextureTransparent(scale, m_Camera.targetTexture, ScaleMode.ScaleToFit);
                }

                if (GUILayout.Button(new GUIContent("Go to POV", "Moves the editor camera to this POV")))
                {
                    var sceneView = SceneView.lastActiveSceneView;

                    sceneView.camera.transform.position = m_VirtualCam.transform.position;
                    sceneView.camera.transform.rotation = m_VirtualCam.transform.rotation;
                    sceneView.camera.fieldOfView = 10;
                    sceneView.AlignViewToObject(sceneView.camera.transform);
                    
                }

                if (GUILayout.Button(new GUIContent("Move to viewport POV", "Moves the camera to the scene view position/rotation")))
                {
                    var sceneView = SceneView.lastActiveSceneView;

                    m_VirtualCam.transform.position = sceneView.camera.transform.position;
                    m_VirtualCam.transform.rotation = sceneView.camera.transform.rotation;

                    EditorUtility.SetDirty(m_VirtualCam.transform);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Camera preview is not available in play mode", MessageType.Info);
            }
        }

        // --------------------------------------------------------------------

        private void ReleaseCamera()
        {
            if (m_Camera)
            {
                DestroyImmediate(m_Camera.gameObject);
                m_Camera = null;
            }
        }

        // --------------------------------------------------------------------

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                ReleaseCamera();
            }
        }
    }
}
