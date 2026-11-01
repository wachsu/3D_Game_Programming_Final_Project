# Mini Project 4.E.2 — Function Selection (Main Menu)

## 1. Overview

The main menu of *Duty Calls: Restroom Rescue*. The player picks a function (Start, Continue, How to Play, Settings, Credits, Quit) with the mouse or the keyboard, in front of an animated 3D backdrop of floating food items.

## 2. Scene

**Scene:** `E_MainMenu_Demo.unity` (`Assets/_Project/Scenes/Sandbox/MiniProjects_E/`)

The scene can be regenerated from the Unity menu **Game > Build Main Menu Scene** (`MainMenuSceneBuilder`).

## 3. How to Run

1. Open `E_MainMenu_Demo.unity`.
2. Press **Play**.
3. Use the mouse, or **Up / Down + Enter** (arrow keys also work), to pick a function. **Esc** goes back.

## 4. Implemented Features

- **Function selection** with mouse and keyboard/gamepad navigation; the first button of each panel is focused automatically.
- **Panels with cross-fade:** Main, How to Play, Settings, Credits, Intro Story.
- **Intro story** shown with a typewriter effect; Space / E / click skips the typing, then starts the game.
- **Continue** is enabled only when `Application.persistentDataPath/save.json` exists (Save & Load is mini project 4.E.1).
- **Settings:** master volume, mute, fullscreen; stored in `PlayerPrefs` and applied on startup.
- **Animated 3D backdrop:** 80 food prefabs generated in a loop, each spinning, bobbing and drifting with its own random parameters; the camera sways slowly.
- **Button feedback:** scale-up animation and a blip sound on hover / selection.
- **Procedural sound:** hover / click / back blips are synthesised in code (no audio assets).
- **Safe start:** if the game scene is not in Build Settings, a toast message is shown instead of an error.

## 5. New vs. Reused

### Newly written
- `Scripts/Core/*` — shared interfaces and `GameEvents` for the whole team.
- `Scripts/UI/MainMenuController.cs`, `SettingsPanel.cs`, `GameSettings.cs`, `TypewriterText.cs`, `MenuButtonFx.cs`, `MenuSfx.cs`, `MenuBackdrop.cs`, `FloatingItem.cs`
- `Scripts/Editor/MainMenuSceneBuilder.cs`

### Reused
- Food prefabs in `Assets/_Project/Prefabs/Food` (used as the backdrop objects).
- Unity built-in UI sprites, TextMeshPro default font, Input System UI module.

## 6. Notes

- `_gameSceneName` in `MainMenuController` is `Main`. That scene is created later, when the game is integrated; until then Start shows a toast after the story.
- The UI is in English because the default TextMeshPro font has no CJK glyphs.
