using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Function-selection main menu: Start (with intro story), Continue, How to Play, Settings, Credits, Quit.
    /// Panels cross-fade; every panel remembers which button should be focused for keyboard / gamepad navigation.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [System.Serializable]
        public class Panel
        {
            public CanvasGroup Group;
            public Selectable FirstSelected;
        }

        [Header("Panels")]
        [SerializeField] private Panel _main;
        [SerializeField] private Panel _howToPlay;
        [SerializeField] private Panel _settings;
        [SerializeField] private Panel _credits;
        [SerializeField] private Panel _story;

        [Header("Main buttons")]
        [SerializeField] private Button _continueButton;

        [Header("Story")]
        [SerializeField] private TypewriterText _storyText;
        [SerializeField] private TMP_Text _storyHint;
        [SerializeField, TextArea(4, 10)] private string _storyBody =
            "A food-safety scare has just torn through the B1 food court.\n\n" +
            "You were lucky - you skipped the buffet. Now people are rushing for the restrooms, " +
            "and nobody knows the way.\n\n" +
            "Someone hands you the floor map. Guide everyone to the restroom before time runs out.";

        [Header("Game")]
        [SerializeField] private string _gameSceneName = "Main";
        [SerializeField] private float _fadeTime = 0.25f;
        [SerializeField] private TMP_Text _toast;

        private Panel _current;
        private Coroutine _fadeRoutine;
        private Coroutine _toastRoutine;
        private bool _loading;

        private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        private void Awake()
        {
            GameSettings.Apply();
            foreach (var p in AllPanels()) SetVisible(p, false, true);
            if (_toast != null) _toast.alpha = 0f;
        }

        private void Start()
        {
            _continueButton.interactable = File.Exists(SavePath);
            _current = _main;
            SetVisible(_main, true, true);
            Focus(_main);
        }

        private void Update()
        {
            var kb = Keyboard.current;
            var mouse = Mouse.current;
            if (kb == null || _loading) return;

            if (_current == _story)
            {
                if (!_storyText.IsTyping) _storyHint.text = "Press SPACE to begin";

                bool advance = kb.spaceKey.wasPressedThisFrame || kb.eKey.wasPressedThisFrame ||
                               (mouse != null && mouse.leftButton.wasPressedThisFrame);
                if (advance)
                    AdvanceStory();
                else if (kb.escapeKey.wasPressedThisFrame)
                    Back();
            }
            else if (_current != _main && kb.escapeKey.wasPressedThisFrame)
            {
                Back();
            }
        }

        // ---- Button callbacks (wired by the scene builder) ----

        public void OnStartClicked()
        {
            Click();
            Show(_story);
            _storyHint.text = "Press SPACE to skip";
            _storyText.Play(_storyBody);
        }

        public void OnContinueClicked()
        {
            Click();
            if (!File.Exists(SavePath))
            {
                ShowToast("No save file found.");
                return;
            }
            LoadGame();
        }

        public void OnHowToPlayClicked() { Click(); Show(_howToPlay); }
        public void OnSettingsClicked() { Click(); Show(_settings); }
        public void OnCreditsClicked() { Click(); Show(_credits); }

        public void OnBackClicked() => Back();

        public void OnQuitClicked()
        {
            Click();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ---- Internals ----

        private void AdvanceStory()
        {
            if (_storyText.IsTyping)
            {
                _storyText.Skip();
                return;
            }
            Click();
            LoadGame();
        }

        private void LoadGame()
        {
            if (!Application.CanStreamedLevelBeLoaded(_gameSceneName))
            {
                ShowToast($"Scene \"{_gameSceneName}\" is not in Build Settings yet.");
                Show(_main);
                return;
            }
            _loading = true;
            SceneManager.LoadSceneAsync(_gameSceneName);
        }

        private void Back()
        {
            if (_current == _main) return;
            if (MenuSfx.Instance != null) MenuSfx.Instance.PlayBack();
            Show(_main);
        }

        private void Click()
        {
            if (MenuSfx.Instance != null) MenuSfx.Instance.PlayClick();
        }

        private void Show(Panel target)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);

            // An interrupted fade can leave panels half-visible; only the outgoing and incoming ones may stay.
            foreach (var p in AllPanels())
                if (p != target && p != _current) SetVisible(p, false, true);

            _fadeRoutine = StartCoroutine(Switch(_current, target));
            _current = target;
        }

        private IEnumerator Switch(Panel from, Panel to)
        {
            if (from != null) SetInteractable(from, false);
            SetInteractable(to, false);
            to.Group.gameObject.SetActive(true);

            for (float t = 0f; t < _fadeTime; t += Time.unscaledDeltaTime)
            {
                float k = t / _fadeTime;
                if (from != null) from.Group.alpha = 1f - k;
                to.Group.alpha = k;
                yield return null;
            }

            if (from != null) SetVisible(from, false, true);
            SetVisible(to, true, true);
            Focus(to);
        }

        private static void SetVisible(Panel p, bool visible, bool immediate)
        {
            p.Group.alpha = visible ? 1f : 0f;
            p.Group.interactable = visible;
            p.Group.blocksRaycasts = visible;
            p.Group.gameObject.SetActive(visible);
        }

        private static void SetInteractable(Panel p, bool value)
        {
            p.Group.interactable = value;
            p.Group.blocksRaycasts = value;
        }

        private static void Focus(Panel p)
        {
            if (EventSystem.current == null || p.FirstSelected == null) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(p.FirstSelected.gameObject);
        }

        private Panel[] AllPanels() => new[] { _main, _howToPlay, _settings, _credits, _story };

        private void ShowToast(string message)
        {
            if (_toast == null) return;
            if (_toastRoutine != null) StopCoroutine(_toastRoutine);
            _toastRoutine = StartCoroutine(ToastRoutine(message));
        }

        private IEnumerator ToastRoutine(string message)
        {
            _toast.text = message;
            _toast.alpha = 1f;
            yield return new WaitForSecondsRealtime(2.2f);
            for (float t = 0f; t < 0.5f; t += Time.unscaledDeltaTime)
            {
                _toast.alpha = 1f - t / 0.5f;
                yield return null;
            }
            _toast.alpha = 0f;
        }
    }
}
