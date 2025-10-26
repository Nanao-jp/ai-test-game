using UnityEngine;

namespace Game.Character
{
    /// <summary>
    /// ゲーム全体の状態管理（シングルトン）
    /// 現在のキャラクター、進行状態などを保持
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("現在のキャラクター")]
        public CharacterInstance currentCharacter;
        
        [Header("ゲーム状態")]
        public int playerCurrency = 1000; // 所持金
        public int playerGems = 100;      // ジェム（課金通貨）
        
        void Awake()
        {
            // シングルトン
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Initialize()
        {
            // テスト用: デフォルトキャラクターを設定
            if (currentCharacter == null)
            {
                currentCharacter = new CharacterInstance();
                // TODO: デフォルトのCharacterDataを読み込む
            }
            
            // レベルアップイベントをリスン
            currentCharacter.OnLevelUp += OnCharacterLevelUp;
            currentCharacter.OnExpGained += OnCharacterExpGained;
        }
        
        void OnCharacterLevelUp(int newLevel)
        {
            Debug.Log($"レベルアップ！ 新しいレベル: {newLevel}");
            // TODO: レベルアップ演出
            // TODO: UI更新
        }
        
        void OnCharacterExpGained(int amount)
        {
            Debug.Log($"経験値 +{amount}");
            // TODO: UI更新
        }
        
        /// <summary>
        /// 戦闘開始（育成データを戦闘シーンに渡す）
        /// </summary>
        public void StartBattle()
        {
            var battleData = new BattleInputData
            {
                hp = currentCharacter.GetTotalHP(),
                attack = currentCharacter.GetTotalAttack(),
                defense = currentCharacter.GetTotalDefense(),
                moveSpeed = currentCharacter.GetTotalMoveSpeed(),
                fireRate = currentCharacter.GetTotalFireRate()
            };
            
            // TODO: 戦闘シーンをロード
            // SceneManager.LoadScene("Battle");
            // BattleManager.Initialize(battleData);
            
            Debug.Log($"戦闘開始: HP={battleData.hp}, ATK={battleData.attack}");
        }
        
        /// <summary>
        /// 戦闘終了時のコールバック
        /// </summary>
        public void OnBattleComplete(BattleResult result)
        {
            Debug.Log($"戦闘終了: 勝利={result.isVictory}, 獲得経験値={result.expGained}");
            
            if (result.isVictory)
            {
                // 経験値付与
                currentCharacter.AddExp(result.expGained);
                
                // 報酬付与
                playerCurrency += result.currencyGained;
                
                // TODO: リザルト画面を表示
            }
            
            // TODO: セーブ
        }
    }
    
    /// <summary>
    /// 戦闘への入力データ（契約）
    /// </summary>
    [System.Serializable]
    public struct BattleInputData
    {
        public float hp;
        public float attack;
        public float defense;
        public float moveSpeed;
        public float fireRate;
    }
    
    /// <summary>
    /// 戦闘結果（契約）
    /// </summary>
    [System.Serializable]
    public struct BattleResult
    {
        public bool isVictory;
        public int expGained;
        public int currencyGained;
        public int score;
    }
}

