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

        private Color _color;

        public void Render(float radius, Color color, bool debug = false)
        {
            _color = color;
            var fillOuter = !debug;
            outerCircleRenderer.Render(
                radius,
                debug ? outerCircleDebugColor : color,
                fillOuter
            );
            
            if (debug)
                innerCircleRenderer.Render(radius * 0.2f, innerCircleDebugColor, true);
        }
        
        public void SetColor(Color newColor)
        {
            _color = newColor;
            outerCircleRenderer.SetColor(newColor);
        }

        public Color GetColor()
        {
            return _color;
        }

        public void SetSortingOrder(int order)
        {
            outerCircleRenderer.SetSortingOrder(order);
            innerCircleRenderer.SetSortingOrder(order + 1);
        }

        /// <summary>
        /// Sways the segments on local space. Since they're all originally at origin this works.
        /// </summary>
        /// <param name="currLocalPos"></param>
        public void Sway(Vector3 currLocalPos)
        {
            outerCircleRenderer.transform.localPosition = currLocalPos;
            innerCircleRenderer.transform.localPosition = currLocalPos;
        }
        
        public Vector3 GetInnerCircleLocalPos()
        {
            return outerCircleRenderer.transform.localPosition;
        }
    }
}
