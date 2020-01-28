/// \author Marcin Misiek
/// \date 20.06.2018 - 19.07.2018
/// 
/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 18.02.2019

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    public class SoundToolKitMaterialAssetLoader
    {
        #region public methods

        /// <summary>
        /// Creates the SoundToolKitMaterial assets for each AcousticMaterial config file in the 
        /// streaming assets directory and deletes StkMaterials that have no corresponding config file.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Materials/Synchronize")]
        public static void Synchronize()
        {
            var soundToolKitMaterial = AssetManager.FindAssets<SoundToolKitMaterial>();
            var relativeJsonPaths = new List<string>();

            foreach (var json in Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, SOUNDTOOLKIT_JSON_MATERIALS_PATH), JSON_EXTENSION, SearchOption.AllDirectories))
            {
                var directoryRelativeToRoot = Path.GetDirectoryName(json.Substring(json.IndexOf(MATERIAL_DIRECTORY) + MATERIAL_DIRECTORY.Length));
                var materialDirectory = Path.Combine(SOUNDTOOLKIT_MATERIALS_PATH, directoryRelativeToRoot);

                var relativeJsonPath = json.Substring(json.IndexOf(SOUNDTOOLKIT_JSON_MATERIALS_PATH));

                if (!soundToolKitMaterial.Any(x => x.FilePath == relativeJsonPath))
                {
                    Directory.CreateDirectory(materialDirectory);

                    CreateAcousticMaterial(materialDirectory, relativeJsonPath);
                }

                relativeJsonPaths.Add(relativeJsonPath);
            }

            var materialsToRemove = soundToolKitMaterial.Where(x => !relativeJsonPaths.Contains(x.FilePath)).ToList();
            materialsToRemove.ForEach(x => AssetManager.Delete(x));

            AssetManager.RemoveEmptySubDirectories(SOUNDTOOLKIT_MATERIALS_PATH);
            AssetManager.Update();
        }

        /// <summary>
        /// Material Synchronization button should only be available in EditMode.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Materials/Synchronize", true)]
        public static bool ValidateSyncButton()
        {
            return !EditorApplication.isPlaying;
        }

        public static void SerializeToFile(SoundToolKitMaterial material)
        {
            if (SoundToolKitManager.Instance != null && material != null)
            {
                // Create json file with serialized material
                var customJsonDirectory = Path.Combine(Path.Combine(Application.streamingAssetsPath, SOUNDTOOLKIT_JSON_MATERIALS_PATH), "Custom");
                Directory.CreateDirectory(customJsonDirectory);
                var name = "Custom " + material.name;

                var serializedData = SoundToolKit.Extensions.Scene.MaterialsSerializer.Serialize(SoundToolKitManager.Instance.StkAudioEngine, material.GetStkMaterial());

                customJsonDirectory = Path.Combine(customJsonDirectory, name);
                customJsonDirectory = Path.ChangeExtension(customJsonDirectory, ".json");

                File.WriteAllText(customJsonDirectory, serializedData);

                // Create SoundToolKitMaterial Asset from a newly serialized material
                var directoryRelativeToRoot = Path.GetDirectoryName(customJsonDirectory.Substring(
                    customJsonDirectory.IndexOf(MATERIAL_DIRECTORY) + MATERIAL_DIRECTORY.Length));

                var materialDirectory = Path.Combine(SOUNDTOOLKIT_MATERIALS_PATH, directoryRelativeToRoot);

                var relativeJsonPath = customJsonDirectory.Substring(
                    customJsonDirectory.IndexOf(SOUNDTOOLKIT_JSON_MATERIALS_PATH));

                Directory.CreateDirectory(materialDirectory);
                CreateAcousticMaterial(materialDirectory, relativeJsonPath);

                SoundToolKitDebug.Log(name + " SoundToolKitMaterial successfully created. " +
                    "Corresponding Material config file available in StreamingAssets/SoundToolKit/Materials/Custom");
            }
        }

        #endregion

        #region private methods

        private static void CreateAcousticMaterial(string assetPath, string relativePath)
        {
            var acousticMaterial = ScriptableObjectUtility.CreateAsset<SoundToolKitMaterial>(assetPath, fileName: Path.GetFileNameWithoutExtension(relativePath));
            acousticMaterial.FilePath = relativePath;

            EditorUtility.SetDirty(acousticMaterial);
        }

        #endregion

        #region private members

        private const string JSON_EXTENSION = "*.json";

        private const string MATERIAL_DIRECTORY = @"Materials\";

        private const string SOUNDTOOLKIT_JSON_MATERIALS_PATH = @"SoundToolKit\Materials";

        private const string SOUNDTOOLKIT_MATERIALS_PATH = @"Assets\SoundToolKit\Plugins\AudioEngine\Assets\Materials";

        #endregion
    }
}
#endif