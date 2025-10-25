using UnityEngine;

namespace Game.Systems.Experience
{
    [DisallowMultipleComponent]
    public sealed class ExpPickup : MonoBehaviour
    {
        [SerializeField] private int _amount = 1;
        [SerializeField] private float _magnetSpeed = 6f;

        private Transform _target;
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;

        public void SetMagnetTarget(Transform t, float speed)
        {
            _target = t;
            _magnetSpeed = Mathf.Max(0.5f, speed);
        }

        private void Awake()
        {
            if ((_rb = GetComponent<Rigidbody2D>()) == null)
            {
                _rb = gameObject.AddComponent<Rigidbody2D>();
            }
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;

            var col = GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
            }
            col.isTrigger = true;
            col.radius = 0.16f;

            _sr = GetComponent<SpriteRenderer>();
            if (_sr == null)
            {
                _sr = gameObject.AddComponent<SpriteRenderer>();
                _sr.sortingOrder = 15;
            }
            var set = Game.Art.SpriteProvider.GetSet();
            if (set != null && set.expSprite != null)
            {
                _sr.sprite = set.expSprite;
                _sr.color = Color.white;
            }
            else
            {
                _sr.sprite = GetWhiteSprite();
                _sr.color = new Color(0.3f, 0.9f, 1f, 0.95f);
            }
        }

        private void Update()
        {
            if (_target == null) return;
            var pos = (Vector2)transform.position;
            var dir = ((Vector2)_target.position - pos).normalized;
            var speed = _magnetSpeed;
            transform.position = pos + dir * speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var exp = other.GetComponentInParent<ExperienceSystem>();
            if (exp == null) return;
            exp.AddExperience(_amount);
            Destroy(gameObject);
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


