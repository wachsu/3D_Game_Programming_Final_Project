namespace Game.Core
{
    /// <summary>
    /// 提供 HUD 顯示用的遊戲狀態。GameManager 實作；UI 示範場景也有自己的實作，
    /// 讓 UI 可以不依賴 GameManager 單獨開發與 demo。
    /// </summary>
    public interface IGameStatus
    {
        int CurrentLevel { get; }       // 1-3
        int CurrentFloor { get; }       // -1 / 3 / 7
        float TimeLeft { get; }         // seconds
        float TimeLimit { get; }        // seconds, <= 0 means no limit
        bool IsPlaying { get; }         // false while loading / cleared / won / lost
    }
}
