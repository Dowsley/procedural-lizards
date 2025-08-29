using UnityEngine;

namespace Behaviours
{
    [RequireComponent(typeof(LineRenderer), typeof(MeshFilter), typeof(MeshRenderer))]
    public class CircleRenderer : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        
        [SerializeField] private int segments = 100;

        private LineRenderer _lineRenderer;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Mesh _mesh;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _mesh = new Mesh();
            _meshFilter.sharedMesh = _mesh;
        }

        public void Render(float radius, Color color, bool fill = false)
        {
            _mesh.Clear();
            _lineRenderer.positionCount = 0;

            SetColor(color);
            if (fill)
            {
                DrawFilled(radius);
            }
            else
            {
                DrawOutlined(radius);
            }
        }
        
        public void SetSortingOrder(int order)
        {
            _lineRenderer.sortingOrder = order;
            _meshRenderer.sortingOrder = order;
        }

        public void SetColor(Color color)
        {
            SetMaterialColor(_meshRenderer.material, color);
            SetMaterialColor(_lineRenderer.material, color);
        }
        
        private static void SetMaterialColor(Material mat, Color color)
        {
            if (mat.HasProperty(BaseColor))
                mat.SetColor(BaseColor, color);
            else if (mat.HasProperty(Color1))
                mat.SetColor(Color1, color);
        }

        private void DrawFilled(float radius)
        {
            int vertexCount = segments + 2;

            var vertices = new Vector3[vertexCount];
            var triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;
            float angle = 0f;
            for (int i = 0; i <= segments; i++)
            {
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                vertices[i + 1] = new Vector3(x, y, 0);
                angle += 2f * Mathf.PI / segments;
            }

            int ti = 0;
            for (int i = 0; i < segments; i++)
            {
                triangles[ti++] = 0;
                triangles[ti++] = i + 1;
                triangles[ti++] = i + 2;
            }

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }

        private void DrawOutlined(float radius)
        {
            _lineRenderer.positionCount = segments + 1;
            float angle = 0f;
            for (int i = 0; i <= segments; i++)
            {
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                _lineRenderer.SetPosition(i, new Vector3(x, y, 0));
                angle += 2f * Mathf.PI / segments;
            }
        }
    }
}
