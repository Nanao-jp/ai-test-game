using UnityEngine;

namespace Game.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class Damageable : MonoBehaviour
    {
        [SerializeField] private Health _health = null!;

        private void Reset()
        {
            _health = GetComponent<Health>();
        }

        private void Awake()
        {
            if (_health == null) _health = GetComponent<Health>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Handle(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            Handle(other);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Handle(other.collider);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            Handle(other.collider);
        }

        private void Handle(Component other)
        {
            var src = other.GetComponent<DamageSource>();
            if (src == null) return;
            if (!src.CanAffectLayer(gameObject.layer)) return;
            src.TryApplyTo(_health);
        }
    }
}


