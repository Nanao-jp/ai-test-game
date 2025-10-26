using UnityEngine;

namespace Core.Audio
{
    /// <summary>
    /// シーン終了時に音が再生されないようにする補助クラス
    /// </summary>
    public static class AudioCleanupHelper
    {
        private static bool _isQuitting = false;
        
        public static bool IsQuitting => _isQuitting;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            _isQuitting = false;
            Application.quitting += OnApplicationQuit;
        }
        
        private static void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}

