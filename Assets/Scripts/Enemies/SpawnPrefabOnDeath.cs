using UnityEngine;
using Game.Combat;

namespace Game.Enemies
{
    [RequireComponent(typeof(Health))]
    [DisallowMultipleComponent]
    public sealed class SpawnPrefabOnDeath : MonoBehaviour
    {
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private Vector3 offset = Vector3.zero;

        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.OnDied += HandleDied;
        }

        private void OnDestroy()
        {
            if (_health != null) _health.OnDied -= HandleDied;
        }

        private void HandleDied()
        {
            if (prefab == null) return;
            Instantiate(prefab, transform.position + offset, Quaternion.identity);
        }

        public void SetPrefab(GameObject go) => prefab = go;
    }
}


