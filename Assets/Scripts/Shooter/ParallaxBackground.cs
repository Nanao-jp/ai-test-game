using UnityEngine;

namespace Game.Shooter
{
    [DisallowMultipleComponent]
    public sealed class ParallaxBackground : MonoBehaviour
    {
        [System.Serializable]
        private sealed class Layer
        {
            public Transform transform;
            public float speedMultiplier = 0.5f; // relative to AutoScroll speed
        }

        [SerializeField] private AutoScrollSystem scroll;
        [SerializeField] private Layer[] layers = new Layer[0];

        private float _prevCamY;
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
            if (scroll == null) scroll = FindAnyObjectByType<AutoScrollSystem>();
            if (_cam != null) _prevCamY = _cam.transform.position.y;
        }

        private void LateUpdate()
        {
            if (_cam == null) return;
            float currentY = _cam.transform.position.y;
            float dy = currentY - _prevCamY;
            _prevCamY = currentY;

            if (dy == 0f) return;

            foreach (var layer in layers)
            {
                if (layer == null || layer.transform == null) continue;
                var pos = layer.transform.position;
                pos.y += dy * layer.speedMultiplier;
                layer.transform.position = pos;
            }
        }
    }
}


