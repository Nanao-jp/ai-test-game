using UnityEngine;

namespace Game.Shooter
{
    [DisallowMultipleComponent]
    public sealed class AutoScrollSystem : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 2.0f; // units per second (positive = up)
        [SerializeField] private bool affectCamera = true;
        [SerializeField] private Transform worldRoot = null!; // optional: move a parent instead of camera

        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void LateUpdate()
        {
            float dy = scrollSpeed * Time.deltaTime; // 正方向=上方向
            if (worldRoot != null)
            {
                worldRoot.position += new Vector3(0f, dy, 0f);
            }
            else if (affectCamera && _cam != null)
            {
                var pos = _cam.transform.position;
                pos.y += dy;
                _cam.transform.position = pos;
            }
        }

        public void SetSpeed(float unitsPerSecond)
        {
            scrollSpeed = unitsPerSecond;
        }

        public void SetAffectCamera(bool value)
        {
            affectCamera = value;
        }

        public void SetWorldRoot(Transform root)
        {
            worldRoot = root;
        }
    }
}


