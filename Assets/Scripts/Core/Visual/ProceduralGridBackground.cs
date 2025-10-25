using UnityEngine;
using Game.Art;

namespace Core.Visual
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ProceduralGridBackground : MonoBehaviour
    {
        [Header("World Size (units)")]
        [SerializeField] private Vector2 worldSize = new Vector2(10f, 30f); // 縦長想定の初期値

        [Header("Grid")]
        [SerializeField] private int textureSize = 128;              // generated texture px
        [SerializeField] private int cellPixels = 16;                // checker size px
        [SerializeField] private Color colorA = new Color(0.13f, 0.16f, 0.20f, 1f);
        [SerializeField] private Color colorB = new Color(0.11f, 0.13f, 0.17f, 1f);
        [SerializeField] private int sortingOrder = -100;

        private SpriteRenderer spriteRenderer = null!;
        [SerializeField] private bool useSpriteSetBackground = true;
        [Header("UV Scroll")]
        [SerializeField] private bool enableUvScroll = false;
        [SerializeField] private float uvScrollSpeedY = 0f; // 単位: テクスチャ座標/秒（+は上方向に流れにくい見た目、-で下方向）
        private Material _mat;

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Generate();
        }

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null) Generate();
            EnsureMaterial();
        }

        public void Generate()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (useSpriteSetBackground)
            {
                var set = SpriteProvider.GetSet();
                if (set != null && set.backgroundSprite != null)
                {
                    spriteRenderer.sprite = set.backgroundSprite;
                    spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                    spriteRenderer.size = worldSize;
                    spriteRenderer.sortingOrder = sortingOrder;
                    EnsureMaterial();
                    return;
                }
            }
            var tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat
            };

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    bool a = ((x / cellPixels) + (y / cellPixels)) % 2 == 0;
                    tex.SetPixel(x, y, a ? colorA : colorB);
                }
            }
            tex.Apply();

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, textureSize, textureSize),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit: 100f,
                extrude: 0,
                meshType: SpriteMeshType.FullRect
            );

            spriteRenderer.sprite = sprite;
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = worldSize;
            spriteRenderer.sortingOrder = sortingOrder;
            EnsureMaterial();
        }

        public void SetWorldSize(Vector2 size)
        {
            worldSize = size;
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.size = worldSize;
        }

        private void EnsureMaterial()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;
            if (_mat == null)
            {
                _mat = spriteRenderer.sharedMaterial;
                if (_mat == null)
                {
                    _mat = new Material(Shader.Find("Sprites/Default"));
                    spriteRenderer.sharedMaterial = _mat;
                }
            }
        }

        private void Update()
        {
            if (!enableUvScroll || Mathf.Approximately(uvScrollSpeedY, 0f)) return;
            if (_mat == null) EnsureMaterial();
            if (_mat == null) return;
            var off = _mat.mainTextureOffset;
            off.y += uvScrollSpeedY * Time.deltaTime;
            _mat.mainTextureOffset = off;
        }
    }
}


