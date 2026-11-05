namespace Game.Core
{
    /// <summary>Implemented by the Scene & Level owner.</summary>
    public interface IClueTrigger
    {
        string ClueId { get; }
        int Floor { get; }
        void Reveal();
    }
}
