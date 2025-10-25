using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody2D body = null!;

        private Vector2 inputMove;

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

        public void OnMove(InputValue value)
        {
            inputMove = value.Get<Vector2>();
        }

        private void FixedUpdate()
        {
            var velocity = inputMove.normalized * moveSpeed;
            body.linearVelocity = velocity;
        }
    }
}


