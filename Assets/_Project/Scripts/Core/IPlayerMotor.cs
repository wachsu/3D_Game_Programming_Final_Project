using UnityEngine;

namespace Game.Core
{
    /// <summary>Implemented by the NPC & Player owner; called by Level and Core code.</summary>
    public interface IPlayerMotor
    {
        void SetSpeedMultiplier(float multiplier, float duration);
        void Jump(float height);
        void Teleport(Vector3 pos, Quaternion rot);
        bool IsMoving { get; }
    }
}
