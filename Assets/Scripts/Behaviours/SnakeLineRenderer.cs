using UnityEngine;

namespace Behaviours
{
    public class SnakeLineRenderer : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        
        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        void Update()
        {
                    
        }
    }
}
