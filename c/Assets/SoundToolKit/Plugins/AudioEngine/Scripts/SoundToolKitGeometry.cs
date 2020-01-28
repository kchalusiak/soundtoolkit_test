/// \author Michal Majewski
/// \date 04.02.2019
///
using SoundToolKit.Unity.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// SoundToolKitGeometry governs loading and reloading of acoustic mesh clusters to SoundToolKit.Scene
    /// </summary>
    [System.Serializable]
    [AddComponentMenu("SoundToolKit/DefaultPrefabComponents/SoundToolKitGeometry")]
    public class SoundToolKitGeometry : MonoBehaviour, ISoundToolKitObserver
    {
        #region editor fields

        [SerializeField]
        private List<SoundToolKitMeshCluster> m_meshClusters = new List<SoundToolKitMeshCluster>();

        #endregion

        #region public properties

        /// <summary>
        /// Clusters of Acoustic Meshes that are loaded to the acoustic scene on the scene start.
        /// </summary>
        public List<SoundToolKitMeshCluster> MeshClusters
        {
            get { return m_meshClusters; }
            set
            {
                if (value != m_meshClusters)
                {
                    m_meshClusters = value;

#if UNITY_EDITOR
                    SynchronizeLoadedClusters();
                    m_previousClusters = value;
#endif
                }
            }
        }

        public bool Initialized { get; private set; }

        #endregion

        #region public methods

        public void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadSceneMode) =>
            {
                InitializeMeshClusters();
            };

            SoundToolKitManager.Instance.ResourceContainer.Geometry = this;
        }

        /// <summary>
        /// Load a given MeshCluster into the scene. Only MeshClusters that are loaded take part in acoustic processing.
        /// </summary>
        public void LoadMeshCluster(SoundToolKitMeshCluster meshCluster)
        {
            if (meshCluster == null)
            {
                MeshClusters.Remove(meshCluster);
                return;
            }

            if (!meshCluster.AcousticMeshNames.Any())
            {
                SoundToolKitDebug.LogWarning("Cannot load a MeshCluster that contains no SoundToolKitMeshes. " +
                    "Please assign SoundToolKitMeshes to a cluster prior to loading it. (Trying to load " +
                    meshCluster.name + " MeshCluster)");
                return;
            }

            if (meshCluster.Model == null)
            {
                meshCluster.CreateInternalModel();
            }

            SoundToolKitManager.Instance.StkAudioEngine.Scene.AddModel(meshCluster.Model);
            meshCluster.OnMeshesModified += ReloadMeshCluster;
        }

        /// <summary>
        /// Unloads a given MeshCluster from the scene. Unloaded Clusters do not take part in sound processing.
        /// </summary>
        public void UnloadMeshCluster(SoundToolKitMeshCluster meshCluster)
        {
            if (meshCluster != null && meshCluster.Model != null && SoundToolKitManager.Instance != null)
            {
                SoundToolKitManager.Instance.StkAudioEngine.Scene.RemoveModel(meshCluster.Model);
            }

            meshCluster.OnMeshesModified -= ReloadMeshCluster;
            meshCluster.ClearInternalModel();
        }

        #endregion

        #region internal properties

        internal MeshDeletionUpdater MeshDeletionUpdater { get; private set; }

        #endregion

        #region internal methods

        internal void InitializeMeshClusters()
        {
            if (!Initialized && SoundToolKitManager.Instance.ResourceContainer.AcousticMeshes.Any())
            {
                InitializeTemporaryMeshCluster();

                MeshClusters.ForEach(x =>
                {
                    if (x != null)
                    {
                        LoadMeshCluster(x);
                    }
                });

                Initialized = true;
            }
        }

        internal SoundToolKitMeshCluster GetTemporaryMeshCluster()
        {
            if (m_temporaryCluster == null)
            {
                InitializeMeshClusters();
            }

            return m_temporaryCluster;
        }

        internal void ClearTemporaryMeshCluster()
        {
            m_temporaryCluster.AcousticMeshNames.Clear();
        }

        internal void ReloadMeshCluster(SoundToolKitMeshCluster meshCluster)
        {
            var scene = SoundToolKitManager.Instance.StkAudioEngine.Scene;

            meshCluster.ClearSerializedData();

            if (meshCluster.Model != null)
            {
                scene.RemoveModel(meshCluster.Model);
                meshCluster.ClearInternalModel();
            }

            if (meshCluster.AcousticMeshNames.Any())
            {
                meshCluster.CreateInternalModel();
                scene.AddModel(meshCluster.Model);
            }
        }

        internal void ClearGeometry()
        {
            MeshClusters.ForEach(x =>
            {
                if (x != null)
                {
                    UnloadMeshCluster(x);
                }
            });
            SoundToolKitManager.Instance.StkAudioEngine.Scene.Clear();
            SoundToolKitManager.Instance.ResourceContainer.Geometry = null;
        }

        internal void SerializeGeometry()
        {
            MeshClusters.ForEach(x =>
            {
                if (x != null && x != m_temporaryCluster)
                {
                    x.Serialize();
                }
                else if (x == m_temporaryCluster)
                {
                    SoundToolKitDebug.Log("Temporary Mesh Cluster can't be serialized.");
                }
            });
        }

        #endregion

        #region private methods

        private void Awake()
        {
            Initialized = false;
            m_temporaryCluster = null;
            MeshDeletionUpdater = new MeshDeletionUpdater();
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
        }

        private void Start()
        {
            MeshClusters.ForEach(x =>
            {
                if (x != null && x != m_temporaryCluster)
                {
                    x.SaveStateForPlaymode();
                }
            });
        }

        private void Update()
        {
            MeshDeletionUpdater.UpdateClusters();

#if UNITY_EDITOR
            if (m_meshClusters != m_previousClusters)
            {
                SynchronizeLoadedClusters();
            }
#endif
        }

        private void OnDestroy()
        {
            if (SoundToolKitManager.Instance != null)
            {
                ClearGeometry();
            }

            MeshClusters.ForEach(x =>
            {
                if (x != null && x != m_temporaryCluster)
                {
                    x.RestoreStateForEditmode();
                }
            });

            m_temporaryCluster = null;
            MeshDeletionUpdater.Destroy();
        }

        private void InitializeTemporaryMeshCluster()
        {
            var meshes = SoundToolKitManager.Instance.ResourceContainer.AcousticMeshes;

            if (meshes.Any(x => x.MeshCluster == null))
            {
                m_temporaryCluster = ScriptableObject.CreateInstance<SoundToolKitMeshCluster>();
                m_temporaryCluster.name = "Temporary Mesh Cluster";
                MeshClusters.Add(m_temporaryCluster);

                SoundToolKitDebug.Log("Temporary SoundToolKitMeshCluster created for the duration of the playtime " +
                    "in order to load all of the SoundToolKitMeshes that have no MeshCluster assigned. \n" +
                    "To make this permanent and suppress this message use the menu " +
                    "Tools/SoundToolKit/Geometry/SynchronizeDefaultGeometry after exiting the PlayMode. \n" +
                    "In order not to load specific SoundToolKitMeshes, assign them to " +
                    "a MeshCluster that is not added to the SoundToolKitGeometry's MeshClusters field.");

                foreach (var mesh in meshes)
                {
                    if (mesh.MeshCluster == null)
                    {
                        mesh.MeshCluster = MeshClusters.Last();
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void SynchronizeLoadedClusters()
        {
            m_previousClusters.ForEach(x =>
            {
                if (x != null && !m_meshClusters.Contains(x) && x.Model != null)
                {
                    UnloadMeshCluster(x);
                }
            });

            m_meshClusters.ForEach(x =>
            {
                if (x != null && !m_previousClusters.Contains(x) && x.Model == null)
                {
                    LoadMeshCluster(x);
                }
            });

            m_previousClusters = new List<SoundToolKitMeshCluster>(m_meshClusters);
        }
#endif

#endregion

        #region private members

        private SoundToolKitMeshCluster m_temporaryCluster;

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private List<SoundToolKitMeshCluster> m_previousClusters = new List<SoundToolKitMeshCluster>();
#endif

#endregion
    }
}
