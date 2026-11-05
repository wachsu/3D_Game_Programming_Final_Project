using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    /// <summary>Reveals text one character at a time; can be skipped.</summary>
    [RequireComponent(typeof(TMP_Text))]
    public class TypewriterText : MonoBehaviour
    {
        [SerializeField] private float _charsPerSecond = 40f;

        private TMP_Text _label;
        private Coroutine _routine;

        public bool IsTyping { get; private set; }

        private void Awake() => _label = GetComponent<TMP_Text>();

        public void Play(string text)
        {
            if (_routine != null) StopCoroutine(_routine);
            _label.text = text;
            _label.maxVisibleCharacters = 0;
            _routine = StartCoroutine(Type());
        }

        public void Skip()
        {
            if (!IsTyping) return;
            if (_routine != null) StopCoroutine(_routine);
            _label.maxVisibleCharacters = int.MaxValue;
            IsTyping = false;
        }

        private IEnumerator Type()
        {
            IsTyping = true;
            _label.ForceMeshUpdate();
            int total = _label.textInfo.characterCount;
            float shown = 0f;
            while (shown < total)
            {
                shown += _charsPerSecond * Time.unscaledDeltaTime;
                _label.maxVisibleCharacters = Mathf.FloorToInt(shown);
                yield return null;
            }
            _label.maxVisibleCharacters = int.MaxValue;
            IsTyping = false;
        }
    }
}
