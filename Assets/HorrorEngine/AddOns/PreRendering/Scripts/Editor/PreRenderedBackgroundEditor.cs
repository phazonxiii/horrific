using UnityEngine;
using UnityEditor;
using Unity.Cinemachine;

namespace PreRenderBackgrounds
{
    [CustomEditor(typeof(PreRenderedBackground))]
    public class PreRenderedBackgroundEditor : Editor
    {
        [SerializeField] private Material m_RenderBgMaterial;
        [SerializeField] private Shader m_Shader;
        
        private float m_Size = 250;

        // --------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PreRenderedBackground prBg = target as PreRenderedBackground;
            CinemachineCamera virtualCam = prBg.GetComponentInParent<CinemachineCamera>(true);

            if (!virtualCam)
            {
                EditorGUILayout.HelpBox("Parent virtual camera not found", MessageType.Error);
                return;
            }

            EditorGUILayout.Separator();

            if (!Application.isPlaying)
            {
                if (GUILayout.Button("Create"))
                {
                    string basePath = prBg.ColorTexture ? AssetDatabase.GetAssetPath(prBg.ColorTexture) : "";
                    string path = EditorUtility.SaveFolderPanel("Select output folder", basePath, "");
                    if (path.Length != 0)
                    {
                        path = path.Substring(path.IndexOf("Assets/")) + "/";
                            
                        prBg.ColorTexture = new Texture2D(prBg.Resolution.x, prBg.Resolution.y, TextureFormat.RGB24, false);
                        prBg.DepthRenderTexture = new RenderTexture(prBg.Resolution.x, prBg.Resolution.y, 16);
                        prBg.DepthRenderTexture.format = RenderTextureFormat.Depth;

                        string name = prBg.Name + "_PRBg";
                        AssetDatabase.CreateAsset(prBg.ColorTexture, path + name + "_Color.asset");
                        AssetDatabase.CreateAsset(prBg.DepthRenderTexture, path + name + "_Depth.asset");
                        AssetDatabase.Refresh();

                        EditorUtility.SetDirty(prBg);

                        Render(prBg);
                    }
                }
            }

            EditorGUILayout.Separator();

            if (prBg.ColorTexture && prBg.DepthRenderTexture)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box("", GUILayout.Height(m_Size));
                EditorGUILayout.EndHorizontal();
                Rect scale = GUILayoutUtility.GetLastRect();
                EditorGUI.DrawTextureTransparent(scale, prBg.ColorTexture, ScaleMode.ScaleToFit);
            }

            if (!Application.isPlaying && prBg.ColorTexture && prBg.DepthRenderTexture)
            {
                if (GUILayout.Button("Render"))
                {
                    Render(prBg);
                    EditorUtility.SetDirty(prBg);
                }
            }
        }

        // --------------------------------------------------------------------

        private void Render(PreRenderedBackground prBg)
        {
            var camGO = EditorUtility.CreateGameObjectWithHideFlags("Bg Render Camera", HideFlags.HideAndDontSave, typeof(Camera));
            var camera = camGO.GetComponent<Camera>();
            prBg.Render(camera);

            EditorUtility.SetDirty(prBg.ColorTexture);

            AssetDatabase.SaveAssetIfDirty(prBg.ColorTexture);
            AssetDatabase.SaveAssetIfDirty(prBg.DepthRenderTexture);
        }

    }
}
