using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HorrorEngine
{
    public class HorrorEngineScripts
    {
        /*
        [MenuItem("Horror Engine/Scripts/Migrate Scene Unlocalized Data")]
        public static void MigrateSceneUnlocalizedData()
        {
            // Find all Choice components in the scene
            Choice[] choices = UnityEngine.Object.FindObjectsOfType<Choice>();
            foreach (var choice in choices)
            {
                choice.EditorOnly_MigrateUnlocalizedData();
            }
        }

        [MenuItem("Horror Engine/Scripts/Migrate Assets Unlocalized Data")]
        public static void MigrateAssetsUnlocalizedData()
        {
            MigrateUnlocalizedItemData();
            MigrateUnlocalizedMapData();
            MigrateUnlocalizedInteractionData();

            // Save changes to the AssetDatabase
            AssetDatabase.SaveAssets();
        }

        public static void MigrateUnlocalizedItemData()
        {
            // Find all ItemData assets in the project
            string[] guids = AssetDatabase.FindAssets("t:ItemData");
            if (guids.Length == 0)
            {
                Debug.LogWarning("No ItemData assets found in the project.");
                return;
            }

            foreach (string guid in guids)
            {
                // Get the path of the asset
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Load the asset
                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (itemData == null)
                {
                    Debug.LogError($"Failed to load ItemData at path: {path}");
                    continue;
                }

                // Record changes for undo functionality
                Undo.RecordObject(itemData, "Migrate ItemData");

                itemData.EditorOnly_MigrateUnlocalizedData();

            }
        }

        public static void MigrateUnlocalizedMapData()
        {
            // Find all MapData assets in the project
            string[] guids = AssetDatabase.FindAssets("t:MapData");
            if (guids.Length == 0)
            {
                Debug.LogWarning("No MapData assets found in the project.");
                return;
            }

            foreach (string guid in guids)
            {
                // Get the path of the asset
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Load the asset
                MapData MapData = AssetDatabase.LoadAssetAtPath<MapData>(path);
                if (MapData == null)
                {
                    Debug.LogError($"Failed to load MapData at path: {path}");
                    continue;
                }

                // Record changes for undo functionality
                Undo.RecordObject(MapData, "Migrate MapData");

                MapData.EditorOnly_MigrateUnlocalizedData();

            }
        }

        private static void MigrateUnlocalizedInteractionData()
        {
            // Find all ItemData assets in the project
            string[] guids = AssetDatabase.FindAssets("t:InteractionData");
            if (guids.Length == 0)
            {
                Debug.LogWarning("No InteractionData assets found in the project.");
                return;
            }

            foreach (string guid in guids)
            {
                // Get the path of the asset
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Load the asset
                InteractionData interactionData = AssetDatabase.LoadAssetAtPath<InteractionData>(path);
                if (interactionData == null)
                {
                    Debug.LogError($"Failed to load ItemData at path: {path}");
                    continue;
                }

                // Record changes for undo functionality
                Undo.RecordObject(interactionData, "Migrate InteractionData");

                interactionData.EditorOnly_MigrateUnlocalizedData();

            }
        }
        */
    }
}