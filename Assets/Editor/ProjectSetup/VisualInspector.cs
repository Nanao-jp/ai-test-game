using System.Linq;
using UnityEditor;
using UnityEngine;
using Game.Art;

namespace Editor.ProjectSetup
{
    public static class VisualInspector
    {
        [MenuItem("Tools/Art/Inspect Player Visual")] 
        public static void InspectPlayerVisual()
        {
            // SpriteSet (Resources)
            var set = Resources.Load<SpriteSet>("Art/SpriteSet_TinySwords");

            // Scene Player
            var player = Object.FindFirstObjectByType<Game.Player.PlayerController2D>();
            Transform visualTr = null;
            SpriteRenderer visualSr = null;
            if (player != null)
            {
                visualTr = player.transform.Find("VisualSprite");
                if (visualTr != null) visualSr = visualTr.GetComponent<SpriteRenderer>();
            }

            string PathOf(Object obj)
            {
                if (obj == null) return "(none)";
                var path = AssetDatabase.GetAssetPath(obj);
                return string.IsNullOrEmpty(path) ? obj.name : path;
            }

            string ReportLine(string label, Object obj) => (obj ? "✔ " : "✖ ") + label + ": " + PathOf(obj);

            var msg = string.Join("\n", new[]
            {
                "[SpriteSet (Resources)]",
                ReportLine("playerSprite", set ? set.playerSprite : null),
                ReportLine("enemySprite", set ? set.enemySprite : null),
                ReportLine("enemyAltSprite", set ? set.enemyAltSprite : null),
                ReportLine("bulletSprite", set ? set.bulletSprite : null),
                ReportLine("expSprite", set ? set.expSprite : null),
                ReportLine("backgroundSprite", set ? set.backgroundSprite : null),
                ReportLine("playerPrefab", set ? set.playerPrefab : null),
                "",
                "[Scene Player]",
                player ? "✔ Player found" : "✖ Player not found",
                ReportLine("VisualSprite Sprite", visualSr ? visualSr.sprite : null),
                visualSr ? ($"SortingOrder: {visualSr.sortingOrder}") : "SortingOrder: (n/a)",
            });

            EditorUtility.DisplayDialog("Player Visual Inspect", msg, "OK");
        }

        [MenuItem("Tools/Art/Force Apply Player Sprite Now")] 
        public static void ForceApplyPlayerSprite()
        {
            var set = Resources.Load<SpriteSet>("Art/SpriteSet_TinySwords");
            if (set == null || set.playerSprite == null)
            {
                EditorUtility.DisplayDialog("Player Visual", "SpriteSetやplayerSpriteが未設定です。", "OK");
                return;
            }
            var player = Object.FindFirstObjectByType<Game.Player.PlayerController2D>();
            if (player == null)
            {
                EditorUtility.DisplayDialog("Player Visual", "シーン上にPlayerが見つかりません。", "OK");
                return;
            }
            var visualTr = player.transform.Find("VisualSprite");
            if (visualTr == null)
            {
                var go = new GameObject("VisualSprite");
                go.transform.SetParent(player.transform, false);
                visualTr = go.transform;
            }
            var sr = visualTr.GetComponent<SpriteRenderer>();
            if (sr == null) sr = visualTr.gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = set.playerSprite;
            sr.sortingOrder = 20;
            sr.color = Color.white;
            Selection.activeObject = visualTr.gameObject;
            EditorGUIUtility.PingObject(sr);
            EditorUtility.DisplayDialog("Player Visual", "PlayerのVisualSpriteへplayerSpriteを適用しました。", "OK");
        }
    }
}


