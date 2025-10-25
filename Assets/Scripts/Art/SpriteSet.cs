using UnityEngine;

namespace Game.Art
{
    [CreateAssetMenu(fileName = "SpriteSet", menuName = "Game/Sprite Set", order = 0)]
    public sealed class SpriteSet : ScriptableObject
    {
        public Sprite playerSprite;
        public Sprite enemySprite;
        public Sprite enemyAltSprite; // プレイヤーと被る場合の代替
        public Sprite bulletSprite;
        public Sprite expSprite;
        public Sprite backgroundSprite;

        [Header("Optional Prefabs (優先適用)")]
        public GameObject playerPrefab;
        public GameObject enemyPrefab;

        [Header("VFX")]
        public Sprite slashSprite;
        public GameObject slashPrefab;
    }
}


