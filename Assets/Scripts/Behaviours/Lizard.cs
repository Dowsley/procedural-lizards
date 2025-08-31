using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Behaviours
{
    [RequireComponent(typeof(LineRenderer))]
    public class Lizard : MonoBehaviour
    {
        [Header("Prefab References")]
        [SerializeField] private Segment segmentPrefab;
        [SerializeField] private Leg legPrefab;
        
        [Header("GameObject References")]
        [SerializeField] private Camera gameCamera;
        [SerializeField] private CircleRenderer leftEye;
        [SerializeField] private CircleRenderer rightEye;
        
        [Header("Visuals")]
        [SerializeField] private bool debug = true;
        [SerializeField] private float segmentRadiusSize = 1f;
        [SerializeField] private AnimationCurve bodyWidthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.2f);
        [SerializeField] private Gradient colorGradient = new();
        [SerializeField] private float gradientScrollSpeedWhenDancing = 0f;

        [Header("Chain")]
        [SerializeField] private int segmentCount = 48;
        [SerializeField] private float linkSize = 1f;
        // [SerializeField] private float angleConstraint = Mathf.PI / 8; // 22 degrees
        
        [Header("Leg")]
        [SerializeField] private float lowerLegRatio = 0.9f; // Compared to upper
        [SerializeField] private float legWidth = 0.1f;
        [SerializeField] private float legLength = 1f;
        [SerializeField] private float legPawRadius = 0.2f;
        [SerializeField] private float legStepThreshold = 0.35f;
        [SerializeField] private float legExtraStrideOffset = 0.5f;
        [SerializeField] private float legStepSpeed = 5f;
        [SerializeField] private float upperLegSpawnAround = 0.1f;
        [SerializeField] private float lowerLegSpawnAround = 0.35f;
        
        [Header("Breathing")]
        [Range(0f, 5f)] [SerializeField] private float breathingSpeed = 1f;
        [Range(0.01f, 0.99f)] [SerializeField] private float breathingMinSizeMult = 0.85f;
        [Range(1f, 2f)] [SerializeField] private float breathingMaxSizeMult = 1f;
        
        [Header("Movement")]
        public bool active = true;
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float maxHeadSpeed = 12f;

        [Header("Dancing")]
        [SerializeField] private bool dancing = true;
        [Range(0f, 20f)] [SerializeField] private float danceSpeed = 1f;
        [Range(0f, 10f)] [SerializeField] private float danceSwayAmplitude = 1f;
        [Range(0f, 2f * Mathf.PI)] [SerializeField] private float dancePhasePerSegment = 0.25f; // radians advanced per segment. Max is TAU.
        [Range(0f, 1f)] [SerializeField] private float lerpColorBackToOriginal = 0.6f;
        [Range(0f, 1f)] [SerializeField] private float lerpPosBackToOriginal = 0.6f;
        
        [Header("Eyes")]
        [SerializeField] private float eyeRadius = 2f;
        [SerializeField] private int spawnEyeAtSegmentIndex = 0;
        [SerializeField] private float distanceBetweenEyes = 0.5f;
        [SerializeField] private Color eyeColor = Color.black;
        [SerializeField] private float eyeHeadwardOffset = 0.1f;
        
        private readonly List<Segment> _segments = new();
        private LineRenderer _debugLineRenderer;
        private float _gradientOffset;
        private float _currentGradientScrollSpeed = 0f;
        
        private readonly List<Leg> _legs = new();
        private Leg UpperLeftLeg => _legs[0];
        private Leg UpperRightLeg => _legs[1];
        private Leg LowerLeftLeg => _legs[2];
        private Leg LowerRightLeg => _legs[3];
        
        private Segment Head => _segments[0];

        private int EyesSortingLayer => segmentCount + 1;
        
        private void Start()
        {
            _debugLineRenderer = GetComponent<LineRenderer>();
            _debugLineRenderer.enabled = debug;
            _debugLineRenderer.positionCount = debug ? segmentCount : 0;
            
            // Segments
            for (int i = 0; i < segmentCount; i++)
            {
                var segment = Instantiate(segmentPrefab, transform);
                segment.transform.position = Vector3.right * linkSize * i;
                float baseT = (float)i / (segmentCount - 1);
                float radius = ComputeSegmentRadius(baseT);
                var color = ComputeSegmentColor(baseT, _gradientOffset);
                segment.Render(radius, color, debug);
                
                // Head on top (the highest order), tail below
                int order = segmentCount - 1 - i;
                segment.SetSortingOrder(order);
                _segments.Add(segment);
            }

            // Legs
            for (int i = 0; i < 4; i++)
            {
                var leg = Instantiate(legPrefab, transform);
                leg.width = legWidth;
                leg.length = i < 2 ? legLength : legLength*lowerLegRatio;
                leg.pawRadius = legPawRadius;
                leg.threshold = legStepThreshold;
                leg.speed = legStepSpeed;
                leg.extraStrideOffset = legExtraStrideOffset;
                _legs.Add(leg);
            }

            int frontIdx = Mathf.Clamp((int)(segmentCount * upperLegSpawnAround), 1, segmentCount - 2);
            int backIdx = Mathf.Clamp((int)(segmentCount * lowerLegSpawnAround), 1, segmentCount - 2);

            UpperLeftLeg.anchor = _segments[frontIdx];
            UpperRightLeg.anchor = _segments[frontIdx];
            LowerLeftLeg.anchor = _segments[backIdx];
            LowerRightLeg.anchor = _segments[backIdx];

            UpperLeftLeg.forwardReference = _segments[frontIdx + 1];
            UpperRightLeg.forwardReference = _segments[frontIdx + 1];
            LowerLeftLeg.forwardReference = _segments[backIdx + 1];
            LowerRightLeg.forwardReference = _segments[backIdx + 1];

            UpperLeftLeg.isRight = false;
            LowerLeftLeg.isRight = false;
            UpperRightLeg.isRight = true;
            LowerRightLeg.isRight = true;

            foreach (var leg in _legs)
                leg.Initialize();
            
            
            // Eyes
            leftEye.SetSortingOrder(EyesSortingLayer);
            rightEye.SetSortingOrder(EyesSortingLayer);
            leftEye.Render(eyeRadius, eyeColor, true);
            rightEye.Render(eyeRadius, eyeColor, true);
        }

        private void Update()
        {
            UpdateBreathing();
            if (dancing)
            {
                AnimateGradient();
                UpdateDancingSway();
            }
            else
            {
                LerpSegmentsToOriginal();
            }

            if (active)
            {
                ComputeNextHeadPos();
                ResolveSegmentConstraints();
                UpdateLegs();
            }

            UpdateEyes();
            if (debug)
                UpdateDebugLine();
        }

        // For color and position
        private void LerpSegmentsToOriginal()
        {
            for (int i = 0; i < segmentCount; i++)
            {
                var seg = _segments[i];
                float baseT = (float)i / (segmentCount - 1);
                
                var currLocalPos = seg.GetInnerCircleLocalPos();
                seg.Sway(Vector3.Lerp(currLocalPos, Vector3.zero, lerpPosBackToOriginal));

                var targetColor = ComputeSegmentColor(baseT, 0f);
                var currColor = seg.GetColor();
                seg.SetColor(Color.Lerp(currColor, targetColor, lerpColorBackToOriginal));
            }
        }

        private void UpdateDancingSway()
        {
            float basePhase = Time.time * danceSpeed;
            for (int i = 0; i < _segments.Count; i++)
            {
                float y = ComputeDanceYOffset(i, basePhase);
                var newPos = Vector3.up * y;
                _segments[i].Sway(newPos);
            }
        }

        private void AnimateGradient()
        {
            _gradientOffset = Mathf.Repeat(_gradientOffset + _currentGradientScrollSpeed * Time.deltaTime, 1f);
            for (int i = 0; i < _segments.Count; i++)
            {
                float baseT = (float)i / (segmentCount - 1);
                var color = ComputeSegmentColor(baseT, _gradientOffset);
                _segments[i].SetColor(color);
            }
        }

        private void ResolveSegmentConstraints()
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

        private void UpdateEyes()
        {
            int previousSegmentIdx = spawnEyeAtSegmentIndex + 1;
            
            var segmentToSpawn = _segments[spawnEyeAtSegmentIndex];
            var previousSegment = _segments[previousSegmentIdx];

            // Compute headward/forward axis
            Vector2 a = segmentToSpawn.transform.position;
            Vector2 b = previousSegment.transform.position;
            Vector2 dir = b - a;
            var forward = dir.sqrMagnitude > Mathf.Epsilon ? dir.normalized : Vector2.right;
            var headward = -forward;
            
            Vector2 right = new(headward.y, -headward.x);
            float halfSeparation = distanceBetweenEyes * 0.5f;
            Vector2 headwardOffset = headward * eyeHeadwardOffset;
            
            // Compute eye positions (offset head-ward, then perpendicular separation)
            Vector2 basePos = a + headwardOffset;
            Vector2 leftEyePos = basePos - right * halfSeparation;
            Vector2 rightEyePos = basePos + right * halfSeparation;

            // Apply dancing sway so eyes follow the head visual offset
            if (dancing)
            {
                float basePhase = Time.time * danceSpeed;
                float yOffset = ComputeDanceYOffset(spawnEyeAtSegmentIndex, basePhase);
                leftEyePos.y += yOffset;
                rightEyePos.y += yOffset;
            }
            
            leftEye.transform.position = leftEyePos;
            rightEye.transform.position = rightEyePos;
        }

        private float ComputeDanceYOffset(int segmentIndex, float basePhase)
        {
            if (!dancing || Mathf.Approximately(danceSwayAmplitude, 0f))
                return 0f;
            float phase = basePhase + segmentIndex * dancePhasePerSegment;
            return Mathf.Sin(phase) * danceSwayAmplitude;
        }

        private void ComputeNextHeadPos()
        {
            Vector2 pointerPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = gameCamera.ScreenToWorldPoint(
                new Vector3(pointerPos.x, pointerPos.y, 0f)
            );
            worldPos.z = 0f;

            var currentHeadPos = Head.transform.position;
            var smoothedTarget = Vector3.Lerp(
                currentHeadPos,
                worldPos,
                moveSpeed * Time.deltaTime
            );
            var step = smoothedTarget - currentHeadPos;
            float maxStep = maxHeadSpeed * Time.deltaTime;
            if (step.magnitude > maxStep)
                smoothedTarget = currentHeadPos + step.normalized * maxStep;
            Head.transform.position = smoothedTarget;
        }
        private void UpdateLegs()
        {
            foreach (var leg in _legs)
                leg.Step();
        }

        // Expands and contracts segments to simulate breathing
        private void UpdateBreathing()
        {
            var sinVal = Mathf.Sin(Time.time * breathingSpeed);
            var mappedSinVal = (sinVal + 1) / 2; // 0..1
            var newScale = Vector3.one * Mathf.Lerp(breathingMinSizeMult, breathingMaxSizeMult, mappedSinVal);
            newScale.z = 1f;
            
            foreach (var segment in _segments)
            {
                segment.transform.localScale = newScale;
            }
        }

        private Color ComputeSegmentColor(float baseT, float gradientOffset)
        {
            float tColor = Mathf.Repeat(baseT + gradientOffset, 1f);
            return colorGradient.Evaluate(tColor);
        }

        private float ComputeSegmentRadius(float baseT)
        {
            return bodyWidthCurve.Evaluate(baseT) * segmentRadiusSize;
        }

        public void ToggleDancing()
        {
            dancing = !dancing;
            _currentGradientScrollSpeed = dancing ? gradientScrollSpeedWhenDancing : 0f;
            // It's expected that LerpSegmentsToOriginal will handle the transition back every frame.
        }
    }
}
