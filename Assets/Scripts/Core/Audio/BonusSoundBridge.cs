using UnityEngine;

namespace Core.Audio
{
    /// <summary>
    /// Space ShooterのBonusスクリプトと音声を繋ぐブリッジ
    /// パワーアップ取得時に音を再生
    /// </summary>
    [RequireComponent(typeof(Bonus))]
    public class BonusSoundBridge : MonoBehaviour
    {
        [Header("Sound Effects")]
        [SerializeField] private AudioClip _pickupSound;
        
        private bool _hasPlayedPickupSound = false;
        
        private void OnDestroy()
        {
            // パワーアップ取得時に音を再生（アプリケーション終了時は除く）
            if (!_hasPlayedPickupSound && _pickupSound != null && Application.isPlaying && !AudioCleanupHelper.IsQuitting)
            {
                AudioSource.PlayClipAtPoint(_pickupSound, transform.position);
                _hasPlayedPickupSound = true;
            }
        }
    }
}

