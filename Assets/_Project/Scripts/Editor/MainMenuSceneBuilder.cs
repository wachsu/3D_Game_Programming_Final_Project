using System.Collections.Generic;
using System.IO;
using Game.UI;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.EditorTools
{
    /// <summary>Builds the Mini Project 4.E.2 main-menu sandbox scene in one click.</summary>
    public static class MainMenuSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/Sandbox/MiniProjects_E/E_MainMenu_Demo.unity";
        private const string FoodPrefabFolder = "Assets/_Project/Prefabs/Food";

        private static readonly Color Ink = new Color(0.06f, 0.09f, 0.13f, 1f);
        private static readonly Color Card = new Color(0.07f, 0.10f, 0.15f, 0.96f);
        private static readonly Color Accent = new Color(1f, 0.62f, 0.20f, 1f);
        private static readonly Color ButtonIdle = new Color(0.14f, 0.20f, 0.28f, 0.95f);
        private static readonly Color TextLight = new Color(0.95f, 0.96f, 0.98f, 1f);
        private static readonly Color TextDim = new Color(0.70f, 0.75f, 0.82f, 1f);

        private static Sprite _box;
        private static Sprite _knob;

        [MenuItem("Game/Build Main Menu Scene")]
        public static void Build()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            _box = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            _knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

            Transform cameraRig = BuildCameraAndLight();
            BuildBackdrop(cameraRig);

            var sfx = new GameObject("MenuSfx");
            sfx.AddComponent<AudioSource>();
            sfx.AddComponent<MenuSfx>();

            var canvas = BuildCanvas();
            BuildEventSystem();
            BuildUi(canvas.transform);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddToBuildSettings(ScenePath);
            Debug.Log("Main menu scene built: " + ScenePath);
        }

        // ---------- 3D backdrop ----------

        private static Transform BuildCameraAndLight()
        {
            var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Ink;
            cam.fieldOfView = 55f;
            camGo.AddComponent<AudioListener>();
            camGo.AddComponent<UniversalAdditionalCameraData>();
            camGo.transform.SetPositionAndRotation(new Vector3(0f, 0f, -6f), Quaternion.identity);

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.95f, 0.85f);
            light.intensity = 1.4f;
            lightGo.AddComponent<UniversalAdditionalLightData>();
            lightGo.transform.rotation = Quaternion.Euler(40f, -30f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.35f, 0.38f, 0.45f);
            return camGo.transform;
        }

        private static void BuildBackdrop(Transform cameraRig)
        {
            var root = new GameObject("MenuBackdrop");
            var backdrop = root.AddComponent<MenuBackdrop>();

            var prefabs = new List<GameObject>();
            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { FoodPrefabFolder }))
                prefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)));

            var so = new SerializedObject(backdrop);
            so.FindProperty("_cameraRig").objectReferenceValue = cameraRig;
            var arr = so.FindProperty("_prefabs");
            arr.arraySize = prefabs.Count;
            for (int i = 0; i < prefabs.Count; i++)
                arr.GetArrayElementAtIndex(i).objectReferenceValue = prefabs[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // ---------- Canvas plumbing ----------

        private static Canvas BuildCanvas()
        {
            var go = new GameObject("MenuCanvas", typeof(RectTransform));
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void BuildEventSystem()
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            var module = go.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }

        // ---------- UI layout ----------

        private static void BuildUi(Transform canvas)
        {
            var controllerGo = new GameObject("MainMenuController");
            var controller = controllerGo.AddComponent<MainMenuController>();

            // Dim vignette over the 3D backdrop so text stays readable.
            var dim = NewImage("Vignette", canvas, new Color(0f, 0f, 0f, 0.35f));
            Stretch(dim.rectTransform);
            dim.raycastTarget = false;

            // --- Main panel ---
            var mainPanel = NewPanel("Panel_Main", canvas);
            var title = NewText("Title", mainPanel.transform, "DUTY CALLS", 120, FontStyles.Bold, Accent, TextAlignmentOptions.Center);
            Place(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -150f), new Vector2(1400f, 150f));
            var sub = NewText("Subtitle", mainPanel.transform, "R E S T R O O M   R E S C U E", 44, FontStyles.Normal, TextLight, TextAlignmentOptions.Center);
            Place(sub.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -250f), new Vector2(1400f, 70f));

            var column = new GameObject("Buttons", typeof(RectTransform));
            column.transform.SetParent(mainPanel.transform, false);
            Place((RectTransform)column.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -110f), new Vector2(560f, 640f));
            var layout = column.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 16f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var start = NewButton("Btn_Start", column.transform, "START GAME", true);
            var cont = NewButton("Btn_Continue", column.transform, "CONTINUE", false);
            var how = NewButton("Btn_HowToPlay", column.transform, "HOW TO PLAY", false);
            var settings = NewButton("Btn_Settings", column.transform, "SETTINGS", false);
            var credits = NewButton("Btn_Credits", column.transform, "CREDITS", false);
            var quit = NewButton("Btn_Quit", column.transform, "QUIT", false);

            var footer = NewText("Footer", mainPanel.transform, "Mini Project 4.E.2  -  Function Selection", 24, FontStyles.Normal, TextDim, TextAlignmentOptions.Center);
            Place(footer.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(1200f, 40f));

            // --- How to play ---
            var howPanel = NewPanel("Panel_HowToPlay", canvas);
            var howCard = NewCard("Card", howPanel.transform, new Vector2(1100f, 760f));
            AddHeading(howCard, "HOW TO PLAY");
            var controls = NewText("Controls", howCard, ControlsText(), 36, FontStyles.Normal, TextLight, TextAlignmentOptions.TopLeft);
            Place(controls.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(900f, 460f));
            controls.richText = true;
            controls.lineSpacing = 12f;
            var howBack = AddBackButton(howCard);

            // --- Settings ---
            var setPanel = NewPanel("Panel_Settings", canvas);
            var setCard = NewCard("Card", setPanel.transform, new Vector2(1100f, 760f));
            AddHeading(setCard, "SETTINGS");
            var volumeLabel = NewText("VolumeLabel", setCard, "Master Volume", 36, FontStyles.Normal, TextLight, TextAlignmentOptions.MidlineLeft);
            Place(volumeLabel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(-250f, 130f), new Vector2(380f, 60f));
            var slider = NewSlider("VolumeSlider", setCard, new Vector2(230f, 130f), new Vector2(460f, 40f));
            var mute = NewToggle("MuteToggle", setCard, "Mute", new Vector2(-180f, 30f));
            var fullscreen = NewToggle("FullscreenToggle", setCard, "Fullscreen", new Vector2(-180f, -60f));
            var setBack = AddBackButton(setCard);

            var settingsPanel = setPanel.gameObject.AddComponent<SettingsPanel>();
            var sps = new SerializedObject(settingsPanel);
            sps.FindProperty("_volumeSlider").objectReferenceValue = slider;
            sps.FindProperty("_muteToggle").objectReferenceValue = mute;
            sps.FindProperty("_fullscreenToggle").objectReferenceValue = fullscreen;
            sps.ApplyModifiedPropertiesWithoutUndo();

            // --- Credits ---
            var credPanel = NewPanel("Panel_Credits", canvas);
            var credCard = NewCard("Card", credPanel.transform, new Vector2(1100f, 760f));
            AddHeading(credCard, "CREDITS");
            var credText = NewText("Body", credCard, CreditsText(), 34, FontStyles.Normal, TextLight, TextAlignmentOptions.Top);
            Place(credText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(900f, 460f));
            credText.richText = true;
            credText.lineSpacing = 10f;
            var credBack = AddBackButton(credCard);

            // --- Story ---
            var storyPanel = NewPanel("Panel_Story", canvas);
            var storyDim = NewImage("Dim", storyPanel.transform, new Color(0f, 0f, 0f, 0.82f));
            Stretch(storyDim.rectTransform);
            storyDim.raycastTarget = false;
            var storyLabel = NewText("StoryText", storyPanel.transform, "", 44, FontStyles.Normal, TextLight, TextAlignmentOptions.Center);
            Place(storyLabel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(1300f, 600f));
            storyLabel.lineSpacing = 14f;
            var typewriter = storyLabel.gameObject.AddComponent<TypewriterText>();
            var hint = NewText("Hint", storyPanel.transform, "Press SPACE to skip", 30, FontStyles.Italic, Accent, TextAlignmentOptions.Center);
            Place(hint.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 70f), new Vector2(800f, 50f));

            // --- Toast ---
            var toast = NewText("Toast", canvas, "", 32, FontStyles.Normal, TextLight, TextAlignmentOptions.Center);
            Place(toast.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 130f), new Vector2(1400f, 60f));
            toast.raycastTarget = false;

            // --- Wire controller ---
            var so = new SerializedObject(controller);
            SetPanel(so, "_main", mainPanel, start);
            SetPanel(so, "_howToPlay", howPanel, howBack);
            SetPanel(so, "_settings", setPanel, slider);
            SetPanel(so, "_credits", credPanel, credBack);
            SetPanel(so, "_story", storyPanel, null);
            so.FindProperty("_continueButton").objectReferenceValue = cont;
            so.FindProperty("_storyText").objectReferenceValue = typewriter;
            so.FindProperty("_storyHint").objectReferenceValue = hint;
            so.FindProperty("_toast").objectReferenceValue = toast;
            so.ApplyModifiedPropertiesWithoutUndo();

            Wire(start, controller.OnStartClicked);
            Wire(cont, controller.OnContinueClicked);
            Wire(how, controller.OnHowToPlayClicked);
            Wire(settings, controller.OnSettingsClicked);
            Wire(credits, controller.OnCreditsClicked);
            Wire(quit, controller.OnQuitClicked);
            Wire(howBack, controller.OnBackClicked);
            Wire(setBack, controller.OnBackClicked);
            Wire(credBack, controller.OnBackClicked);
        }

        private static string ControlsText()
        {
            return "<color=#FFA033><b>W A S D</b></color><pos=240>Move\n" +
                   "<color=#FFA033><b>Mouse</b></color><pos=240>Look around\n" +
                   "<color=#FFA033><b>E</b></color><pos=240>Talk to someone / start guiding\n" +
                   "<color=#FFA033><b>R</b></color><pos=240>Jump (and tell a child to jump)\n" +
                   "<color=#FFA033><b>F</b></color><pos=240>Push a movable obstacle\n\n" +
                   "<color=#B3BFD1>Lead everyone to the restroom before the timer runs out.</color>";
        }

        private static string CreditsText()
        {
            return "<color=#FFA033><b>Scene & Level</b></color>\nFloors, shops, escalators, clues\n\n" +
                   "<color=#FFA033><b>NPC & Player</b></color>\nMovement, escort AI, restroom trigger\n\n" +
                   "<color=#FFA033><b>Game Logic & UI</b></color>\nStory, rules, timer, menus, audio\n\n" +
                   "<color=#B3BFD1>Low-poly art and sounds: see the project References.</color>";
        }

        // ---------- UI factory helpers ----------

        private static void SetPanel(SerializedObject so, string field, CanvasGroup group, Selectable first)
        {
            var p = so.FindProperty(field);
            p.FindPropertyRelative("Group").objectReferenceValue = group;
            p.FindPropertyRelative("FirstSelected").objectReferenceValue = first;
        }

        private static void Wire(Button button, UnityAction action)
        {
            UnityEventTools.AddPersistentListener(button.onClick, action);
        }

        private static CanvasGroup NewPanel(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            Stretch((RectTransform)go.transform);
            return go.AddComponent<CanvasGroup>();
        }

        private static Transform NewCard(string name, Transform parent, Vector2 size)
        {
            var img = NewImage(name, parent, Card);
            img.sprite = _box;
            img.type = Image.Type.Sliced;
            Place(img.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, size);
            return img.transform;
        }

        private static void AddHeading(Transform card, string text)
        {
            var t = NewText("Heading", card, text, 64, FontStyles.Bold, Accent, TextAlignmentOptions.Center);
            Place(t.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -70f), new Vector2(900f, 90f));
        }

        private static Button AddBackButton(Transform card)
        {
            var b = NewButton("Btn_Back", card, "BACK", false);
            Place((RectTransform)b.transform, new Vector2(0.5f, 0f), new Vector2(0f, 80f), new Vector2(360f, 76f));
            return b;
        }

        private static Image NewImage(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            return img;
        }

        private static TextMeshProUGUI NewText(string name, Transform parent, string text, float size,
            FontStyles style, Color color, TextAlignmentOptions align)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = size;
            t.fontStyle = style;
            t.color = color;
            t.alignment = align;
            t.raycastTarget = false;
            t.textWrappingMode = TextWrappingModes.Normal;
            return t;
        }

        private static Button NewButton(string name, Transform parent, string label, bool primary)
        {
            var img = NewImage(name, parent, Color.white);
            img.sprite = _box;
            img.type = Image.Type.Sliced;
            img.rectTransform.sizeDelta = new Vector2(560f, 76f);

            var element = img.gameObject.AddComponent<LayoutElement>();
            element.preferredHeight = 76f;
            element.minHeight = 76f;

            var button = img.gameObject.AddComponent<Button>();
            button.targetGraphic = img;
            Color idle = primary ? Accent : ButtonIdle;
            var colors = button.colors;
            colors.normalColor = idle;
            colors.highlightedColor = Color.Lerp(idle, Color.white, 0.25f);
            colors.selectedColor = Color.Lerp(idle, Color.white, 0.25f);
            colors.pressedColor = Color.Lerp(idle, Color.black, 0.25f);
            colors.disabledColor = new Color(idle.r, idle.g, idle.b, 0.35f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            img.gameObject.AddComponent<MenuButtonFx>();

            var text = NewText("Label", img.transform, label, 38, FontStyles.Bold,
                primary ? Ink : TextLight, TextAlignmentOptions.Center);
            Stretch(text.rectTransform);
            return button;
        }

        private static Slider NewSlider(string name, Transform parent, Vector2 pos, Vector2 size)
        {
            var root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            Place((RectTransform)root.transform, new Vector2(0.5f, 0.5f), pos, size);

            var bg = NewImage("Background", root.transform, ButtonIdle);
            bg.sprite = _box;
            bg.type = Image.Type.Sliced;
            bg.rectTransform.anchorMin = new Vector2(0f, 0.25f);
            bg.rectTransform.anchorMax = new Vector2(1f, 0.75f);
            bg.rectTransform.offsetMin = bg.rectTransform.offsetMax = Vector2.zero;

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(root.transform, false);
            var fillAreaRt = (RectTransform)fillArea.transform;
            fillAreaRt.anchorMin = new Vector2(0f, 0.25f);
            fillAreaRt.anchorMax = new Vector2(1f, 0.75f);
            fillAreaRt.offsetMin = new Vector2(5f, 0f);
            fillAreaRt.offsetMax = new Vector2(-15f, 0f);

            var fill = NewImage("Fill", fillArea.transform, Accent);
            fill.sprite = _box;
            fill.type = Image.Type.Sliced;
            fill.rectTransform.sizeDelta = new Vector2(10f, 0f);

            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(root.transform, false);
            var handleAreaRt = (RectTransform)handleArea.transform;
            Stretch(handleAreaRt);
            handleAreaRt.offsetMin = new Vector2(10f, 0f);
            handleAreaRt.offsetMax = new Vector2(-10f, 0f);

            var handle = NewImage("Handle", handleArea.transform, TextLight);
            handle.sprite = _knob;
            handle.rectTransform.sizeDelta = new Vector2(36f, 36f);

            var slider = root.AddComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.8f;
            return slider;
        }

        private static Toggle NewToggle(string name, Transform parent, string label, Vector2 pos)
        {
            var root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            Place((RectTransform)root.transform, new Vector2(0.5f, 0.5f), pos, new Vector2(520f, 60f));

            var box = NewImage("Background", root.transform, ButtonIdle);
            box.sprite = _box;
            box.type = Image.Type.Sliced;
            Place(box.rectTransform, new Vector2(0f, 0.5f), new Vector2(30f, 0f), new Vector2(56f, 56f));

            var check = NewImage("Checkmark", box.transform, Accent);
            check.sprite = _box;
            check.type = Image.Type.Sliced;
            Place(check.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(34f, 34f));

            var text = NewText("Label", root.transform, label, 36, FontStyles.Normal, TextLight, TextAlignmentOptions.MidlineLeft);
            Place(text.rectTransform, new Vector2(0f, 0.5f), new Vector2(300f, 0f), new Vector2(420f, 60f));

            var toggle = root.AddComponent<Toggle>();
            toggle.targetGraphic = box;
            toggle.graphic = check;
            toggle.isOn = false;
            root.AddComponent<MenuButtonFx>();
            return toggle;
        }

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        private static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }

        private static void AddToBuildSettings(string path)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            foreach (var s in scenes)
                if (s.path == path) { s.enabled = true; EditorBuildSettings.scenes = scenes.ToArray(); return; }
            scenes.Insert(0, new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
