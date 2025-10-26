using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using Game.Character;

namespace ProjectSetup
{
    /// <summary>
    /// キャラクター育成システムのワンクリックセットアップ
    /// </summary>
    public static class CharacterSystemSetup
    {
        [MenuItem("Tools/Character System/Setup Test (1-Click)", priority = 100)]
        public static void SetupCharacterSystemTest()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。停止してから実行してください。");
                return;
            }

            Debug.Log("=== キャラクター育成システムのセットアップ開始 ===");

            // 1. テストキャラクターデータを作成
            var characterData = CreateTestCharacterData();

            // 2. GameManagerをシーンに追加
            SetupGameManager(characterData);

            // 3. CharacterSystemTestをシーンに追加
            SetupCharacterSystemTest(characterData);

            // 4. シーンを保存
            EditorSceneManager.SaveOpenScenes();

            Debug.Log("=== セットアップ完了！ ===");
            Debug.Log("プレイボタンを押してテストしてください。");
            Debug.Log("キー操作: [E]経験値, [L]レベルアップ, [I]ステータス, [S]セーブ, [O]ロード");
            
            // 完了ダイアログ
            EditorUtility.DisplayDialog(
                "セットアップ完了",
                "キャラクター育成システムのテストセットアップが完了しました！\n\n" +
                "プレイボタンを押してテストしてください。\n\n" +
                "操作方法:\n" +
                "[E] 経験値 +50\n" +
                "[L] レベルアップ\n" +
                "[I] ステータス表示\n" +
                "[S] セーブ\n" +
                "[O] ロード\n" +
                "[B] 戦闘シミュレーション",
                "OK"
            );
        }

        /// <summary>
        /// テスト用キャラクターデータを作成
        /// </summary>
        static CharacterData CreateTestCharacterData()
        {
            const string path = "Assets/Data/Characters/Character_TestGirl.asset";
            
            // 既に存在する場合は再利用
            var existing = AssetDatabase.LoadAssetAtPath<CharacterData>(path);
            if (existing != null)
            {
                Debug.Log($"既存のキャラクターデータを使用: {path}");
                return existing;
            }

            // フォルダ作成
            System.IO.Directory.CreateDirectory("Assets/Data");
            System.IO.Directory.CreateDirectory("Assets/Data/Characters");

            // CharacterDataを作成
            var characterData = ScriptableObject.CreateInstance<CharacterData>();
            characterData.characterName = "テストガール";
            characterData.rarity = Rarity.SSR;
            
            // 基礎ステータス
            characterData.baseHP = 100f;
            characterData.baseAttack = 10f;
            characterData.baseDefense = 5f;
            characterData.baseMoveSpeed = 5f;
            characterData.baseFireRate = 1f;
            
            // 成長カーブ（レベル1→100で3倍）
            characterData.hpGrowth = AnimationCurve.EaseInOut(0, 1, 1, 3);
            characterData.attackGrowth = AnimationCurve.EaseInOut(0, 1, 1, 3);
            characterData.defenseGrowth = AnimationCurve.EaseInOut(0, 1, 1, 2);

            // アセットとして保存
            AssetDatabase.CreateAsset(characterData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"テストキャラクターデータを作成: {path}");
            return characterData;
        }

        /// <summary>
        /// GameManagerをシーンに追加
        /// </summary>
        static void SetupGameManager(CharacterData characterData)
        {
            // 既に存在する場合はスキップ
            var existing = Object.FindObjectOfType<GameManager>();
            if (existing != null)
            {
                Debug.Log("GameManagerは既にシーンに存在します。");
                
                // キャラクターデータを設定
                if (existing.currentCharacter == null || existing.currentCharacter.data == null)
                {
                    existing.currentCharacter = new CharacterInstance
                    {
                        data = characterData,
                        currentLevel = 1,
                        currentExp = 0
                    };
                    EditorUtility.SetDirty(existing);
                }
                
                return;
            }

            // GameObjectを作成
            var go = new GameObject("GameManager");
            var manager = go.AddComponent<GameManager>();
            
            // 初期キャラクターを設定
            manager.currentCharacter = new CharacterInstance
            {
                data = characterData,
                currentLevel = 1,
                currentExp = 0
            };

            // Undo対応
            Undo.RegisterCreatedObjectUndo(go, "Create GameManager");
            
            Debug.Log("GameManagerをシーンに追加しました。");
        }

        /// <summary>
        /// CharacterSystemTestをシーンに追加
        /// </summary>
        static void SetupCharacterSystemTest(CharacterData characterData)
        {
            // 既に存在する場合は削除して再作成
            var existing = Object.FindObjectOfType<CharacterSystemTest>();
            if (existing != null)
            {
                Debug.Log("既存のCharacterSystemTestを削除します。");
                Undo.DestroyObjectImmediate(existing.gameObject);
            }

            // GameObjectを作成
            var go = new GameObject("CharacterSystemTest");
            var test = go.AddComponent<CharacterSystemTest>();

            // テストキャラクターを設定
            var so = new SerializedObject(test);
            so.FindProperty("testCharacter").objectReferenceValue = characterData;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Undo対応
            Undo.RegisterCreatedObjectUndo(go, "Create CharacterSystemTest");

            Debug.Log("CharacterSystemTestをシーンに追加しました。");
        }

        [MenuItem("Tools/Character System/Remove Test Setup", priority = 101)]
        public static void RemoveCharacterSystemTest()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。");
                return;
            }

            bool removed = false;

            // GameManagerを削除
            var manager = Object.FindObjectOfType<GameManager>();
            if (manager != null)
            {
                Undo.DestroyObjectImmediate(manager.gameObject);
                Debug.Log("GameManagerを削除しました。");
                removed = true;
            }

            // CharacterSystemTestを削除
            var test = Object.FindObjectOfType<CharacterSystemTest>();
            if (test != null)
            {
                Undo.DestroyObjectImmediate(test.gameObject);
                Debug.Log("CharacterSystemTestを削除しました。");
                removed = true;
            }

            if (removed)
            {
                EditorSceneManager.SaveOpenScenes();
                Debug.Log("テストセットアップを削除しました。");
            }
            else
            {
                Debug.Log("削除するオブジェクトがありません。");
            }
        }

        [MenuItem("Tools/Character System/Create Character Data", priority = 102)]
        public static void CreateCharacterDataMenu()
        {
            // 入力ダイアログはないので、デフォルト名で作成
            var characterData = ScriptableObject.CreateInstance<CharacterData>();
            characterData.characterName = "新しいキャラクター";
            characterData.rarity = Rarity.R;
            characterData.baseHP = 100f;
            characterData.baseAttack = 10f;
            characterData.baseDefense = 5f;
            characterData.baseMoveSpeed = 5f;
            characterData.baseFireRate = 1f;
            
            // 成長カーブ設定
            characterData.hpGrowth = AnimationCurve.EaseInOut(0, 1, 1, 3);
            characterData.attackGrowth = AnimationCurve.EaseInOut(0, 1, 1, 3);
            characterData.defenseGrowth = AnimationCurve.EaseInOut(0, 1, 1, 2);

            // 保存パスを取得（選択中のフォルダ）
            string path = "Assets/Data/Characters/NewCharacter.asset";
            
            // フォルダ作成
            System.IO.Directory.CreateDirectory("Assets/Data/Characters");
            
            // ユニークな名前を生成
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            
            // 保存
            AssetDatabase.CreateAsset(characterData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // 選択状態にする
            EditorGUIUtility.PingObject(characterData);
            Selection.activeObject = characterData;
            
            Debug.Log($"キャラクターデータを作成: {path}");
        }
    }
}

