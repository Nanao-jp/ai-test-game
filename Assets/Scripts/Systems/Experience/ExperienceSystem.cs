using System;
using UnityEngine;

namespace Game.Systems.Experience
{
    [DisallowMultipleComponent]
    public sealed class ExperienceSystem : MonoBehaviour
    {
        [SerializeField] private int[] _levelThresholds = new[] { 5, 12, 22, 36, 52, 70, 90 };
        [SerializeField] private int _currentLevel = 1;
        [SerializeField] private int _currentExp = 0;

        public event Action<int> OnLevelUp;

        public int CurrentLevel => _currentLevel;
        public int CurrentExp => _currentExp;
        public int ExpToNext => GetThresholdForLevel(_currentLevel);

        public float GetExpRatioToNext()
        {
            var t = ExpToNext;
            return t <= 0 ? 1f : Mathf.Clamp01((float)_currentExp / t);
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0) return;
            _currentExp += amount;
            var guard = 64; // safety
            while (guard-- > 0)
            {
                var toNext = ExpToNext;
                if (toNext <= 0 || _currentExp < toNext) break;
                _currentExp -= toNext;
                _currentLevel++;
                OnLevelUp?.Invoke(_currentLevel);
            }
        }

        private int GetThresholdForLevel(int level)
        {
            var idx = Mathf.Clamp(level - 1, 0, int.MaxValue);
            if (idx < _levelThresholds.Length) return _levelThresholds[idx];
            // beyond predefined, grow linearly
            var last = _levelThresholds[_levelThresholds.Length - 1];
            var extra = (idx - _levelThresholds.Length + 1) * 20;
            return last + extra;
        }
    }
}


