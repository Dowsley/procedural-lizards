using UnityEngine;

namespace Behaviours
{
    public class Segment : MonoBehaviour
    {
        [SerializeField] private CircleRenderer innerCircleRenderer;
        [SerializeField] private CircleRenderer outerCircleRenderer;
        

        public void DrawCircle(float radius, bool debugInnerCircle = false)
        {
            outerCircleRenderer.SetOutlineEnabled(debugInnerCircle);
            outerCircleRenderer.SetFillEnabled(!debugInnerCircle);
            outerCircleRenderer.DrawCircle(radius);
            if (debugInnerCircle)
                innerCircleRenderer.DrawCircle(radius * 0.2f);
        }

        public void Clear()
        {
            outerCircleRenderer.Clear();
            innerCircleRenderer.Clear();
        }

        public void SetOuterFillColor(Color color)
        {
            outerCircleRenderer.SetFillColor(color);
        }

        public void SetSortingOrder(int order)
        {
            outerCircleRenderer.SetSortingOrder(order);
            innerCircleRenderer.SetSortingOrder(order + 1);
        }
    }
}
