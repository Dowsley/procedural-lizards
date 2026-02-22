using UnityEngine;

namespace Behaviours
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        [SerializeField] private Vector3 offset = new(0f, 12f, -8f);
        [SerializeField] private float smoothSpeed = 5f;

        private void LateUpdate()
        {
            if (!target)
                return;

            Vector3 desiredPosition = target.position + offset;
            float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
        }
    }
}
