/// \author Michal Majewski
/// \date 19.04.2019
///
using System.Collections.Generic;
using System.Linq;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Handles updating a specific MeshCluster that a given StkMesh has been deleted by the user.
    /// Should not be tiggered on Scene reload or ApplicationQuit, which is the desired behaviour.
    /// Additionally, ensures that when multiple Meshes from a given cluster are deleted in one frame,
    /// the cluster is reloaded only once (with all the meshes gone) and not for every mesh after deletion.
    /// Updates the AcousticMeshRenderer as well.
    /// </summary>
    internal sealed class MeshDeletionUpdater
    {
        #region internal methods

        /// <summary>
        /// To be called OnDestroy() of a given StkMesh to ensure it's respective MeshCluster is notified.
        /// Operates on name to not be nullified when Mesh object is deleted and also because Clusters operate on mesh names.
        /// </summary>
        internal void QueueForUpdate(string meshName, SoundToolKitMeshCluster cluster)
        {
            if (cluster != null)
            {
                m_meshMeshClusterPairs.Add(meshName, cluster);
                m_updateQueued = true;
            }
        }

        internal bool UpdateQueued()
        {
            return m_updateQueued;
        }

        /// <summary>
        /// To be called on update following the deletion of Meshes to ensure Clusters get rebuilt.
        /// </summary>
        internal void UpdateClusters()
        {
            if (m_meshMeshClusterPairs.Any())
            {
                foreach (var pair in m_meshMeshClusterPairs)
                {
                    if (pair.Value != null)
                    {
                        pair.Value.AcousticMeshNames.Remove(pair.Key);
                    }
                }

                foreach (var cluster in m_meshMeshClusterPairs.Values)
                {
                    cluster.RaiseModified();
                }

                SoundToolKitManager.Instance.ResourceContainer.RaiseMeshesChanged();
                m_meshMeshClusterPairs.Clear();
                m_updateQueued = false;
            }
        }

        internal void Destroy()
        {
            m_meshMeshClusterPairs.Clear();
        }

        #endregion

        #region private members

        private Dictionary<string, SoundToolKitMeshCluster> m_meshMeshClusterPairs =
            new Dictionary<string, SoundToolKitMeshCluster>();

        private bool m_updateQueued = false;

        #endregion
    }
}
