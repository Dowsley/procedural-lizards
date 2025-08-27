using UnityEngine;

namespace Behaviours
{
    public class CircleRenderer : MonoBehaviour
    {
        [SerializeField] public int segments = 100;
        
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void DrawCircle(float radius)
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
