using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor.SceneManagement;
using Game.Character.UI;

namespace ProjectSetup
{
    /// <summary>
    /// キャラクター育成UIのワンクリック生成
    /// </summary>
    public static class CharacterUISetup
    {
        [MenuItem("Tools/Character System/Create Character UI (1-Click)", priority = 110)]
        public static void CreateCharacterUI()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。");
                return;
            }

            Debug.Log("=== キャラクター育成UI生成開始 ===");

            // Canvasを探す、なければ作成
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                canvas = CreateCanvas();
            }

            // キャラクター育成UIを生成
            GameObject uiRoot = CreateCharacterUIHierarchy(canvas);

            // CharacterUIManagerを設定
            SetupCharacterUIManager(uiRoot);

            // 選択状態にする
            Selection.activeGameObject = uiRoot;

            // シーンを保存
            EditorSceneManager.SaveOpenScenes();

            Debug.Log("=== キャラクター育成UI生成完了 ===");
            
            EditorUtility.DisplayDialog(
                "UI生成完了",
                "キャラクター育成UIを生成しました！\n\n" +
                "Hierarchyの「CharacterUI」を確認してください。\n\n" +
                "プレイボタンを押すとUIが動作します。\n" +
                "「訓練」ボタンで経験値が増えます。",
                "OK"
            );
        }

        static Canvas CreateCanvas()
        {
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920); // 縦画面
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // EventSystemも必要
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();
            }
            
            Debug.Log("Canvasを作成しました");
            return canvas;
        }

        static GameObject CreateCharacterUIHierarchy(Canvas canvas)
        {
            // ルート
            GameObject root = new GameObject("CharacterUI");
            root.transform.SetParent(canvas.transform, false);
            RectTransform rootRT = root.AddComponent<RectTransform>();
            rootRT.anchorMin = Vector2.zero;
            rootRT.anchorMax = Vector2.one;
            rootRT.sizeDelta = Vector2.zero;

            // 背景
            CreateBackground(root);

            // ヘッダー（キャラ名、レベル）
            CreateHeader(root);

            // キャラクター立ち絵エリア
            CreateCharacterPortrait(root);

            // 経験値バー
            CreateExpBar(root);

            // ステータス表示
            CreateStatsPanel(root);

            // ボタン群
            CreateButtons(root);

            // レベルアップエフェクト
            CreateLevelUpEffect(root);

            return root;
        }

        static void CreateBackground(GameObject parent)
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(parent.transform, false);
            RectTransform rt = bg.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            Image img = bg.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.15f, 1f); // 暗めの背景
        }

        static void CreateHeader(GameObject parent)
        {
            GameObject header = new GameObject("Header");
            header.transform.SetParent(parent.transform, false);
            RectTransform rt = header.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -50);
            rt.sizeDelta = new Vector2(0, 100);

            // キャラ名
            CreateText(header, "CharacterName", new Vector2(-200, 0), new Vector2(400, 50), "キャラクター名", 36);

            // レベル
            CreateText(header, "LevelText", new Vector2(200, 0), new Vector2(200, 50), "Lv.1", 32);
        }

        static void CreateCharacterPortrait(GameObject parent)
        {
            GameObject portraitArea = new GameObject("CharacterPortrait");
            portraitArea.transform.SetParent(parent.transform, false);
            RectTransform rt = portraitArea.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.6f);
            rt.anchorMax = new Vector2(0.5f, 0.6f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(400, 600);

            Image img = portraitArea.AddComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.4f, 1f); // プレースホルダー
            
            // タッチ入力ハンドラを追加
            portraitArea.AddComponent<TouchInputHandler>();
            
            // ラベル
            CreateText(portraitArea, "Label", new Vector2(0, -320), new Vector2(400, 40), "※立ち絵エリア", 20);
        }

        static void CreateExpBar(GameObject parent)
        {
            GameObject expPanel = new GameObject("ExpPanel");
            expPanel.transform.SetParent(parent.transform, false);
            RectTransform rt = expPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.35f);
            rt.anchorMax = new Vector2(0.5f, 0.35f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(800, 80);

            // EXPラベル
            CreateText(expPanel, "ExpLabel", new Vector2(-300, 0), new Vector2(150, 40), "EXP", 24);

            // スライダー
            GameObject sliderGO = new GameObject("ExpSlider");
            sliderGO.transform.SetParent(expPanel.transform, false);
            RectTransform sliderRT = sliderGO.AddComponent<RectTransform>();
            sliderRT.anchorMin = new Vector2(0.5f, 0.5f);
            sliderRT.anchorMax = new Vector2(0.5f, 0.5f);
            sliderRT.pivot = new Vector2(0.5f, 0.5f);
            sliderRT.anchoredPosition = new Vector2(50, 0);
            sliderRT.sizeDelta = new Vector2(500, 30);

            Slider slider = sliderGO.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 0;

            // スライダーの背景
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(sliderGO.transform, false);
            RectTransform bgRT = bg.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.sizeDelta = Vector2.zero;
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            // スライダーの塗りつぶし
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGO.transform, false);
            RectTransform fillAreaRT = fillArea.AddComponent<RectTransform>();
            fillAreaRT.anchorMin = Vector2.zero;
            fillAreaRT.anchorMax = Vector2.one;
            fillAreaRT.sizeDelta = Vector2.zero;

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRT = fill.AddComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = Vector2.one;
            fillRT.sizeDelta = Vector2.zero;
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = new Color(0.2f, 0.6f, 1f, 1f); // 青

            slider.fillRect = fillRT;
            slider.targetGraphic = fillImg;

            // 経験値テキスト
            CreateText(expPanel, "ExpText", new Vector2(350, 0), new Vector2(200, 40), "0 / 100", 24);
        }

        static void CreateStatsPanel(GameObject parent)
        {
            GameObject statsPanel = new GameObject("StatsPanel");
            statsPanel.transform.SetParent(parent.transform, false);
            RectTransform rt = statsPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.2f);
            rt.anchorMax = new Vector2(0.5f, 0.2f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(800, 150);

            // 背景
            Image bg = statsPanel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.8f);

            // ステータステキスト
            CreateText(statsPanel, "HPText", new Vector2(-300, 40), new Vector2(350, 40), "HP: 100", 28);
            CreateText(statsPanel, "AttackText", new Vector2(100, 40), new Vector2(350, 40), "ATK: 10", 28);
            CreateText(statsPanel, "DefenseText", new Vector2(-300, -20), new Vector2(350, 40), "DEF: 5", 28);
            CreateText(statsPanel, "SpeedText", new Vector2(100, -20), new Vector2(350, 40), "SPD: 5.0", 28);
        }

        static void CreateButtons(GameObject parent)
        {
            GameObject buttonPanel = new GameObject("ButtonPanel");
            buttonPanel.transform.SetParent(parent.transform, false);
            RectTransform rt = buttonPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.08f);
            rt.anchorMax = new Vector2(0.5f, 0.08f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(900, 120);

            // ボタン生成
            CreateButton(buttonPanel, "TrainingButton", new Vector2(-280, 0), "訓練", new Color(0.3f, 0.6f, 0.3f));
            CreateButton(buttonPanel, "LevelUpButton", new Vector2(0, 0), "レベルアップ", new Color(0.6f, 0.3f, 0.3f));
            CreateButton(buttonPanel, "EquipmentButton", new Vector2(280, 0), "装備", new Color(0.4f, 0.4f, 0.6f));
        }

        static GameObject CreateButton(GameObject parent, string name, Vector2 position, string label, Color color)
        {
            GameObject btnGO = new GameObject(name);
            btnGO.transform.SetParent(parent.transform, false);
            RectTransform rt = btnGO.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = new Vector2(240, 100);

            Image img = btnGO.AddComponent<Image>();
            img.color = color;

            Button btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = img;

            // ボタンテキスト
            CreateText(btnGO, "Text", Vector2.zero, new Vector2(200, 80), label, 28);

            return btnGO;
        }

        static void CreateLevelUpEffect(GameObject parent)
        {
            GameObject effect = new GameObject("LevelUpEffectPanel");
            effect.transform.SetParent(parent.transform, false);
            RectTransform rt = effect.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            // 半透明背景
            Image bgImg = effect.AddComponent<Image>();
            bgImg.color = new Color(0, 0, 0, 0.7f);

            // エフェクトテキスト
            GameObject textGO = CreateText(effect, "LevelUpEffectText", Vector2.zero, new Vector2(800, 300), "LEVEL UP!\nLv.2", 64);
            
            TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.color = Color.yellow;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
            }

            // 最初は非表示
            effect.SetActive(false);
        }

        static GameObject CreateText(GameObject parent, string name, Vector2 position, Vector2 size, string text, int fontSize)
        {
            GameObject textGO = new GameObject(name);
            textGO.transform.SetParent(parent.transform, false);
            RectTransform rt = textGO.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;

            TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            return textGO;
        }

        static void SetupCharacterUIManager(GameObject root)
        {
            CharacterUIManager manager = root.AddComponent<CharacterUIManager>();

            SerializedObject so = new SerializedObject(manager);

            // UI要素を自動リンク
            so.FindProperty("characterNameText").objectReferenceValue = root.transform.Find("Header/CharacterName")?.GetComponent<TextMeshProUGUI>();
            so.FindProperty("levelText").objectReferenceValue = root.transform.Find("Header/LevelText")?.GetComponent<TextMeshProUGUI>();
            so.FindProperty("expText").objectReferenceValue = root.transform.Find("ExpPanel/ExpText")?.GetComponent<TextMeshProUGUI>();
            so.FindProperty("expSlider").objectReferenceValue = root.transform.Find("ExpPanel/ExpSlider")?.GetComponent<Slider>();

            so.FindProperty("hpText").objectReferenceValue = root.transform.Find("StatsPanel/HPText")?.GetComponent<TextMeshProUGUI>();
            so.FindProperty("attackText").objectReferenceValue = root.transform.Find("StatsPanel/AttackText")?.GetComponent<TextMeshProUGUI>();
            so.FindProperty("defenseText").objectReferenceValue = root.transform.Find("StatsPanel/DefenseText")?.GetComponent<TextMeshProUGUI>();
            so.FindProperty("speedText").objectReferenceValue = root.transform.Find("StatsPanel/SpeedText")?.GetComponent<TextMeshProUGUI>();

            so.FindProperty("levelUpButton").objectReferenceValue = root.transform.Find("ButtonPanel/LevelUpButton")?.GetComponent<Button>();
            so.FindProperty("trainingButton").objectReferenceValue = root.transform.Find("ButtonPanel/TrainingButton")?.GetComponent<Button>();
            so.FindProperty("equipmentButton").objectReferenceValue = root.transform.Find("ButtonPanel/EquipmentButton")?.GetComponent<Button>();

            so.FindProperty("levelUpEffectPanel").objectReferenceValue = root.transform.Find("LevelUpEffectPanel")?.gameObject;
            so.FindProperty("levelUpEffectText").objectReferenceValue = root.transform.Find("LevelUpEffectPanel/LevelUpEffectText")?.GetComponent<TextMeshProUGUI>();

            so.FindProperty("characterPortrait").objectReferenceValue = root.transform.Find("CharacterPortrait")?.GetComponent<Image>();

            so.ApplyModifiedPropertiesWithoutUndo();

            Debug.Log("CharacterUIManagerを設定しました");
        }
    }
}

