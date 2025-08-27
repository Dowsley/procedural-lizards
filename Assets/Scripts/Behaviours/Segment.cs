using UnityEngine;

namespace Behaviours
{
    public class Segment : MonoBehaviour
    {
        private CircleRenderer _circleRenderer;
        
        private void Awake()
        {
            _circleRenderer = GetComponent<CircleRenderer>();
        }

        public void DrawCircle(float radius)
        {
            _circleRenderer.DrawCircle(radius);
        }

        public void Clear()
        {
            _circleRenderer.Clear();
        }
    }
}
