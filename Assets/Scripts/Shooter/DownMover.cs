using UnityEngine;

namespace Game.Shooter
{
    [DisallowMultipleComponent]
    public sealed class DownMover : MonoBehaviour
    {
        [SerializeField] private float speed = 2.0f; // units/s downward

        private void Update()
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

        public void SetSpeed(float s)
        {
            speed = s;
        }
    }
}


