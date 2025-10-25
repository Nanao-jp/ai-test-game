using UnityEngine;

namespace Game.Shooter
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BackgroundFollower : MonoBehaviour
    {
        [SerializeField] private Vector2 worldSize = new Vector2(12f, 200f);
        [SerializeField] private bool followX = true;
        [SerializeField] private bool followY = true;
        [SerializeField] private float yOffset = 0f;

        private SpriteRenderer _sr;
        private Camera _cam;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _cam = Camera.main;
            if (_sr != null)
            {
                _sr.drawMode = SpriteDrawMode.Tiled;
                _sr.size = worldSize;
                _sr.sortingOrder = -100;
            }
        }

        private void LateUpdate()
        {
            if (_cam == null) { _cam = Camera.main; if (_cam == null) return; }
            var pos = transform.position;
            if (followX) pos.x = _cam.transform.position.x;
            if (followY) pos.y = _cam.transform.position.y + yOffset;
            transform.position = pos;
        }

        public void SetWorldSize(Vector2 size)
        {
            worldSize = size;
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _sr.size = worldSize;
        }

        public void SetFollowX(bool value)
        {
            followX = value;
        }

        public void SetFollowY(bool value)
        {
            followY = value;
        }
    }
}


