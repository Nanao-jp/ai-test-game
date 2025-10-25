using UnityEngine;
using Game.Core;
using Game.Combat;
using Game.Enemies;

namespace Game.Shooter
{
    [DisallowMultipleComponent]
    public sealed class FormationSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 1.5f;
        [SerializeField] private int maxAlive = 25;
        [SerializeField] private Vector2 spawnAreaX = new Vector2(-4.5f, 4.5f);
        [SerializeField] private float spawnYOffset = 8f; // カメラ上端の少し上
        private Camera _cam;
        [SerializeField] private float enemyDownSpeed = 2.5f;

        private float _timer;
        private int _alive;

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            // Alive count bookkeeping (cheap): recalc occasionally is fine for prototype
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            _timer = spawnInterval;

            if (CountAlive() >= maxAlive) return;
            SpawnOne();
        }

        private int CountAlive()
        {
            // Not perfect, but sufficient for prototype: count objects named "Enemy" root-level
            var enemies = GameObject.FindGameObjectsWithTag("Untagged");
            int count = 0;
            foreach (var go in enemies)
            {
                if (go != null && go.name == "Enemy") count++;
            }
            _alive = count;
            return _alive;
        }

        private void SpawnOne()
        {
            if (_cam == null) _cam = Camera.main;
            var top = _cam != null ? _cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, -_cam.transform.position.z)).y : 8f;
            float x = Random.Range(spawnAreaX.x, spawnAreaX.y);
            var go = new GameObject("Enemy");
            go.layer = LayerUtil.Resolve("Enemy", LayerUtil.Resolve("Default"));
            go.transform.position = new Vector3(x, top + spawnYOffset, 0f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 5;
            sr.sprite = DefaultSprite();
            sr.color = new Color(0.95f, 0.35f, 0.35f, 1f);

            var mover = go.AddComponent<DownMover>();
            mover.SetSpeed(enemyDownSpeed);

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.3f;

            var health = go.AddComponent<Health>();
            health.SetMax(20f, true);
            go.AddComponent<Damageable>();

            var contactDamage = go.AddComponent<DamageSource>();
            contactDamage.Amount = 10f;
            contactDamage.CooldownSeconds = 0.2f;
            // 敵→プレイヤーのみ（自傷防止）
            contactDamage.DamageLayers = LayerUtil.MaskFor("Player", LayerUtil.Resolve("Default"));

            go.AddComponent<EnemyDropOnDeath>();

            var killer = go.AddComponent<ScreenBoundsKiller>();
            killer.SetDestroyOnExit(true);
        }

        private static Sprite s_Default;
        private static Sprite DefaultSprite()
        {
            if (s_Default != null) return s_Default;
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
            s_Default = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            return s_Default;
        }
    }
}


