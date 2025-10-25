using UnityEngine;

namespace Game.Art
{
    public static class SpriteProvider
    {
        private static SpriteSet _cached;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void ResetCacheOnLoad()
        {
            _cached = null;
        }

        public static SpriteSet GetSet()
        {
            if (_cached != null && HasAnySprite(_cached)) return _cached;
            var set = Resources.Load<SpriteSet>("Art/SpriteSet_TinySwords");
            if (set == null || !HasAnySprite(set))
            {
                var all = Resources.LoadAll<SpriteSet>(string.Empty);
                set = all != null && all.Length > 0 ? all[0] : null;
            }
            _cached = set;
            return _cached;
        }

        private static bool HasAnySprite(SpriteSet set)
        {
            if (set == null) return false;
            return set.playerSprite != null || set.enemySprite != null || set.enemyAltSprite != null || set.bulletSprite != null || set.expSprite != null || set.backgroundSprite != null;
        }
    }
}


