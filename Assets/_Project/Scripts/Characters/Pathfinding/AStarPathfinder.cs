using System.Collections.Generic;
using UnityEngine;

namespace Game.Characters.Pathfinding
{
    // A* 搜尋 (mini project 4.B.1)
    public static class AStarPathfinder
    {
        // h(n)：Manhattan distance (d = |p1.x – p2.x| + |p1.y – p2.y|)
        public static float Heuristic(GridNode a, GridNode b, float nodeSize)
        {
            // 計算a,b兩格距離 (Manhattan distance)再乘上格子大小，得到實際距離
            int dx = Mathf.Abs(a.X - b.X);
            int dz = Mathf.Abs(a.Z - b.Z);
            return (dx + dz) * nodeSize;
        }
        
        // A* 主迴圈：找到終點就回傳終點那格，找不到就回傳 null
        // 路徑藏在終點一路往回的 Parent 裡，D4 再把它整理成清單
        public static GridNode FindPath(AStarGrid grid, Vector3 startPosition, Vector3 goalPosition)
        {
            GridNode start = grid.NodeFromWorld(startPosition);
            GridNode goal = grid.NodeFromWorld(goalPosition);

            List<GridNode> open = new List<GridNode>();     // OPEN：鄰居還沒展開的格子
            List<GridNode> closed = new List<GridNode>();   // CLOSED：鄰居已經展開的格子

            // Add START to OPEN list / Initialize cost of the START node
            start.G = 0f;                                       // g(n)：從起點走到這格的實際成本
            start.H = Heuristic(start, goal, grid.NodeSize);    // h(n)：從這格到終點的估計成本
            start.Parent = null;                                // 起點沒有前面一格
            open.Add(start);

            // while OPEN not empty
            while (open.Count > 0)
            {
                // get node n from OPEN that has the lowest f(n)=g(n)+h(n)
                GridNode n = open[0];                   // 先假設第一個格子是 f(n) 最小的
                foreach (GridNode candidate in open)    // 再去比出 f(n) 最小的格子
                {
                    if (candidate.F < n.F)
                        n = candidate;
                }

                // if n is GOAL then return path
                if (n == goal)
                    return n;

                // move n to CLOSED
                open.Remove(n);
                closed.Add(n);

                // 檢查每個鄰居，計算新的成本
                foreach (GridNode next in grid.GetNeighbours(n))
                {
                    // g(n') = g(n) + c(n, n')：每走一格的成本 = 格子大小
                    float newG = n.G + grid.NodeSize;
                    // calculate h(n')
                    float newH = Heuristic(next, goal, grid.NodeSize);
                    float newF = newG + newH;

                    // if n' in OPEN and !f(n'_new) better, continue
                    if (open.Contains(next) && newF >= next.F)
                        continue;
                    // if n' in CLOSED and !f(n'_new) better, continue
                    if (closed.Contains(next) && newF >= next.F)
                        continue;

                    // remove any n' from OPEN and CLOSED
                    open.Remove(next);
                    closed.Remove(next);

                    // add n as n' 's parent，並記下新的 g、h
                    next.G = newG;
                    next.H = newH;
                    next.Parent = n;

                    // add n' to OPEN
                    open.Add(next);
                }
            }

            // if we get to here, then there is No Solution
            return null;
        }

        // 從終點沿著 Parent 倒推回起點，整理成「起點 → 終點」的座標清單（世界座標）
        // goal 是 null (找不到路) 時，回傳空的清單
        public static List<Vector3> BuildPath(GridNode goal)
        {
            List<Vector3> path = new List<Vector3>();

            GridNode node = goal;
            while (node != null)
            {
                path.Add(node.WorldPosition);
                node = node.Parent;
            }

            // 上面是從終點往回收集的，順序是「終點 → 起點」，要倒過來
            path.Reverse();
            return path;
        }
    }
}