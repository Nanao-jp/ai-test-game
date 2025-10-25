using UnityEngine;
using UnityEngine.UI;
using Game.Combat;

namespace Game.UI
{
    [DisallowMultipleComponent]
    public sealed class PlayerHealthHud : MonoBehaviour
    {
        private const string CanvasName = "HUD_Canvas";
        private const string BarRootName = "PlayerHP";

        [SerializeField] private Health _playerHealth = null!;
        [SerializeField] private Image _barFill = null!;

        private static Sprite s_UISprite;
        private static Sprite GetUISprite()
        {
            if (s_UISprite != null) return s_UISprite;
            const int size = 8;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            s_UISprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            return s_UISprite;
        }

        public static void EnsureExists(Health playerHealth)
        {
            // Find or create a helper object to host this component
            var existing = FindFirstObjectByType<PlayerHealthHud>();
            if (existing != null)
            {
                existing._playerHealth = playerHealth;
                return;
            }

            var go = new GameObject("PlayerHealthHud");
            DontDestroyOnLoad(go);
            var hud = go.AddComponent<PlayerHealthHud>();
            hud._playerHealth = playerHealth;
        }

        private void Start()
        {
            EnsureCanvasAndBar();
        }

        private void Update()
        {
            if (_playerHealth == null || _barFill == null) return;
            var ratio = _playerHealth.MaxHealth <= 0f ? 0f : Mathf.Clamp01(_playerHealth.CurrentHealth / _playerHealth.MaxHealth);
            _barFill.fillAmount = ratio;
        }

        private void EnsureCanvasAndBar()
        {
            // Canvas
            var canvasGo = GameObject.Find(CanvasName);
            if (canvasGo == null)
            {
                canvasGo = new GameObject(CanvasName);
                var canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGo.AddComponent<CanvasScaler>();
                canvasGo.AddComponent<GraphicRaycaster>();
                DontDestroyOnLoad(canvasGo);
            }

            // Bar root
            var barRoot = GameObject.Find(BarRootName);
            if (barRoot == null)
            {
                barRoot = new GameObject(BarRootName);
                barRoot.transform.SetParent(canvasGo.transform, false);
                var rt = barRoot.AddComponent<RectTransform>();
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(16f, -16f);
                rt.sizeDelta = new Vector2(240f, 20f);

                // Background
                var bgGo = new GameObject("BG");
                bgGo.transform.SetParent(barRoot.transform, false);
                var bgRt = bgGo.AddComponent<RectTransform>();
                bgRt.anchorMin = new Vector2(0f, 0f);
                bgRt.anchorMax = new Vector2(1f, 1f);
                bgRt.offsetMin = Vector2.zero;
                bgRt.offsetMax = Vector2.zero;
                var bgImg = bgGo.AddComponent<Image>();
                bgImg.sprite = GetUISprite();
                bgImg.type = Image.Type.Sliced;
                bgImg.color = new Color(0f, 0f, 0f, 0.6f);

                // Fill
                var fillGo = new GameObject("Fill");
                fillGo.transform.SetParent(barRoot.transform, false);
                var fillRt = fillGo.AddComponent<RectTransform>();
                fillRt.anchorMin = new Vector2(0f, 0f);
                fillRt.anchorMax = new Vector2(1f, 1f);
                fillRt.offsetMin = new Vector2(2f, 2f);
                fillRt.offsetMax = new Vector2(-2f, -2f);
                var fillImg = fillGo.AddComponent<Image>();
                fillImg.sprite = GetUISprite();
                fillImg.color = new Color(0.2f, 0.9f, 0.2f, 0.95f);
                fillImg.type = Image.Type.Filled;
                fillImg.fillMethod = Image.FillMethod.Horizontal;
                fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
                fillImg.fillAmount = 1f;
                _barFill = fillImg;
            }
            else
            {
                // 既存のUIがある場合はFill参照を確実に取得
                var fillTr = barRoot.transform.Find("Fill");
                if (fillTr != null)
                {
                    var img = fillTr.GetComponent<Image>();
                    if (img != null)
                    {
                        _barFill = img;
                        if (_barFill.sprite == null)
                        {
                            _barFill.sprite = GetUISprite();
                            _barFill.type = Image.Type.Filled;
                        }
                    }
                }
            }
        }
    }
}


