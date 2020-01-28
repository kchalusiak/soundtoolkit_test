/// \author Marcin Misiek
/// \date 15.06.2018

using SoundToolKit.Extensions.Diagnostics;
using SoundToolKit.Unity.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoundToolKit.Unity.Extensions
{
    /// <summary>
    /// This component allows to display sound paths from the engine.
    /// After adding this component by default there is a setting enableLiveSoundPaths set to true.
    /// Colors of the specific segments of each path are defined in SoundSegmentsColors.
    /// SoundPath is created from segments which indicate what effect has taken place.
    /// </summary>
    [AddComponentMenu("SoundToolKit/DefaultPrefabComponents/SoundPathRenderer")]
    public class SoundPathRenderer : MonoBehaviour, ISoundToolKitObserver
    {
        #region editor fields

        [SerializeField]
        private float m_lineWidth = 0.0035f;

        [SerializeField]
        private bool m_updateSoundPaths = true;

        [SerializeField]
        private SoundSegmentsColors m_segmentsColors = new SoundSegmentsColors();

        [SerializeField]
        private SoundSegmentsPermissions m_segmentsPermissions = new SoundSegmentsPermissions();

        #endregion

        #region public properties

        /// <summary>
        /// Sets the setting enableLiveSoundPaths to true and allows to render those paths.
        /// </summary>
        public bool UpdateSoundPaths
        {
            get { return m_updateSoundPaths; }
            set
            {
                m_updateSoundPaths = value;
                if (m_soundPathsCache != null)
                {
                    m_soundPathsCache.CachingEnabled = value;
                }
            }
        }

        public SoundSegmentsColors SegmentsColors
        {
            get { return m_segmentsColors; }
            set
            {
                m_segmentsColors = value;
                m_segmentsColors.Update();
            }
        }

        public SoundSegmentsPermissions SegmentsPermissions
        {
            get { return m_segmentsPermissions; }
            set
            {
                m_segmentsPermissions = value;
                m_segmentsPermissions.Update();
            }
        }

        /// <summary>
        /// Width of each segment line.
        /// </summary>
        public float LineWidth
        {
            get { return m_lineWidth; }
            set
            {
                if (value > MAX_LINE_WIDTH)
                {
                    m_lineWidth = MAX_LINE_WIDTH;
                }
                else if (value < MIN_LINE_WIDTH)
                {
                    m_lineWidth = MIN_LINE_WIDTH;
                }
                else
                {
                    m_lineWidth = value;
                }
            }
        }

        public bool Initialized { get; private set; }

        #endregion

        #region public methods

        /// <summary>
        /// Draw live sound paths based upon render properties.
        /// </summary>
        public void Render()
        {
            if (m_soundPathsCache.CachingEnabled)
            {
                var newPaths = m_soundPathsCache.GetCachedPaths();
                if (newPaths != null)
                {
                    m_soundPaths = newPaths;
                }
            }
            else
            {
                m_soundPaths = new SoundPathsCache.SoundPath[0];
            }

            foreach (PathVertexType type in Enum.GetValues(typeof(PathVertexType)))
            {
                if (SegmentsPermissions.Get(type))
                {
                    RenderSegment(type);
                }
                else
                {
                    StopRenderingSegment(type);
                }
            }

        }

        /// <summary>
        /// Clears all lines.
        /// </summary>
        public void StopRendering()
        {
            foreach (PathVertexType type in Enum.GetValues(typeof(PathVertexType)))
            {
                StopRenderingSegment(type);
            }
        }

        #endregion

        #region public methods

        public void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                m_soundPathsCache = new SoundPathsCache(soundToolKitManager.StkAudioEngine);

                IntializeSegmentsLineRenderer();

#if !UNITY_EDITOR
                UpdateSoundPathDrawer();
#else
                m_soundPathsCache.CachingEnabled = UpdateSoundPaths;
#endif
            }
        }

        #endregion

        #region private methods

        private void RenderSegment(PathVertexType type)
        {
            var points = new List<Vector3>();

            foreach (var path in m_soundPaths)
            {
                for (int i = 0; i < path.vertices.Count - 1; ++i)
                {
                    if (path.vertices[i].Type == type)
                    {
                        points.Add(Vector3Extensions.ToUnityEngine(path.vertices[i].Position));
                        points.Add(Vector3Extensions.ToUnityEngine(path.vertices[i + 1].Position));
                    }
                }
            }

            if (points.Count > 1)
            {
                var lineRenderer = m_lineRendererByEffectType[type];

                lineRenderer.material.color = SegmentsColors.Get(type);
#if UNITY_5_5
                lineRenderer.numPositions = points.Count;
#else
                lineRenderer.positionCount = points.Count;
#endif
                lineRenderer.startWidth = m_lineWidth;
                lineRenderer.endWidth = m_lineWidth;

                lineRenderer.SetPositions(points.ToArray());
            }
            else
            {
                StopRenderingSegment(type);
            }
        }

        private void StopRenderingSegment(PathVertexType type)
        {
            var lineRenderer = m_lineRendererByEffectType[type];
#if UNITY_5_5
            lineRenderer.numPositions = 0;
#else
            lineRenderer.positionCount = 0;
#endif
        }

        private void IntializeSegmentsLineRenderer()
        {
            m_lineRendererByEffectType = new Dictionary<PathVertexType, LineRenderer>();

            foreach (PathVertexType type in Enum.GetValues(typeof(PathVertexType)))
            {
                var segmentObject = new GameObject(type.ToString());
                var lineRenderer = segmentObject.AddComponent<LineRenderer>();

                segmentObject.transform.parent = transform;

                lineRenderer.material = new UnityEngine.Material(Shader.Find("Unlit/Color"));

                m_lineRendererByEffectType.Add(type, lineRenderer);
            }
        }

        #region unity methods

        private void Awake()
        {
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
        }

        private void Update()
        {
            if (m_soundPathsCache != null)
            {
                if (m_updateSoundPaths)
                {
                    Render();
                }
                else
                {
                    StopRendering();
                }
            }
        }

        private void OnDisable()
        {
            StopRendering();
        }

        private void OnDestroy()
        {
            StopRendering();

            foreach (var path in m_soundPaths)
            {
                path.vertices.Clear();
            }

            m_soundPaths = null;
            m_lineRendererByEffectType.Clear();
            m_soundPathsCache.Dispose();
            SoundToolKitManager.Instance.Finish();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            UpdateSoundPathDrawer();
        }

#endif

        #endregion

        private void UpdateSoundPathDrawer()
        {
            SegmentsColors = m_segmentsColors;
            SegmentsPermissions = m_segmentsPermissions;
            UpdateSegments();

            LineWidth = m_lineWidth;
            UpdateSoundPaths = m_updateSoundPaths;
        }

        private void UpdateSegments()
        {
            if (m_segmentsColors != null)
            {
                m_segmentsColors.Update();
            }

            if (m_segmentsPermissions != null)
            {
                m_segmentsPermissions.Update();
            }
        }

        #endregion

        #region private members

        private SoundPathsCache m_soundPathsCache;

        private SoundPathsCache.SoundPath[] m_soundPaths = new SoundPathsCache.SoundPath[0];

        private Dictionary<PathVertexType, LineRenderer> m_lineRendererByEffectType;

        private const float MAX_LINE_WIDTH = 0.01f;

        private const float MIN_LINE_WIDTH = 0.0005f;

        #endregion
    }
}
