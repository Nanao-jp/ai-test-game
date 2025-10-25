using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ProjectSetup
{
    public static class ShooterSetup
    {
        [MenuItem("Tools/Shooter/Setup (Reset & Apply Assets)", priority = 0)]
        public static void Setup()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。停止してから実行してください。");
                return;
            }

            // New clean scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Camera
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                cam.tag = "MainCamera";
                cam.transform.position = new Vector3(0, 0, -10);
            }
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            if (cam.GetComponent<Core.Camera2D.AspectEnforcerPortrait>() == null)
            {
                cam.gameObject.AddComponent<Core.Camera2D.AspectEnforcerPortrait>();
            }

            // Bootstrap & Overlay
            var boot = new GameObject("Bootstrap");
            boot.AddComponent<Core.Bootstrap.Bootstrap>();
            if (boot.GetComponent<Game.Core.Debugging.RuntimeDebugOverlay>() == null)
            {
                boot.AddComponent<Game.Core.Debugging.RuntimeDebugOverlay>();
            }
            // ConsoleLogRecorder は静的初期化で動くが、明示的に型参照してロードを確実化
            System.Type.GetType("Game.Core.Debugging.ConsoleLogRecorder, Assembly-CSharp");

            // Background
            var bg = new GameObject("Background");
            var grid = bg.AddComponent<Core.Visual.ProceduralGridBackground>();
            grid.Generate();
            var bgPrefab = FindAsset<GameObject>("Assets/Space Shooter Template FREE/Prefabs/Background/Backgrounds.prefab");
            if (bgPrefab == null) bgPrefab = FindAsset<GameObject>("Backgrounds 1.prefab");
            if (bgPrefab != null)
            {
                var inst = Object.Instantiate(bgPrefab, bg.transform);
                inst.name = inst.name.Replace("(Clone)", "");
            }

            // Systems
            var systems = new GameObject("ShooterSystems");
            var scroll = systems.AddComponent<Game.Shooter.AutoScrollSystem>();
            scroll.SetAffectCamera(true);

            // Player
            var player = new GameObject("Player");
            var rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            var pc = player.AddComponent<Game.Player.PlayerController2D>();
            var soPc = new SerializedObject(pc);
            soPc.FindProperty("shooterMode").boolValue = true;
            soPc.FindProperty("fixedShootDirection").vector2Value = Vector2.up;
            // Apply Space Shooter assets if found
            var ship = FindAsset<GameObject>("Assets/Space Shooter Template FREE/Prefabs/Player.prefab");
            var bullet = FindAsset<GameObject>("Assets/Space Shooter Template FREE/Prefabs/Projectiles/Player_Short_Lazer.prefab");
            if (ship != null) soPc.FindProperty("shipVisualPrefab").objectReferenceValue = ship;
            if (bullet != null) soPc.FindProperty("bulletPrefab").objectReferenceValue = bullet;
            soPc.ApplyModifiedPropertiesWithoutUndo();

#if ENABLE_INPUT_SYSTEM
            var pi = player.GetComponent<PlayerInput>();
            if (pi == null) pi = player.AddComponent<PlayerInput>();
            var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            if (actions != null)
            {
                pi.actions = actions;
                // map強制とイベント/ポーリング両対応
                pi.defaultActionMap = "Player";
                pi.notificationBehavior = PlayerNotifications.SendMessages;
            }
#endif
            player.transform.position = new Vector3(0, -4f, 0);

            // Spawner
            var spawner = new GameObject("FormationSpawner");
            spawner.AddComponent<Game.Shooter.FormationSpawner>();
            var soSp = new SerializedObject(spawner.GetComponent<Game.Shooter.FormationSpawner>());
            soSp.FindProperty("spawnYOffset").floatValue = 14f;
            soSp.ApplyModifiedPropertiesWithoutUndo();

            // Save & set as start scene
            var path = "Assets/Scenes/Stage_VerticalShoot.unity";
            EditorSceneManager.SaveScene(scene, path);
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (asset != null) EditorSceneManager.playModeStartScene = asset;
            EnsureBuildSettingsOnly(path);
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

            Debug.Log("Shooter setup complete: clean scene, assets applied, start scene set.");
        }

        // ショートカットは残すが、メニューは最小限
        [MenuItem("Tools/Shooter/Open Shooter Scene", priority = 50)]
        public static void OpenScene()
        {
            const string path = "Assets/Scenes/Stage_VerticalShoot.unity";
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (asset != null) EditorSceneManager.OpenScene(path);
        }

        // 旧拡張の掃除も統合
        [MenuItem("Tools/Shooter/Setup + Cleanup", priority = 1)]
        public static void SetupWithCleanup()
        {
            // 定義を付与して旧メニューをビルドから隠す
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            const string define = "HIDE_LEGACY_MENUS";
            if (!defines.Contains(define))
            {
                defines = string.IsNullOrEmpty(defines) ? define : (defines + ";" + define);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
            }

            // シーン内の旧要素を掃除
            foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (go == null) continue;
                if (go.name.Contains("EnemySpawner") || go.name.Contains("Slash"))
                {
                    Object.DestroyImmediate(go);
                }
            }

            Setup();
        }

        private static void EnsureBuildSettingsOnly(string path)
        {
            var scenes = new EditorBuildSettingsScene[] { new EditorBuildSettingsScene(path, true) };
            EditorBuildSettings.scenes = scenes;
        }

        private static T FindAsset<T>(string exactPathOrNameContains) where T : Object
        {
            if (exactPathOrNameContains.StartsWith("Assets/"))
            {
                var obj = AssetDatabase.LoadAssetAtPath<T>(exactPathOrNameContains);
                if (obj != null) return obj;
            }
            var guids = AssetDatabase.FindAssets("");
            foreach (var g in guids)
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                if (p.ToLowerInvariant().Contains(exactPathOrNameContains.ToLowerInvariant()))
                {
                    var obj = AssetDatabase.LoadAssetAtPath<T>(p);
                    if (obj != null) return obj;
                }
            }
            return null;
        }
    }
}


