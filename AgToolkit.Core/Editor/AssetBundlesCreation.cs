using System.IO;
using UnityEditor;
using UnityEngine;

namespace AgToolkit.Core.Editor
{
    public class AssetBundlesCreation
    {
    #if UNITY_EDITOR
        private static string _AssetBundleDirectory = "Assets/StreamingAssets";

        [MenuItem("Assets/Create/AgToolkit/AssetBundle/CreateBundleName")]
        public static void CreateBundleName() 
        {
            string dirPath = AssetDatabase.GetAssetPath(Selection.GetFiltered<Object>(SelectionMode.Assets)[0]);

            if (string.IsNullOrEmpty(dirPath)) return;

            string dirName = new DirectoryInfo(dirPath).Name;

            foreach (string asset in Directory.GetFiles(dirPath, "*.asset"))
            {
                AssetImporter.GetAtPath(asset).assetBundleName = dirName;
            }
        }

        [MenuItem("Assets/Create/AgToolkit/AssetBundle/CreateBundleNameRecursively")]
        public static void CreateBundleNameRecursively() 
        {
            string dirPath = AssetDatabase.GetAssetPath(Selection.GetFiltered<Object>(SelectionMode.Assets)[0]);

            if (string.IsNullOrEmpty(dirPath)) return;

            string dirName = new DirectoryInfo(dirPath).Name;

            foreach (string dir in Directory.GetDirectories(dirPath))
            {
                foreach (string asset in Directory.GetFiles(dir, "*.asset"))
                {
                    AssetImporter.GetAtPath(asset).assetBundleName = dirName;
                }
            }
        }

        [MenuItem("AgToolkit/AssetBundle/Build")]
        public static void BuildAllAssetBundles()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(_AssetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(_AssetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
    #endif
    }
}