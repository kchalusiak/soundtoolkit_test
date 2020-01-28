/// \author Michal Majewski
/// \date 05.02.2019
///
using SoundToolKit.Unity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// StkMeshCluster is a representation of a group of StkMeshes that can be serialized and deserialized at once.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "SoundToolKit/SoundToolKitMeshCluster")]
    public class SoundToolKitMeshCluster : ScriptableObject
    {
        #region editor fields

        [SerializeField]
        private List<string> m_meshes = new List<string>();

        [SerializeField, HideInInspector]
        private SerializedGeometry m_serializedGeometry = null;

        #endregion

        #region public properties

        /// <summary>
        /// SoundToolKitMeshes currently being part of this MeshCluster, displayed by their names. 
        /// </summary>
        /// <note>Names of StkMeshes within a cluster need to be unique.</note>
        public List<string> AcousticMeshNames
        {
            get { return m_meshes; }
            private set
            {
                if (value != m_meshes)
                {
                    m_meshes = value;
                }
            }
        }

        /// <summary>
        /// Whether a given MeshCluster has a valid corresponding binary file with it's Meshes' serialized data.
        /// </summary>
        public bool Serialized { get { return m_serializedGeometry != null && m_serializedGeometry.Valid; } }

        #endregion

        #region internal properties

        internal Model Model { get; private set; }

        #endregion

        #region internal events

        public event Action<SoundToolKitMeshCluster> OnMeshesModified;

        #endregion

        #region internal methods

        internal void Add(string meshName)
        {
            if (!AcousticMeshNames.Contains(meshName))
            {
                AcousticMeshNames.Add(meshName);
                RaiseModified();
                ClearSerializedData();
            }
            else
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlaying)
                {
                    SoundToolKitDebug.LogWarning("Attempted to add a StkMesh " + meshName + " to MeshCluster "
                        + name + " but a mesh with that name is already present in that cluster. Aborting.\n" +
                        "You can use the menu Tools/SoundToolKit/Geometry/MakeMeshNamesUnique to remedy this automatically.");
                }
#endif
            }
        }

        internal void Remove(string meshName)
        {
            if (AcousticMeshNames.Contains(meshName))
            {
                AcousticMeshNames.Remove(meshName);
                RaiseModified();
                ClearSerializedData();
            }
        }

        internal void RaiseModified()
        {
            if (OnMeshesModified != null)
            {
                OnMeshesModified(this);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        internal void CreateInternalModel()
        {
            List<SoundToolKitMesh> stkMeshes = new List<SoundToolKitMesh>();
            Mesh[] internalMeshes = new Mesh[AcousticMeshNames.Count];

            for (var i = 0; i < AcousticMeshNames.Count; ++i)
            {
                var mesh = SoundToolKitManager.Instance.ResourceContainer.AcousticMeshes.FirstOrDefault(x => x.CombinedName() == AcousticMeshNames[i]);
                if (mesh != null)
                {
                    stkMeshes.Add(mesh);
                    internalMeshes[i] = mesh.GetStkMesh();
                }
                else
                {
                    SoundToolKitDebug.LogError("Mesh " + AcousticMeshNames[i] + " not found while trying to create internal MeshCluster.");
                }
            }

            Model = SoundToolKitManager.Instance.StkAudioEngine.ResourcesFactory.CreateModel(internalMeshes);

            if (Serialized)
            {
                RestoreGeometry();
            }
            else
            {
                SetGeometry(stkMeshes);
            }
        }

        internal void Serialize()
        {
#if UNITY_EDITOR
            if (Model == null)
            {
                SoundToolKitDebug.LogWarning("Cannot serialize MeshCluster: " + name + "'s geometry data." +
                    " No internal SoundToolKitModel exists for that Cluster.");
                return;
            }

            ClearSerializedData();
            m_serializedGeometry = MeshClusterUtility.CreateSerializedGeometry(name + "_SerializedGeometry");
            Model.SaveGeometry(data => m_serializedGeometry.WriteData(name, data));
            SoundToolKitManager.Instance.Finish();

            m_playSessionGeometry = m_serializedGeometry;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        internal void ClearInternalModel()
        {
            if (Model != null)
            {
                Model.Dispose();
                Model = null;
            }
        }

        internal void ClearSerializedData()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (Serialized)
                {
                    m_serializedGeometry.EraseFromDisk();
                }
            }

            m_serializedGeometry = null;
#endif
        }

        internal void SaveStateForPlaymode()
        {
            m_playSessionMeshes = new List<string>(AcousticMeshNames);
            m_playSessionGeometry = m_serializedGeometry;
        }

        internal void RestoreStateForEditmode()
        {
            AcousticMeshNames = m_playSessionMeshes;
            m_serializedGeometry = m_playSessionGeometry;
        }

        #endregion

        #region private methods

        private void RestoreGeometry()
        {
            Model.RestoreGeometry(m_serializedGeometry.ReadData());
            SoundToolKitDebug.Log("SerializedGeometry restored for MeshCluster: " + name);
        }

        private void SetGeometry(List<SoundToolKitMesh> meshes)
        {
            meshes.ForEach(x => x.SetFaces());
        }

        private void OnDestroy()
        {
            if (Model != null)
            {
                Model.Dispose();
            }
        }

        #endregion

        #region private members

        private List<string> m_playSessionMeshes;
        private SerializedGeometry m_playSessionGeometry;

        #endregion
    }
}
