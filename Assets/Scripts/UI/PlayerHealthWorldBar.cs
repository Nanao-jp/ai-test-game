using UnityEngine;
using Game.Combat;

namespace Game.UI
{
    [DisallowMultipleComponent]
    public sealed class PlayerHealthWorldBar : MonoBehaviour
    {
        [SerializeField] private Health _playerHealth = null!;
        [SerializeField] private Vector2 _size = new Vector2(1.8f, 0.18f);
        [SerializeField] private float _offsetY = -0.6f; // プレイヤーの少し下
        [SerializeField] private Color _bgColor = new Color(0f, 0f, 0f, 0.6f);
        [SerializeField] private Color _fillColor = new Color(0.2f, 0.9f, 0.2f, 0.95f);
        [SerializeField] private int _sortingOrder = 50;

        private Transform _barRoot;
        private SpriteRenderer _bg;
        private SpriteRenderer _fill;

        public static void AttachTo(GameObject player, Health health)
        {
            var existing = player.GetComponentInChildren<PlayerHealthWorldBar>();
            if (existing != null)
            {
                existing._playerHealth = health;
                return;
            }

            var go = new GameObject("PlayerHealthWorldBar");
            go.transform.SetParent(player.transform, false);
            var bar = go.AddComponent<PlayerHealthWorldBar>();
            bar._playerHealth = health;
        }

        private void OnEnable()
        {
            BuildIfNeeded();
            if (_playerHealth != null)
            {
                _playerHealth.OnDied += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnDied -= HandleDied;
            }
        }

        private void HandleDied()
        {
            if (_fill != null)
            {
                SetRatio(0f);
            }
        }

        private void LateUpdate()
        {
            if (_playerHealth == null) return;
            var ratio = _playerHealth.MaxHealth <= 0f ? 0f : Mathf.Clamp01(_playerHealth.CurrentHealth / _playerHealth.MaxHealth);
            SetRatio(ratio);
            if (_barRoot != null)
            {
                _barRoot.localPosition = new Vector3(0f, _offsetY, 0f);
            }
        }

        private void SetRatio(float r)
        {
            if (_fill == null) return;
            r = Mathf.Clamp01(r);
            var w = _size.x;
            var h = _size.y;
            var spr = _fill.sprite;
            if (spr == null) return;
            var baseW = spr.bounds.size.x;
            var baseH = spr.bounds.size.y;
            if (baseW <= 0f || baseH <= 0f) return;
            var sx = (w * r) / baseW;
            var sy = h / baseH;
            _fill.transform.localScale = new Vector3(sx, sy, 1f);
            _fill.transform.localPosition = new Vector3((-w * 0.5f) + (w * r * 0.5f), 0f, 0f);
        }

        private void BuildIfNeeded()
        {
            if (_barRoot != null) return;
            _barRoot = new GameObject("BarRoot").transform;
            _barRoot.SetParent(transform, false);
            _barRoot.localPosition = new Vector3(0f, _offsetY, 0f);

            var bgGo = new GameObject("BG");
            bgGo.transform.SetParent(_barRoot, false);
            _bg = bgGo.AddComponent<SpriteRenderer>();
            _bg.sprite = GetWhiteSprite();
            _bg.color = _bgColor;
            _bg.sortingOrder = _sortingOrder;
            _bg.drawMode = SpriteDrawMode.Simple;
            // scale to desired size
            var bgBaseW = _bg.sprite.bounds.size.x;
            var bgBaseH = _bg.sprite.bounds.size.y;
            if (bgBaseW > 0f && bgBaseH > 0f)
            {
                _bg.transform.localScale = new Vector3(_size.x / bgBaseW, _size.y / bgBaseH, 1f);
            }

            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(_barRoot, false);
            _fill = fillGo.AddComponent<SpriteRenderer>();
            _fill.sprite = GetWhiteSprite();
            _fill.color = _fillColor;
            _fill.sortingOrder = _sortingOrder + 1;
            _fill.drawMode = SpriteDrawMode.Simple;
            // initialize full size
            var fBaseW = _fill.sprite.bounds.size.x;
            var fBaseH = _fill.sprite.bounds.size.y;
            if (fBaseW > 0f && fBaseH > 0f)
            {
                _fill.transform.localScale = new Vector3(_size.x / fBaseW, _size.y / fBaseH, 1f);
            }
            _fill.transform.localPosition = new Vector3(0f, 0f, 0f);
        }

        private static Sprite s_White;
        private static Sprite GetWhiteSprite()
        {
            if (s_White != null) return s_White;
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
            s_White = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            return s_White;
        }
    }
}


