using UnityEngine;

namespace ShadowEscape.CameraControl
{
    /// <summary>
    /// Smoothly follows a target (the player) with configurable offset,
    /// smoothing, and optional bounding box to keep the camera within level limits.
    /// Attach to: Main Camera.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
        [SerializeField] private float smoothTime = 0.2f;

        [Header("Optional Bounds")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private Vector3 velocity = Vector3.zero;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothed = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

            if (useBounds)
            {
                smoothed.x = Mathf.Clamp(smoothed.x, minBounds.x, maxBounds.x);
                smoothed.y = Mathf.Clamp(smoothed.y, minBounds.y, maxBounds.y);
            }

            transform.position = smoothed;
        }

        public void SetTarget(Transform newTarget) => target = newTarget;
    }
}
