using System.Collections.Generic;
using UnityEngine;

namespace Game.Characters.Pathfinding
{
    // 沿著路徑每隔固定距離放一顆小球
    // 小球數量 = 路徑長度 / 間距 + 1，所以路徑越長，小球越多
    public class PathMarkers : MonoBehaviour
    {
        [SerializeField] private GameObject _markerPrefab;   // 小球的 prefab
        [SerializeField] private float _spacing = 1f;        // 每隔幾公尺放一顆
        [SerializeField] private float _height = 0.15f;      // 離地多高

        // 清掉舊的小球，沿著新路徑放新的小球
        public void Show(List<Vector3> path)
        {
            Clear();
            if (path.Count == 0)
                return;   // 找不到路：清掉就好，不放新的

            // ① 路徑總長度 = 每一段的長度加起來
            float length = 0f;
            for (int i = 0; i < path.Count - 1; i++)
                length += Vector3.Distance(path[i], path[i+1]);

            // ② 小球數量依長度決定 (植樹問題：+1 是起點那顆)
            int count = Mathf.FloorToInt(length / _spacing) + 1;

            // ③ 第 k 顆放在「從起點沿路徑走 k * _spacing 公尺」的地方
            for (int k = 0; k < count; k++)
            {
                Vector3 position = PointAtDistance(path, k * _spacing);
                Instantiate(_markerPrefab, position + Vector3.up * _height, Quaternion.identity, transform);
            }
        }

        // 刪掉所有小球 (小球都是這個物件的子物件)
        public void Clear()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }

        // 從路徑起點沿著路徑走 distance 公尺，會停在哪一點
        private static Vector3 PointAtDistance(List<Vector3> path, float distance)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                float segment = Vector3.Distance(path[i], path[i + 1]);   // 這一段多長

                // 剩下的距離在這一段裡面：在這段上按比例找出位置
                if (distance <= segment)
                    return Vector3.Lerp(path[i], path[i + 1], distance / segment);

                // 剩下的距離比這段長：走完這段，扣掉這段的長度，換下一段
                distance -= segment;
            }

            // 超過整條路徑 (計算誤差)：就放在終點
            return path[path.Count - 1];
        }
    }
}