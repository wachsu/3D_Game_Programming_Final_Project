using System.Collections.Generic;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 小地圖：正上方正交攝影機畫到 RenderTexture，再顯示在圓形 RawImage 裡；
    /// 玩家固定在中心（北方朝上），玩家箭頭跟著朝向轉，NPC 以彩色圓點標示，
    /// 超出範圍的 NPC 會貼在圓邊緣指出方向。
    /// </summary>
    public class MinimapView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RawImage _mapImage;
        [SerializeField] private RectTransform _markerLayer;     // 和 _mapImage 同大小、同中心
        [SerializeField] private Sprite _dotSprite;

        [Header("Camera")]
        [SerializeField] private float _orthoSize = 18f;          // 半徑（公尺）
        [SerializeField] private float _cameraHeight = 25f;       // 攝影機在玩家上方多高
        [SerializeField] private float _farClip = 32f;            // 越小越不會看到上層樓
        [SerializeField] private int _textureSize = 256;

        [Header("Markers")]
        [SerializeField] private float _npcMarkerSize = 16f;
        [SerializeField] private float _playerMarkerSize = 22f;

        private Camera _camera;
        private RenderTexture _texture;
        private Transform _player;
        private RectTransform _playerMarker;
        private RectTransform _playerFacing;
        private readonly List<IRescueTarget> _targets = new List<IRescueTarget>();
        private readonly List<Image> _markers = new List<Image>();

        private void Awake()
        {
            _texture = new RenderTexture(_textureSize, _textureSize, 16) { name = "MinimapRT" };
            _mapImage.texture = _texture;

            var camGo = new GameObject("MinimapCamera");
            camGo.transform.SetParent(transform, false);
            _camera = camGo.AddComponent<Camera>();
            _camera.orthographic = true;
            _camera.orthographicSize = _orthoSize;
            _camera.farClipPlane = _farClip;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color(0.08f, 0.10f, 0.14f, 1f);
            _camera.targetTexture = _texture;
            _camera.cullingMask = ~(1 << LayerMask.NameToLayer("UI"));
            camGo.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            CreatePlayerMarker();
        }

        private void OnDestroy()
        {
            if (_texture != null) _texture.Release();
        }

        public void SetTargets(IReadOnlyList<IRescueTarget> targets)
        {
            _targets.Clear();
            _targets.AddRange(targets);

            foreach (var m in _markers)
                if (m != null) Destroy(m.gameObject);
            _markers.Clear();

            foreach (var t in _targets)
            {
                var go = new GameObject("Marker_" + t.Id, typeof(RectTransform), typeof(Image));
                go.transform.SetParent(_markerLayer, false);
                var img = go.GetComponent<Image>();
                img.sprite = _dotSprite;
                img.raycastTarget = false;
                img.rectTransform.sizeDelta = Vector2.one * _npcMarkerSize;
                _markers.Add(img);
            }
            _playerMarker.SetAsLastSibling();
        }

        private void LateUpdate()
        {
            if (_player == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) _player = p.transform;
                else return;
            }

            Vector3 pp = _player.position;
            _camera.transform.position = new Vector3(pp.x, pp.y + _cameraHeight, pp.z);

            // 玩家朝向箭頭
            float yaw = _player.eulerAngles.y;
            _playerFacing.localRotation = Quaternion.Euler(0f, 0f, -yaw);

            float radius = _markerLayer.rect.width * 0.5f;
            float pixelsPerMeter = radius / _orthoSize;
            for (int i = 0; i < _targets.Count && i < _markers.Count; i++)
            {
                var t = _targets[i];
                var marker = _markers[i];
                if (t == null || t.Transform == null || marker == null) continue;

                Vector3 d = t.Transform.position - pp;
                Vector2 pos = new Vector2(d.x, d.z) * pixelsPerMeter;
                float maxLen = radius - _npcMarkerSize * 0.5f;
                bool outside = pos.magnitude > maxLen;
                if (outside) pos = pos.normalized * maxLen;

                marker.rectTransform.anchoredPosition = pos;
                Color c = ColorOf(t);
                c.a = outside ? 0.6f : 1f;
                marker.color = c;
                marker.enabled = t.State != RescueState.Arrived;   // 已抵達的不再顯示
            }
        }

        private void CreatePlayerMarker()
        {
            var go = new GameObject("PlayerMarker", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(_markerLayer, false);
            _playerMarker = (RectTransform)go.transform;
            _playerMarker.sizeDelta = Vector2.one * _playerMarkerSize;
            var img = go.GetComponent<Image>();
            img.sprite = _dotSprite;
            img.color = Color.white;
            img.raycastTarget = false;

            // 朝向指示：繞著玩家點旋轉的小點
            var arm = new GameObject("Facing", typeof(RectTransform));
            arm.transform.SetParent(_playerMarker, false);
            _playerFacing = (RectTransform)arm.transform;
            _playerFacing.sizeDelta = _playerMarker.sizeDelta;

            var tip = new GameObject("Tip", typeof(RectTransform), typeof(Image));
            tip.transform.SetParent(_playerFacing, false);
            var tipRt = (RectTransform)tip.transform;
            tipRt.sizeDelta = Vector2.one * (_playerMarkerSize * 0.5f);
            tipRt.anchoredPosition = new Vector2(0f, _playerMarkerSize * 0.75f);
            var tipImg = tip.GetComponent<Image>();
            tipImg.sprite = _dotSprite;
            tipImg.color = new Color(1f, 0.85f, 0.2f);
            tipImg.raycastTarget = false;
        }

        private static Color ColorOf(IRescueTarget t)
        {
            switch (t.Type)
            {
                case NpcType.Child: return new Color(1f, 0.55f, 0.85f);
                case NpcType.Elderly: return new Color(0.7f, 0.65f, 1f);
                default: return new Color(0.45f, 0.85f, 1f);
            }
        }
    }
}
