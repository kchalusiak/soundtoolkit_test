/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 01.03.2019
///
#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    public class MeshClusterUtility
    {
        #region public methods

        /// <summary>
        /// Add all meshes that have no MeshCluster assigned to the default MeshCluster and add it to the StkManager.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Geometry/SynchronizeDefaultGeometry")]
        public static void SynchronzeDefaultMeshCluster()
        {
            var geometrySceneObject = Object.FindObjectOfType<SoundToolKitGeometry>();

            if (geometrySceneObject != null)
            {
                geometrySceneObject.MeshClusters.ForEach(x =>
                {
                    if (x == null)
                    {
                        geometrySceneObject.MeshClusters.Remove(x);
                    }
                });

                var oldDefaultCluster = geometrySceneObject.MeshClusters.FirstOrDefault(x => x.name == DefaultSceneClusterName());
                if (oldDefaultCluster)
                {
                    GetCorrespondingMeshes(oldDefaultCluster).ForEach(x =>
                    {
                        if (x != null)
                        {
                            x.MeshCluster = null;
                        }
                    });

                    geometrySceneObject.MeshClusters.Remove(oldDefaultCluster);
                }
            }
            else
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(GEOMETRY_PREFAB_PATH, typeof(GameObject)));
                geometrySceneObject = Object.FindObjectOfType<SoundToolKitGeometry>();

                if (geometrySceneObject == null)
                {
                    SoundToolKitDebug.LogWarning("SoundToolKitGeometry prefab failed to instantiate automatically.");
                }
                else
                {
                    EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                }
            }

            var cluster = GetDefaultMeshCluster();
            var meshes = Object.FindObjectsOfType<SoundToolKitMesh>();

            foreach (var mesh in meshes)
            {
                if (mesh.MeshCluster != null && mesh.MeshCluster.name == DefaultSceneClusterName())
                {
                    mesh.MeshCluster = null;
                }
            }

            foreach (var mesh in meshes)
            {
                if (mesh.MeshCluster == null)
                {
                    mesh.MeshCluster = cluster;
                }
            }

            if (geometrySceneObject != null)
            {
                geometrySceneObject.MeshClusters.Add(cluster);
                EditorUtility.SetDirty(geometrySceneObject);
            }

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        /// <summary>
        /// SynchronizeDefaultGeometry button should only be available in EditMode.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Geometry/SynchronizeDefaultGeometry", true)]
        public static bool ValidateSynchronizeButton()
        {
            return !EditorApplication.isPlaying;
        }

        /// <summary>
        /// Renames all the AcousticMeshes in a way that their CombinedName (name of the Mesh object
        ///  + name of the parent object if applicable) is unique within one MeshCluster.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Geometry/MakeMeshNamesUnique")]
        public static void MakeMeshNamesUnique()
        {
            var meshes = Object.FindObjectsOfType<SoundToolKitMesh>().ToList();

            foreach (var meshesWithSameCluster in meshes.GroupBy(x => x.MeshCluster))
            {
                var nameList = new List<string>();
                foreach (var mesh in meshesWithSameCluster)
                {
                    if (!nameList.Contains(mesh.CombinedName()))
                    {
                        nameList.Add(mesh.CombinedName());
                    }
                    else
                    {
                        var originalName = mesh.gameObject.name;

                        while (nameList.Contains(mesh.CombinedName()))
                        {
                            mesh.gameObject.name += "'";
                        }

                        SoundToolKitDebug.Log("Mesh: " + originalName + " renamed to: " + mesh.gameObject.name);

                        EditorUtility.SetDirty(mesh.gameObject);
                        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

                        nameList.Add(mesh.CombinedName());
                    }
                }
            }
        }

        /// <summary>
        /// MakeMeshNamesUnique button should only be available in EditMode.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Geometry/MakeMeshNamesUnique", true)]
        public static bool ValidateRenameButton()
        {
            return !EditorApplication.isPlaying;
        }

        #endregion

        #region internal methods

        internal static SoundToolKitMeshCluster GetDefaultMeshCluster()
        {
            var databasePath = DEFAULT_MESH_CLUSTER_PATH + @"\" + DefaultSceneClusterName() + ".asset";

            if (File.Exists(DefaultClusterFilePath()))
            {
                AssetDatabase.DeleteAsset(databasePath);
            }

            Directory.CreateDirectory(DEFAULT_MESH_CLUSTER_PATH);
            AssetDatabase.Refresh();

            var defaultCluster = ScriptableObject.CreateInstance<SoundToolKitMeshCluster>();

            AssetDatabase.CreateAsset(defaultCluster, databasePath);

            EditorUtility.SetDirty(defaultCluster);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return defaultCluster;
        }

        internal static SerializedGeometry CreateSerializedGeometry(string filename)
        {
            var geometry = ScriptableObject.CreateInstance<SerializedGeometry>();
            var path = DEFAULT_MESH_CLUSTER_PATH + @"\" + filename + ".asset";

            AssetDatabase.CreateAsset(geometry, path);

            EditorUtility.SetDirty(geometry);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return geometry;
        }

        internal static List<SoundToolKitMesh> GetCorrespondingMeshes(SoundToolKitMeshCluster meshCluster)
        {
            var meshes = new List<SoundToolKitMesh>();
            var sceneMeshes = Object.FindObjectsOfType<SoundToolKitMesh>();

            meshCluster.AcousticMeshNames.ForEach(meshName =>
            {
                var match = sceneMeshes.ToList().FirstOrDefault(x => x.name == meshName);
                if (match != null)
                {
                    meshes.Add(match);
                }
            });

            return meshes;
        }

        #endregion

        #region private methods

        private static string DefaultClusterFilePath()
        {
            var defaultClusterFolder = Path.Combine(Application.dataPath, DEFAULT_MESH_CLUSTER_NO_ASSETS_PATH);
            var defaultClusterFilePath = Path.Combine(defaultClusterFolder, DefaultSceneClusterName());
            return Path.ChangeExtension(defaultClusterFilePath, ".asset");
        }

        private static string DefaultSceneClusterName()
        {
            var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            return DEFAULT_MESH_CLUSTER_NAME + "_" + sceneName;
        }

        #endregion

        #region private members

        private const string DEFAULT_MESH_CLUSTER_PATH = @"Assets\SoundToolKit\Plugins\AudioEngine\Assets\MeshClusters";

        private const string DEFAULT_MESH_CLUSTER_NO_ASSETS_PATH = @"SoundToolKit\Plugins\AudioEngine\Assets\MeshClusters";

        private const string DEFAULT_MESH_CLUSTER_NAME = @"DefaultMeshCluster";

        private const string GEOMETRY_PREFAB_PATH = @"Assets\SoundToolKit\Plugins\AudioEngine\Prefabs\SoundToolKitGeometry.prefab";

        #endregion
    }
}
#endif