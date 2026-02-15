using UnityEngine;

namespace Behaviours
{
    public class Segment : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SphereRenderer innerSphereRenderer;
        [SerializeField] private SphereRenderer outerSphereRenderer;
        [SerializeField] private SphereRenderer shadowRenderer;

        [Header("Debug Settings")]
        [SerializeField] private Color outerCircleDebugColor = Color.white;
        [SerializeField] private Color innerCircleDebugColor = Color.red;

        private Color _color;

        public void Render(float radius, Color color, bool debug = false)
        {
            _color = color;
            var fillOuter = !debug;
            outerSphereRenderer.Render(
                radius,
                debug ? outerCircleDebugColor : color,
                fillOuter
            );

            innerSphereRenderer.gameObject.SetActive(debug);
            if (debug)
                innerSphereRenderer.Render(radius * 0.2f, innerCircleDebugColor, true);
        }

        public void SetColor(Color newColor)
        {
            _color = newColor;
            outerSphereRenderer.SetColor(newColor);
        }

        public Color GetColor()
        {
            return _color;
        }

        public void SetSortingOrder(int order)
        {
            // No-op: depth buffer handles ordering in 3D
        }

        /// <summary>
        /// Sways the segments on local space. Since they're all originally at origin this works.
        /// </summary>
        public void Sway(Vector3 currLocalPos)
        {
            outerSphereRenderer.transform.localPosition = currLocalPos;
            innerSphereRenderer.transform.localPosition = currLocalPos;
            if (shadowRenderer != null)
                shadowRenderer.transform.localPosition = currLocalPos;
        }

        public Vector3 GetInnerCircleLocalPos()
        {
            return outerSphereRenderer.transform.localPosition;
        }

        public void UpdateShadow()
        {
            if (shadowRenderer == null)
                return;

            var worldPos = outerSphereRenderer.transform.position;
            shadowRenderer.transform.position = new Vector3(worldPos.x, 0.01f, worldPos.z);

            var scale = outerSphereRenderer.transform.localScale;
            shadowRenderer.transform.localScale = new Vector3(scale.x, 0.01f, scale.z);
        }
    }
}
