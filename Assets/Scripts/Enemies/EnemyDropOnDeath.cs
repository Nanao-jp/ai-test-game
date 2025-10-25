using UnityEngine;
using Game.Combat;
using Game.Systems.Experience;

namespace Game.Enemies
{
    [RequireComponent(typeof(Health))]
    [DisallowMultipleComponent]
    public sealed class EnemyDropOnDeath : MonoBehaviour
    {
        [SerializeField] private int _expAmount = 1;

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
            SpawnPickup(transform.position, _expAmount);
        }

        private static void SpawnPickup(Vector3 pos, int amount)
        {
            var go = new GameObject("ExpPickup");
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 12;
            sr.sprite = GetWhiteSprite();
            sr.color = new Color(0.3f, 0.9f, 1f, 0.95f);
            var pickup = go.AddComponent<ExpPickup>();
            // amount is serialized in component created later if needed; default is 1
            go.layer = 0; // Default
        }

        private static Sprite s_White;
        private static Sprite GetWhiteSprite()
        {
            if (s_White != null) return s_White;
            const int size = 8;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            s_White = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            return s_White;
        }
    }
}


