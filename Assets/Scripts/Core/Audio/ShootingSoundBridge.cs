using UnityEngine;

namespace Core.Audio
{
    /// <summary>
    /// Space ShooterのPlayerShootingスクリプトと音声を繋ぐブリッジ
    /// 射撃時に音を再生
    /// </summary>
    [RequireComponent(typeof(PlayerShooting))]
    public class ShootingSoundBridge : MonoBehaviour
    {
        [Header("Sound Effects")]
        [SerializeField] private AudioClip _shootSound;
        [SerializeField] private AudioSource _audioSource;
        
        private PlayerShooting _playerShooting;
        private float _lastShotTime = -1f;
        
        private void Awake()
        {
            _playerShooting = GetComponent<PlayerShooting>();
            
            // AudioSourceを取得または作成
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
        }
        
        private void Update()
        {
            // PlayerShootingのnextFireが更新されたら（射撃した）音を再生
            if (_playerShooting != null && _playerShooting.nextFire > _lastShotTime && _shootSound != null)
            {
                _audioSource.PlayOneShot(_shootSound);
                _lastShotTime = _playerShooting.nextFire;
            }
        }
    }
}

