using System.Collections.Generic;
using Game.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>救援清單：每個 IRescueTarget 一列，顯示名字、類型與目前狀態（Waiting / Following / Wandering / Done）。</summary>
    public class RescueListPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;      // 放列的容器（建議掛 VerticalLayoutGroup）
        [SerializeField] private float _rowHeight = 44f;
        [SerializeField] private float _fontSize = 24f;

        private static readonly Color Idle = new Color(1f, 1f, 1f, 0.9f);
        private static readonly Color Follow = new Color(0.45f, 0.85f, 1f, 1f);
        private static readonly Color Wander = new Color(1f, 0.6f, 0.25f, 1f);
        private static readonly Color Done = new Color(0.45f, 0.95f, 0.5f, 1f);

        private sealed class Row
        {
            public IRescueTarget Target;
            public Image Background;
            public TMP_Text Name;
            public TMP_Text Status;
        }

        private readonly List<Row> _rows = new List<Row>();

        public int Count => _rows.Count;

        /// <summary>目標清單改變時重建所有列。</summary>
        public void SetTargets(IReadOnlyList<IRescueTarget> targets)
        {
            foreach (var row in _rows)
                if (row.Background != null) Destroy(row.Background.gameObject);
            _rows.Clear();

            foreach (var t in targets)
                _rows.Add(CreateRow(t));
            Refresh();
        }

        /// <summary>每個 Update 更新狀態文字與顏色。</summary>
        public void Refresh()
        {
            foreach (var row in _rows)
            {
                if (row.Target == null || row.Name == null) continue;
                var s = row.Target.State;
                row.Status.text = s == RescueState.Arrived ? "Done" : s.ToString();
                row.Status.color = ColorOf(s);
                row.Name.color = s == RescueState.Arrived ? Done : Idle;
                row.Name.fontStyle = s == RescueState.Arrived ? FontStyles.Strikethrough : FontStyles.Normal;
            }
        }

        private Row CreateRow(IRescueTarget t)
        {
            var go = new GameObject("Row_" + t.Id, typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            go.transform.SetParent(_content, false);
            var bg = go.GetComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.35f);
            bg.raycastTarget = false;
            go.GetComponent<LayoutElement>().preferredHeight = _rowHeight;

            var name = CreateText(go.transform, "Name", TextAlignmentOptions.MidlineLeft, 0f, 0.6f);
            name.text = $"{t.DisplayName}  <size=70%><color=#AAB4C0>{t.Type}</color></size>";
            var status = CreateText(go.transform, "Status", TextAlignmentOptions.MidlineRight, 0.6f, 1f);

            return new Row { Target = t, Background = bg, Name = name, Status = status };
        }

        private TMP_Text CreateText(Transform parent, string name, TextAlignmentOptions align, float xMin, float xMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = new Vector2(xMin, 0f);
            rt.anchorMax = new Vector2(xMax, 1f);
            rt.offsetMin = new Vector2(10f, 0f);
            rt.offsetMax = new Vector2(-10f, 0f);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.fontSize = _fontSize;
            t.alignment = align;
            t.raycastTarget = false;
            t.enableWordWrapping = false;
            return t;
        }

        private static Color ColorOf(RescueState s)
        {
            switch (s)
            {
                case RescueState.Following: return Follow;
                case RescueState.Wandering: return Wander;
                case RescueState.Arrived: return Done;
                default: return Idle;
            }
        }
    }
}
