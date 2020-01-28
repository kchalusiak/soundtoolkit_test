/// \author Marcin Misiek
/// \date 14.06.2018

using System.Collections.Generic;
using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    internal class MeshTrianglesExtractor
    {
        #region public constructor

        public MeshTrianglesExtractor(UnityEngine.Mesh mesh, Transform transform)
        {
            Debug.Assert(mesh != null);
            Debug.Assert(transform != null);

            m_mesh = mesh;

            TriangleVertices = ExtractTriangles(transform);
        }

        public MeshTrianglesExtractor(UnityEngine.Mesh mesh)
        {
            Debug.Assert(mesh != null);

            m_mesh = mesh;

            TriangleVertices = ExtractTriangles();
        }

        #endregion

        #region public properties

        public List<Vector3> TriangleVertices { get; private set; }

        #endregion

        #region private methods

        private List<Vector3> ExtractTriangles()
        {
            return ExtractTriangles(m_mesh.vertices);
        }

        private List<Vector3> ExtractTriangles(Transform transform)
        {
            var vertices = Translate(m_mesh.vertices, transform);

            return ExtractTriangles(vertices);
        }

        private List<Vector3> ExtractTriangles(Vector3[] vertices)
        {
            var triangles = new List<Vector3>();

            for (var index = 0; index < m_mesh.subMeshCount; index++)
            {
                triangles.AddRange(ExtractTriangles(vertices, index));
            }

            return triangles;
        }

        private List<Vector3> ExtractTriangles(Vector3[] vertices, int subMesh)
        {
            var triangles = new List<Vector3>();
            var indicies = m_mesh.GetTriangles(subMesh);

            for (var i = 0; i < indicies.Length / 3; i++)
            {
                var xVertex = vertices[indicies[3 * i + 0]];
                var yVertex = vertices[indicies[3 * i + 1]];
                var zVertex = vertices[indicies[3 * i + 2]];

                triangles.Add(xVertex);
                triangles.Add(yVertex);
                triangles.Add(zVertex);
            }

            return triangles;
        }

        private Vector3[] Translate(Vector3[] points, Transform transform)
        {
            var transformedPoints = new List<Vector3>();

            foreach (var point in points)
            {
                transformedPoints.Add(transform.TransformPoint(point));
            }

            return transformedPoints.ToArray();
        }

        #endregion

        #region private members

        private UnityEngine.Mesh m_mesh;

        #endregion
    }
}
