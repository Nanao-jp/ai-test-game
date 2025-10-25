using System.Collections.Generic;
using UnityEngine;
using Game.Combat;
using Game.Core;
using Game.Art;

namespace Game.Enemies
{
    [DisallowMultipleComponent]
    public sealed class EnemySpawner : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target = null!;

        [Header("Spawn")]
        [SerializeField] private float spawnIntervalSeconds = 1.25f;
        [SerializeField] private int maxAlive = 15;
        [SerializeField] private float spawnRadius = 15f;
        [SerializeField] private int burstCount = 1;

        [Header("Enemy Visual")] 
        [SerializeField] private Color enemyColor = new Color(0.9f, 0.3f, 0.35f, 1f);

        private float spawnTimer;
        private readonly List<GameObject> spawned = new List<GameObject>(64);

        public void SetTarget(Transform t) => target = t;

        private void Start()
        {
            if (target == null)
            {
                var playerGo = GameObject.Find("Player");
                if (playerGo != null) target = playerGo.transform;
            }
        }

        private void Update()
        {
            if (target == null) return;

            // Clean dead refs
            for (int i = spawned.Count - 1; i >= 0; i--)
            {
                if (spawned[i] == null) spawned.RemoveAt(i);
            }

            if (spawned.Count >= maxAlive) return;

            spawnTimer -= Time.deltaTime;
            if (spawnTimer > 0f) return;

            spawnTimer = spawnIntervalSeconds;
            for (int i = 0; i < burstCount && spawned.Count < maxAlive; i++)
            {
                var e = SpawnOne();
                spawned.Add(e);
            }
        }

        private GameObject SpawnOne()
        {
            // Position on a ring around target
            var angle = Random.Range(0f, Mathf.PI * 2f);
            var pos = target.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;

            var go = new GameObject("Enemy");
            go.layer = LayerUtil.Resolve("Enemy", LayerUtil.Resolve("Default"));
            go.transform.position = pos;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // Simple visual
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 5;
            var set = SpriteProvider.GetSet();
            if (set != null)
            {
                var enemySprite = set.enemySprite;
                if (set.playerSprite != null && enemySprite == set.playerSprite && set.enemyAltSprite != null)
                {
                    enemySprite = set.enemyAltSprite;
                }
                if (enemySprite != null)
                {
                    sr.sprite = enemySprite;
                    sr.color = Color.white;
                }
                else
                {
                    sr.sprite = GetDefaultSprite();
                    sr.color = enemyColor;
                }
            }
            else
            {
                sr.sprite = GetDefaultSprite();
                sr.color = enemyColor;
            }

            var ctrl = go.AddComponent<EnemyController2D>();
            ctrl.SetTarget(target);

            // Collider + Combat
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.3f;

            var health = go.AddComponent<Health>();
            health.SetMax(30f, true);
            go.AddComponent<Damageable>();

            var contactDamage = go.AddComponent<DamageSource>();
            contactDamage.Amount = 5f;
            contactDamage.CooldownSeconds = 0.5f;
            contactDamage.DamageLayers = LayerUtil.MaskFor("Player", LayerUtil.Resolve("Default"));

            // 経験値ドロップ
            go.AddComponent<EnemyDropOnDeath>();

            return go;
        }

        private static Sprite s_DefaultSprite;

        private static Sprite GetDefaultSprite()
        {
            if (s_DefaultSprite != null) return s_DefaultSprite;
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
            s_DefaultSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            return s_DefaultSprite;
        }

        private void OnDrawGizmosSelected()
        {
            if (target == null) return;
            Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.25f);
            Gizmos.DrawWireSphere(target.position, spawnRadius);
        }
    }
}


