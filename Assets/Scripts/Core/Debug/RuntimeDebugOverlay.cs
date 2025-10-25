using UnityEngine;
using Game.Core.Debugging;

namespace Game.Core.Debugging
{
    [DisallowMultipleComponent]
    public sealed class RuntimeDebugOverlay : MonoBehaviour
    {
        private GUIStyle _style;

        private void Awake()
        {
            _style = new GUIStyle
            {
                fontSize = 14,
                normal = new GUIStyleState { textColor = Color.white }
            };
        }

        private void OnGUI()
        {
            if (!DebugFlags.Overlay) return;
            var cam = Camera.main;
            var player = GameObject.Find("Player");
            var pc = player ? player.GetComponent<Game.Player.PlayerController2D>() : null;
            var health = player ? player.GetComponent<Game.Combat.Health>() : null;
            GUILayout.BeginArea(new Rect(10, 10, 420, 200));
            GUILayout.Label($"Camera y={cam?.transform.position.y:F2} size={cam?.orthographicSize:F2}", _style);
            if (pc != null)
            {
                GUILayout.Label($"Player vel={pc.GetComponent<Rigidbody2D>()?.linearVelocity}", _style);
                GUILayout.Label($"ShooterMode={pc != null}", _style);
            }
            if (health != null)
            {
                GUILayout.Label($"HP={health.CurrentHealth:F1}/{health.MaxHealth:F1}", _style);
            }
            GUILayout.EndArea();
        }
    }
}


