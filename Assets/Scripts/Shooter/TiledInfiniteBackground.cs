using System.Collections.Generic;
using UnityEngine;

namespace Game.Shooter
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class TiledInfiniteBackground : MonoBehaviour
    {
        [SerializeField] private int segmentCount = 3;
        [SerializeField] private float segmentWorldHeight = 10f;
        [SerializeField] private float startYOffset = 0f;
        [SerializeField] private bool vertical = true; // true: stack vertically for vertical shooters

        private readonly List<Transform> _segments = new List<Transform>(8);
        private Camera _cam;
        private SpriteRenderer _sr;

        private void Awake()
        {
            _cam = Camera.main;
            _sr = GetComponent<SpriteRenderer>();
            _sr.drawMode = SpriteDrawMode.Tiled;

            BuildSegments();
        }

        private void BuildSegments()
        {
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            if (_sr == null)
            {
                Debug.LogWarning("TiledInfiniteBackground: SpriteRenderer not found.");
                return;
            }
            // Clear existing children
            for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
            _segments.Clear();

            for (int i = 0; i < Mathf.Max(2, segmentCount); i++)
            {
                var go = new GameObject($"BG_Segment_{i}");
                go.transform.SetParent(transform, false);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = _sr != null ? _sr.sprite : null;
                sr.drawMode = SpriteDrawMode.Tiled;
                var width = _sr != null ? _sr.size.x : 12f;
                sr.size = new Vector2(width, segmentWorldHeight);
                sr.sortingOrder = _sr != null ? _sr.sortingOrder : -100;

                float offset = (i * segmentWorldHeight) + startYOffset;
                go.transform.localPosition = vertical ? new Vector3(0f, offset, 0f) : new Vector3(0f, 0f, 0f);
                _segments.Add(go.transform);
            }
            // Hide the source renderer (used as template only)
            _sr.enabled = false;
        }

        private void LateUpdate()
        {
            if (_cam == null) _cam = Camera.main;
            if (_cam == null || _segments.Count == 0) return;

            float camBottom = _cam.transform.position.y - _cam.orthographicSize;
            float camTop = _cam.transform.position.y + _cam.orthographicSize;

            // Find highest and lowest segment
            int lowestIdx = 0, highestIdx = 0;
            for (int i = 1; i < _segments.Count; i++)
            {
                if (_segments[i].position.y < _segments[lowestIdx].position.y) lowestIdx = i;
                if (_segments[i].position.y > _segments[highestIdx].position.y) highestIdx = i;
            }

            // If the lowest segment is above camera bottom (moving down) we don't need to do anything.
            // When camera goes up and the lowest is fully below view, move it above the highest.
            float lowestTop = _segments[lowestIdx].position.y + (segmentWorldHeight * 0.5f);
            float highestTop = _segments[highestIdx].position.y + (segmentWorldHeight * 0.5f);

            // Threshold when lowest is far below camera bottom
            float threshold = camBottom - segmentWorldHeight * 0.5f;
            if (lowestTop < threshold)
            {
                var t = _segments[lowestIdx];
                float newY = highestTop + (segmentWorldHeight);
                t.position = new Vector3(t.position.x, newY - (segmentWorldHeight * 0.5f), t.position.z);
            }
        }

        public void Configure(int segments, float segmentHeight)
        {
            segmentCount = Mathf.Max(2, segments);
            segmentWorldHeight = Mathf.Max(1f, segmentHeight);
            BuildSegments();
        }
    }
}


