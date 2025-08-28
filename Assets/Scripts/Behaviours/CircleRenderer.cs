using UnityEngine;

namespace Behaviours
{
    [RequireComponent(typeof(LineRenderer), typeof(MeshFilter), typeof(MeshRenderer))]
    public class CircleRenderer : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int segments = 100;
        [SerializeField] private bool fill;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Mesh _mesh;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _mesh = new Mesh();
            _meshFilter.sharedMesh = _mesh;
        }

        public void DrawCircle(float radius)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.positionCount = segments + 1;
                float angle = 0f;
                for (int i = 0; i <= segments; i++)
                {
                    float x = Mathf.Cos(angle) * radius;
                    float y = Mathf.Sin(angle) * radius;
                    lineRenderer.SetPosition(i, new Vector3(x, y, 0));
                    angle += 2f * Mathf.PI / segments;
                }
            }
            else
            {
                lineRenderer.positionCount = 0;
            }

            if (fill && _meshRenderer.enabled)
                DrawFilled(radius);
            else
                _mesh.Clear();
        }

        public void Clear()
        {
            lineRenderer.positionCount = 0;
            _mesh.Clear();
        }

        public void SetOutlineEnabled(bool value)
        {
            lineRenderer.enabled = value;
            if (!value)
                lineRenderer.positionCount = 0;
        }

        public void SetFillEnabled(bool value)
        {
            _meshRenderer.enabled = value;
            if (!value) _mesh.Clear();
        }

        public void SetSortingOrder(int order)
        {
            lineRenderer.sortingOrder = order;
            _meshRenderer.sortingOrder = order;
        }

        public void SetFillColor(Color color)
        {
            if (!fill)
                return;
            var mat = _meshRenderer.material;
            if (mat.HasProperty(BaseColor))
                mat.SetColor(BaseColor, color);
            else if (mat.HasProperty(Color1)) mat.SetColor(Color1, color);
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
    }
}
