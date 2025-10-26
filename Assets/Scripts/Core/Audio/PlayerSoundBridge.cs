using UnityEngine;

namespace Core.Audio
{
    /// <summary>
    /// Space ShooterのPlayerスクリプトと音声を繋ぐブリッジ
    /// プリセットのスクリプトは変更せず、音だけを追加
    /// </summary>
    [RequireComponent(typeof(Player))]
    public class PlayerSoundBridge : MonoBehaviour
    {
        [Header("Sound Effects")]
        [SerializeField] private AudioClip _deathSound;
        
        private Player _player;
        private bool _hasPlayedDeathSound = false;
        
        private void Awake()
        {
            _player = GetComponent<Player>();
        }
        
        private void OnDestroy()
        {
            // プレイヤー死亡時に音を再生（アプリケーション終了時は除く）
            if (!_hasPlayedDeathSound && _deathSound != null && Application.isPlaying && !AudioCleanupHelper.IsQuitting)
            {
                AudioSource.PlayClipAtPoint(_deathSound, transform.position);
                _hasPlayedDeathSound = true;
            }
        }
    }
}

