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

        private void Awake()
        {
            BuildGrid();
        }

        // 建立網格
        public void BuildGrid()
        {
            // 問題一的公式：總長度/每格大小，再轉成整數
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