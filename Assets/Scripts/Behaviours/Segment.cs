using UnityEngine;

namespace Behaviours
{
    public class Segment : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CircleRenderer innerCircleRenderer;
        [SerializeField] private CircleRenderer outerCircleRenderer;

        [Header("Debug Settings")]
        [SerializeField] private Color outerCircleDebugColor = Color.white;
        [SerializeField] private Color innerCircleDebugColor = Color.red;

        public void Render(float radius, Color color, bool debug = false)
        {
            var fillOuter = !debug;
            outerCircleRenderer.Render(
                radius,
                debug ? outerCircleDebugColor : color,
                fillOuter
            );
            
            if (debug)
                innerCircleRenderer.Render(radius * 0.2f, innerCircleDebugColor, true);
        }
        
        public void SetColor(Color color)
        {
            outerCircleRenderer.SetColor(color);
        }

        public void SetSortingOrder(int order)
        {
            outerCircleRenderer.SetSortingOrder(order);
            innerCircleRenderer.SetSortingOrder(order + 1);
        }
    }
}
