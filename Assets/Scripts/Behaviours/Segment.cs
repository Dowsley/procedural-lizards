using UnityEngine;
using UnityEngine.Serialization;

namespace Behaviours
{
    public class Segment : MonoBehaviour
    {
        [SerializeField] private CircleRenderer innerCircleRenderer;
        [SerializeField] private CircleRenderer outerCircleRenderer;
        

        public void DrawCircle(float radius)
        {
            outerCircleRenderer.DrawCircle(radius);
            innerCircleRenderer.DrawCircle(radius*0.2f);
        }

        public void Clear()
        {
            outerCircleRenderer.Clear();
            innerCircleRenderer.Clear();
        }
    }
}
