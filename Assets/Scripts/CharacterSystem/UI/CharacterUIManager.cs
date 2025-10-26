using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Game.Character.UI
{
    /// <summary>
    /// キャラクター育成画面のUI管理
    /// レベル、経験値、ステータスを表示
    /// タッチ/スワイプ操作対応
    /// </summary>
    public class CharacterUIManager : MonoBehaviour
    {
        [Header("UI要素")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI expText;
        [SerializeField] private Slider expSlider;
        
        [Header("ステータス表示")]
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private TextMeshProUGUI defenseText;
        [SerializeField] private TextMeshProUGUI speedText;
        
        [Header("ボタン")]
        [SerializeField] private Button levelUpButton;
        [SerializeField] private Button trainingButton;
        [SerializeField] private Button equipmentButton;
        
        [Header("演出用パネル")]
        [SerializeField] private GameObject levelUpEffectPanel;
        [SerializeField] private TextMeshProUGUI levelUpEffectText;
        
        [Header("キャラクタービジュアル")]
        [SerializeField] private Image characterPortrait;
        [SerializeField] private Image rarityIcon;
        
        private CharacterInstance currentCharacter;
        
        void Start()
        {
            // ボタンイベント登録
            if (levelUpButton != null)
                levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
            
            if (trainingButton != null)
                trainingButton.onClick.AddListener(OnTrainingButtonClicked);
            
            if (equipmentButton != null)
                equipmentButton.onClick.AddListener(OnEquipmentButtonClicked);
            
            // レベルアップエフェクトは最初は非表示
            if (levelUpEffectPanel != null)
                levelUpEffectPanel.SetActive(false);
            
            // GameManagerからキャラクターを取得
            if (GameManager.Instance != null && GameManager.Instance.currentCharacter != null)
            {
                SetCharacter(GameManager.Instance.currentCharacter);
            }
        }
        
        /// <summary>
        /// 表示するキャラクターを設定
        /// </summary>
        public void SetCharacter(CharacterInstance character)
        {
            currentCharacter = character;
            
            // イベント登録
            currentCharacter.OnLevelUp += OnCharacterLevelUp;
            currentCharacter.OnExpGained += OnCharacterExpGained;
            
            // キャラクタービジュアル
            if (characterPortrait != null && currentCharacter.data.portrait != null)
                characterPortrait.sprite = currentCharacter.data.portrait;
            
            // 初期表示
            UpdateUI();
        }
        
        /// <summary>
        /// UI全体を更新
        /// </summary>
        void UpdateUI()
        {
            if (currentCharacter == null) return;
            
            // 基本情報
            if (characterNameText != null)
                characterNameText.text = currentCharacter.data.characterName;
            
            if (levelText != null)
                levelText.text = $"Lv.{currentCharacter.currentLevel}";
            
            // 経験値
            int currentExp = currentCharacter.currentExp;
            int expToNext = currentCharacter.GetExpToNextLevel();
            
            if (expText != null)
                expText.text = $"{currentExp} / {expToNext}";
            
            if (expSlider != null)
            {
                expSlider.maxValue = expToNext;
                expSlider.value = currentExp;
            }
            
            // ステータス
            if (hpText != null)
                hpText.text = $"HP: {currentCharacter.GetTotalHP():F0}";
            
            if (attackText != null)
                attackText.text = $"ATK: {currentCharacter.GetTotalAttack():F0}";
            
            if (defenseText != null)
                defenseText.text = $"DEF: {currentCharacter.GetTotalDefense():F0}";
            
            if (speedText != null)
                speedText.text = $"SPD: {currentCharacter.GetTotalMoveSpeed():F1}";
            
            // レベルアップボタンの有効/無効
            if (levelUpButton != null)
            {
                bool canLevelUp = currentCharacter.currentExp >= currentCharacter.GetExpToNextLevel() 
                                  && currentCharacter.currentLevel < 100;
                levelUpButton.interactable = canLevelUp;
            }
        }
        
        /// <summary>
        /// 経験値スライダーをアニメーション
        /// </summary>
        void AnimateExpBar(int previousExp, int newExp)
        {
            if (expSlider == null) return;
            
            // シンプルアニメーション（DOTweenなし版）
            StartCoroutine(AnimateExpBarCoroutine(previousExp, newExp));
        }
        
        IEnumerator AnimateExpBarCoroutine(int from, int to)
        {
            float duration = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Ease Out Cubic
                t = 1f - Mathf.Pow(1f - t, 3f);
                expSlider.value = Mathf.Lerp(from, to, t);
                yield return null;
            }
            
            expSlider.value = to;
        }
        
        /// <summary>
        /// レベルアップエフェクトを表示
        /// </summary>
        void ShowLevelUpEffect(int newLevel)
        {
            if (levelUpEffectPanel == null) return;
            
            // パネルを表示
            levelUpEffectPanel.SetActive(true);
            
            if (levelUpEffectText != null)
                levelUpEffectText.text = $"LEVEL UP!\nLv.{newLevel}";
            
            // シンプルアニメーション（DOTweenなし版）
            StartCoroutine(LevelUpEffectCoroutine());
        }
        
        IEnumerator LevelUpEffectCoroutine()
        {
            Transform t = levelUpEffectPanel.transform;
            
            // スケールアップ
            float duration = 0.3f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(0f, 1.2f, elapsed / duration);
                t.localScale = Vector3.one * scale;
                yield return null;
            }
            
            // 少し縮む
            duration = 0.1f;
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(1.2f, 1f, elapsed / duration);
                t.localScale = Vector3.one * scale;
                yield return null;
            }
            
            // 待機
            yield return new WaitForSeconds(1.5f);
            
            // フェードアウト
            duration = 0.2f;
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(1f, 0f, elapsed / duration);
                t.localScale = Vector3.one * scale;
                yield return null;
            }
            
            levelUpEffectPanel.SetActive(false);
        }
        
        /// <summary>
        /// ステータステキストをポップアニメーション
        /// </summary>
        void PopStatText(TextMeshProUGUI text)
        {
            if (text == null) return;
            
            StartCoroutine(PopStatTextCoroutine(text));
        }
        
        IEnumerator PopStatTextCoroutine(TextMeshProUGUI text)
        {
            Transform t = text.transform;
            Color originalColor = text.color;
            Vector3 originalScale = t.localScale;
            
            // スケールアップ
            float duration = 0.15f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float scale = 1f + 0.2f * Mathf.Sin(elapsed / duration * Mathf.PI);
                t.localScale = originalScale * scale;
                text.color = Color.Lerp(originalColor, Color.yellow, Mathf.Sin(elapsed / duration * Mathf.PI));
                yield return null;
            }
            
            t.localScale = originalScale;
            text.color = originalColor;
        }
        
        // === イベントハンドラ ===
        
        void OnCharacterLevelUp(int newLevel)
        {
            UpdateUI();
            ShowLevelUpEffect(newLevel);
            
            // ステータステキストをポップ
            PopStatText(hpText);
            PopStatText(attackText);
            PopStatText(defenseText);
        }
        
        void OnCharacterExpGained(int amount)
        {
            int previousExp = currentCharacter.currentExp - amount;
            AnimateExpBar(previousExp, currentCharacter.currentExp);
            UpdateUI();
        }
        
        // === ボタンイベント ===
        
        void OnLevelUpButtonClicked()
        {
            if (currentCharacter == null) return;
            
            // ボタンをパンチアニメーション
            if (levelUpButton != null)
                StartCoroutine(ButtonPunchAnimation(levelUpButton.transform));
            
            // レベルアップ実行
            int expNeeded = currentCharacter.GetExpToNextLevel();
            if (currentCharacter.currentExp >= expNeeded && currentCharacter.currentLevel < 100)
            {
                currentCharacter.AddExp(0); // レベルアップ処理をトリガー
            }
        }
        
        void OnTrainingButtonClicked()
        {
            if (currentCharacter == null) return;
            
            // ボタンアニメーション
            if (trainingButton != null)
                StartCoroutine(ButtonPunchAnimation(trainingButton.transform));
            
            // 訓練で経験値獲得
            currentCharacter.AddExp(50);
            
            Debug.Log("訓練を実行！ 経験値 +50");
        }
        
        void OnEquipmentButtonClicked()
        {
            // ボタンアニメーション
            if (equipmentButton != null)
                StartCoroutine(ButtonPunchAnimation(equipmentButton.transform));
            
            Debug.Log("装備画面を開く（未実装）");
            // TODO: 装備画面のシーンを開く
        }
        
        IEnumerator ButtonPunchAnimation(Transform button)
        {
            Vector3 originalScale = button.localScale;
            float duration = 0.1f;
            float elapsed = 0f;
            
            // 縮む
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                button.localScale = originalScale * (1f - 0.1f * (elapsed / duration));
                yield return null;
            }
            
            // 戻る
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                button.localScale = originalScale * (0.9f + 0.1f * (elapsed / duration));
                yield return null;
            }
            
            button.localScale = originalScale;
        }
        
        void OnDestroy()
        {
            // イベント解除
            if (currentCharacter != null)
            {
                currentCharacter.OnLevelUp -= OnCharacterLevelUp;
                currentCharacter.OnExpGained -= OnCharacterExpGained;
            }
            
            // コルーチンのクリーンアップ
            StopAllCoroutines();
        }
    }
}

