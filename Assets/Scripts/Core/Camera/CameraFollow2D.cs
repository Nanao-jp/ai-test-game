using UnityEngine;

namespace Core.Camera2D
{
    [DisallowMultipleComponent]
    public sealed class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target = null!;
        [SerializeField] private float smoothTime = 0.1f;
        private Vector3 velocity;

        public void SetTarget(Transform t) => target = t;

        private void LateUpdate()
        {
            if (target == null) return;
            var current = transform.position;
            var desired = new Vector3(target.position.x, target.position.y, current.z);
            transform.position = Vector3.SmoothDamp(current, desired, ref velocity, smoothTime);
        }
    }
}


