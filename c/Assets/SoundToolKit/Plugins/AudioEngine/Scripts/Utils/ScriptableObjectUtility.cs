/// \author Marcin Misiek
/// \date 20.06.2018

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    public static class ScriptableObjectUtility
    {
        /// <summary>
        // This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAsset<T>(string assetPath, string fileName) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(assetPath + "/" + fileName + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
    }
}
#endif