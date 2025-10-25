using System.Linq;
using UnityEditor;
using UnityEngine;
using Game.Art;

namespace Editor.ProjectSetup
{
    public sealed class ArtSetupWindow : EditorWindow
    {
        private SpriteSet _set;
        private Object _playerSpriteObj;
        private Object _enemySpriteObj;
        private Object _enemyAltSpriteObj;
        private Object _bulletSpriteObj;
        private Object _expSpriteObj;
        private Object _backgroundSpriteObj;
        private Object _playerPrefabObj;
        private Object _slashSpriteObj;
        private Object _slashPrefabObj;

        [MenuItem("Tools/Art/Setup Panel")] 
        public static void Open()
        {
            var w = GetWindow<ArtSetupWindow>(true, "Art Setup", true);
            w.minSize = new Vector2(420, 480);
            w.Show();
        }

        private void OnEnable()
        {
            _set = Resources.Load<SpriteSet>("Art/SpriteSet_TinySwords");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("SpriteSet (Resources)", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Loaded Set", _set, typeof(SpriteSet), false);
            }
            if (_set == null)
            {
                if (GUILayout.Button("Create/Load SpriteSet"))
                {
                    _set = CreateOrLoadSpriteSet();
                }
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sprites", EditorStyles.boldLabel);
            _playerSpriteObj = ObjectFieldSprite("Player", _playerSpriteObj, _set.playerSprite, s => _set.playerSprite = s);
            _enemySpriteObj = ObjectFieldSprite("Enemy", _enemySpriteObj, _set.enemySprite, s => _set.enemySprite = s);
            _enemyAltSpriteObj = ObjectFieldSprite("Enemy Alt", _enemyAltSpriteObj, _set.enemyAltSprite, s => _set.enemyAltSprite = s);
            _bulletSpriteObj = ObjectFieldSprite("Bullet", _bulletSpriteObj, _set.bulletSprite, s => _set.bulletSprite = s);
            _expSpriteObj = ObjectFieldSprite("Exp", _expSpriteObj, _set.expSprite, s => _set.expSprite = s);
            _backgroundSpriteObj = ObjectFieldSprite("Background", _backgroundSpriteObj, _set.backgroundSprite, s => _set.backgroundSprite = s);
            _slashSpriteObj = ObjectFieldSprite("Slash VFX Sprite", _slashSpriteObj, _set.slashSprite, s => _set.slashSprite = s);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prefabs (Optional)", EditorStyles.boldLabel);
            _playerPrefabObj = EditorGUILayout.ObjectField("Player Prefab", _set.playerPrefab, typeof(GameObject), false);
            _set.playerPrefab = _playerPrefabObj as GameObject;
            _slashPrefabObj = EditorGUILayout.ObjectField("Slash VFX Prefab", _set.slashPrefab, typeof(GameObject), false);
            _set.slashPrefab = _slashPrefabObj as GameObject;

            EditorGUILayout.Space();
            if (GUILayout.Button("Save SpriteSet"))
            {
                EditorUtility.SetDirty(_set);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shortcuts", EditorStyles.boldLabel);
            if (GUILayout.Button("Apply Tiny Swords (Quick)"))
            {
                ApplyTinySwordsSprites.Apply();
                _set = Resources.Load<SpriteSet>("Art/SpriteSet_TinySwords");
                Repaint();
            }
            if (GUILayout.Button("Force Apply Player Sprite Now"))
            {
                VisualInspector.ForceApplyPlayerSprite();
            }
            if (GUILayout.Button("Inspect Player Visual"))
            {
                VisualInspector.InspectPlayerVisual();
            }
        }

        private Object ObjectFieldSprite(string label, Object cache, Sprite current, System.Action<Sprite> assign)
        {
            var obj = EditorGUILayout.ObjectField(label, cache ? cache : current as Object, typeof(Object), false);
            if (obj != cache)
            {
                cache = obj;
                var sprite = ExtractSprite(obj);
                if (sprite != null)
                {
                    assign(sprite);
                }
            }
            return cache;
        }

        private static Sprite ExtractSprite(Object obj)
        {
            if (obj == null) return null;
            if (obj is Sprite s) return s;
            var path = AssetDatabase.GetAssetPath(obj);
            var reps = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            return reps?.OfType<Sprite>().FirstOrDefault();
        }

        private static SpriteSet CreateOrLoadSpriteSet()
        {
            var dir = "Assets/Resources/Art";
            if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets/Resources", "Art");
            var existing = AssetDatabase.FindAssets("t:SpriteSet", new[] { dir }).Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault();
            if (!string.IsNullOrEmpty(existing)) return AssetDatabase.LoadAssetAtPath<SpriteSet>(existing);
            var set = ScriptableObject.CreateInstance<SpriteSet>();
            var path = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(dir, "SpriteSet_TinySwords.asset"));
            AssetDatabase.CreateAsset(set, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return set;
        }
    }
}


