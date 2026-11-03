using UnityEngine;

namespace Game.Characters.Pathfinding
{
    // 網格上的一格 (mini project 4.B.1)
    public class GridNode
    {
        public int X;
        public int Z;
        public Vector3 WorldPosition;   // 這格中心點的世界座標
        public bool Walkable;        // 能不能走

        // 建構子：建立這一格時，把四個資料一次設定好
        public GridNode(int x, int z, Vector3 worldPosition, bool walkable)
        {
            X = x;
            Z = z;
            WorldPosition = worldPosition;
            Walkable = walkable;
        }
    }
}