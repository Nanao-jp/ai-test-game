using System;

namespace Game.Systems
{
    public static class GameEvents
    {
        public static Action OnPlayerDied;
        public static Action OnEnemyDied;

        public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
        public static void RaiseEnemyDied() => OnEnemyDied?.Invoke();
    }
}


