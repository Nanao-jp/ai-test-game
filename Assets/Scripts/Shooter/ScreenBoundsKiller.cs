using UnityEngine;

namespace Game.Shooter
{
    [DisallowMultipleComponent]
    public sealed class ScreenBoundsKiller : MonoBehaviour
    {
        [SerializeField] private float margin = 1.0f; // units outside the screen to allow
        [SerializeField] private bool destroyOnExit = true;
        [SerializeField] private Transform target = null!; // if null, self

        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            if (_cam == null) return;
            var t = target != null ? target : transform;
            var pos = t.position;
            var min = _cam.ViewportToWorldPoint(new Vector3(0, 0, -_cam.transform.position.z));
            var max = _cam.ViewportToWorldPoint(new Vector3(1, 1, -_cam.transform.position.z));

            if (pos.x < min.x - margin || pos.x > max.x + margin || pos.y < min.y - margin || pos.y > max.y + margin)
            {
                if (destroyOnExit)
                {
                    Destroy(gameObject);
                }
                enabled = false;
            }
        }

        public void SetDestroyOnExit(bool value)
        {
            destroyOnExit = value;
        }

        public void SetMargin(float value)
        {
            margin = Mathf.Max(0f, value);
        }
    }
}


