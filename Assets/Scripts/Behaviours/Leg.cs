using UnityEngine;

namespace Behaviours
{
    [RequireComponent(typeof(LineRenderer))]
    public class Leg : MonoBehaviour
    {
        [SerializeField] private Segment segmentPrefab;
        
        [Header("Setup")]
        public float width = 0.1f;
        public float length = 1f;
        public float pawRadius = 0.2f;

        [Header("Step")] 
        public float stepThreshold = 0.35f;
        public float stepDuration = 0.12f;
        public float stepLift = 0.15f;

        // Runtime
        private LineRenderer _lineRenderer;
        private Segment _pawSegment;
    }
}
