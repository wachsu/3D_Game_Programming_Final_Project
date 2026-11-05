using UnityEngine;

namespace Game.Core
{
    /// <summary>Implemented by the NPC & Player owner.</summary>
    public interface IRescueTarget
    {
        string Id { get; }
        string DisplayName { get; }
        NpcType Type { get; }
        RescueState State { get; }
        Transform Transform { get; }
    }
}
