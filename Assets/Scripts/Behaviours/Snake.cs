using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviours
{
    [RequireComponent(typeof(LineRenderer))]
    public class Snake : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera gameCamera;
        [SerializeField] private Segment segmentPrefab;

        [Header("Debug")]

        [Header("Visuals")]
        [SerializeField] private bool debug = true;
        [SerializeField] private float segmentRadiusSize = 1f;
        [SerializeField] private AnimationCurve bodyWidthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.2f);
        [SerializeField] private Gradient colorGradient = new Gradient();

        [Header("Chain")]
        [SerializeField] private int segmentCount = 48;
        [SerializeField] private float linkSize = 1f;
        // [SerializeField] private float angleConstraint = Mathf.PI / 8; // 22 degrees
        
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        
        private readonly List<Segment> _segments = new();
        private LineRenderer _debugLineRenderer;
        
        private Segment Head => _segments[0]; 
        
        private void Start()
        {
            GetComponents();
            
            if (debug)
            {
                _debugLineRenderer.enabled = true;
                _debugLineRenderer.positionCount = segmentCount;
                _debugLineRenderer.numCornerVertices = 8;
                _debugLineRenderer.numCapVertices = 8;
            }
            else
            {
                _debugLineRenderer.positionCount = 0;
                _debugLineRenderer.enabled = false;
            }
            
            for (int i = 0; i < segmentCount; i++)
            {
                var segment = Instantiate(segmentPrefab, transform);
                segment.transform.position = Vector3.right * linkSize * i;
                float t = segmentCount > 1 ? (float)i / (segmentCount - 1) : 0f;
                float radius = bodyWidthCurve.Evaluate(t) * segmentRadiusSize;
                segment.DrawCircle(radius, debug);
                segment.SetOuterFillColor(colorGradient.Evaluate(t));
                
                // Head on top (the highest order), tail below
                int order = segmentCount - 1 - i;
                segment.SetSortingOrder(order);
                _segments.Add(segment);
            }
        }

        private void Update()
        {
            Vector2 pointerPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = gameCamera.ScreenToWorldPoint(
                new Vector3(pointerPos.x, pointerPos.y, 0f)
            );
            worldPos.z = 0f;
            
            Head.transform.position = Vector3.Lerp(
                Head.transform.position, 
                worldPos, 
                moveSpeed * Time.deltaTime
            );
            
            ResolveConstraints();
            Render();
        }

        private void GetComponents()
        {
            _debugLineRenderer = GetComponent<LineRenderer>();
        }

        private void ResolveConstraints()
        {
            for (int i = 1; i < _segments.Count; i++)
            {
                _segments[i].transform.position = Utils.Constraints.BasicDistanceConstraint(
                    _segments[i].transform.position,
                    _segments[i-1].transform.position,
                    linkSize
                );
            }
        }

        private void UpdateDebugLine()
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                _debugLineRenderer.SetPosition(i, _segments[i].transform.position);
            } 
        }

        private void Render()
        {
            if (debug)
                UpdateDebugLine();
        }
    }
}
