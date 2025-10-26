using UnityEngine;

namespace Game.Character
{
    /// <summary>
    /// セーブ/ロード管理
    /// 試作段階: PlayerPrefs + JSON
    /// リリース時: Easy Save 3に差し替え
    /// </summary>
    public static class SaveManager
    {
        private const string SAVE_KEY = "GameSaveData";
        
        /// <summary>
        /// ゲームデータを保存
        /// </summary>
        public static void Save()
        {
            var saveData = new SaveData
            {
                // キャラクターデータ
                characterDataPath = AssetPathHelper.GetAssetPath(GameManager.Instance.currentCharacter.data),
                characterLevel = GameManager.Instance.currentCharacter.currentLevel,
                characterExp = GameManager.Instance.currentCharacter.currentExp,
                characterAffection = GameManager.Instance.currentCharacter.affection,
                
                // 装備
                equippedWeaponId = GameManager.Instance.currentCharacter.equippedWeaponId,
                equippedArmorId = GameManager.Instance.currentCharacter.equippedArmorId,
                equippedAccessoryId = GameManager.Instance.currentCharacter.equippedAccessoryId,
                
                // プレイヤー状態
                playerCurrency = GameManager.Instance.playerCurrency,
                playerGems = GameManager.Instance.playerGems,
            };
            
            string json = JsonUtility.ToJson(saveData, true);
            
            // 試作版: PlayerPrefs
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
            
            // リリース版: Easy Save 3（コメントアウト）
            // ES3.Save(SAVE_KEY, saveData);
            
            Debug.Log("セーブ完了");
        }
        
        /// <summary>
        /// ゲームデータを読み込み
        /// </summary>
        public static void Load()
        {
            // 試作版: PlayerPrefs
            if (!PlayerPrefs.HasKey(SAVE_KEY))
            {
                Debug.Log("セーブデータなし");
                return;
            }
            
            string json = PlayerPrefs.GetString(SAVE_KEY);
            var saveData = JsonUtility.FromJson<SaveData>(json);
            
            // リリース版: Easy Save 3（コメントアウト）
            // var saveData = ES3.Load<SaveData>(SAVE_KEY);
            
            // データを復元
            var manager = GameManager.Instance;
            
            // キャラクターデータを読み込み
            var characterData = AssetPathHelper.LoadAsset<CharacterData>(saveData.characterDataPath);
            if (characterData != null)
            {
                manager.currentCharacter = new CharacterInstance
                {
                    data = characterData,
                    currentLevel = saveData.characterLevel,
                    currentExp = saveData.characterExp,
                    affection = saveData.characterAffection,
                    equippedWeaponId = saveData.equippedWeaponId,
                    equippedArmorId = saveData.equippedArmorId,
                    equippedAccessoryId = saveData.equippedAccessoryId
                };
            }
            
            manager.playerCurrency = saveData.playerCurrency;
            manager.playerGems = saveData.playerGems;
            
            Debug.Log("ロード完了");
        }
        
        /// <summary>
        /// セーブデータを削除
        /// </summary>
        public static void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            // ES3.DeleteKey(SAVE_KEY);
            Debug.Log("セーブデータ削除");
        }
    }
    
    /// <summary>
    /// セーブデータ構造
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        // キャラクター
        public string characterDataPath;
        public int characterLevel;
        public int characterExp;
        public int characterAffection;
        
        // 装備
        public string equippedWeaponId;
        public string equippedArmorId;
        public string equippedAccessoryId;
        
        // プレイヤー状態
        public int playerCurrency;
        public int playerGems;
    }
    
    /// <summary>
    /// アセットパスのヘルパー
    /// </summary>
    public static class AssetPathHelper
    {
        public static string GetAssetPath(Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(obj);
#else
            // ランタイムではリソースパスを返す
            return obj != null ? obj.name : "";
#endif
        }
        
        public static T LoadAsset<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
#else
            // ランタイムではResourcesから読み込む
            return Resources.Load<T>(path);
#endif
        }
    }
}

