using UnityEngine;

namespace Game.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public sealed class EnemyController2D : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private Transform target = null!;
        [SerializeField] private Rigidbody2D body = null!;

        public void SetTarget(Transform t) => target = t;

        private void Reset()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void Awake()
        {
            if (body == null) body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (target == null) return;
            var dir = ((Vector2)(target.position - transform.position)).normalized;
            body.linearVelocity = dir * moveSpeed;
        }
    }
}


