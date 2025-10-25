using UnityEngine;
using Game.Art;

namespace Game.Combat
{
    public static class SlashVfx
    {
        public static void Spawn(Vector2 center, Vector2 forward, float length, float thickness, Color color, float duration, int sortingOrder)
        {
            var set = SpriteProvider.GetSet();
            GameObject go;
            SpriteRenderer sr;
            if (set != null && set.slashPrefab != null)
            {
                go = Object.Instantiate(set.slashPrefab);
                sr = go.GetComponentInChildren<SpriteRenderer>();
                if (sr == null) sr = go.AddComponent<SpriteRenderer>();
                sr.sortingOrder = sortingOrder;
            }
            else
            {
                go = new GameObject("SlashVfx");
                sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = set != null && set.slashSprite != null ? set.slashSprite : GetWhite();
                sr.sortingOrder = sortingOrder;
            }
            sr.color = color;
            go.transform.position = center;

            // Rotate to face forward
            var angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Scale to desired size
            var baseW = sr.sprite.bounds.size.x;
            var baseH = sr.sprite.bounds.size.y;
            if (baseW <= 0f || baseH <= 0f)
            {
                baseW = 1f; baseH = 1f;
            }
            go.transform.localScale = new Vector3(length / baseW, thickness / baseH, 1f);

            Object.Destroy(go, duration);
        }

        private static Sprite s_White;
        private static Sprite GetWhite()
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


