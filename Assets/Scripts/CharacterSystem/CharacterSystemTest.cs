using UnityEngine;

namespace Game.Character
{
    /// <summary>
    /// 育成システムのテスト用スクリプト
    /// キー入力でレベルアップ、経験値追加、セーブ/ロードをテスト
    /// </summary>
    public class CharacterSystemTest : MonoBehaviour
    {
        [Header("テスト用キャラクターデータ")]
        [Tooltip("Assets/Create/Game/Character Data で作成したキャラクターを設定")]
        public CharacterData testCharacter;
        
        void Start()
        {
            // GameManagerの初期化確認
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManagerがシーンに配置されていません！");
                return;
            }
            
            // テストキャラクターを設定
            if (testCharacter != null)
            {
                GameManager.Instance.currentCharacter = new CharacterInstance
                {
                    data = testCharacter,
                    currentLevel = 1,
                    currentExp = 0
                };
                
                // イベント登録
                GameManager.Instance.currentCharacter.OnLevelUp += OnLevelUp;
                GameManager.Instance.currentCharacter.OnExpGained += OnExpGained;
                
                Debug.Log("=== 育成システムテスト開始 ===");
                Debug.Log($"キャラクター: {testCharacter.characterName}");
                PrintStatus();
                Debug.Log("\n操作方法:");
                Debug.Log("  [E] 経験値 +50");
                Debug.Log("  [L] レベルアップ（強制）");
                Debug.Log("  [S] セーブ");
                Debug.Log("  [O] ロード");
                Debug.Log("  [I] ステータス表示");
                Debug.Log("  [B] 戦闘開始シミュレーション");
            }
            else
            {
                Debug.LogError("testCharacterが設定されていません！Inspectorで設定してください。");
            }
        }
        
        void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.currentCharacter == null)
                return;
            
            var character = GameManager.Instance.currentCharacter;
            
            // [E] 経験値を追加
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("\n--- 経験値追加 ---");
                character.AddExp(50);
                PrintStatus();
            }
            
            // [L] 強制レベルアップ
            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("\n--- 強制レベルアップ ---");
                character.AddExp(character.GetExpToNextLevel());
                PrintStatus();
            }
            
            // [S] セーブ
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("\n--- セーブ ---");
                SaveManager.Save();
                Debug.Log("セーブ完了");
            }
            
            // [O] ロード
            if (Input.GetKeyDown(KeyCode.O))
            {
                Debug.Log("\n--- ロード ---");
                SaveManager.Load();
                
                // イベント再登録
                if (GameManager.Instance.currentCharacter != null)
                {
                    GameManager.Instance.currentCharacter.OnLevelUp += OnLevelUp;
                    GameManager.Instance.currentCharacter.OnExpGained += OnExpGained;
                    PrintStatus();
                }
            }
            
            // [I] ステータス表示
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("\n--- 現在のステータス ---");
                PrintStatus();
            }
            
            // [B] 戦闘開始シミュレーション
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("\n--- 戦闘開始シミュレーション ---");
                GameManager.Instance.StartBattle();
                
                // 戦闘結果をシミュレート（3秒後）
                Invoke(nameof(SimulateBattleEnd), 3f);
            }
        }
        
        void SimulateBattleEnd()
        {
            Debug.Log("\n--- 戦闘終了（シミュレーション）---");
            
            var result = new BattleResult
            {
                isVictory = true,
                expGained = 100,
                currencyGained = 500,
                score = 10000
            };
            
            GameManager.Instance.OnBattleComplete(result);
            PrintStatus();
        }
        
        void OnLevelUp(int newLevel)
        {
            Debug.Log($"🎉 レベルアップ！ Lv.{newLevel}");
        }
        
        void OnExpGained(int amount)
        {
            Debug.Log($"💫 経験値 +{amount}");
        }
        
        void PrintStatus()
        {
            var character = GameManager.Instance.currentCharacter;
            
            Debug.Log($"キャラクター名: {character.data.characterName}");
            Debug.Log($"レアリティ: {character.data.rarity}");
            Debug.Log($"現在レベル: {character.currentLevel}");
            Debug.Log($"現在経験値: {character.currentExp} / {character.GetExpToNextLevel()}");
            Debug.Log($"HP: {character.GetTotalHP():F1}");
            Debug.Log($"攻撃力: {character.GetTotalAttack():F1}");
            Debug.Log($"防御力: {character.GetTotalDefense():F1}");
            Debug.Log($"移動速度: {character.GetTotalMoveSpeed():F1}");
            Debug.Log($"射撃速度: {character.GetTotalFireRate():F1}");
            Debug.Log($"所持金: {GameManager.Instance.playerCurrency}");
        }
    }
}

