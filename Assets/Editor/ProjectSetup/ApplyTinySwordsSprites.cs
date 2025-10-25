using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Game.Art;

namespace Editor.ProjectSetup
{
    public static class ApplyTinySwordsSprites
    {
        private const string TinyRoot = "Assets/Art/TinySwords";
        private const string SpriteSetResourcesDir = "Assets/Resources/Art";

        [MenuItem("Tools/Art/Apply Tiny Swords (Quick)")]
        public static void Apply()
        {
            var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { TinyRoot });
            var spritePaths = spriteGuids
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            if (spritePaths.Length == 0)
            {
                EditorUtility.DisplayDialog("Tiny Swords", "Sprites not found under " + TinyRoot, "OK");
                return;
            }

            Sprite pickPlayer = FindFirst(spritePaths, new[] { "warrior_blue", "warrior-blue", "warriorblue", "warrior", "hero", "knight", "player", "unit", "character", "soldier" });
            Sprite pickEnemy = FindFirstExcluding(spritePaths, new[] { "slime", "enemy", "orc", "goblin", "skeleton", "mob" }, pickPlayer);
            Sprite pickEnemyAlt = FindFirstExcluding(spritePaths, new[] { "slime", "enemy", "orc", "goblin", "skeleton", "mob", "archer", "rogue" }, pickPlayer);
            Sprite pickBullet = FindFirst(spritePaths, new[] { "arrow", "bullet", "projectile", "fx" });
            Sprite pickExp = FindFirst(spritePaths, new[] { "gem", "coin", "crystal", "pickup", "diamond" });
            Sprite pickBg = FindFirst(spritePaths, new[] { "ground", "grass", "dirt", "tile", "floor" });

            // Fallbacks if not found by keywords
            if (pickPlayer == null) pickPlayer = LoadAt(spritePaths.FirstOrDefault());
            if (pickEnemy == null || IsSame(pickEnemy, pickPlayer)) pickEnemy = LoadAt(spritePaths.FirstOrDefault(p => !IsSamePath(p, pickPlayer)) ?? spritePaths.Last());
            if (pickBullet == null) pickBullet = LoadAt(spritePaths.FirstOrDefault(p => p.ToLowerInvariant().Contains("arrow") || p.ToLowerInvariant().Contains("fx")) ?? spritePaths.Last());
            if (pickExp == null) pickExp = LoadAt(spritePaths.FirstOrDefault(p => p.ToLowerInvariant().Contains("gem") || p.ToLowerInvariant().Contains("coin") || p.ToLowerInvariant().Contains("pickup")) ?? spritePaths.First());
            if (pickBg == null) pickBg = LoadAt(spritePaths.FirstOrDefault(p => p.ToLowerInvariant().Contains("ground") || p.ToLowerInvariant().Contains("grass") || p.ToLowerInvariant().Contains("tile")) ?? spritePaths.First());

            var set = ScriptableObject.CreateInstance<SpriteSet>();
            set.playerSprite = pickPlayer;
            set.enemySprite = pickEnemy;
            set.bulletSprite = pickBullet;
            set.expSprite = pickExp;
            set.backgroundSprite = pickBg;
            set.enemyAltSprite = pickEnemyAlt != null && !IsSame(pickEnemyAlt, pickEnemy) ? pickEnemyAlt : null;

            var dir = SpriteSetResourcesDir;
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder(dir))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Art");
            }
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, "SpriteSet_TinySwords.asset"));
            AssetDatabase.CreateAsset(set, assetPath);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Tiny Swords", "SpriteSet created under Resources: \n" + assetPath + "\nランタイムで自動ロードされます。", "OK");
        }

        private static Sprite FindFirst(string[] paths, string[] keys)
        {
            foreach (var k in keys)
            {
                var p = paths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).ToLowerInvariant().Contains(k));
                if (!string.IsNullOrEmpty(p))
                {
                    return AssetDatabase.LoadAssetAtPath<Sprite>(p);
                }
            }
            return null;
        }

        private static Sprite FindFirstExcluding(string[] paths, string[] keys, Sprite exclude)
        {
            foreach (var k in keys)
            {
                var p = paths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).ToLowerInvariant().Contains(k) && !IsSamePath(x, exclude));
                if (!string.IsNullOrEmpty(p))
                {
                    return AssetDatabase.LoadAssetAtPath<Sprite>(p);
                }
            }
            return null;
        }

        private static Sprite LoadAt(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static SpriteSet EnsureSpriteSet(out string assetPath)
        {
            EnsureResourcesFolders();
            assetPath = AssetDatabase.FindAssets("t:SpriteSet", new[] { SpriteSetResourcesDir })
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();
            SpriteSet set;
            if (string.IsNullOrEmpty(assetPath))
            {
                set = ScriptableObject.CreateInstance<SpriteSet>();
                assetPath = Path.Combine(SpriteSetResourcesDir, "SpriteSet_TinySwords.asset");
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                AssetDatabase.CreateAsset(set, assetPath);
            }
            else
            {
                set = AssetDatabase.LoadAssetAtPath<SpriteSet>(assetPath);
            }
            return set;
        }

        [MenuItem("Tools/Art/Set Player (Warrior_Blue Prefab/Sprite)")]
        public static void SetPlayerWarriorBlue()
        {
            // 探索
            var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { TinyRoot });
            var paths = spriteGuids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
            var target = paths.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p).ToLowerInvariant().Contains("warrior_blue")
                                                   || Path.GetFileNameWithoutExtension(p).ToLowerInvariant().Contains("warrior-blue")
                                                   || Path.GetFileNameWithoutExtension(p).ToLowerInvariant().Contains("warriorblue")
                                                   || Path.GetFileNameWithoutExtension(p).ToLowerInvariant().Contains("warrior"));
            if (string.IsNullOrEmpty(target))
            {
                EditorUtility.DisplayDialog("Tiny Swords", "Warrior_Blue に該当するSpriteが見つかりませんでした。", "OK");
                return;
            }

            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(target);
            if (sp == null)
            {
                EditorUtility.DisplayDialog("Tiny Swords", "Spriteの読み込みに失敗しました:\n" + target, "OK");
                return;
            }

            // 既存のSpriteSetを取得 or 作成
            var set = EnsureSpriteSet(out var setPath);

            set.playerSprite = sp;
            // Warrior_Blue と同階層にPrefabがあれば優先設定
            var dir = Path.GetDirectoryName(target).Replace("\\", "/");
            var prefabPath = Directory.GetFiles(dir, "*.prefab", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(prefabPath))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath.Replace("\\", "/"));
                if (prefab != null) set.playerPrefab = prefab;
            }
            EditorUtility.SetDirty(set);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Tiny Swords", "Playerを Warrior_Blue に設定しました (Prefab優先):\n" + setPath, "OK");
        }

        // Assign from current selection (supports .aseprite root asset by pulling first Sprite sub-asset)
        [MenuItem("Tools/Art/Assign Player Sprite From Selection")] 
        public static void AssignPlayerSpriteFromSelection()
        {
            var obj = Selection.activeObject;
            if (obj == null)
            {
                EditorUtility.DisplayDialog("Assign Player Sprite", "ProjectウィンドウでSprite（または.aseprite）を選択してください。", "OK");
                return;
            }
            var path = AssetDatabase.GetAssetPath(obj);
            Sprite sprite = obj as Sprite;
            if (sprite == null)
            {
                var reps = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                sprite = reps?.OfType<Sprite>().FirstOrDefault();
            }
            if (sprite == null)
            {
                EditorUtility.DisplayDialog("Assign Player Sprite", "選択からSpriteが抽出できませんでした。", "OK");
                return;
            }

            var set = EnsureSpriteSet(out var setPath);
            set.playerSprite = sprite;
            EditorUtility.SetDirty(set);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Assign Player Sprite", "playerSprite を設定しました:\n" + setPath, "OK");
        }

        [MenuItem("Tools/Art/Assign Enemy Sprite From Selection")] 
        public static void AssignEnemySpriteFromSelection()
        {
            var obj = Selection.activeObject;
            if (obj == null)
            {
                EditorUtility.DisplayDialog("Assign Enemy Sprite", "ProjectウィンドウでSprite（または.aseprite）を選択してください。", "OK");
                return;
            }
            var path = AssetDatabase.GetAssetPath(obj);
            Sprite sprite = obj as Sprite;
            if (sprite == null)
            {
                var reps = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                sprite = reps?.OfType<Sprite>().FirstOrDefault();
            }
            if (sprite == null)
            {
                EditorUtility.DisplayDialog("Assign Enemy Sprite", "選択からSpriteが抽出できませんでした。", "OK");
                return;
            }

            var set = EnsureSpriteSet(out var setPath);
            set.enemySprite = sprite;
            EditorUtility.SetDirty(set);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Assign Enemy Sprite", "enemySprite を設定しました:\n" + setPath, "OK");
        }

        [MenuItem("Tools/Art/Validate SpriteSet")]
        public static void ValidateSpriteSet()
        {
            EnsureResourcesFolders();
            var setPath = AssetDatabase.FindAssets("t:SpriteSet", new[] { SpriteSetResourcesDir })
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(setPath))
            {
                EditorUtility.DisplayDialog("SpriteSet", "Resources/Art に SpriteSet がありません。\nTools → Art → Apply Tiny Swords (Quick) を実行してください。", "OK");
                return;
            }
            var set = AssetDatabase.LoadAssetAtPath<SpriteSet>(setPath);
            string Report(string name, Object obj) => obj ? $"✔ {name}: {AssetDatabase.GetAssetPath(obj)}" : $"✖ {name}: (none)";
            var msg = string.Join("\n", new[]
            {
                Report("playerSprite", set.playerSprite),
                Report("enemySprite", set.enemySprite),
                Report("enemyAltSprite", set.enemyAltSprite),
                Report("bulletSprite", set.bulletSprite),
                Report("expSprite", set.expSprite),
                Report("backgroundSprite", set.backgroundSprite),
            });
            EditorUtility.DisplayDialog("SpriteSet Validate", msg, "OK");
        }

        [MenuItem("Tools/Art/Set Enemy Sprite (Auto Non-Player)")]
        public static void SetEnemyAutoNonPlayer()
        {
            // 探索
            var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { TinyRoot });
            var paths = spriteGuids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            // 既存のSpriteSet
            EnsureResourcesFolders();
            var setPath = AssetDatabase.FindAssets("t:SpriteSet", new[] { SpriteSetResourcesDir })
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();
            SpriteSet set;
            if (string.IsNullOrEmpty(setPath))
            {
                set = ScriptableObject.CreateInstance<SpriteSet>();
                setPath = Path.Combine(SpriteSetResourcesDir, "SpriteSet_TinySwords.asset");
                setPath = AssetDatabase.GenerateUniqueAssetPath(setPath);
                AssetDatabase.CreateAsset(set, setPath);
            }
            else
            {
                set = AssetDatabase.LoadAssetAtPath<SpriteSet>(setPath);
            }

            var player = set.playerSprite;
            var enemy = FindFirstExcluding(paths, new[] { "slime", "enemy", "orc", "goblin", "skeleton", "mob" }, player);
            if (enemy == null)
            {
                var any = paths.FirstOrDefault(p => !IsSamePath(p, player));
                enemy = LoadAt(any);
            }
            if (enemy == null)
            {
                EditorUtility.DisplayDialog("Tiny Swords", "敵用Spriteが見つかりませんでした。", "OK");
                return;
            }

            set.enemySprite = enemy;
            EditorUtility.SetDirty(set);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Tiny Swords", "Enemy Sprite を自動設定しました:\n" + setPath, "OK");
        }

        private static void EnsureResourcesFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder(SpriteSetResourcesDir))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Art");
            }
        }

        private static bool IsSame(Sprite a, Sprite b)
        {
            if (a == null || b == null) return false;
            return AssetDatabase.GetAssetPath(a) == AssetDatabase.GetAssetPath(b);
        }

        private static bool IsSamePath(string path, Sprite s)
        {
            if (s == null || string.IsNullOrEmpty(path)) return false;
            return AssetDatabase.GetAssetPath(s) == path;
        }

        // --- Convenience setters -----------------------------------------------------------
        [MenuItem("Tools/Art/Set Enemy Sprite (Goblin/Slime Auto)")]
        public static void SetEnemyGoblinSlime()
        {
            EnsureResourcesFolders();
            var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { TinyRoot });
            var paths = spriteGuids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            var setPath = AssetDatabase.FindAssets("t:SpriteSet", new[] { SpriteSetResourcesDir })
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(setPath))
            {
                EditorUtility.DisplayDialog("Tiny Swords", "SpriteSet が見つかりません。先に Apply Tiny Swords (Quick) を実行してください。", "OK");
                return;
            }
            var set = AssetDatabase.LoadAssetAtPath<SpriteSet>(setPath);
            var enemy = FindFirstExcluding(paths, new[] { "goblin", "slime", "orc", "skeleton" }, set.playerSprite);
            if (enemy == null)
            {
                EditorUtility.DisplayDialog("Tiny Swords", "敵候補（goblin/slime等）が見つかりませんでした。", "OK");
                return;
            }
            set.enemySprite = enemy;
            if (set.enemyAltSprite == null)
            {
                var alt = FindFirstExcluding(paths, new[] { "slime", "goblin", "orc", "skeleton" }, set.playerSprite);
                if (alt != null && !IsSame(alt, enemy)) set.enemyAltSprite = alt;
            }
            EditorUtility.SetDirty(set);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Tiny Swords", "Enemy Sprite を設定しました:\n" + setPath, "OK");
        }

        [MenuItem("Tools/Art/Set Exp Sprite (Gem/Coin Auto)")]
        public static void SetExpGemCoin()
        {
            EnsureResourcesFolders();
            var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { TinyRoot });
            var paths = spriteGuids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            var setPath = AssetDatabase.FindAssets("t:SpriteSet", new[] { SpriteSetResourcesDir })
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(setPath))
            {
                EditorUtility.DisplayDialog("Tiny Swords", "SpriteSet が見つかりません。先に Apply Tiny Swords (Quick) を実行してください。", "OK");
                return;
            }
            var set = AssetDatabase.LoadAssetAtPath<SpriteSet>(setPath);
            var exp = FindFirst(paths, new[] { "gem", "coin", "crystal", "pickup", "diamond", "loot", "bag", "shard" });
            if (exp == null)
            {
                // 自動生成（円形のPNGを生成してSprite化）
                exp = EnsureGeneratedExpSprite();
            }
            set.expSprite = exp;
            EditorUtility.SetDirty(set);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Tiny Swords", "Exp Sprite を設定しました:\n" + setPath, "OK");
        }

        private static Sprite EnsureGeneratedExpSprite()
        {
            var genDir = Path.Combine(SpriteSetResourcesDir, "Generated");
            if (!AssetDatabase.IsValidFolder(genDir))
            {
                AssetDatabase.CreateFolder(SpriteSetResourcesDir, "Generated");
            }
            var pngPath = Path.Combine(genDir, "Exp_Default.png");
            if (!File.Exists(pngPath))
            {
                const int size = 48;
                var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                tex.hideFlags = HideFlags.HideAndDontSave;
                var center = (size - 1) * 0.5f;
                var radius = size * 0.38f;
                var innerRadius = radius - 1.5f;
                var colFill = new Color(0.2f, 0.9f, 1f, 1f);
                var colEdge = new Color(1f, 1f, 1f, 1f);
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        var dx = x - center;
                        var dy = y - center;
                        var d = Mathf.Sqrt(dx * dx + dy * dy);
                        if (d <= radius)
                        {
                            tex.SetPixel(x, y, d >= innerRadius ? colEdge : colFill);
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                }
                tex.Apply();
                var bytes = tex.EncodeToPNG();
                File.WriteAllBytes(pngPath, bytes);
                Object.DestroyImmediate(tex);

                AssetDatabase.ImportAsset(pngPath);
                var importer = AssetImporter.GetAtPath(pngPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spritePixelsPerUnit = 100f;
                    importer.filterMode = FilterMode.Point;
                    importer.alphaIsTransparency = true;
                    importer.SaveAndReimport();
                }
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
        }
    }
}


