using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviours
{
    public class SnakeController : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private CircleRenderer segmentPrefab;
        
        [SerializeField] private float segmentSize = 1f;
        [SerializeField] private int segmentCount = 48;
        [SerializeField] private float linkSize = 1f;
        [SerializeField] private float angleConstraint = Mathf.PI / 8; // 22 degrees
        [SerializeField] private float moveSpeed = 8f;
        
        private List<CircleRenderer> _segments = new();
        // private SnakeLineRenderer _lineRenderer;
        // private SnakeMeshBuilder _meshBuilder;
        
        private CircleRenderer Head => _segments[0]; 
        
        private void Start()
        {
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
                new Vector3(pointerPos.x, pointerPos.y, 10f) // 10f = distance from camera for orthographic
            );
            
            Head.transform.position = Vector3.Lerp(
                Head.transform.position, 
                worldPos, 
                moveSpeed * Time.deltaTime
            );
            
            ResolveConstraints();
        }

        private void ResolveConstraints()
        {
            for (int i = 1; i < _segments.Count; i++)
            {
                _segments[i].transform.position = Utils.Constraints.ConstrainDistance(
                    _segments[i].transform.position,
                    _segments[i-1].transform.position,
                    linkSize
                );
            }
        }
    }
}
