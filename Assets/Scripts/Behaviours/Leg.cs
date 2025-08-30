using UnityEngine;
using UnityEngine.Serialization;

namespace Behaviours
{
    [RequireComponent(typeof(LineRenderer))]
    public class Leg : MonoBehaviour
    {
        [SerializeField] private Segment segmentPrefab;
        
        [Header("References")]
        public Segment anchor;
        public Segment forwardReference;
        
        [Header("Setup")]
        public float width = 0.1f;
        public float length = 1f;
        public float pawRadius = 0.2f;

        [Header("Kinematics")]
        public bool isRight = true;
        public float extraStrideOffset = 0f;

        [Header("Step")]
        public Vector2 home;
        public float threshold = 1.5f;
        public float speed = 2f;

        // Runtime
        private LineRenderer _lineRenderer;
        private Segment _pawSegment;
        private Vector2 _currentHome;
        private bool _initialized;

        private void Awake()
        {
            _pawSegment = Instantiate(segmentPrefab, transform);
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
        }

        // Manual update I can easily control from parent snake
        public void Step()
        {
            if (!_initialized)
                return;
            
            SetColor(anchor.GetColor());
            
            // Compute body direction using the next segment (accounts for body angle)
            Vector2 forward = ComputeForward();
            Vector2 right = new(forward.y, -forward.x);
            Vector2 headward = -forward;
            float sideSign = isRight ? 1f : -1f;

            Vector2 anchorPos = anchor.transform.position;
            Vector2 nominalHome = anchorPos + right * (sideSign * length) + headward * extraStrideOffset;

            // Trigger a step only when the paw lags behind the anchor along the headward axis
            float headwardProj = Vector2.Dot((Vector2)_pawSegment.transform.position - anchorPos, headward);
            if (headwardProj < -threshold)
                _currentHome = nominalHome;

            home = _currentHome;
            _pawSegment.transform.position = Vector3.Lerp(
                _pawSegment.transform.position,
                home,
                speed * Time.deltaTime
            );

            // Draw leg
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
            _lineRenderer.SetPosition(0, anchor.transform.position);
            _lineRenderer.SetPosition(1, _pawSegment.transform.position);
        }

        public void Initialize()
        {
            if (anchor == null)
                return;

            // Compute initial home based on current body orientation
            Vector2 forward = ComputeForward();
            Vector2 right = new(forward.y, -forward.x);
            Vector2 headward = -forward;
            float sideSign = isRight ? 1f : -1f;

            Vector2 anchorPos = anchor.transform.position;
            Vector2 nominalHome = anchorPos + right * (sideSign * length) + headward * extraStrideOffset;

            _currentHome = nominalHome;
            _pawSegment.transform.position = _currentHome;

            // Visual setup
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
            _pawSegment.Render(pawRadius, Color.black, false);
            SetColor(anchor.GetColor());

            _initialized = true;
        }
        
        public void SetColor(Color color)
        {
            _pawSegment.SetColor(color);
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
        }

        private Vector2 ComputeForward()
        {
            if (!forwardReference)
                return Vector2.right;
            
            Vector2 a = anchor.transform.position;
            Vector2 b = forwardReference.transform.position;
            Vector2 dir = b - a;
            return dir.sqrMagnitude > Mathf.Epsilon ? dir.normalized : Vector2.right;
        }
    }
}
