using UnityEngine;

namespace Game.Core.Debugging
{
    public static class DebugFlags
    {
        private const string KeyVerbose = "Game.Debug.Verbose";
        private const string KeyOverlay = "Game.Debug.Overlay";

        public static bool Verbose
        {
            get => PlayerPrefs.GetInt(KeyVerbose, 0) == 1;
            set { PlayerPrefs.SetInt(KeyVerbose, value ? 1 : 0); PlayerPrefs.Save(); }
        }

        public static bool Overlay
        {
            get => PlayerPrefs.GetInt(KeyOverlay, 0) == 1;
            set { PlayerPrefs.SetInt(KeyOverlay, value ? 1 : 0); PlayerPrefs.Save(); }
        }
    }
}


