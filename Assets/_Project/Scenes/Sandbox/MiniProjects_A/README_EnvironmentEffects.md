# Mini Project 4.D.1 — Environment Effects

## 1. Overview

This mini project demonstrates three interactive environmental effects in an indoor toy-floor playground using Unity: a ball pit that slows the player down, a trampoline that repeatedly launches the player upward, and a curved toy-car track that increases the player's movement speed.

The effects are activated when the player enters the corresponding areas. The playground is designed as a potential part of the toy floor in the final shopping mall game.

## 2. Scene

**Scene:** `A_EnvironmentEffects_Demo.unity`

**Location:** `Assets/_Project/Scenes/Sandbox/MiniProjects_A/`

The scene is an indoor play area with a ball pit in the lower-left section, a trampoline area in the upper-left section, and a curved toy-car track on the right. Additional playground equipment, toys, and decorations are arranged around the three interactive zones.

## 3. How to Run

1. Open the Unity project.
2. Open `A_EnvironmentEffects_Demo.unity`.
3. Press **Play**.
4. Use **W/A/S/D** to move and the **mouse** to control the camera.
5. Walk into the ball pit to experience reduced movement speed and push the balls.
6. Enter the trampoline area to bounce repeatedly, or press **R** to jump manually.
7. Move onto the curved toy-car track to experience increased movement speed.

## 4. Implemented Features

### Ball Pit — Slow Movement and Physics

- A trigger zone reduces the player's walking speed to **0.4×** inside the ball pit and restores normal speed on exit.
- The player's `CharacterController.stepOffset` is temporarily reduced to avoid stepping onto the small balls, then restored when leaving.
- The player can push balls using collision detection and `Rigidbody` velocity.
- A coroutine generates up to **300** randomly placed, colored balls in batches, with collision checks; this also demonstrates loop-based object generation.

### Trampoline — Continuous Jumping

- A trigger zone identifies the player while they remain in the trampoline area.
- The player automatically jumps again when grounded and after the bounce delay.
- The jump uses a configurable height and the player movement script's gravity-based jump calculation.
- The character takes an upright pose while in the air.

### Curved Toy-Car Track — Speed Boost

- Entering the track increases movement speed to **1.8×**.
- Multiple track trigger zones report their enter/exit events to one `SpeedTrackController`.
- A `HashSet` tracks occupied zones so the speed boost is only removed after the player has left all track zones.

### Character Animation

- The character plays an **Idle** animation when not moving.
- Movement animation changes with the player's walking speed, including slower movement in the ball pit and faster movement on the track.
- The character switches to an upright pose when airborne on the trampoline or during a jump.
- The Animator is updated using movement, running, and grounded states to keep the character's visual state consistent with movement.

### Scene Design

The three effects are placed in visually distinct playground areas so that the player can compare normal movement with slowing, bouncing, and acceleration. Imported low-poly assets and Unity components are used to construct the playground setting.

## 5. Scripts

**Location:** `Assets/_Project/Scripts/MiniProjects/A/Environment/`

| Script | Purpose |
|---|---|
| `EnvironmentPlayerMovement.cs` | Controls camera-relative movement, character rotation, gravity, jumping, animation, speed multiplier, and ball pushing on collision. |
| `BallPitZone.cs` | Applies the ball-pit speed multiplier and temporary step-offset change when the player enters or leaves. |
| `BallPitSpawner.cs` | Generates colored balls in batches at randomized, collision-checked positions. |
| `JumpPadZone.cs` | Detects the player on the trampoline and triggers repeated jumps when grounded. |
| `SpeedTrackZone.cs` | Relays each track segment's trigger enter/exit events to the parent controller. |
| `SpeedTrackController.cs` | Coordinates the speed boost across overlapping track segments using a `HashSet`. |

## 6. Original and Reused Work

### New Implementation

- Three interactive environment zones: ball-pit slowdown, trampoline jumping, and curved-track acceleration.
- Trigger logic that changes and restores player speed as the character enters and exits zones.
- Temporary step-offset adjustment and collision-based ball pushing in the ball pit.
- Automatic repeated trampoline jumps with grounded and cooldown checks.
- Track speed boost coordinated across multiple trigger zones.
- Character animation setup for idle, different movement speeds, and the airborne upright pose.
- Coroutine-based generation of numerous colored balls and the arrangement of the three decorated play areas.

### Reused or Adapted Components

- Player character model and camera setup used in the previous Game Portals mini project.
- Player movement concepts adapted for the environment-effects demo, with additional speed, jump, animation, and collision behavior.
- External playground, toy, classroom, and claw-machine 3D assets used as environment props.
- Unity built-in `CharacterController`, colliders, triggers, `Rigidbody` physics, and Gizmos.

## 7. Controls

| Input | Action |
|---|---|
| W / A / S / D | Move player |
| Mouse | Control camera |
| R | Jump |
| Enter ball pit | Slow down and push balls |
| Enter trampoline | Bounce repeatedly |
| Enter curved track | Speed up |

## 8. Assets and References

The following external assets were used in the project:

- **Kenney — Toy Car Kit**  
  https://kenney.nl/assets/toy-car-kit  
  Toy car and track-related models.

- **Unity Asset Store — Claw Machine Free**  
  https://assetstore.unity.com/packages/3d/props/claw-machine-free-409820  
  Claw-machine prop.

- **Unity Asset Store — 9t5 Low Poly Playground**  
  https://assetstore.unity.com/packages/3d/characters/9t5-low-poly-playground-102503  
  Playground assets.

- **reodesuxz — Low Poly Playground & Park Pack**  
  https://reodesuxz.itch.io/low-poly-playground-park-pack  
  Low-poly playground and park props.

- **3DAssets.dev — Trampoline Park and Soft Play**  
  https://3dassets.dev/packs/trampoline-park-and-soft-play  
  Trampoline and indoor soft-play models.

- **3DAssets.dev — Nursery and Primary Classroom**  
  https://3dassets.dev/packs/nursery-and-primary-classroom  
  Classroom and play-area props.

- **Hooded Adventurer — Quaternius / Poly Pizza**  
  https://poly.pizza/m/y9KWOVG21R  
  Player character model reused from the previous mini project.

- **Unity Built-in Components**  
  `CharacterController`, colliders, trigger events, `Rigidbody`, and Gizmos.
