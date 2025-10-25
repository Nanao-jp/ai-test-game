using UnityEngine;

namespace Game.Core
{
    public static class LayerUtil
    {
        public static int Resolve(string layerName, int fallbackLayer = 0)
        {
            var l = LayerMask.NameToLayer(layerName);
            return l < 0 ? fallbackLayer : l;
        }

        public static int MaskFor(string layerName, int fallbackLayer = 0)
        {
            return 1 << Resolve(layerName, fallbackLayer);
        }
    }
}


