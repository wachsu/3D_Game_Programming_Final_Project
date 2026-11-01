using UnityEngine;

namespace Game.Core
{
    // 依 MiniProject.md 第 5 節的共用介面。Core 由鍾君逸負責；其正式版本推上 repo 後以該版本為準。

    public enum NpcType { Adult, Child, Elderly }
    public enum RescueState { Waiting, Following, Wandering, Arrived }

    public interface IRescueTarget
    {   // 盧語沁 實作
        string Id { get; }
        string DisplayName { get; }
        NpcType Type { get; }
        RescueState State { get; }
        Transform Transform { get; }
    }

    public interface IClueTrigger
    {   // 徐畹茜 實作
        string ClueId { get; }
        int Floor { get; }
        void Reveal();
    }

    public interface IPlayerMotor
    {   // 盧語沁 實作，徐畹茜、鍾君逸 呼叫
        void SetSpeedMultiplier(float multiplier, float duration);
        void Jump(float height);
        void Teleport(Vector3 pos, Quaternion rot);
        bool IsMoving { get; }
    }

    public interface ISaveable
    {   // 需存檔元件實作，鍾君逸 的 E.1 收集
        string SaveId { get; }
        string CaptureState();          // JsonUtility.ToJson
        void RestoreState(string json);
    }
}
