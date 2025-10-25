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
        [SerializeField] private GameObject bulletPrefab = null; // 外部アセットの弾プレハブを使用可
        [SerializeField] private GameObject shipVisualPrefab = null; // 縦STG用の機体見た目（任意）
        [SerializeField] private bool enableInputPollingFallback = true; // PlayerInputイベントが来ない場合のフォールバック
        [Header("Shooter Mode")]
        [SerializeField] private bool shooterMode = false; // 縦STGなどで固定方向に射撃
        [SerializeField] private Vector2 fixedShootDirection = Vector2.up;
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
            var piLog = GetComponent<PlayerInput>();
            if (Game.Core.Debugging.DebugFlags.Verbose)
            {
                UnityEngine.Debug.Log($"[PlayerInput] present={(piLog!=null)} actions={(piLog?piLog.actions:null)} map={(piLog?piLog.defaultActionMap:null)} behavior={(piLog?piLog.notificationBehavior.ToString():"-")}");
            }
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
            // 縦STGでは近接を付けない
            if (!shooterMode)
            {
                if (GetComponent<MeleeWeapon>() == null)
                {
                    var melee = gameObject.AddComponent<MeleeWeapon>();
                }
            }

            var rootSr = GetComponent<SpriteRenderer>();
            if (shooterMode)
            {
                // 縦STGではSpriteProvider経由の外観適用を行わない（前の見た目を完全排除）
                var oldVisual = transform.Find("Visual") ? transform.Find("Visual").gameObject : null;
                if (oldVisual != null) Destroy(oldVisual);
                var oldVisualSprite = transform.Find("VisualSprite") ? transform.Find("VisualSprite").gameObject : null;
                if (oldVisualSprite != null) Destroy(oldVisualSprite);

                if (shipVisualPrefab != null)
                {
                    if (rootSr == null) rootSr = gameObject.AddComponent<SpriteRenderer>();
                    rootSr.enabled = false; // 見た目はPrefabに委ねる
                    var vis = Instantiate(shipVisualPrefab, transform);
                    vis.name = "Visual";
                    PrepareVisual(vis.transform);
                }
                else
                {
                    if (rootSr == null) rootSr = gameObject.AddComponent<SpriteRenderer>();
                    rootSr.enabled = true;
                    rootSr.sprite = GetDefaultSprite();
                    rootSr.color = new Color(0.7f, 0.9f, 1f, 1f);
                    rootSr.sortingOrder = 20;
                }
            }
            else
            {
                var set = SpriteProvider.GetSet();
                // Prefab優先
                if (set != null && set.playerPrefab != null && transform.Find("Visual") == null)
                {
                    var prefab = set.playerPrefab;
                    var vis = Instantiate(prefab, transform);
                    vis.name = "Visual";
                }
                // ルートのSpriteRendererは使わず、子に専用を用意
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
        }

        private void Start()
        {
            // 念のため起動直後にもう一度プレイヤースプライトを適用（ロード順の揺らぎ対策）
            if (!shooterMode)
            {
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
        }

        public void OnMove(InputValue value)
        {
            inputMove = value.Get<Vector2>();
            if (Game.Core.Debugging.DebugFlags.Verbose)
            {
                UnityEngine.Debug.Log($"[OnMove] value={inputMove}");
            }
            if (!shooterMode && inputMove.sqrMagnitude > 0.01f)
            {
                lastAimDir = inputMove.normalized;
                var melee = GetComponent<MeleeWeapon>();
                if (melee != null) melee.SetAimFromInput(lastAimDir);
            }
        }

#if ENABLE_INPUT_SYSTEM
        // 念のためUI側からのInputAction解除でイベントが飛ばない場合にも拾う
        private void OnEnable()
        {
            var pi = GetComponent<PlayerInput>();
            if (pi != null && pi.actions != null)
            {
                var move = pi.actions.FindAction("Move");
                if (move != null)
                {
                    move.performed += ctx => { inputMove = ctx.ReadValue<Vector2>(); if (Game.Core.Debugging.DebugFlags.Verbose) UnityEngine.Debug.Log($"[Action] Move={inputMove}"); };
                    move.canceled += ctx => { inputMove = Vector2.zero; if (Game.Core.Debugging.DebugFlags.Verbose) UnityEngine.Debug.Log("[Action] Move canceled"); };
                }
            }
        }
#endif

        private void FixedUpdate()
        {
            var velocity = inputMove.normalized * moveSpeed;
            body.linearVelocity = velocity;
            if (Game.Core.Debugging.DebugFlags.Verbose)
            {
                UnityEngine.Debug.Log($"[Player] input={inputMove} vel={body.linearVelocity}");
            }
        }

        private void Update()
        {
            // 入力イベントが来ない場合のフォールバック（WASD/カーソル/ゲームパッド）
            if (enableInputPollingFallback && inputMove.sqrMagnitude < 0.0001f)
            {
#if ENABLE_INPUT_SYSTEM
                Vector2 polled = Vector2.zero;
                var k = Keyboard.current;
                if (k != null)
                {
                    float x = (k.dKey.isPressed || k.rightArrowKey.isPressed ? 1f : 0f) - (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1f : 0f);
                    float y = (k.wKey.isPressed || k.upArrowKey.isPressed ? 1f : 0f) - (k.sKey.isPressed || k.downArrowKey.isPressed ? 1f : 0f);
                    polled = new Vector2(x, y);
                }
                var g = Gamepad.current;
                if (g != null)
                {
                    polled += g.leftStick.ReadValue();
                }
                if (polled.sqrMagnitude > 0.01f)
                {
                    inputMove = Vector2.ClampMagnitude(polled, 1f);
                    if (Game.Core.Debugging.DebugFlags.Verbose)
                    {
                        UnityEngine.Debug.Log($"[PollInput] used value={inputMove}");
                    }
                }
#endif
            }
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                shootTimer = autoShootInterval;
                FireAutoBullet();
            }
        }

        private void FireAutoBullet()
        {
            // 縦STG時は固定方向、それ以外は最後の入力方向
            var dir = shooterMode ? (fixedShootDirection.sqrMagnitude > 0.001f ? fixedShootDirection.normalized : Vector2.up)
                                  : (lastAimDir.sqrMagnitude > 0.001f ? lastAimDir.normalized : Vector2.right);
            GameObject go;
            if (bulletPrefab != null)
            {
                go = Instantiate(bulletPrefab);
                go.name = go.name.Replace("(Clone)", "");
                go.layer = LayerUtil.Resolve("PlayerProjectile", LayerUtil.Resolve("Default"));
                go.transform.position = transform.position + (Vector3)(dir * 0.25f);

                // 必要コンポーネントの確保（無くても安全に動くように）
                var rb0 = go.GetComponent<Rigidbody2D>();
                if (rb0 == null) rb0 = go.AddComponent<Rigidbody2D>();
                rb0.gravityScale = 0f;
                rb0.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb0.linearVelocity = dir * bulletSpeed;

                var dmg0 = go.GetComponent<DamageSource>();
                if (dmg0 == null) dmg0 = go.AddComponent<DamageSource>();
                dmg0.Amount = bulletDamage;
                dmg0.CooldownSeconds = 0f;
                // 弾が自分や味方に当たらないよう確実に敵レイヤのみに設定
                dmg0.DamageLayers = LayerUtil.MaskFor("Enemy", LayerUtil.Resolve("Default"));
                dmg0.DestroyOnHit = true;

                var col0 = go.GetComponent<Collider2D>();
                if (col0 == null)
                {
                    var c = go.AddComponent<CircleCollider2D>();
                    c.isTrigger = true;
                    c.radius = 0.12f;
                }

                if (go.GetComponent<BulletLifetime>() == null) go.AddComponent<BulletLifetime>();
            }
            else
            {
                go = new GameObject("Bullet");
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
        }

        private void PrepareVisual(Transform visualRoot)
        {
            if (visualRoot == null) return;
            visualRoot.localPosition = Vector3.zero;
            visualRoot.localRotation = Quaternion.identity;
            visualRoot.localScale = Vector3.one;

            // Remove physics and damaging components from visual prefab to avoid self-collisions
            var rb2ds = visualRoot.GetComponentsInChildren<Rigidbody2D>(true);
            foreach (var r in rb2ds)
            {
                Destroy(r);
            }
            var cols = visualRoot.GetComponentsInChildren<Collider2D>(true);
            foreach (var c in cols)
            {
                Destroy(c);
            }
            var dmg = visualRoot.GetComponentsInChildren<DamageSource>(true);
            foreach (var d in dmg)
            {
                Destroy(d);
            }

            // Disable any control scripts shipped with external prefabs
            var behaviours = visualRoot.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var b in behaviours)
            {
                if (b == null) continue;
                // Skip SpriteRenderer/Animator-like components (not MonoBehaviour) automatically
                // For safety, disable all scripts under the visual tree
                b.enabled = false;
            }
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


