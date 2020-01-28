/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 09.05.2019
///
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using SoundToolKit.Unity.Extensions;
using System.IO;
using UnityEditor.SceneManagement;

namespace SoundToolKit.Unity.Utils
{
    public static class SoundToolKitPrefabUtility
    {
        #region public methods

        /// <summary>
        /// Instantiate and place on the scene all of the SoundToolKit provided prefabs that are not currently present on the scene.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Prefabs/InstantiateAll")]
        public static void InstantiateAllStkPrefabs()
        {
            InstantiateExtensions();
            InstantiateGeometry();
            InstantiateManager();
            InstantiateSceneConfig();
            InstantiateStatus();
            InstantiateAI();
        }

        /// <summary>
        /// Instantiate and place on the scene the SoundToolKitExtensions prefab, composed of Sound Path and Acoustic Mesh Renderers.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Prefabs/InstantiateExtensions")]
        public static void InstantiateExtensions()
        {
            if (Object.FindObjectOfType<SoundPathRenderer>() == null && Object.FindObjectOfType<AcousticMeshRenderer>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(
                    Path.Combine(PREFABS_PATH, EXTENSIONS), typeof(GameObject)));

                SoundToolKitDebug.Log(EXTENSIONS + " instatntiated and placed on the scene.");
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
            else
            {
                SoundToolKitDebug.Log("SoundPathRenderer and/or AcousticMeshDrawer present on the scene. " +
                    "Additional " + EXTENSIONS + " not instantiated.");
            }
        }

        /// <summary>
        /// Instantiate and place on the scene the SoundToolKitGeometry prefab.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Prefabs/InstantiateGeometry")]
        public static void InstantiateGeometry()
        {
            if (Object.FindObjectOfType<SoundToolKitGeometry>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(
                    Path.Combine(PREFABS_PATH, GEOMETRY), typeof(GameObject)));

                SoundToolKitDebug.Log(GEOMETRY + " instatntiated and placed on the scene.");
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
            else
            {
                SoundToolKitDebug.Log("SoundToolKitGeometry component present on the scene. " +
                    "Additional " + GEOMETRY + " not instantiated.");
            }
        }

        /// <summary>
        /// Instantiate and place on the scene the SoundToolKitManager prefab.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Prefabs/InstantiateManager")]
        public static void InstantiateManager()
        {
            if (Object.FindObjectOfType<SoundToolKitManager>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(
                    Path.Combine(PREFABS_PATH, MANAGER), typeof(GameObject)));

                SoundToolKitDebug.Log(MANAGER + " instatntiated and placed on the scene.");
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
            else
            {
                SoundToolKitDebug.Log("SoundToolKitManager component present on the scene. " +
                    "Additional " + MANAGER + " not instantiated.");
            }
        }

        /// <summary>
        /// Instantiate and place on the scene the SoundToolKitSceneConfiguration prefab.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Prefabs/InstantiateSceneConfig")]
        public static void InstantiateSceneConfig()
        {
            if (Object.FindObjectOfType<SoundToolKitSceneConfiguration>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(
                    Path.Combine(PREFABS_PATH, SCENE_CONFIG), typeof(GameObject)));

                SoundToolKitDebug.Log(SCENE_CONFIG + " instatntiated and placed on the scene.");
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
            else
            {
                SoundToolKitDebug.Log("SoundToolKitSceneConfiguration component present on the scene. " +
                    "Additional " + SCENE_CONFIG + " not instantiated.");
            }
        }

        /// <summary>
        /// Instantiate and place on the scene the SoundToolKitStatus prefab.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Prefabs/InstantiateStatus")]
        public static void InstantiateStatus()
        {
            if (Object.FindObjectOfType<SoundToolKitStatus>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(
                    Path.Combine(PREFABS_PATH, STATUS), typeof(GameObject)));

                SoundToolKitDebug.Log(STATUS + " instatntiated and placed on the scene.");
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
            else
            {
                SoundToolKitDebug.Log("SoundToolKitStatus component present on the scene. " +
                    "Additional " + STATUS + " not instantiated.");
            }
        }

        /// <summary>
        /// Instantiate and place on the scene the SoundToolKitAI prefab.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Prefabs/InstantiateStkAI")]
        public static void InstantiateAI()
        {
            if (Object.FindObjectOfType<SoundToolKitAI>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(
                    Path.Combine(PREFABS_PATH, AI), typeof(GameObject)));

                SoundToolKitDebug.Log(AI + " instatntiated and placed on the scene.");
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
            else
            {
                SoundToolKitDebug.Log("SoundToolKitAI component present on the scene. " +
                    "Additional " + AI + " not instantiated.");
            }
        }

        #endregion

        #region private members

        private const string PREFABS_PATH = @"Assets\SoundToolKit\Plugins\AudioEngine\Prefabs";

        private const string EXTENSIONS = @"SoundToolKitExtensions.prefab";
        private const string GEOMETRY = @"SoundToolKitGeometry.prefab";
        private const string MANAGER = @"SoundToolKitManager.prefab";
        private const string SCENE_CONFIG = @"SoundToolKitSceneConfig.prefab";
        private const string STATUS = @"SoundToolKitStatus.prefab";
        private const string AI = @"SoundToolKitAI.prefab";

        #endregion
    }
}
#endif