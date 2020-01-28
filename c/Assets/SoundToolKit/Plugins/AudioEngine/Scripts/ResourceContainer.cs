/// \author Marcin Misiek
/// \date 08.06.2018
/// \author Michal Majewski
/// \date 06.03.2019

using SoundToolKit.Unity.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// ResourceContainer gathers all StkComponents currently in use by StkManager.
    /// </summary>
    public class ResourcesContainer
    {
        #region public constructor

        public ResourcesContainer()
        {
            m_audioSources = new List<SourceComponent>();
            m_acousticMeshes = new List<SoundToolKitMesh>();
            m_acousticMaterials = new List<SoundToolKitMaterial>();
            m_playbacks = new List<SoundToolKitPlayback>();
        }

        #endregion

        #region public events

        public event System.Action OnMeshesChanged;

        #endregion

        #region public properties

        public SoundToolKitListener AudioListener
        {
            get { return m_audioListener; }
            set
            {
                SoundToolKitDebug.Assert(value != null, "Listener is null");

                if (value != m_audioListener)
                {
                    if (m_audioListener == null)
                    {
                        m_audioListener = value;
                    }
                    else
                    {
                        SoundToolKitDebug.LogError("Critical error. Only one SoundToolKitListener is allowed on the scene.");
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        UnityEngine.Application.Quit();
#endif
                    }
                }
            }
        }

        public SoundToolKitGeometry Geometry
        {
            get { return m_geometry; }
            set
            {
                if (value != m_geometry)
                {
                    if (value != null && m_geometry != null)
                    {
                        SoundToolKitDebug.LogError("Only one SoundToolKitGeometry component is allowed on the scene. Please delete the redundant component. Aborting.");
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        UnityEngine.Application.Quit();
#endif
                    }
                    else
                    {
                        m_geometry = value;
                    }
                }
            }
        }

        public SoundToolKitSceneConfiguration SceneConfiguration
        {
            get { return m_sceneConfig; }
            set
            {
                if (value != m_sceneConfig)
                {
                    if (value != null && m_sceneConfig != null)
                    {
                        SoundToolKitDebug.LogError("Only one SoundToolKitSceneConfiguration component should be placed on a given scene.\n" +
                            "Only values from one of them will be taken into account.\n" +
                            "Please delete the redundant component.");
                    }
                    else
                    {
                        m_sceneConfig = value;
                    }
                }
            }
        }

        public SoundToolKitStatus Status
        {
            get { return m_status; }
            set
            {
                if (value != m_status)
                {
                    if (value != null && m_status != null)
                    {
                        SoundToolKitDebug.Log("Only one SoundToolKitStatus is advised per scene. As such, the redundant StkStatus will be deleted.");
                        Object.Destroy(value);
                    }
                    else
                    {
                        m_status = value;
                    }
                }
            }
        }

        public ReadOnlyCollection<SourceComponent> AudioSources
        {
            get { return m_audioSources.AsReadOnly(); }
        }

        public ReadOnlyCollection<SoundToolKitMesh> AcousticMeshes
        {
            get { return m_acousticMeshes.AsReadOnly(); }
        }

        public ReadOnlyCollection<SoundToolKitMaterial> AcousticMaterials
        {
            get { return m_acousticMaterials.AsReadOnly(); }
        }

        public ReadOnlyCollection<SoundToolKitPlayback> Playbacks
        {
            get { return m_playbacks.AsReadOnly(); }
        }

        #endregion

        #region public methods

        public void Add(SourceComponent audioSource)
        {
            // <STK.LITE> - Why are we even here?
            if (AudioSources.Count >= StkLite.AvailableSources())
            {
                return;
            }
            // </STK.LITE>

            SoundToolKitDebug.Assert(audioSource != null, "Source component is null");

            if (!AudioSources.Contains(audioSource))
            {
                m_audioSources.Add(audioSource);
            }
            else
            {
                SoundToolKitDebug.LogWarning("Source component already registered.");
            }
        }

        public void Add(SoundToolKitMesh acousticMesh)
        {
            SoundToolKitDebug.Assert(acousticMesh != null, "Mesh is null");

            if (!AcousticMeshes.Contains(acousticMesh))
            {
                m_acousticMeshes.Add(acousticMesh);
                RaiseMeshesChanged();
            }
            else
            {
                SoundToolKitDebug.LogWarning("Mesh already registered.");
            }
        }

        public void Add(SoundToolKitMaterial acousticMaterial)
        {
            SoundToolKitDebug.Assert(acousticMaterial != null, "Material is null");

            if (!AcousticMaterials.Contains(acousticMaterial))
            {
                m_acousticMaterials.Add(acousticMaterial);
            }
            else
            {
                SoundToolKitDebug.LogWarning("Material already registered.");
            }
        }

        public void Add(SoundToolKitPlayback playback)
        {
            // <STK.LITE> - What's the point of it all?
            if (Playbacks.Count >= StkLite.AvailableSources())
            {
                return;
            }
            // </STK.LITE>

            SoundToolKitDebug.Assert(playback != null, "Playback is null");

            if (!Playbacks.Contains(playback))
            {
                m_playbacks.Add(playback);
            }
            else
            {
                SoundToolKitDebug.LogWarning("Playback already registered.");
            }
        }

        public void Remove(SourceComponent audioSource)
        {
            SoundToolKitDebug.Assert(audioSource != null, "Source component is null");

            if (AudioSources.Contains(audioSource))
            {
                m_audioSources.Remove(audioSource);
            }
            else
            {
                SoundToolKitDebug.LogWarning("Source component wasn't registered.");
            }
        }

        public void Remove(SoundToolKitMesh acousticMesh)
        {
            SoundToolKitDebug.Assert(acousticMesh != null, "Mesh is null");

            if (AcousticMeshes.Contains(acousticMesh))
            {
                m_acousticMeshes.Remove(acousticMesh);
            }
            else
            {
                SoundToolKitDebug.LogWarning("Mesh wasn't registered.");
            }
        }

        public void Remove(SoundToolKitMaterial acousticMaterial)
        {
            SoundToolKitDebug.Assert(acousticMaterial != null, "Material is null");

            if (AcousticMaterials.Contains(acousticMaterial))
            {
                m_acousticMaterials.Remove(acousticMaterial);
            }
            else
            {
                SoundToolKitDebug.LogWarning("Material wasn't registered.");
            }
        }

        public void Remove(SoundToolKitPlayback playback)
        {
            SoundToolKitDebug.Assert(playback != null, "Playback is null");

            if (Playbacks.Contains(playback))
            {
                m_playbacks.Remove(playback);
            }
            else
            {
                SoundToolKitDebug.LogWarning("Playback wasn't registered.");
            }
        }

        public void Clear()
        {
            m_audioListener = null;
            m_geometry = null;

            m_audioSources.Clear();
            m_acousticMeshes.Clear();
            m_acousticMaterials.Clear();
            m_playbacks.Clear();
        }

        #endregion

        #region internal methods

        internal void RaiseMeshesChanged()
        {
            if (OnMeshesChanged != null)
            {
                OnMeshesChanged();
            }
        }

        #endregion

        #region private members

        private SoundToolKitListener m_audioListener;
        private SoundToolKitGeometry m_geometry;
        private SoundToolKitSceneConfiguration m_sceneConfig;
        private SoundToolKitStatus m_status;

        private List<SourceComponent> m_audioSources;
        private List<SoundToolKitMesh> m_acousticMeshes;
        private List<SoundToolKitMaterial> m_acousticMaterials;
        private List<SoundToolKitPlayback> m_playbacks;

        #endregion
    }
}
