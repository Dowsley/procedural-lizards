using UnityEngine;
using UnityEngine.Serialization;

namespace Behaviours
{
    public class CircleRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int segments = 100;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void DrawCircle(float radius)
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

        public void Clear()
        {
            lineRenderer.positionCount = 0;
        }
    }
}
