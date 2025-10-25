using UnityEngine;

namespace Core.Camera2D
{
    // 画面比率が横長でも 9:16 の表示領域を維持する（余白にピラーボックス）
    [RequireComponent(typeof(Camera))]
    public sealed class AspectEnforcerPortrait : MonoBehaviour
    {
        [SerializeField] private Vector2 targetAspect = new Vector2(9, 16);

        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographic = true;
        }

        private void OnPreCull()
        {
            Enforce();
        }

        private void Enforce()
        {
            float target = targetAspect.x / targetAspect.y; // 0.5625 for 9:16
            float window = (float)Screen.width / Screen.height;

            if (window > target)
            {
                // 横が広い→左右に余白
                float desiredWidth = target / window;
                float x = (1f - desiredWidth) * 0.5f;
                _cam.rect = new Rect(x, 0f, desiredWidth, 1f);
            }
            else
            {
                // 縦が広い→上下に余白
                float desiredHeight = window / target;
                float y = (1f - desiredHeight) * 0.5f;
                _cam.rect = new Rect(0f, y, 1f, desiredHeight);
            }
        }
    }
}


