using UnityEngine;

namespace Core.Audio
{
    /// <summary>
    /// Space ShooterのEnemyスクリプトと音声を繋ぐブリッジ
    /// 敵死亡時に音を再生
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    public class EnemySoundBridge : MonoBehaviour
    {
        [Header("Sound Effects")]
        [SerializeField] private AudioClip _deathSound;
        
        private bool _hasPlayedDeathSound = false;
        
        private void OnDestroy()
        {
            // 敵死亡時に音を再生（アプリケーション終了時は除く）
            if (!_hasPlayedDeathSound && _deathSound != null && Application.isPlaying && !AudioCleanupHelper.IsQuitting)
            {
                AudioSource.PlayClipAtPoint(_deathSound, transform.position);
                _hasPlayedDeathSound = true;
            }
        }
    }
}

