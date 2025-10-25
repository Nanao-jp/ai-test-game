using UnityEngine;

namespace Game.Combat
{
    [DisallowMultipleComponent]
    public sealed class DamageSource : MonoBehaviour
    {
        [SerializeField] private float _amount = 10f;
        [SerializeField] private float _cooldownSeconds = 0.5f;
        [SerializeField] private LayerMask _damageLayers;
        [SerializeField] private bool _destroyOnHit = false;

        private float _lastAppliedTime = -999f;

        public float Amount
        {
            get => _amount;
            set => _amount = Mathf.Max(0f, value);
        }

        public float CooldownSeconds
        {
            get => _cooldownSeconds;
            set => _cooldownSeconds = Mathf.Max(0f, value);
        }

        public LayerMask DamageLayers
        {
            get => _damageLayers;
            set => _damageLayers = value;
        }

        public bool DestroyOnHit
        {
            get => _destroyOnHit;
            set => _destroyOnHit = value;
        }

        public bool CanAffectLayer(int layer)
        {
            return (_damageLayers.value & (1 << layer)) != 0;
        }

        public bool TryApplyTo(Health health)
        {
            if (health == null) return false;
            if (!IsCooldownReady()) return false;

            health.TakeDamage(_amount);
            _lastAppliedTime = Time.time;
            if (_destroyOnHit)
            {
                Destroy(gameObject);
            }
            return true;
        }

        private bool IsCooldownReady()
        {
            return Time.time - _lastAppliedTime >= _cooldownSeconds;
        }
    }
}


