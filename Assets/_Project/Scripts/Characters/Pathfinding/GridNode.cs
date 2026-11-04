using UnityEngine;

namespace Game.Characters.Pathfinding
{
    // A* path finding (mini project 4.B.1): 把地板切成 0.5 m 的格子，標出哪些格子能走
    public class GridNode
    {
        public int X;
        public int Z;
        public Vector3 WorldPosition;   // 這格中心點的世界座標
        public bool Walkable;        // 能不能走

        // ---- A* 搜尋用的資料 (f(n) = g(n) + h(n)) ----
        public float G;            // g(n)：從起點走到這格的實際成本
        public float H;            // h(n)：從這格到終點的估計成本
        public GridNode Parent;    // 從哪一格走過來的，找到終點後用來倒推路徑
        public float F => G + H;   // f(n)：自動算出 g + h

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