namespace Game.Core
{
    /// <summary>Implemented by any component that needs saving; collected by the Save &amp; Load system.</summary>
    public interface ISaveable
    {
        string SaveId { get; }
        string CaptureState();          // JsonUtility.ToJson
        void RestoreState(string json);
    }
}
