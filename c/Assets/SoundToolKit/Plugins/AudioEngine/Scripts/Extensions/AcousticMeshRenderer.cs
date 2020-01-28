/// \author Marcin Misiek
/// \date 21.06.2018
/// \author Michal Majewski
/// \date 02.05.2019

#pragma warning disable 0414

using SoundToolKit.Extensions;
using SoundToolKit.Unity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundToolKit.Unity.Extensions
{
    [AddComponentMenu("SoundToolKit/DefaultPrefabComponents/AcousticMeshRenderer")]
    public class AcousticMeshRenderer : MonoBehaviour, ISoundToolKitObserver
    {
        #region editor fields

        [SerializeField]
        private bool m_renderAcousticMeshes = false;

        [SerializeField]
        private bool m_hideVisualMeshes = false;

        [SerializeField]
        private int m_meshClusterCount = 0;

        [SerializeField]
        private int m_meshCount = 0;

        [SerializeField]
        private int m_triangleCount = 0;

        #endregion

        /// <summary>
        /// Setting it to true will display the Acoustic Meshes taking part in SoundToolKit sound processing.
        /// </summary>
        /// <note>AcousticMeshes that are disabled (either as a Component or by deselecting the EnabledAsAcousticObstacle control) will not be rendered to reflect the true state of STK-processed Acoustic Scene.</note>
        public bool RenderAcousticMeshes
        {
            get { return m_renderAcousticMeshes; }
            set
            {
                m_renderAcousticMeshes = value;
            }
        }

        /// <summary>
        /// Setting it to true will hide all of the visual meshes on the scene - except the ones created 
        /// by rendering AcouticMeshes via this component. This allows to check the integrity and shape of
        /// Acoustic Scene without the distraction and obstruction of visual scene being rendered on top of it.
        /// </summary>
        /// <note>This will also hide meshes such as the Player character mesh etc.</note>
        public bool HideVisualMeshes
        {
            get { return m_hideVisualMeshes; }
            set
            {
                m_hideVisualMeshes = value;
            }
        }

        /// <summary>
        /// The number of MeshClusters currently present on the scene.
        /// </summary>
        public int MeshClusterCount
        {
            get { return m_meshClusterCount; }
            private set
            {
                m_meshClusterCount = value;
            }
        }

        /// <summary>
        /// The number of active Acoustic Meshes currently present on the scene.
        /// </summary>
        public int MeshCount
        {
            get { return m_meshCount; }
            private set
            {
                m_meshCount = value;
            }
        }

        /// <summary>
        /// The number of active Acoustic Meshes' triangles currently present on the scene.
        /// </summary>
        public int TriangleCount
        {
            get { return m_triangleCount; }
            private set
            {
                m_triangleCount = value;
            }
        }

        public bool Initialized { get; private set; }

        #region public methods

        public void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                SoundToolKitDebug.Assert(soundToolKitManager.StkAudioEngine != null, "AudioEngine is null");

                m_editorExtensions = new EditorExtensions(soundToolKitManager.StkAudioEngine);
                m_resourcesContainer = soundToolKitManager.ResourceContainer;

                m_resourcesContainer.OnMeshesChanged += RedrawAcoustic;

#if !UNITY_EDITOR
                UpdateMeshDrawer();
#endif
            }
        }

        #endregion

        #region private methods

        private void RenderAcoustic()
        {
            MeshClusterCount = SoundToolKitManager.Instance.ResourceContainer.Geometry.MeshClusters.Count;
            MeshCount = 0;
            TriangleCount = 0;

            foreach (var acousticMesh in m_resourcesContainer.AcousticMeshes)
            {
                if (acousticMesh.enabled && acousticMesh.EnabledAsAcousticObstacle)
                {
                    MeshCount++;
                    var acousticMeshObject = new GameObject(acousticMesh.name);

                    acousticMeshObject.transform.parent = transform;

                    var meshRenderer = acousticMeshObject.AddComponent<MeshRenderer>();
                    m_renderedAcousticMeshes.Add(meshRenderer);

                    var meshFilter = acousticMeshObject.AddComponent<MeshFilter>();
                    var mesh = new UnityEngine.Mesh();

                    UnityEngine.Material visualMaterial;

                    if (HasVisualMaterial(acousticMesh))
                    {
                        visualMaterial = acousticMesh.Material.VisualMaterial;
                    }
                    else
                    {
                        visualMaterial = new UnityEngine.Material(Shader.Find("Standard")) { color = Color.gray };
                    }

                    meshRenderer.material = visualMaterial;

                    acousticMesh.GetStkMesh().GetFaceVertices(
                        new Action<List<Numerics.Vector3>>(vertices => RenderMesh(mesh, vertices, meshFilter)));
                }
            }
        }

        private void RenderMesh(UnityEngine.Mesh mesh, List<Numerics.Vector3> vertices, MeshFilter meshFilter)
        {
            var verticesUnity = new List<Vector3>();

            for (var vertexIndex = 0; vertexIndex < (vertices.Count / VERTICES_PER_FACE); vertexIndex++)
            {
                TriangleCount++;

                var vertexA = Vector3Extensions.ToUnityEngine(vertices[VERTICES_PER_FACE * vertexIndex + 0]);
                var vertexB = Vector3Extensions.ToUnityEngine(vertices[VERTICES_PER_FACE * vertexIndex + 1]);
                var vertexC = Vector3Extensions.ToUnityEngine(vertices[VERTICES_PER_FACE * vertexIndex + 2]);

                // Doubled to render from both sides
                verticesUnity.AddRange(new List<Vector3> { vertexA, vertexB, vertexC });
                verticesUnity.AddRange(new List<Vector3> { vertexC, vertexB, vertexA });
            }

            mesh.vertices = verticesUnity.ToArray();
            mesh.triangles = Enumerable.Range(0, vertices.Count * 2).ToArray();
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
        }

        private void StopRenderingAcoustic()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            m_renderedAcousticMeshes.Clear();
        }

        private void RedrawAcoustic()
        {
            if (m_renderAcousticMeshes && m_renderedPreviousState)
            {
                StopRenderingAcoustic();
                RenderAcoustic();
            }
        }

        private bool HasVisualMaterial(SoundToolKitMesh acousticMesh)
        {
            return acousticMesh.Material != null && acousticMesh.Material.VisualMaterial != null;
        }

        private void HideVisual()
        {
            var meshRenderers = FindObjectsOfType<MeshRenderer>().ToList();
            meshRenderers.ForEach(x =>
            {
                if (!m_renderedAcousticMeshes.Contains(x) && x.enabled)
                {
                    x.enabled = false;
                    m_hiddenVisualMeshes.Add(x);
                }
            });
        }

        private void ShowVisual()
        {
            m_hiddenVisualMeshes.ForEach(x =>
            {
                if (x != null)
                {
                    x.enabled = true;
                }
            });

            m_hiddenVisualMeshes.Clear();
        }

        #region unity methods

        private void Awake()
        {
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
        }

        private void Update()
        {
            if (m_renderAcousticMeshes && !m_renderedPreviousState)
            {
                RenderAcoustic();
                m_renderedPreviousState = true;
            }
            else if (!m_renderAcousticMeshes && m_renderedPreviousState)
            {
                StopRenderingAcoustic();
                m_renderedPreviousState = false;
            }

            if (m_hideVisualMeshes && !m_hiddenPreviousState)
            {
                HideVisual();
                m_hiddenPreviousState = true;
            }
            else if (!m_hideVisualMeshes && m_hiddenPreviousState)
            {
                ShowVisual();
                m_hiddenPreviousState = false;
            }
        }

        private void OnDisable()
        {
            if (m_renderAcousticMeshes)
            {
                StopRenderingAcoustic();
                m_renderedPreviousState = true;
            }
            else
            {
                m_renderedPreviousState = false;
            }

            if (m_hideVisualMeshes)
            {
                ShowVisual();
                m_hiddenPreviousState = true;
            }
            else
            {
                m_hiddenPreviousState = false;
            }
        }

        private void OnEnable()
        {
            if (m_renderedPreviousState)
            {
                RenderAcoustic();
            }

            if (m_hiddenPreviousState)
            {
                HideVisual();
            }
        }

        private void OnDestroy()
        {
            m_editorExtensions.Dispose();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateMeshDrawer();
        }
#endif

        #endregion

        private void UpdateMeshDrawer()
        {
            RenderAcousticMeshes = m_renderAcousticMeshes;
            HideVisualMeshes = m_hideVisualMeshes;
        }

        #endregion

        #region private members

        private ResourcesContainer m_resourcesContainer;
        private EditorExtensions m_editorExtensions;

        private List<MeshRenderer> m_hiddenVisualMeshes = new List<MeshRenderer>();
        private List<MeshRenderer> m_renderedAcousticMeshes = new List<MeshRenderer>();

        private bool m_renderedPreviousState = false;
        private bool m_hiddenPreviousState = false;

        private const int VERTICES_PER_FACE = 3;

        #endregion
    }
}