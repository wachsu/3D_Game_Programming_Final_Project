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
        // 路徑藏在終點一路往回的 Parent 裡，BuildPath 再把它整理成清單
        public static GridNode FindPath(AStarGrid grid, Vector3 startPosition, Vector3 goalPosition)
        {
            GridNode start = grid.NodeFromWorld(startPosition);
            GridNode goal = grid.NodeFromWorld(goalPosition);

            // 終點本身不能走 (例如點在柱子裡)：一定找不到路，不用搜完整張網格
            if (!goal.Walkable)
                return null;

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

        // Straight path：從目前的點能直接走到的就連直線，中間的點不要
        // 只留下「真的需要轉彎」的點
        public static List<Vector3> StraightenPath(AStarGrid grid, List<Vector3> path)
        {
            if (path.Count <= 2)
                return path;   // 只有起點和終點，已經是直線了

            List<Vector3> result = new List<Vector3>();
            result.Add(path[0]);

            int anchor = 0;   // 目前站在第幾個點 (最後一個留下來的點)
            for (int i = 2; i < path.Count; i++)
            {
                // 從 anchor 直接走到第 i 個點會撞到障礙物
                // → 第 i - 1 個點是必要的轉彎點，留下來，並從那裡繼續
                if (!grid.IsLineWalkable(path[anchor], path[i]))
                {
                    result.Add(path[i-1]);
                    anchor = i - 1;
                }
            }

            result.Add(path[path.Count - 1]);
            return result;
        }

        // Smooth path (投影片第 166 頁)：用 Catmull-Rom spline 把轉角變成圓滑的曲線
        // 曲線碰到障礙物的話，就退回原本的路徑
        public static List<Vector3> SmoothPath(AStarGrid grid, List<Vector3> path, float spacing)
        {
            if (path.Count <= 2)
                return path;   // 只有一段直線，沒有轉角可以平滑

            List<Vector3> result = new List<Vector3>();
            for (int i = 0; i < path.Count - 1; i++)
            {
                // 四個控制點：這一段是 p2 → p3，p1 和 p4 是前、後各一個點
                // 頭尾沒有前一個 / 後一個點時，就用自己
                Vector3 p1 = path[Mathf.Max(i - 1, 0)];
                Vector3 p2 = path[i];
                Vector3 p3 = path[i+1];
                Vector3 p4 = path[Mathf.Min(i + 2, path.Count - 1)];

                // 這一段每隔 spacing 公尺取一個點 (u 從 0 到 1，不含 1，因為 1 就是下一段的開頭)
                int samples = Mathf.Max(1, Mathf.CeilToInt(Vector3.Distance(p2, p3) / spacing));
                for (int k = 0; k < samples; k++)
                {
                    float u = (float)k / samples;
                    result.Add(CatmullRom(p1, p2, p3, p4, u));
                }
            }
            result.Add(path[path.Count - 1]);   // 終點

            // 檢查曲線的每一小段有沒有碰到障礙物
            for (int i = 0; i < result.Count - 1; i++)
            {
                if (!grid.IsLineWalkable(result[i], result[i + 1]))
                    return path;   // 碰到了：不要平滑，用原本的路徑
            }

            return result;
        }

        // 投影片第 166 頁的公式：u = 0 時是 p2，u = 1 時是 p3
        private static Vector3 CatmullRom(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float u)
        {
            float u2 = u * u;
            float u3 = u2 * u;

            return p1 * (-0.5f * u3 + u2 - 0.5f * u)
                 + p2 * (1.5f * u3 - 2.5f * u2 + 1f)
                 + p3 * (-1.5f * u3 + 2f * u2 + 0.5f * u)
                 + p4 * (0.5f * u3 - 0.5f * u2);
        }
    }
}