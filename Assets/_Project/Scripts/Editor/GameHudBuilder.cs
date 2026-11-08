using Game.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Game.EditorTools
{
    /// <summary>建立 4.E.3 遊戲內 HUD（計時條、救援清單、小地圖、狀態文字）。Main 與 UI 示範場景共用。</summary>
    public static class GameHudBuilder
    {
        private static readonly Color Panel = new Color(0.05f, 0.08f, 0.12f, 0.75f);

        public static GameHud Build()
        {
            var box = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            var knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

            var canvasGo = new GameObject("HUD Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            var root = canvasGo.transform;

            // ---- 計時條（上方置中）----
            var timerRoot = NewRect("TimerBar", root, new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(760f, 40f));
            var timerBg = timerRoot.gameObject.AddComponent<Image>();
            timerBg.sprite = box; timerBg.type = Image.Type.Sliced; timerBg.color = new Color(0f, 0f, 0f, 0.6f);
            var fillRt = NewRect("Fill", timerRoot, Vector2.zero, Vector2.one);
            fillRt.offsetMin = new Vector2(4f, 4f); fillRt.offsetMax = new Vector2(-4f, -4f);
            var fill = fillRt.gameObject.AddComponent<Image>();
            fill.sprite = box;
            fill.type = Image.Type.Filled; fill.fillMethod = Image.FillMethod.Horizontal; fill.fillOrigin = 0;
            var timeLabel = NewText("TimeLabel", timerRoot, 28, TextAlignmentOptions.Center);
            Stretch(timeLabel.rectTransform);
            var timerBar = timerRoot.gameObject.AddComponent<TimerBar>();
            Set(timerBar, "_fill", fill);
            Set(timerBar, "_label", timeLabel);

            // ---- 關卡文字（計時條下方）----
            var levelText = NewText("LevelText", root, 30, TextAlignmentOptions.Center);
            Anchor(levelText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -88f), new Vector2(600f, 40f));

            // ---- 剩餘人數（左上）----
            var remaining = NewText("RemainingText", root, 34, TextAlignmentOptions.TopLeft);
            Anchor(remaining.rectTransform, new Vector2(0f, 1f), new Vector2(30f, -30f), new Vector2(520f, 50f));

            // ---- 救援清單（右上）----
            var listRoot = NewRect("RescueList", root, new Vector2(1f, 1f), new Vector2(-30f, -30f), new Vector2(420f, 420f));
            var listBg = listRoot.gameObject.AddComponent<Image>();
            listBg.sprite = box; listBg.type = Image.Type.Sliced; listBg.color = Panel;
            var title = NewText("Title", listRoot, 26, TextAlignmentOptions.Midline);
            title.text = "RESCUE LIST";
            title.color = new Color(1f, 0.75f, 0.3f);
            title.rectTransform.anchorMin = new Vector2(0f, 1f); title.rectTransform.anchorMax = new Vector2(1f, 1f);
            title.rectTransform.pivot = new Vector2(0.5f, 1f);
            title.rectTransform.anchoredPosition = Vector2.zero; title.rectTransform.sizeDelta = new Vector2(0f, 44f);
            var content = NewRect("Content", listRoot, Vector2.zero, Vector2.one);
            content.offsetMin = new Vector2(8f, 8f); content.offsetMax = new Vector2(-8f, -52f);
            var vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 6f; vlg.childControlWidth = true; vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;
            var rescuePanel = listRoot.gameObject.AddComponent<RescueListPanel>();
            Set(rescuePanel, "_content", content);

            // ---- 小地圖（左下，圓形）----
            var mapRoot = NewRect("Minimap", root, new Vector2(0f, 0f), new Vector2(40f, 40f), new Vector2(300f, 300f));
            mapRoot.pivot = new Vector2(0f, 0f);
            var ring = mapRoot.gameObject.AddComponent<Image>();
            ring.sprite = knob; ring.color = new Color(1f, 1f, 1f, 0.9f);
            var maskRt = NewRect("Mask", mapRoot, Vector2.zero, Vector2.one);
            maskRt.offsetMin = new Vector2(6f, 6f); maskRt.offsetMax = new Vector2(-6f, -6f);
            var maskImg = maskRt.gameObject.AddComponent<Image>();
            maskImg.sprite = knob; maskImg.color = Color.white;
            maskRt.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            var rawRt = NewRect("Map", maskRt, Vector2.zero, Vector2.one);
            var raw = rawRt.gameObject.AddComponent<RawImage>();
            var markers = NewRect("Markers", maskRt, Vector2.zero, Vector2.one);
            var minimap = mapRoot.gameObject.AddComponent<MinimapView>();
            Set(minimap, "_mapImage", raw);
            Set(minimap, "_markerLayer", markers);
            Set(minimap, "_dotSprite", knob);

            // ---- 中央訊息 ----
            var center = NewText("CenterText", root, 96, TextAlignmentOptions.Center);
            Anchor(center.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1400f, 400f));
            center.text = "";

            var hud = canvasGo.AddComponent<GameHud>();
            Set(hud, "_timerBar", timerBar);
            Set(hud, "_rescueList", rescuePanel);
            Set(hud, "_minimap", minimap);
            Set(hud, "_levelText", levelText);
            Set(hud, "_remainingText", remaining);
            Set(hud, "_centerText", center);

            if (Object.FindFirstObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

            return hud;
        }

        // ---------- helpers ----------

        private static void Set(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static RectTransform NewRect(string name, Transform parent, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            var rt = NewRect(name, parent, anchor, anchor);
            rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            return rt;
        }

        private static RectTransform NewRect(string name, Transform parent, Vector2 min, Vector2 max)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = min; rt.anchorMax = max;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            return rt;
        }

        private static void Anchor(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }

        private static TextMeshProUGUI NewText(string name, Transform parent, float size, TextAlignmentOptions align)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.fontSize = size; t.alignment = align; t.color = Color.white; t.raycastTarget = false;
            t.text = name;
            return t;
        }
    }
}
