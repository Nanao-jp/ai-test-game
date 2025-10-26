using UnityEngine;

namespace Game.Character
{
    /// <summary>
    /// キャラクターの基本データ（ScriptableObject）
    /// レベルアップ時の成長率などを定義
    /// </summary>
    [CreateAssetMenu(fileName = "Character", menuName = "Game/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("基本情報")]
        public string characterName = "キャラクター名";
        public Rarity rarity = Rarity.N;
        
        [Header("ビジュアル")]
        public Sprite portrait;      // 立ち絵
        public Sprite cardArt;       // カード絵（ガチャ用）
        
        [Header("基礎ステータス（レベル1時点）")]
        public float baseHP = 100f;
        public float baseAttack = 10f;
        public float baseDefense = 5f;
        public float baseMoveSpeed = 5f;
        public float baseFireRate = 1f;
        
        [Header("成長率（レベル1→100への倍率）")]
        [Tooltip("横軸: レベル1(0.0)→100(1.0), 縦軸: 倍率")]
        public AnimationCurve hpGrowth = AnimationCurve.Linear(0, 1, 1, 3);
        public AnimationCurve attackGrowth = AnimationCurve.Linear(0, 1, 1, 3);
        public AnimationCurve defenseGrowth = AnimationCurve.Linear(0, 1, 1, 2);
        
        [Header("ボイス")]
        public AudioClip voiceHome;
        public AudioClip voiceBattleStart;
        public AudioClip voiceBattleWin;
        
        /// <summary>
        /// 指定レベルでのHP
        /// </summary>
        public float GetHP(int level)
        {
            float t = Mathf.Clamp01((level - 1) / 99f); // 1→0.0, 100→1.0
            return baseHP * hpGrowth.Evaluate(t);
        }
        
        /// <summary>
        /// 指定レベルでの攻撃力
        /// </summary>
        public float GetAttack(int level)
        {
            float t = Mathf.Clamp01((level - 1) / 99f);
            return baseAttack * attackGrowth.Evaluate(t);
        }
        
        /// <summary>
        /// 指定レベルでの防御力
        /// </summary>
        public float GetDefense(int level)
        {
            float t = Mathf.Clamp01((level - 1) / 99f);
            return baseDefense * defenseGrowth.Evaluate(t);
        }
        
        /// <summary>
        /// 指定レベルでの移動速度
        /// </summary>
        public float GetMoveSpeed(int level)
        {
            // 移動速度は固定（レベルで変わらない）
            return baseMoveSpeed;
        }
        
        /// <summary>
        /// 指定レベルでの射撃速度
        /// </summary>
        public float GetFireRate(int level)
        {
            // 射撃速度は固定
            return baseFireRate;
        }
    }
    
    /// <summary>
    /// レアリティ
    /// </summary>
    public enum Rarity
    {
        N,      // ノーマル
        R,      // レア
        SR,     // スーパーレア
        SSR     // スーパースーパーレア
    }
}

