using UnityEngine;

namespace Game.Player
{
    [DisallowMultipleComponent]
    public sealed class BulletLifetime : MonoBehaviour
    {
        [SerializeField] private float lifetimeSeconds = 3f;
        private float timer;

        private void OnEnable()
        {
            timer = lifetimeSeconds;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}


