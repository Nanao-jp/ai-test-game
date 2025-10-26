using UnityEngine;
using System;

namespace Game.Character
{
    /// <summary>
    /// 実行時のキャラクターインスタンス
    /// セーブ/ロード対象
    /// </summary>
    [System.Serializable]
    public class CharacterInstance
    {
        public CharacterData data;
        
        [Header("育成状態")]
        public int currentLevel = 1;
        public int currentExp = 0;
        public int affection = 0; // 好感度（0-100）
        
        [Header("装備")]
        public string equippedWeaponId;
        public string equippedArmorId;
        public string equippedAccessoryId;
        
        // イベント
        public event Action<int> OnLevelUp;
        public event Action<int> OnExpGained;
        public event Action<int> OnAffectionChanged;
        
        /// <summary>
        /// 次のレベルまでに必要な経験値
        /// </summary>
        public int GetExpToNextLevel()
        {
            // 指数的成長: 100 * 1.5^(level-1)
            return Mathf.FloorToInt(100 * Mathf.Pow(1.5f, currentLevel - 1));
        }
        
        /// <summary>
        /// 経験値を追加（レベルアップ判定含む）
        /// </summary>
        public void AddExp(int amount)
        {
            if (currentLevel >= 100) return; // レベル上限
            
            currentExp += amount;
            OnExpGained?.Invoke(amount);
            
            // レベルアップ判定
            while (currentExp >= GetExpToNextLevel() && currentLevel < 100)
            {
                currentExp -= GetExpToNextLevel();
                currentLevel++;
                OnLevelUp?.Invoke(currentLevel);
                
                Debug.Log($"{data.characterName} がレベル {currentLevel} にアップ！");
            }
        }
        
        /// <summary>
        /// 好感度を追加
        /// </summary>
        public void AddAffection(int amount)
        {
            affection = Mathf.Clamp(affection + amount, 0, 100);
            OnAffectionChanged?.Invoke(affection);
        }
        
        // === 最終ステータス計算（装備込み）===
        
        /// <summary>
        /// 最終HP（レベル + 装備ボーナス）
        /// </summary>
        public float GetTotalHP()
        {
            float hp = data.GetHP(currentLevel);
            
            // TODO: 装備ボーナスを追加
            // if (equippedWeapon != null) hp += equippedWeapon.hpBonus;
            
            return hp;
        }
        
        /// <summary>
        /// 最終攻撃力（レベル + 装備ボーナス）
        /// </summary>
        public float GetTotalAttack()
        {
            float attack = data.GetAttack(currentLevel);
            
            // TODO: 装備ボーナスを追加
            
            return attack;
        }
        
        /// <summary>
        /// 最終防御力（レベル + 装備ボーナス）
        /// </summary>
        public float GetTotalDefense()
        {
            float defense = data.GetDefense(currentLevel);
            
            // TODO: 装備ボーナスを追加
            
            return defense;
        }
        
        /// <summary>
        /// 最終移動速度
        /// </summary>
        public float GetTotalMoveSpeed()
        {
            return data.GetMoveSpeed(currentLevel);
        }
        
        /// <summary>
        /// 最終射撃速度
        /// </summary>
        public float GetTotalFireRate()
        {
            return data.GetFireRate(currentLevel);
        }
    }
}

