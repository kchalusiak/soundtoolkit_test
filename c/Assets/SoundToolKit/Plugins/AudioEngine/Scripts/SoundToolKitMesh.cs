/// \author Marcin Misiek
/// \date 14.06.2018

using SoundToolKit.Unity.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// StkMesh convert UnityMesh to triangles which can be used by Stk to create acoustic geometry.
    /// This component should be placed with a UnityMesh. This mesh will be automatically converted to 
    /// acoustic geometry in Stk.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(Transform))]
    [AddComponentMenu("SoundToolKit/SoundToolKitMesh")]
    public sealed class SoundToolKitMesh : MonoBehaviour, ISoundToolKitObserver
    {
        #region editor fields

        [SerializeField]
        private SoundToolKitMaterial m_acousticMaterial = null;

        [SerializeField]
        private bool m_enabledAsAcousticObstacle = true;

        [SerializeField]
        private SoundToolKitMeshCluster m_meshCluster = null;

        #endregion

        #region public properties

        /// <summary>
        /// UnityMesh currently in use by Stk.
        /// </summary>
        public UnityEngine.Mesh Mesh
        {
            get { return m_mesh; }
            private set
            {
                if (value != Mesh && value != null)
                {
                    m_mesh = value;

                    if (!m_mesh.isReadable)
                    {
                        SoundToolKitDebug.LogError("UnityMesh: " + m_mesh.name + " is not readable change Read/Write must be enabled in settings.");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// This method can disable this mesh as an obstacle for sound paths in STK Engine processing.
        /// Enabled by default.
        /// </summary>
        public bool EnabledAsAcousticObstacle
        {
            get { return m_acousticMesh != null ? m_acousticMesh.EnabledAsObstacle : m_enabledAsAcousticObstacle; }
            set
            {
                m_enabledAsAcousticObstacle = value;

                if (m_acousticMesh != null)
                {
                    m_acousticMesh.EnabledAsObstacle = m_enabledAsAcousticObstacle;
                }
            }
        }

        /// <summary>
        /// Converted faces of UnityMesh
        /// </summary>
        public ReadOnlyCollection<Vector3> Faces
        {
            get { return m_faceVertices.AsReadOnly(); }
        }

        /// <summary>
        /// StkMaterial which is used on this mesh.
        /// </summary>
        public SoundToolKitMaterial Material
        {
            get { return m_acousticMaterial; }
            set
            {
                m_acousticMaterial = value;

                if (m_acousticMesh != null && m_acousticMaterial != null)
                {
                    m_acousticMesh.Material = m_acousticMaterial.GetStkMaterial();
                }
            }
        }

        /// <summary>
        /// Mesh Cluster that this mesh is part of.
        /// </summary>
        public SoundToolKitMeshCluster MeshCluster
        {
            get { return m_meshCluster; }
            set
            {
                if (value != m_previousCluster)
                {
                    if (m_previousCluster != null)
                    {
                        m_previousCluster.Remove(CombinedName());
                    }

                    m_meshCluster = value;

                    if (m_meshCluster != null)
                    {
                        m_meshCluster.Add(CombinedName());
                    }

                    m_previousCluster = m_meshCluster;
                }
            }
        }

        public bool Initialized { get; private set; }

        #endregion

        #region public methods 

        public void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                SoundToolKitDebug.Assert(soundToolKitManager.StkAudioEngine != null, "Audio engine is not initialized. Logical error.");

                m_acousticMesh = soundToolKitManager.StkAudioEngine.ResourcesFactory.CreateMesh();
                m_acousticMesh.Name = gameObject.name;

                UpdateProperties();

                var mesh = GetComponent<MeshFilter>().mesh;
                if (mesh != null)
                {
                    Mesh = mesh;
                }
                else
                {
                    SoundToolKitDebug.LogWarning("UnityEngine.Mesh not present in this component. Cannot AutoRegister.");
                }

                soundToolKitManager.ResourceContainer.Add(acousticMesh: this);

                if (soundToolKitManager.ResourceContainer.Geometry != null && MeshCluster == null)
                {
                    MeshCluster = soundToolKitManager.ResourceContainer.Geometry.GetTemporaryMeshCluster();
                }

                Initialized = true;
            }
        }

        #endregion

        #region internal methods

        internal Mesh GetStkMesh()
        {
            return m_acousticMesh;
        }

        internal void SetFaces()
        {
            if (Mesh == null)
            {
                SoundToolKitDebug.LogError("Trying to add an object that does not contain a valid MeshFilter " +
                    "to the acoustic geometry of the scene (object: " + name + "). Aborting.");
                return;
            }

            var extractor = new MeshTrianglesExtractor(m_mesh, transform);
            m_faceVertices = extractor.TriangleVertices;

            var faceVerticesNumerics = new List<Numerics.Vector3>();
            m_faceVertices.ForEach(vertex => faceVerticesNumerics.Add(Vector3Extensions.ToNumerics(vertex)));

            m_acousticMesh.FaceVertices = faceVerticesNumerics;
        }

        internal string CombinedName()
        {
            var parent = gameObject.transform.parent;
            if (parent != null)
            {
                return parent.gameObject.name + "_" + gameObject.name;
            }
            else
            {
                return gameObject.name;
            }
        }

        #endregion

        #region private methods

        private void Awake()
        {
            Initialized = false;
            m_previousCluster = m_meshCluster;
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
        }

        private void OnDisable()
        {
            if (m_acousticMesh.EnabledAsObstacle)
            {
                m_acousticMesh.EnabledAsObstacle = false;

                m_enabledAsObstaclePreviousState = true;
            }
            else
            {
                m_enabledAsObstaclePreviousState = false;
            }
        }

        private void OnEnable()
        {
            if (m_enabledAsObstaclePreviousState)
            {
                m_acousticMesh.EnabledAsObstacle = true;
            }
        }

        private void OnDestroy()
        {
            var soundToolKitManager = SoundToolKitManager.Instance;

            if (soundToolKitManager != null)
            {
                var geometry = soundToolKitManager.ResourceContainer.Geometry;
                if (geometry != null && MeshCluster != null)
                {
                    geometry.MeshDeletionUpdater.QueueForUpdate(CombinedName(), MeshCluster);
                }

                soundToolKitManager.ResourceContainer.Remove(acousticMesh: this);
            }

            var isLastMeshWithGivenMaterial = !soundToolKitManager.ResourceContainer.AcousticMeshes.Any(x => x.Material == Material);
            if (isLastMeshWithGivenMaterial && Material != null)
            {
                Material.GetStkMaterial().Dispose();
            }

            m_acousticMesh.Dispose();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && !Initialized)
            {
                return;
            }

            UpdateProperties();
        }
#endif

        private void UpdateProperties()
        {
            EnabledAsAcousticObstacle = m_enabledAsAcousticObstacle;
            Material = m_acousticMaterial;
            MeshCluster = m_meshCluster;
        }

        private void CheckMeshClusterSynchronization()
        {
            if (MeshCluster != null && !MeshCluster.AcousticMeshNames.Contains(CombinedName()))
            {
                SoundToolKitDebug.LogWarning("Mesh: " + name + " was assigned to MeshCluster: " +
                    MeshCluster.name + " but was not found on that Cluster's MeshList.\n" +
                    "Probably a desynchronization occured due to deleting Meshes from the scene.\n" +
                    "Please make sure everything is assigned correctly for the scene geometry to work as intended.");
            }
        }

        #endregion

        #region private members

        private List<Vector3> m_faceVertices = new List<Vector3>();

        private Mesh m_acousticMesh;

        private UnityEngine.Mesh m_mesh;

        private bool m_enabledAsObstaclePreviousState;

        [SerializeField, HideInInspector]
        private SoundToolKitMeshCluster m_previousCluster;

        #endregion
    }
}
