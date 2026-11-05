using System.Collections.Generic;
using UnityEngine;

namespace Game.Characters.Pathfinding
{
    // 測試工具：在 Scene 視窗畫出兩個點之間的 A* 路徑
    // 起點或終點移動時才重新計算
    public class PathTester : MonoBehaviour
    {
        [SerializeField] private AStarGrid _grid;
        [SerializeField] private Transform _start;
        [SerializeField] private Transform _goal;
        [SerializeField] private PathMarkers _markers;   // 可以不拖，不拖就不放小球

        private List<Vector3> _path = new List<Vector3>();

        // 上一次計算時的位置，用來判斷有沒有移動
        private Vector3 _lastStart = Vector3.positiveInfinity;
        private Vector3 _lastGoal = Vector3.positiveInfinity;

        private void Update()
        {
            // 還沒在 Inspector 拉好參考，就先不做事
            if (_grid == null || _start == null || _goal == null)
                return;

            // 起點和終點都沒動，就不用重算
            if (_start.position == _lastStart && _goal.position == _lastGoal)
                return;

            _lastStart = _start.position;
            _lastGoal = _goal.position;

            GridNode goalNode = AStarPathfinder.FindPath(_grid, _lastStart, _lastGoal);
            _path = AStarPathfinder.BuildPath(goalNode);
            if (_markers != null)
                _markers.Show(_path);

            Debug.Log(goalNode == null
                ? "[PathTester] 找不到路"
                : $"[PathTester] 路徑長 {goalNode.G} m，共 {_path.Count} 個點，第一個點 {_path[0]}");
        }

        // 在 Scene 視窗畫出路徑：青色線 + 每格一顆黃色小球
        private void OnDrawGizmos()
        {
            Vector3 lift = Vector3.up * 0.1f;   // 往上抬一點，才不會被地板蓋住

            Gizmos.color = Color.cyan;
            for (int i = 0; i < _path.Count - 1; i++)
                Gizmos.DrawLine(_path[i] + lift, _path[i+1] + lift);

            Gizmos.color = Color.yellow;
            foreach (Vector3 point in _path)
                Gizmos.DrawSphere(point + lift, 0.08f);
        }
    }
}