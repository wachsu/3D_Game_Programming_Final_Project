using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    /// <summary>Scale-up animation and blip sound when a button is hovered or keyboard-selected.</summary>
    public class MenuButtonFx : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private float _hoverScale = 1.08f;
        [SerializeField] private float _speed = 12f;

        private Vector3 _baseScale;
        private float _target = 1f;

        private void Awake() => _baseScale = transform.localScale;

        private void OnEnable()
        {
            transform.localScale = _baseScale;
            _target = 1f;
        }

        private void Update()
        {
            Vector3 goal = _baseScale * _target;
            transform.localScale = Vector3.Lerp(transform.localScale, goal, 1f - Mathf.Exp(-_speed * Time.unscaledDeltaTime));
        }

        public void OnPointerEnter(PointerEventData e) => Highlight(true);
        public void OnPointerExit(PointerEventData e) => Highlight(false);
        public void OnSelect(BaseEventData e) => Highlight(true);
        public void OnDeselect(BaseEventData e) => Highlight(false);

        private void Highlight(bool on)
        {
            _target = on ? _hoverScale : 1f;
            if (on && MenuSfx.Instance != null) MenuSfx.Instance.PlayHover();
        }
    }
}
