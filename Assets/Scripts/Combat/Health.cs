using System;
using UnityEngine;
using Game.Systems;

namespace Game.Combat
{
    [DisallowMultipleComponent]
    public sealed class Health : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth = 0f;
        [SerializeField] private bool _destroyOnDeath = true;

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsDead => _currentHealth <= 0f;

        public event Action OnDied;

        private bool _deathInvoked;

        private void Awake()
        {
            if (_currentHealth <= 0f) _currentHealth = _maxHealth;
        }

        public void SetMax(float max, bool fillCurrent = true)
        {
            _maxHealth = Mathf.Max(1f, max);
            if (fillCurrent) _currentHealth = _maxHealth;
        }

        public void Heal(float amount)
        {
            if (amount <= 0f || IsDead) return;
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;
            if (amount <= 0f) return;

            _currentHealth -= amount;
            if (_currentHealth <= 0f)
            {
                _currentHealth = 0f;
                if (_deathInvoked) return;
                _deathInvoked = true;
                OnDied?.Invoke();

                // Broadcast simple events for now
                if (gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    GameEvents.RaisePlayerDied();
                }
                else if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    GameEvents.RaiseEnemyDied();
                }

                if (_destroyOnDeath)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}


