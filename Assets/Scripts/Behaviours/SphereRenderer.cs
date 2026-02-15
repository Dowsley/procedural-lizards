using UnityEngine;

namespace Behaviours
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SphereRenderer : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private const int LonSegments = 8;
        private const int LatSegments = 6;

        private static Mesh _sharedSphereMesh;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propBlock;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _propBlock = new MaterialPropertyBlock();

            if (_sharedSphereMesh == null)
                _sharedSphereMesh = GenerateLowPolySphere(LonSegments, LatSegments);

            _meshFilter.sharedMesh = _sharedSphereMesh;
        }

        public void Render(float radius, Color color, bool fill = false)
        {
            transform.localScale = Vector3.one * (radius * 2f);
            SetColor(color);
        }

        public void SetSortingOrder(int order)
        {
            // No-op: depth buffer handles ordering in 3D
        }

        public void SetColor(Color color)
        {
            if (_propBlock == null)
                _propBlock = new MaterialPropertyBlock();

            _propBlock.SetColor(BaseColor, color);
            _meshRenderer.SetPropertyBlock(_propBlock);
        }

        private static Mesh GenerateLowPolySphere(int lonSegments, int latSegments)
        {
            var mesh = new Mesh { name = "LowPolySphere" };

            int vertCount = (lonSegments + 1) * (latSegments + 1);
            var vertices = new Vector3[vertCount];
            var normals = new Vector3[vertCount];
            int idx = 0;

            for (int lat = 0; lat <= latSegments; lat++)
            {
                float theta = Mathf.PI * lat / latSegments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                for (int lon = 0; lon <= lonSegments; lon++)
                {
                    float phi = 2f * Mathf.PI * lon / lonSegments;
                    float x = sinTheta * Mathf.Cos(phi);
                    float y = cosTheta;
                    float z = sinTheta * Mathf.Sin(phi);

                    vertices[idx] = new Vector3(x, y, z) * 0.5f;
                    normals[idx] = new Vector3(x, y, z).normalized;
                    idx++;
                }
            }

            int triCount = lonSegments * latSegments * 6;
            var triangles = new int[triCount];
            int ti = 0;

            for (int lat = 0; lat < latSegments; lat++)
            {
                for (int lon = 0; lon < lonSegments; lon++)
                {
                    int current = lat * (lonSegments + 1) + lon;
                    int next = current + lonSegments + 1;

                    triangles[ti++] = current;
                    triangles[ti++] = next;
                    triangles[ti++] = current + 1;

                    triangles[ti++] = current + 1;
                    triangles[ti++] = next;
                    triangles[ti++] = next + 1;
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
