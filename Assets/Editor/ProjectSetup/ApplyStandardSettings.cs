using System.IO;
using System.Linq; // For ToList/Any
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif
#if UNITY_RENDER_PIPELINE_UNIVERSAL
using UnityEngine.Rendering.Universal;
#endif

namespace ProjectSetup
{
    public static class ApplyStandardSettings
    {
        private const string UrpSourceAssetPath = "Assets/Settings/UniversalRP.asset";
        private const string UrpPcAssetPath = "Assets/Settings/UniversalRP_PC.asset";
        private const string UrpMobileAssetPath = "Assets/Settings/UniversalRP_Mobile.asset";

        [MenuItem("Tools/Project Setup/Apply Standard Settings", priority = 0)]
        public static void ApplyAll()
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                EnsureTagsAndLayers();
                ConfigurePhysics2D();
                ConfigureTimeAndQuality();
#if UNITY_RENDER_PIPELINE_UNIVERSAL
                ConfigureUrpAssets();
#endif
                AssetDatabase.SaveAssets();
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            Debug.Log("Project standard settings have been applied.");
        }

        [MenuItem("Tools/Project Setup/Prepare Current Scene", priority = 10)]
        public static void PrepareCurrentScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。停止してから再実行してください。");
                return;
            }
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("No active scene. Please open a scene and try again.");
                return;
            }

            // Bootstrap object
            var go = GameObject.Find("Bootstrap");
            if (go == null)
            {
                go = new GameObject("Bootstrap");
                UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Bootstrap");
            }
            if (go.GetComponent<Core.Bootstrap.Bootstrap>() == null)
            {
                go.AddComponent<Core.Bootstrap.Bootstrap>();
            }

            // Ensure EventSystem (Input System UI module)
            var existingEs = Object.FindFirstObjectByType<EventSystem>();
            if (existingEs == null)
            {
                var ev = new GameObject("EventSystem");
                existingEs = ev.AddComponent<EventSystem>();
                UnityEditor.Undo.RegisterCreatedObjectUndo(ev, "Create EventSystem");
            }
            // Remove StandaloneInputModule (旧InputManager) and add InputSystemUIInputModule
            var standalone = existingEs.GetComponent<StandaloneInputModule>();
            if (standalone != null)
            {
                UnityEditor.Undo.DestroyObjectImmediate(standalone);
            }
#if ENABLE_INPUT_SYSTEM
            if (existingEs.GetComponent<InputSystemUIInputModule>() == null)
            {
                existingEs.gameObject.AddComponent<InputSystemUIInputModule>();
            }
#endif

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);

            // Add open scene to Build Settings
            var path = scene.path;
            if (!string.IsNullOrEmpty(path))
            {
                var scenes = EditorBuildSettings.scenes.ToList();
                if (!scenes.Any(s => s.path == path))
                {
                    scenes.Add(new EditorBuildSettingsScene(path, true));
                    EditorBuildSettings.scenes = scenes.ToArray();
                }
            }

            Debug.Log("Current scene prepared: Bootstrap placed and added to Build Settings.");
        }

        [MenuItem("Tools/Project Setup/Create/Player With Camera", priority = 20)]
        public static void CreatePlayerWithCamera()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。停止してから再実行してください。");
                return;
            }

            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("No active scene. Please open a scene and try again.");
                return;
            }

            // Create Player
            var player = GameObject.Find("Player");
            if (player == null)
            {
                player = new GameObject("Player");
                player.layer = LayerMask.NameToLayer("Player");
                var rb = player.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                player.AddComponent<Game.Player.PlayerController2D>();
            }

            // Ensure visual sprite exists
            var srExisting = player.GetComponent<SpriteRenderer>();
            if (srExisting == null)
            {
                srExisting = player.AddComponent<SpriteRenderer>();
                srExisting.sortingOrder = 10;
                var builtin = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                if (builtin != null) srExisting.sprite = builtin;
                srExisting.color = new Color(0.95f, 0.95f, 0.95f, 1f);
            }

#if ENABLE_INPUT_SYSTEM
            // Ensure PlayerInput wired to actions and to send OnMove messages
            var pi = player.GetComponent<PlayerInput>();
            if (pi == null) pi = player.AddComponent<PlayerInput>();
            var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            if (actions != null)
            {
                pi.actions = actions;
                pi.defaultActionMap = "Player";
                pi.notificationBehavior = PlayerNotifications.SendMessages;
            }
#endif

            // Attach to Main Camera
            var cam = Camera.main;
            if (cam != null)
            {
                var follow = cam.GetComponent<Core.Camera2D.CameraFollow2D>();
                if (follow == null) follow = cam.gameObject.AddComponent<Core.Camera2D.CameraFollow2D>();
                follow.SetTarget(player.transform);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("Player and Camera follow are set.");
        }

        [MenuItem("Tools/Project Setup/Create/Background Grid", priority = 21)]
        public static void CreateBackgroundGrid()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。停止してから再実行してください。");
                return;
            }
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (!scene.IsValid()) return;
            var bg = GameObject.Find("Background");
            if (bg == null)
            {
                bg = new GameObject("Background");
                var sr = bg.AddComponent<SpriteRenderer>();
                var grid = bg.AddComponent<Core.Visual.ProceduralGridBackground>();
                grid.Generate();
            }
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("Background grid created.");
        }

        [MenuItem("Tools/Project Setup/Create/Enemy Spawner", priority = 22)]
        public static void CreateEnemySpawner()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Playモード中は実行できません。停止してから再実行してください。");
                return;
            }

            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (!scene.IsValid()) return;

            var sp = GameObject.Find("EnemySpawner");
            if (sp == null)
            {
                sp = new GameObject("EnemySpawner");
                var comp = sp.AddComponent<Game.Enemies.EnemySpawner>();
                var player = GameObject.Find("Player");
                if (player != null) comp.SetTarget(player.transform);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("Enemy spawner created.");
        }

        private static void EnsureTagsAndLayers()
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");
            var layersProp = tagManager.FindProperty("layers");

            EnsureArrayString(tagsProp, "Enemy");
            EnsureArrayString(tagsProp, "Projectile");
            EnsureArrayString(tagsProp, "Pickup");
            EnsureArrayString(tagsProp, "Boss");

            EnsureLayer(layersProp, "Player");
            EnsureLayer(layersProp, "Enemy");
            EnsureLayer(layersProp, "Projectile");
            EnsureLayer(layersProp, "Pickup");
            EnsureLayer(layersProp, "Environment");

            tagManager.ApplyModifiedProperties();
        }

        private static void EnsureArrayString(SerializedProperty arrayProp, string value)
        {
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                if (arrayProp.GetArrayElementAtIndex(i).stringValue == value) return;
            }
            arrayProp.InsertArrayElementAtIndex(arrayProp.arraySize);
            arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1).stringValue = value;
        }

        private static void EnsureLayer(SerializedProperty layersProp, string layerName)
        {
            // Layers 0-7 are reserved; find an empty slot from 8..31
            for (int i = 8; i < 32; i++)
            {
                var sp = layersProp.GetArrayElementAtIndex(i);
                if (sp == null) break;
                if (sp.stringValue == layerName) return;
            }
            for (int i = 8; i < 32; i++)
            {
                var sp = layersProp.GetArrayElementAtIndex(i);
                if (sp != null && string.IsNullOrEmpty(sp.stringValue))
                {
                    sp.stringValue = layerName;
                    return;
                }
            }
            Debug.LogWarning($"No empty user layer slot left to add '{layerName}'.");
        }

        private static void ConfigurePhysics2D()
        {
            Physics2D.queriesHitTriggers = true;

            int player = LayerMask.NameToLayer("Player");
            int enemy = LayerMask.NameToLayer("Enemy");
            int projectile = LayerMask.NameToLayer("Projectile");
            int pickup = LayerMask.NameToLayer("Pickup");

            if (player >= 0 && enemy >= 0)
                Physics2D.IgnoreLayerCollision(player, enemy, false);
            if (projectile >= 0 && enemy >= 0)
                Physics2D.IgnoreLayerCollision(projectile, enemy, false);
            if (projectile >= 0 && player >= 0)
                Physics2D.IgnoreLayerCollision(projectile, player, true);
            if (pickup >= 0 && player >= 0)
                Physics2D.IgnoreLayerCollision(pickup, player, false);
        }

        private static void ConfigureTimeAndQuality()
        {
            Time.fixedDeltaTime = 1f / 50f; // 0.02
            QualitySettings.vSyncCount = 0;
            // targetFrameRate はプレイ時初期化のほうが確実。別途ブートストラップで設定。
        }

#if UNITY_RENDER_PIPELINE_UNIVERSAL
        private static void ConfigureUrpAssets()
        {
            if (!File.Exists(UrpSourceAssetPath))
            {
                Debug.LogWarning($"URP asset not found at {UrpSourceAssetPath}. Skipping URP configuration.");
                return;
            }

            if (!File.Exists(UrpPcAssetPath))
                AssetDatabase.CopyAsset(UrpSourceAssetPath, UrpPcAssetPath);
            if (!File.Exists(UrpMobileAssetPath))
                AssetDatabase.CopyAsset(UrpSourceAssetPath, UrpMobileAssetPath);

            var urpPc = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(UrpPcAssetPath);
            var urpMobile = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(UrpMobileAssetPath);

            if (urpPc != null)
            {
                urpPc.renderScale = 1.0f;
                urpPc.supportsMainLightShadows = true;
                urpPc.shadowDistance = 25f;
                urpPc.msaaSampleCount = 1;
                EditorUtility.SetDirty(urpPc);
            }
            if (urpMobile != null)
            {
                urpMobile.renderScale = 0.9f;
                urpMobile.supportsMainLightShadows = false;
                urpMobile.shadowDistance = 10f;
                urpMobile.msaaSampleCount = 1;
                EditorUtility.SetDirty(urpMobile);
            }

            if (urpPc != null)
            {
                GraphicsSettings.renderPipelineAsset = urpPc;
                EditorUtility.SetDirty(GraphicsSettings.renderPipelineAsset);
            }
        }
#endif
    }
}


