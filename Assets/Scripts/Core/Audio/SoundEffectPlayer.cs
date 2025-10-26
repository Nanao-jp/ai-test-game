using UnityEngine;

namespace Core.Audio
{
    /// <summary>
    /// 汎用的なサウンド再生スクリプト
    /// Space Shooterのプレハブに追加して使用
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private bool _playOnStart = false;
        [SerializeField] private bool _playOnDestroy = false;
        
        private AudioSource _audioSource;
        private static GameObject _audioPlayerPrefab;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource != null && _clip != null)
            {
                _audioSource.clip = _clip;
                _audioSource.playOnAwake = false;
            }
        }
        
        private void Start()
        {
            if (_playOnStart && Application.isPlaying && !AudioCleanupHelper.IsQuitting)
            {
                // カメラの視界内でのみ音を再生
                if (IsVisibleFromCamera())
                {
                    Play();
                }
            }
        }
        
        /// <summary>
        /// カメラの視界内にいるかチェック
        /// </summary>
        private bool IsVisibleFromCamera()
        {
            if (Camera.main == null) return true;
            
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
            
            // 視界内 + 少し余裕を持たせる（1.5倍）
            return viewportPoint.x >= -0.5f && viewportPoint.x <= 1.5f &&
                   viewportPoint.y >= -0.5f && viewportPoint.y <= 1.5f &&
                   viewportPoint.z > 0;
        }
        
        public void Play()
        {
            if (_clip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_clip);
            }
        }
        
        /// <summary>
        /// オブジェクト破壊時に音を再生（破壊前に別のGameObjectで再生）
        /// </summary>
        public void PlayOnDestruction()
        {
            if (_clip != null && Application.isPlaying && !AudioCleanupHelper.IsQuitting)
            {
                // 破壊後も音が鳴るように、一時的なGameObjectで再生
                AudioSource.PlayClipAtPoint(_clip, transform.position);
            }
        }
        
        private void OnDestroy()
        {
            if (_playOnDestroy && Application.isPlaying && !AudioCleanupHelper.IsQuitting)
            {
                PlayOnDestruction();
            }
        }
    }
}

