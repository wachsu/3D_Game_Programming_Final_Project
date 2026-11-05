using System;

namespace Game.Core
{
    public static class GameEvents
    {
        public static event Action<IRescueTarget> RescueStarted, RescueCompleted;
        public static event Action<IClueTrigger> ClueFound;
        public static event Action<int, int> FloorChanged;   // from, to
        public static event Action<int> LevelCleared;        // levelIndex 1-3
        public static event Action GameWon, GameLost;

        public static void RaiseRescueStarted(IRescueTarget t) => RescueStarted?.Invoke(t);
        public static void RaiseRescueCompleted(IRescueTarget t) => RescueCompleted?.Invoke(t);
        public static void RaiseClueFound(IClueTrigger c) => ClueFound?.Invoke(c);
        public static void RaiseFloorChanged(int from, int to) => FloorChanged?.Invoke(from, to);
        public static void RaiseLevelCleared(int level) => LevelCleared?.Invoke(level);
        public static void RaiseGameWon() => GameWon?.Invoke();
        public static void RaiseGameLost() => GameLost?.Invoke();
    }
}
