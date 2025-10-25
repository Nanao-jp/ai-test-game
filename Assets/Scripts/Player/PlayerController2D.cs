using UnityEngine;
using UnityEngine.InputSystem;
using Game.Combat;
using Game.Core;
using Game.UI;
using Game.Systems.Experience;
using Game.Art;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody2D body = null!;
        [Header("Combat")]
        [SerializeField] private Health health = null!;
        [SerializeField] private Damageable damageable = null!;
        [SerializeField] private float autoShootInterval = 0.5f;
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float bulletDamage = 10f;
        private float shootTimer;

        private Vector2 inputMove;
        private Vector2 lastAimDir = Vector2.right;

        private void Reset()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            health = GetComponent<Health>();
            damageable = GetComponent<Damageable>();
            var col = GetComponent<Collider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
                var cc = col as CircleCollider2D;
                if (cc != null) cc.radius = 0.25f;
            }
        }

        private void Awake()
        {
            if (body == null) body = GetComponent<Rigidbody2D>();
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            if (health == null) health = GetComponent<Health>();
            if (health == null) health = gameObject.AddComponent<Health>();
            if (damageable == null) damageable = GetComponent<Damageable>();
            if (damageable == null) damageable = gameObject.AddComponent<Damageable>();
            gameObject.layer = LayerUtil.Resolve("Player", LayerUtil.Resolve("Default"));
            if (GetComponent<Collider2D>() == null)
            {
                var col = gameObject.AddComponent<CircleCollider2D>();
                col.isTrigger = false;
                var cc = col as CircleCollider2D;
                if (cc != null) cc.radius = 0.25f;
            }

            // 左上HUDは一旦停止。プレイヤー足元にワールドバーを表示
            PlayerHealthWorldBar.AttachTo(gameObject, health);

            // 経験値システムとマグネットを付与
            if (GetComponent<ExperienceSystem>() == null) gameObject.AddComponent<ExperienceSystem>();
            if (GetComponent<Magnet>() == null) gameObject.AddComponent<Magnet>();
            if (GetComponent<MeleeWeapon>() == null)
            {
                var melee = gameObject.AddComponent<MeleeWeapon>();
            }

            var set = SpriteProvider.GetSet();
            // Prefab優先
            if (set != null && set.playerPrefab != null && transform.Find("Visual") == null)
            {
                var prefab = set.playerPrefab;
                var vis = Instantiate(prefab, transform);
                vis.name = "Visual";
            }

            // ルートのSpriteRendererは使わず、子に専用を用意
            var rootSr = GetComponent<SpriteRenderer>();
            if (rootSr != null) rootSr.enabled = false;
            var visual = transform.Find("VisualSprite");
            if (visual == null)
            {
                var go = new GameObject("VisualSprite");
                go.transform.SetParent(transform, false);
                visual = go.transform;
            }
            var sr = visual.GetComponent<SpriteRenderer>();
            if (sr == null) sr = visual.gameObject.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 20;
            sr.color = Color.white;
            if (set != null && set.playerSprite != null)
            {
                sr.sprite = set.playerSprite;
            }
            else
            {
                sr.sprite = GetDefaultSprite();
            }
        }

        private void Start()
        {
            // 念のため起動直後にもう一度プレイヤースプライトを適用（ロード順の揺らぎ対策）
            var set = SpriteProvider.GetSet();
            var visual = transform.Find("VisualSprite");
            if (visual != null)
            {
                var sr = visual.GetComponent<SpriteRenderer>();
                if (sr != null && set != null && set.playerSprite != null)
                {
                    sr.sprite = set.playerSprite;
                }
            }
        }

        public void OnMove(InputValue value)
        {
            inputMove = value.Get<Vector2>();
            if (inputMove.sqrMagnitude > 0.01f)
            {
                lastAimDir = inputMove.normalized;
                var melee = GetComponent<MeleeWeapon>();
                if (melee != null) melee.SetAimFromInput(lastAimDir);
            }
        }

        private void FixedUpdate()
        {
            var velocity = inputMove.normalized * moveSpeed;
            body.linearVelocity = velocity;
        }

        private void Update()
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                shootTimer = autoShootInterval;
                FireAutoBullet();
            }
        }

        private void FireAutoBullet()
        {
            // 入力の最後の方向に射出
            var dir = lastAimDir.sqrMagnitude > 0.001f ? lastAimDir.normalized : Vector2.right;
            var go = new GameObject("Bullet");
            go.layer = LayerUtil.Resolve("PlayerProjectile", LayerUtil.Resolve("Default"));
            go.transform.position = transform.position + (Vector3)(dir * 0.25f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = dir * bulletSpeed;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 15;
            var set = SpriteProvider.GetSet();
            if (set != null && set.bulletSprite != null)
            {
                sr.sprite = set.bulletSprite;
                sr.color = Color.white;
            }
            else
            {
                sr.sprite = GetDefaultSprite();
                sr.color = Color.yellow;
            }

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.12f;

            var dmg = go.AddComponent<DamageSource>();
            dmg.Amount = bulletDamage;
            dmg.CooldownSeconds = 0f;
            dmg.DamageLayers = LayerUtil.MaskFor("Enemy", LayerUtil.Resolve("Default"));
            dmg.DestroyOnHit = true;

            go.AddComponent<BulletLifetime>();
        }

        private static Sprite s_DefaultSprite;
        private static Sprite GetDefaultSprite()
        {
            if (s_DefaultSprite != null) return s_DefaultSprite;
            const int size = 4;
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
    }
}


