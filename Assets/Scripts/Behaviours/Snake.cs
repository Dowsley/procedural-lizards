using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviours
{
    public class Snake : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private Segment segmentPrefab;
        
        [SerializeField] private bool debug = true;
        [SerializeField] private float segmentSize = 1f;
        [SerializeField] private int segmentCount = 48;
        [SerializeField] private float linkSize = 1f;
        // [SerializeField] private float angleConstraint = Mathf.PI / 8; // 22 degrees
        [SerializeField] private float moveSpeed = 8f;
        
        private readonly List<Segment> _segments = new();
        private LineRenderer _lineRenderer;
        // private SnakeMeshBuilder _meshBuilder;
        
        private Segment Head => _segments[0]; 
        
        private void Start()
        {
            GetComponents();
            if (debug)
                DebugSetup();
            
            for (int i = 0; i < segmentCount; i++)
            {
                var segment = Instantiate(segmentPrefab, transform);
                segment.transform.position = Vector3.right * linkSize * i;
                segment.DrawCircle(segmentSize);
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
            if (debug)
                DebugUpdate();
        }

        private void GetComponents()
        {
            _lineRenderer = GetComponent<LineRenderer>();
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

        private void UpdateLine()
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                _lineRenderer.SetPosition(i, _segments[i].transform.position);
            } 
        }

        private void DebugSetup()
        {
            _lineRenderer.positionCount = segmentCount;
        }

        private void DebugUpdate()
        {
            UpdateLine();
        }
    }
}
