using System.Collections.Generic;
using UnityEngine;

namespace Game.Characters.Pathfinding
{
    // A* path finding (mini project 4.B.1): 把地板切成 0.5 m 的格子，標出哪些格子能走
    public class AStarGrid : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private Vector2 _worldSize = new Vector2(40f, 30f);   // X * Z (公尺)
        [SerializeField] private float _nodeSize = 0.5f;

        [Header("Obstacle Check")]
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private float _agentRadius = 0.3f;

        private GridNode[,] _nodes;
        private int _countX, _countZ;
        public float NodeSize => _nodeSize;   // 讓其他腳本讀得到格子大小 (只能讀，不能改)

        private void Awake()
        {
            BuildGrid();
        }

        // 建立網格
        public void BuildGrid()
        {
            // 格數 = 總長度 / 每格大小，再轉成整數
            _countX = Mathf.RoundToInt(_worldSize.x / _nodeSize);
            _countZ = Mathf.RoundToInt(_worldSize.y / _nodeSize);

            // 建立一個 _countX * _countZ 的空陣列
            _nodes = new GridNode[_countX, _countZ];

            // 地板左下角的世界座標
            Vector3 bottomLeft = transform.position - new Vector3(_worldSize.x / 2f, 0f, _worldSize.y / 2f);

            int blockedCount = 0;   // 統計有幾格不能走，用來檢查結果
            for (int x = 0; x < _countX; x++)
            {
                for (int z = 0; z < _countZ; z++)
                {
                    // 這格中心點 = 左下角 + 編號 * 格子大小 + 半格
                    Vector3 center = bottomLeft + new Vector3(x * _nodeSize + _nodeSize / 2f, 0f, z * _nodeSize + _nodeSize / 2f);

                    // 計算角色形狀膠囊的大小 (半徑 _agentRadius、高 1.7 m) 會不會碰到障礙物，會碰到: blocked = true
                    Vector3 bottom = center + Vector3.up * _agentRadius;
                    Vector3 top = center + Vector3.up * (1.7f - _agentRadius);
                    bool blocked = Physics.CheckCapsule(bottom, top, _agentRadius, _obstacleMask);

                    bool walkable = !blocked;
                    _nodes[x, z] = new GridNode(x, z, center, walkable);
                    if (!walkable)
                        blockedCount++;
                }
            }

            Debug.Log($"[AStarGrid] 網格大小 {_countX} x {_countZ}，不能走的格子 {blockedCount} 格");
        }

        // 世界座標 -> 所在的格子
        public GridNode NodeFromWorld(Vector3 worldPosition)
        {
            // ① 地板左下角的世界座標 (和 BuildGrid 裡算法一樣)
            Vector3 bottomLeft = transform.position - new Vector3(_worldSize.x / 2f, 0f, _worldSize.y / 2f);

            // ② 這個點離左下角多遠（世界座標）
            Vector3 local = worldPosition - bottomLeft;

            // ③ 世界座標轉成所在格子（除以格子大小，再無條件捨去）
            int x = Mathf.FloorToInt(local.x / _nodeSize);
            int z = Mathf.FloorToInt(local.z / _nodeSize);

            // ④ 限制在 0 ~ 格數-1，點在地板外時才不會超出陣列
            x = Mathf.Clamp(x, 0, _countX - 1);
            z = Mathf.Clamp(z, 0, _countZ - 1);

            return _nodes[x, z];
        }

        // 上下左右四個鄰居中，能走的那些 (CanMove(n, direction))
        public List<GridNode> GetNeighbours(GridNode node)
        {
            List<GridNode> result = new List<GridNode>();

            // 四個方向的位移：右、左、前 (+Z)、後 (-Z)
            int[] dx = { 1, -1, 0, 0 };
            int[] dz = { 0, 0, 1, -1 };

            for (int i = 0; i < 4; i++)
            {
                // 鄰居的格子編號 = 自己的編號 + 位移
                int nx = node.X + dx[i];
                int nz = node.Z + dz[i];

                // 超出網格範圍就跳過
                if (nx < 0 || nx >= _countX || nz < 0 || nz >= _countZ)
                    continue;

                // 不能走 (紅格子) 也跳過
                GridNode neighbour = _nodes[nx, nz];
                if (!neighbour.Walkable)
                    continue;

                result.Add(neighbour);
            }

            return result;
        }

        // 兩點之間的直線有沒有穿過不能走的格子 (Connect nodes if no obstacles between them)
        // 沿著直線每隔 1/4 格取一個點，檢查它所在的格子
        public bool IsLineWalkable(Vector3 from, Vector3 to)
        {
            float distance = Vector3.Distance(from, to);
            float step = _nodeSize / 4f;
            int samples = Mathf.Max(1, Mathf.CeilToInt(distance / step));

            for (int i = 0; i <= samples; i++)
            {
                float t = (float)i / samples;
                Vector3 point = Vector3.Lerp(from, to, t);
                if (!NodeFromWorld(point).Walkable)
                    return false;   // 碰到紅格子，這條直線不能走
            }

            return true;   // 每個點都在能走的格子上
        }

        // 在 Scene 視窗畫出網格 (只在 Editor 裡看得到，遊戲裡不會顯示)
        private void OnDrawGizmos()
        {
            // 1. 白色線框：整個網格的範圍，沒按 Play 也看得到
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(_worldSize.x, 0f, _worldSize.y));

            // 2. 還沒建網格 (沒按 Play) 就先不畫格子，避免 NullReferenceException
            if (_nodes == null)
                return;

            // 3. 不能走的格子畫成半透明紅色方塊
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Vector3 cubeSize = new(_nodeSize * 0.9f, 0.05f, _nodeSize * 0.9f);
            foreach (GridNode node in _nodes)
            {
                if (node.Walkable)
                    continue;   // 能走的格子不畫，跳到下一格

                Gizmos.DrawCube(node.WorldPosition + Vector3.up * 0.03f, cubeSize);
            }
        }
    }
}