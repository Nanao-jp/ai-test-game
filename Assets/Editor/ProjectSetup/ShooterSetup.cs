using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using System.Text;

namespace ProjectSetup
{
    public static class ShooterSetup
    {
        private const string DEMO_SCENE_PATH = "Assets/Space Shooter Template FREE/Scenes/Demo_Scene.unity";

        [MenuItem("Tools/Shooter/Setup (Space Shooter Preset)", priority = 0)]
        public static void Setup()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。停止してから実行してください。");
                return;
            }

            // Demo_Sceneを開く
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(DEMO_SCENE_PATH);
            if (asset == null)
            {
                Debug.LogError($"Space Shooter Template FREEのデモシーンが見つかりません: {DEMO_SCENE_PATH}");
                return;
            }

            // Demo_Sceneをスタートシーンに設定
            EditorSceneManager.playModeStartScene = asset;
            EnsureBuildSettingsOnly(DEMO_SCENE_PATH);
            EditorSceneManager.OpenScene(DEMO_SCENE_PATH, OpenSceneMode.Single);

            // アスペクト比強制をカメラに追加（任意）
            var cam = Camera.main;
            if (cam != null)
            {
            if (cam.GetComponent<Core.Camera2D.AspectEnforcerPortrait>() == null)
            {
                cam.gameObject.AddComponent<Core.Camera2D.AspectEnforcerPortrait>();
                    Debug.Log("AspectEnforcerPortrait を Main Camera に追加しました。");
                }
            }

            // ログ機能の確実な初期化（静的初期化を強制）
            System.Type.GetType("Game.Core.Debugging.ConsoleLogRecorder, Assembly-CSharp");

            WriteSetupSummary();

            Debug.Log($"Setup完了: Space Shooter Template FREEのプリセット（{DEMO_SCENE_PATH}）を開きました。");
        }

        [MenuItem("Tools/Shooter/Open Demo Scene", priority = 1)]
        public static void OpenDemoScene()
        {
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(DEMO_SCENE_PATH);
            if (asset != null)
            {
                EditorSceneManager.OpenScene(DEMO_SCENE_PATH, OpenSceneMode.Single);
            }
            else
            {
                Debug.LogError($"デモシーンが見つかりません: {DEMO_SCENE_PATH}");
            }
        }

        [MenuItem("Tools/Shooter/Open Player Settings", priority = 2)]
        public static void OpenPlayerSettings()
        {
            SettingsService.OpenProjectSettings("Project/Player");
        }

        [MenuItem("Tools/Shooter/Setup Sound Effects", priority = 10)]
        public static void SetupSoundEffects()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。停止してから実行してください。");
                return;
            }

            // 音声クリップをロード
            var deathSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-09.wav");
            var powerupSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-07.wav");
            var enemyDeathSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-48.wav");

            if (deathSound == null || powerupSound == null || enemyDeathSound == null)
            {
                Debug.LogError("音声ファイルが見つかりません。Casual Game Sounds U6がインポートされているか確認してください。");
                return;
            }

            int playerCount = 0, bonusCount = 0, vfxCount = 0;

            // シーン内の全オブジェクトに音声設定
            foreach (var player in Object.FindObjectsByType<Player>(FindObjectsSortMode.None))
            {
                SetupPlayerSound(player.gameObject, deathSound);
                playerCount++;
            }

            foreach (var bonus in Object.FindObjectsByType<Bonus>(FindObjectsSortMode.None))
            {
                SetupBonusSound(bonus.gameObject, powerupSound);
                bonusCount++;
            }

            // 古いEnemySoundBridgeを削除（もう使わない）
            RemoveOldEnemySoundBridges();
            
            // プレハブ設定
            SetupPlayerPrefabSound("Assets/Space Shooter Template FREE/Prefabs/Player.prefab", deathSound);
            SetupBonusPrefabs(powerupSound);
            
            // 爆発VFXに音を設定
            vfxCount += SetupExplosionVFX(enemyDeathSound);

            Debug.Log($"音声設定完了: Player={playerCount}個, Bonus={bonusCount}個, ExplosionVFX={vfxCount}個 + プレハブ");
        }

        private static void SetupPlayerSound(GameObject player, AudioClip deathSound)
        {
            // PlayerSoundBridge（射撃音は削除）
            var playerBridge = player.GetComponent<Core.Audio.PlayerSoundBridge>();
            if (playerBridge == null) playerBridge = player.AddComponent<Core.Audio.PlayerSoundBridge>();
            var soPlayer = new SerializedObject(playerBridge);
            soPlayer.FindProperty("_deathSound").objectReferenceValue = deathSound;
            soPlayer.ApplyModifiedPropertiesWithoutUndo();

            // ShootingSoundBridgeを削除（不要）
            var shootBridge = player.GetComponent<Core.Audio.ShootingSoundBridge>();
            if (shootBridge != null)
            {
                Object.DestroyImmediate(shootBridge, true);
            }
        }

        private static void SetupBonusSound(GameObject bonus, AudioClip pickupSound)
        {
            var bridge = bonus.GetComponent<Core.Audio.BonusSoundBridge>();
            if (bridge == null) bridge = bonus.AddComponent<Core.Audio.BonusSoundBridge>();
            var so = new SerializedObject(bridge);
            so.FindProperty("_pickupSound").objectReferenceValue = pickupSound;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetupPlayerPrefabSound(string prefabPath, AudioClip deathSound)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) return;

            var player = prefab.GetComponent<Player>();
            if (player != null)
            {
                SetupPlayerSound(prefab, deathSound);
                EditorUtility.SetDirty(prefab);
                AssetDatabase.SaveAssets();
            }
        }

        private static int SetupExplosionVFX(AudioClip explosionSound)
        {
            int count = 0;
            
            // 敵の爆発VFXプレハブを検索
            var explosionPaths = new[]
            {
                "Assets/Space Shooter Template FREE/Prefabs/VFX/Enemy Explosion.prefab",
                "Assets/Space Shooter Template FREE/Prefabs/VFX/Enemy Explosion PC.prefab",
                "Assets/Space Shooter Template FREE/Prefabs/VFX/Enemy Explosion_02.prefab",
                "Assets/Space Shooter Template FREE/Prefabs/VFX/Enemy Explosion_02_PC.prefab"
            };

            foreach (var path in explosionPaths)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    // SoundEffectPlayerを追加
                    var soundPlayer = prefab.GetComponent<Core.Audio.SoundEffectPlayer>();
                    if (soundPlayer == null)
                    {
                        soundPlayer = prefab.AddComponent<Core.Audio.SoundEffectPlayer>();
                    }
                    
                    var so = new SerializedObject(soundPlayer);
                    so.FindProperty("_clip").objectReferenceValue = explosionSound;
                    so.FindProperty("_playOnStart").boolValue = true;
                    so.FindProperty("_playOnDestroy").boolValue = false;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    
                    EditorUtility.SetDirty(prefab);
                    count++;
                }
            }
            
            AssetDatabase.SaveAssets();
            return count;
        }

        private static void SetupBonusPrefabs(AudioClip pickupSound)
        {
            var guids = AssetDatabase.FindAssets("t:Prefab Power", new[] { "Assets/Space Shooter Template FREE" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.GetComponent<Bonus>() != null)
                {
                    SetupBonusSound(prefab, pickupSound);
                    EditorUtility.SetDirty(prefab);
                }
            }
            AssetDatabase.SaveAssets();
        }

        private static void RemoveOldEnemySoundBridges()
        {
            // シーン内の敵から古いEnemySoundBridgeを削除
            foreach (var enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                var oldBridge = enemy.GetComponent<Core.Audio.EnemySoundBridge>();
                if (oldBridge != null)
                {
                    Object.DestroyImmediate(oldBridge);
                }
            }
            
            // 敵プレハブからも削除
            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Space Shooter Template FREE/Prefabs/Enemies" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.GetComponent<Enemy>() != null)
                {
                    var oldBridge = prefab.GetComponent<Core.Audio.EnemySoundBridge>();
                    if (oldBridge != null)
                    {
                        Object.DestroyImmediate(oldBridge, true);
                        EditorUtility.SetDirty(prefab);
                    }
                }
            }
            AssetDatabase.SaveAssets();
        }

        private static void EnsureBuildSettingsOnly(string path)
        {
            var scenes = new EditorBuildSettingsScene[] { new EditorBuildSettingsScene(path, true) };
            EditorBuildSettings.scenes = scenes;
        }

        private static void WriteSetupSummary()
        {
            try
            {
                var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                var logsDir = Path.Combine(projectRoot, "Logs");
                Directory.CreateDirectory(logsDir);
                var summaryPath = Path.Combine(logsDir, "setup_summary.txt");

                var sb = new StringBuilder();
                sb.AppendLine("=== Shooter Setup Summary ===");
                sb.AppendLine($"Scene: {DEMO_SCENE_PATH}");
                sb.AppendLine("Mode: Space Shooter Template FREE プリセット");
                sb.AppendLine($"Date: {System.DateTime.Now}");
                sb.AppendLine("");
                sb.AppendLine("カスタム実装は削除され、Space Shooterのプリセットをそのまま使用します。");
                sb.AppendLine("Input: Legacy Input (UnityEngine.Input) を使用");
                sb.AppendLine("");
                sb.AppendLine("有効な拡張機能:");
                sb.AppendLine("  - ConsoleLogRecorder (ログ出力)");
                sb.AppendLine("  - AspectEnforcerPortrait (9:16アスペクト比強制)");

                File.WriteAllText(summaryPath, sb.ToString(), Encoding.UTF8);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"セットアップサマリの書き込みに失敗: {e.Message}");
            }
        }
    }
}
