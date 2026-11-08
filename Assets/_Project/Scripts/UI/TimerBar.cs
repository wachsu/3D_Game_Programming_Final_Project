using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>計時條：填滿比例 = 剩餘時間；顏色由綠漸變到紅，剩 25% 以下開始脈動。</summary>
    public class TimerBar : MonoBehaviour
    {
        [SerializeField] private Image _fill;                 // Image Type = Filled, Horizontal
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Gradient _colors = DefaultGradient();
        [SerializeField, Range(0f, 1f)] private float _warnRatio = 0.25f;
        [SerializeField] private float _pulseSpeed = 8f;

        private RectTransform _rect;
        private Vector3 _baseScale = Vector3.one;

        private void Awake()
        {
            _rect = (RectTransform)transform;
            _baseScale = _rect.localScale;
        }

        /// <param name="limit">≤ 0 表示無限時。</param>
        public void SetTime(float timeLeft, float limit)
        {
            if (limit <= 0f)
            {
                SetRatio(1f);
                if (_label != null) _label.text = "--:--";
                return;
            }

            timeLeft = Mathf.Clamp(timeLeft, 0f, limit);
            SetRatio(timeLeft / limit);
            if (_label != null)
            {
                int total = Mathf.CeilToInt(timeLeft);
                _label.text = $"{total / 60:00}:{total % 60:00}";
            }
        }

        private void SetRatio(float ratio)
        {
            if (_fill != null)
            {
                _fill.fillAmount = ratio;
                _fill.color = _colors.Evaluate(1f - ratio);
            }

            if (_rect == null) return;
            if (ratio <= _warnRatio && ratio > 0f)
            {
                float pulse = 1f + 0.04f * Mathf.Sin(Time.unscaledTime * _pulseSpeed);
                _rect.localScale = _baseScale * pulse;
            }
            else
            {
                _rect.localScale = _baseScale;
            }
        }

        private static Gradient DefaultGradient()
        {
            var g = new Gradient();
            g.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0.30f, 0.85f, 0.40f), 0f),
                    new GradientColorKey(new Color(0.98f, 0.80f, 0.20f), 0.55f),
                    new GradientColorKey(new Color(0.92f, 0.22f, 0.20f), 1f),
                },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            return g;
        }
    }
}
