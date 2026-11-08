using System.Collections.Generic;
using Game.Core;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// 遊戲內 HUD（mini project 4.E.3）：整合計時條、救援清單、小地圖與中央訊息。
    /// 只透過 IGameStatus / IRescueTarget / GameEvents 取得資料，不依賴其他人的具體類別。
    /// </summary>
    public class GameHud : MonoBehaviour
    {
        [SerializeField] private TimerBar _timerBar;
        [SerializeField] private RescueListPanel _rescueList;
        [SerializeField] private MinimapView _minimap;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _remainingText;
        [SerializeField] private TMP_Text _centerText;
        [SerializeField] private float _scanInterval = 1f;

        private IGameStatus _status;
        private readonly List<IRescueTarget> _targets = new List<IRescueTarget>();
        private float _nextScan;
        private float _centerHideAt = -1f;
        private bool _centerSticky;

        private void OnEnable()
        {
            GameEvents.LevelCleared += OnLevelCleared;
            GameEvents.GameWon += OnGameWon;
            GameEvents.GameLost += OnGameLost;
            GameEvents.RescueCompleted += OnRescueCompleted;
            GameEvents.FloorChanged += OnFloorChanged;
        }

        private void OnDisable()
        {
            GameEvents.LevelCleared -= OnLevelCleared;
            GameEvents.GameWon -= OnGameWon;
            GameEvents.GameLost -= OnGameLost;
            GameEvents.RescueCompleted -= OnRescueCompleted;
            GameEvents.FloorChanged -= OnFloorChanged;
        }

        private void Update()
        {
            if (Time.unscaledTime >= _nextScan)
            {
                _nextScan = Time.unscaledTime + _scanInterval;
                if (_status == null || (_status is Object o && o == null)) _status = FindStatus();
                RescanTargets();
            }

            if (_status != null)
            {
                if (_timerBar != null) _timerBar.SetTime(_status.TimeLeft, _status.TimeLimit);
                if (_levelText != null) _levelText.text = $"Level {_status.CurrentLevel}  ·  {FloorName(_status.CurrentFloor)}";
                if (_centerSticky && _status.IsPlaying) { _centerSticky = false; SetCenter("", 0f); }
            }

            if (_remainingText != null)
            {
                int remaining = 0;
                foreach (var t in _targets)
                    if (t != null && t.State != RescueState.Arrived) remaining++;
                _remainingText.text = $"Remaining: {remaining} / {_targets.Count}";
            }

            if (_rescueList != null) _rescueList.Refresh();

            if (_centerHideAt > 0f && Time.unscaledTime >= _centerHideAt)
            {
                _centerHideAt = -1f;
                if (_centerText != null) _centerText.text = "";
            }
        }

        // ---------- data ----------

        private static IGameStatus FindStatus()
        {
            foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
                if (mb is IGameStatus s) return s;
            return null;
        }

        private void RescanTargets()
        {
            var found = new List<IRescueTarget>();
            foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
                if (mb is IRescueTarget t) found.Add(t);
            found.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));

            if (SameTargets(found)) return;
            _targets.Clear();
            _targets.AddRange(found);
            if (_rescueList != null) _rescueList.SetTargets(_targets);
            if (_minimap != null) _minimap.SetTargets(_targets);
        }

        private bool SameTargets(List<IRescueTarget> found)
        {
            if (found.Count != _targets.Count) return false;
            for (int i = 0; i < found.Count; i++)
                if (!ReferenceEquals(found[i], _targets[i])) return false;
            return true;
        }

        // ---------- messages ----------

        private void OnLevelCleared(int level) => SetCenter($"Level {level} Clear!", 2.5f);
        private void OnGameWon() => SetCenter("YOU WIN!\n<size=50%>Press Enter to restart</size>", -1f);
        private void OnGameLost() => SetCenter("TIME UP\n<size=50%>Press Enter to retry</size>", -1f);

        private void OnRescueCompleted(IRescueTarget t)
        {
            if (t != null) SetCenter($"{t.DisplayName} made it!", 1.5f);
        }

        private void OnFloorChanged(int from, int to) => SetCenter(FloorName(to), 2f);

        private void SetCenter(string msg, float duration)
        {
            if (_centerText == null) return;
            _centerText.text = msg;
            _centerSticky = duration < 0f;
            _centerHideAt = duration > 0f ? Time.unscaledTime + duration : -1f;
        }

        private static string FloorName(int floor) => floor < 0 ? $"B{-floor}" : $"{floor}F";
    }
}
