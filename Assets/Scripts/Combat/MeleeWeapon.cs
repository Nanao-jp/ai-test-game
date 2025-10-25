using UnityEngine;
using Game.Core;

namespace Game.Combat
{
    [DisallowMultipleComponent]
    public sealed class MeleeWeapon : MonoBehaviour
    {
        [SerializeField] private float swingInterval = 0.8f;
        [SerializeField] private float arcDegrees = 90f;
        [SerializeField] private float range = 1.4f;
        [SerializeField] private float forwardOffset = 0.7f; // 判定・VFXを前方に出す距離
        [SerializeField] private float damage = 12f;
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private Color slashColor = new Color(1f, 1f, 0.2f, 0.85f);
        [SerializeField] private float slashThickness = 0.22f;
        [SerializeField] private float slashDuration = 0.12f;
        [Header("Scale Up (Hit/VFX)")]
        [SerializeField] private float extraRange = 0.6f;           // 追加の射程（判定）
        [SerializeField] private float extraArcDegrees = 30f;       // 追加の扇角（判定）
        [SerializeField] private int vfxCount = 2;                  // 複数本の斬撃VFX
        [SerializeField] private float vfxAngleSpread = 25f;        // VFXの角度ばらし（合計 / 片側）
        [SerializeField] private float vfxLengthMultiplier = 1.4f;  // VFX長さ倍率
        [SerializeField] private float vfxThicknessMultiplier = 1.75f; // VFX太さ倍率
        [Header("Orientation")]
        [SerializeField] private bool swingOrthogonalToMove = true;   // 移動方向に対して直交方向で斬る
        [SerializeField] private bool snapToCardinal = true;          // 完全な縦/横にスナップ

        private float timer;
        private Rigidbody2D _rb;
        private Vector2 _lastAim = Vector2.right;
        private Game.Player.PlayerController2D _player;

        private void Reset()
        {
            hitLayers = LayerUtil.MaskFor("Enemy", LayerUtil.Resolve("Default"));
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _player = GetComponent<Game.Player.PlayerController2D>();
            if (hitLayers.value == 0)
            {
                hitLayers = LayerUtil.MaskFor("Enemy", LayerUtil.Resolve("Default"));
            }
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = swingInterval;
                PerformSwing();
            }
        }

        private void PerformSwing()
        {
            var forward = GetAimDirection();
            var aimForOffset = forward;
            var swingDir = forward;
            if (snapToCardinal)
            {
                // 進行方向を縦/横にスナップ
                if (Mathf.Abs(forward.x) >= Mathf.Abs(forward.y))
                    forward = new Vector2(Mathf.Sign(forward.x), 0f);
                else
                    forward = new Vector2(0f, Mathf.Sign(forward.y));
            }
            if (swingOrthogonalToMove)
            {
                // 直交方向（90度回転）。横を向いてる時は縦、上向きの時は横
                swingDir = new Vector2(-forward.y, forward.x);
            }
            var centerHit = (Vector2)transform.position + forward * forwardOffset;

            var hitRange = range + Mathf.Max(0f, extraRange);
            var hitArc = arcDegrees + Mathf.Max(0f, extraArcDegrees);
            var hits = Physics2D.OverlapCircleAll(centerHit, hitRange, hitLayers.value);
            foreach (var c in hits)
            {
                if (c == null) continue;
                // 自身・子孫は除外
                if (c.transform.IsChildOf(transform)) continue;
                if (_rb != null && c.attachedRigidbody == _rb) continue;
                var to = ((Vector2)c.transform.position - centerHit).normalized;
                var ang = Vector2.Angle(swingDir, to);
                if (ang > hitArc * 0.5f) continue;
                var h = c.GetComponent<Health>();
                if (h != null)
                {
                    h.TakeDamage(damage);
                }
            }

            // simple debug gizmo lifetime
            _lastSwingPos = centerHit;
            _lastSwingForward = swingDir;
            _lastSwingTime = Time.time;

            // VFXは見やすいようにさらに前方へ（長さの半分）
            var baseLen = hitRange * Mathf.Max(0.5f, vfxLengthMultiplier);
            var baseThick = slashThickness * Mathf.Max(0.5f, vfxThicknessMultiplier);
            var centerVfx = (Vector2)transform.position + aimForOffset * (forwardOffset + hitRange * 0.5f);

            // 複数本、角度ばらし
            int count = Mathf.Max(1, vfxCount);
            float totalSpread = Mathf.Max(0f, vfxAngleSpread);
            for (int i = 0; i < count; i++)
            {
                float t = count == 1 ? 0f : Mathf.Lerp(-totalSpread, totalSpread, (float)i / (count - 1));
                var dir = Quaternion.Euler(0f, 0f, t) * swingDir;
                SlashVfx.Spawn(centerVfx, dir, baseLen, baseThick, slashColor, slashDuration, 30);
            }
        }

        private Vector2 GetAimDirection()
        {
            // Playerの最後の入力方向を優先
            if (_player != null)
            {
                var dir = GetPlayerAimDir();
                if (dir.sqrMagnitude > 0.0001f)
                {
                    _lastAim = dir;
                    return _lastAim;
                }
            }
            if (_rb != null && _rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                _lastAim = _rb.linearVelocity.normalized;
                return _lastAim;
            }
            return _lastAim;
        }

        private Vector2 GetPlayerAimDir()
        {
            // PlayerController2D の public getter を設けない設計なので、Transformから推測
            // 入力ベクトルを直接は取得せず、最後のAimはMeleeWeapon側で保持
            // ここでは Rigidbody 速度がない時は最後のAimを返すのみ
            return _lastAim;
        }

        // Player側から最後の入力方向を伝えるためのフック
        public void SetAimFromInput(Vector2 dir)
        {
            if (dir.sqrMagnitude > 0.0001f)
            {
                _lastAim = dir.normalized;
            }
        }

        // Debug gizmos
        private Vector2 _lastSwingPos;
        private Vector2 _lastSwingForward;
        private float _lastSwingTime;

        private void OnDrawGizmosSelected()
        {
            if (Time.time - _lastSwingTime > 0.5f) return;
            Gizmos.color = new Color(1f, 1f, 0.2f, 0.25f);
            Gizmos.DrawWireSphere(_lastSwingPos, range);
        }
    }
}


