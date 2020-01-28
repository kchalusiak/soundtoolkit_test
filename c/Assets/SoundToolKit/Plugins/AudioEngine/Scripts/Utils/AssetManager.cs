/// \author Marcin Misiek
/// \date 19.07.2018

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    public class AssetManager
    {
        public static List<T> FindAssets<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name).ToList();
            var assets = new List<T>();

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));

                assets.Add(asset);
            }

            return assets;
        }

        public static void Delete<T>(T asset) where T : Object
        {
            var assetPath = AssetDatabase.GetAssetPath(asset.GetInstanceID());

            AssetDatabase.DeleteAsset(assetPath);
        }

        public static void Update()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RemoveEmptySubDirectories(string rootPath)
        {
            foreach (string subdirectory in Directory.GetDirectories(rootPath))
            {
                string[] files = Directory.GetFiles(subdirectory, "*.*");

                if (files.Length == 0)
                {
                    Directory.Delete(subdirectory);
                }
            }
        }
    }
}
#endif