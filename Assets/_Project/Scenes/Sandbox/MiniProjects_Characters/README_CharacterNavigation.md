# Mini Project 4.A.2 — Character Navigation

## 1. Overview

This mini project demonstrates a keyboard- and mouse-controlled humanoid character moving through a sandbox environment in Unity.

The player walks, runs, and jumps around 13 obstacles while a third-person Cinemachine camera follows. The character's legs are animated with Idle, Walk, and Run cycles that blend by speed. A second two-legged character (an NPC in a suit) stands in the scene and turns to face the player when they come close.

This player controller is the base of the final game's protagonist and implements the shared `IPlayerMotor` interface used by other team members (portals, speed zones, jump pads).

## 2. Scene

**Scene:** `A2_CharacterNavigation.unity`

Located in `Assets/_Project/Scenes/Sandbox/MiniProjects_Characters/`.

## 3. How to Run

1. Open the Unity project (Unity 6000.3.23f1).
2. Open `A2_CharacterNavigation.unity`.
3. Press **Play**, then **click inside the Game view** to give it keyboard focus and lock the cursor.
4. Use **W/A/S/D** to move, hold **Left Shift** to run, press **R** to jump.
5. Move the mouse to orbit the camera and use the scroll wheel to zoom.
6. Walk up to the NPC in the suit — he turns to face you.
7. Press **Esc** to release the cursor.

## 4. Implemented Features

### Player Movement (`PlayerMotor`)
- Camera-relative movement: W always moves toward where the camera is looking.
- Walk (2 m/s) and run (5 m/s) with smooth acceleration and deceleration.
- Smooth turning toward the movement direction (no instant snapping).
- Jumping with gravity, using `CharacterController`.
- Obstacles block the player; when pushing against a wall the walk animation slows down instead of walking in place.
- Implements `IPlayerMotor`: `SetSpeedMultiplier`, `Jump`, `Teleport`, `IsMoving`, so teammates' components can control the player without depending on this class.
- Optional on-screen debug display (input, grounded state, speed, position).

### Leg Animation
- Animator Controller with a 1D Blend Tree driven by the player's actual horizontal speed: Idle (0 m/s), Walk (2 m/s), Run (5 m/s).
- The legs animate while standing (Idle) and while moving (Walk/Run), and the animation follows the real speed, so it slows down when the player is blocked.

### Third-Person Camera (`ThirdPersonCameraInput`)
- Cinemachine 3 camera orbiting the player (Orbital Follow + Rotation Composer).
- Mouse orbits horizontally (360°) and vertically (clamped), scroll wheel zooms between about 2 m and 8 m.
- Cursor locks on click and is released with Esc.
- Deoccluder pulls the camera in when a pillar or counter blocks the view.

### Second Two-Legged Character (`NpcIdle`)
- A suited NPC that plays the Idle animation, sharing the player's Animator Controller.
- Turns smoothly toward the player within 3 m and back to its original direction when the player leaves.

### Environment
- A 40 × 30 m test area with 13 obstacles built from Unity primitives on the `Obstacle` layer (cylinders and boxes of different sizes: pillars, counters, rails, boards, a bench, and bins).
- Obstacles are placed with walkable aisles between them; the same layout is reused by the A* mini project (4.B.1).

## 5. Requirement Checklist (Project Exercises PDF, 4.A.2)

| Requirement | How it is met |
|---|---|
| Use the mouse and/or keyboard to control a human-like character | W/A/S/D, Shift, R for the character; mouse for the camera |
| At least 10 obstacles | 13 obstacles with colliders on the `Obstacle` layer |
| Another character with at least 2 legs, a house, a tree, or a water stream | `NPC_Suit`, a second humanoid character |
| Legs animated properly while moving or standing | Idle / Walk / Run Blend Tree driven by real speed |

## 6. Scripts

| Script | Purpose |
|---|---|
| `Characters/Player/PlayerMotor.cs` | Player movement, jumping, animation speed, and the `IPlayerMotor` implementation. |
| `Characters/Player/ThirdPersonCameraInput.cs` | Mouse orbit, scroll zoom, and cursor lock for the Cinemachine camera. |
| `Characters/Npc/NpcIdle.cs` | Idle NPC that turns to face the nearby player. |
| `Core/GameInterfaces.cs` | Team-shared interfaces (from the team spec); placeholder until the Core owner's version lands. |
| `Characters/Editor/PlayerSetupTool.cs` | Editor menu: builds the Animator Controller and `Player.prefab`. |
| `Characters/Editor/CameraSetupTool.cs` | Editor menu: adds the third-person camera to the open scene. |
| `Characters/Editor/NpcSetupTool.cs` | Editor menu: places the idle NPC in the open scene. |
| `Characters/Editor/ModelMeasureTool.cs` | Editor menu: measures a model's height to set the import scale. |

## 7. Original and Reused Work

This mini project was built from scratch for the team project; it does not reuse code from the course assignments.

### New Implementation
- `PlayerMotor`: movement, running, jumping, collision handling, animation speed, and `IPlayerMotor`.
- `ThirdPersonCameraInput`: mouse orbit, zoom, and cursor lock logic.
- `NpcIdle`: the NPC's look-at behaviour.
- Animator Controller setup (Blend Tree thresholds matched to movement speed).
- Sandbox scene layout and obstacle placement.

### Reused or External Components
- Character models and animation clips: Quaternius Animated Men Pack (CC0). Only the import scale (0.3426, so the character is 1.7 m tall) and loop settings were changed.
- Cinemachine 3 package components (Orbital Follow, Rotation Composer, Deoccluder, Brain).
- `GameInterfaces.cs` is copied from the team specification, not written for this exercise.
- The four Editor tools only automate setup in the Unity Editor; they are development aids, not part of the exercise.
- Unity built-in 3D primitives for the obstacles.

## 8. Controls

| Input | Action |
|---|---|
| W / A / S / D | Move |
| Left Shift (hold) | Run |
| R | Jump |
| Mouse | Orbit the camera |
| Scroll wheel | Zoom the camera |
| Left click in Game view | Lock the cursor |
| Esc | Release the cursor |

## 9. Assets and References

- **Quaternius — Animated Men Pack (CC0):** https://quaternius.com/packs/animatedmen.html
- **Unity Cinemachine 3:** https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/index.html
- **Unity Input System:** https://docs.unity3d.com/Packages/com.unity.inputsystem@1.20/manual/index.html
- Unity built-in 3D primitives.

## 10. Notes

This is an independent mini project demo scene and can be tested separately from the final level scenes. `Player.prefab` (in `Assets/_Project/Prefabs/Characters/`) already contains `PlayerMotor`, so it can be dropped into other scenes.
