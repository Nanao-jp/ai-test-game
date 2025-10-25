using UnityEngine;
using Game.Systems.Experience;

namespace Game.Player
{
    [DisallowMultipleComponent]
    public sealed class Magnet : MonoBehaviour
    {
        [SerializeField] private float _radius = 2.5f;
        [SerializeField] private float _pullSpeed = 7.5f;

        private void Update()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, _radius);
            foreach (var c in hits)
            {
                if (c == null) continue;
                var pickup = c.GetComponent<ExpPickup>();
                if (pickup == null) continue;
                pickup.SetMagnetTarget(transform, _pullSpeed);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 1f, 0.25f);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}


