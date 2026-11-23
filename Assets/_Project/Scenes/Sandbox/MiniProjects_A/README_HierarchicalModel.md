# Mini Project 4.A.1 — Hierarchical Model

## 1. Overview

This mini project demonstrates a ceiling-mounted toy plane ride in an indoor playground built in Unity. The ride uses a three-level transform hierarchy (**A → B → C**). Each level has its own motion and inherits the motion of its parents.

- **A — Rotor:** The central rotor turns around its local Y axis.
- **B — Arms:** Six arms attached to the rotor swing around their local X axes.
- **C — Planes:** A toy plane hangs from each arm and moves periodically along the Y axis of its parent arm, using a sine function.

The model has two levels of joints, from rotor to arm and from arm to plane, and includes more than five components. The player can reverse the motion of each level independently using the keyboard and is pushed away on contact with the moving ride parts.

## 2. Scene

**Scene:** `A_HierarchicalModel_Demo.unity`

**Location:** `Assets/_Project/Scenes/Sandbox/MiniProjects_A/`

The scene reuses the indoor playground from the Environment Effects mini project, including the ball pit, trampoline area, and curved toy-car track. The plane ride hangs from the ceiling, and the player can walk around and underneath it to see how the rotor, arms, and planes move together.

Ceiling sections at different heights define the indoor play areas. Overhead lights illuminate the playground, and spotlights highlight areas such as the ball pit and nearby play equipment.

## 3. How to Run

1. Open the Unity project.
2. Open `A_HierarchicalModel_Demo.unity`.
3. Press **Play**.
4. Use **W/A/S/D** to move and the **mouse** to control the camera.
5. Walk toward the plane ride and observe the rotor, arms, and planes moving together.
6. Press **J** to reverse the rotor, **K** to reverse the arms, and **L** to reverse the direction of the planes' bobbing animation.
7. Walk into the path of a moving arm or plane to see the ride push the character away.

## 4. Implemented Features

### Hierarchical Structure

```text
PlaneRide                     (PlaneRideAnimator, PlaneRidePusher)
├── Static                    CeilingPlate, Bearing
└── A_Rotor                   A: rotates around local Y
    ├── Column, Canopy, ArmHub, BottomBall, CanopyFlag_0–5
    └── ArmMount_0–5          fixed 60° offsets around the rotor
        └── B_Arm_i           B: swings around local X
            ├── Joint, Beam
            └── C_Plane_i     C: bobs along B's local Y
                ├── HangerRod
                └── Plane_Model
                    └── Helice  propeller spins around local Z
```

- Each child is positioned and rotated in its parent's local space. When a parent moves, all of its descendants move with it.
- The `ArmMount_i` objects place the six arms at fixed 60° intervals. Each arm can then animate its own X-axis rotation independently.

### Level A — Rotor

- The rotor turns continuously around its local Y axis at **40°/s**.
- `Mathf.Repeat` wraps the rotation angle within 0–360°.
- All six arms and planes orbit the center as the rotor turns.

### Level B — Arms

- Each arm swings around its local X axis between **−8°** and **+8°** at **18°/s**, reversing direction at each limit.
- The arms start at different angles and with alternating swing directions, so neighboring arms move out of phase.

### Level C — Planes

- Each plane bobs along its parent arm's local Y axis using a sine wave with an amplitude of **0.15 Unity units** and a frequency of **0.8 Hz**.
- A phase offset of **2π/6** between neighboring planes creates a wave across the six planes.
- Each propeller spins around its local Z axis.

### Interactive Direction Control

- **J** reverses the rotor's rotation direction (A).
- **K** reverses the current swing direction of all arms (B).
- **L** reverses the progression of the sine-wave animation for the planes (C).
- Each input is reported in the Unity Console. The other levels continue animating when one level reverses.

### Player Interaction — Push Away

- The rotor uses a kinematic `Rigidbody`, and moving parts such as the column, arms, and planes have colliders.
- An overlap capsule based on the player's `CharacterController` checks for contact with the ride.
- The script estimates each contacted part's velocity from the change in its collider bounds center. It separates the player from the part and pushes the player in the part's direction of movement.
- The push fades over a short duration so the character slides away smoothly.

### Camera

- The third-person camera follows the player while they walk around the playground.
- Camera obstacle avoidance moves the camera closer to the player when walls or ceilings block the view.

### Scene Design

- The ceiling plate and bearing anchor the ride to the ceiling while the rotor and its descendants move below it.
- Ceiling sections at different heights define the playground, including raised sections over the ball pit and trampoline area.
- Overhead lighting and spotlights illuminate the play areas and make the ride and surrounding equipment easier to see.

## 5. Scripts

**Location:** `Assets/_Project/Scripts/MiniProjects/A/HierarchicalModel/`

| Script | Purpose |
|---|---|
| `PlaneRideAnimator.cs` | Animates the rotor, arms, planes, and propellers, and handles the J/K/L direction controls. |
| `PlaneRidePusher.cs` | Detects contact with moving ride parts and pushes the player away based on the parts' motion. |

**Reused scripts:**

| Script | Purpose |
|---|---|
| `EnvironmentPlayerMovement.cs` | Controls player movement, jumping, and animation. |
| `CameraFollow.cs` | Follows the player and handles camera controls. |
| `CameraObstacleAvoidance.cs` | Moves the camera closer when scene geometry blocks the view. |

## 6. Original and Reused Work

### New Implementation

- A three-level hierarchical plane ride with a rotating rotor, six articulated arms, and six suspended planes.
- Separate animations for the rotor's Y-axis rotation, the arms' limited X-axis swings, and the planes' periodic motion in their parent arms' local frames.
- Phase offsets for the plane animations and local Z-axis rotation for the propellers.
- Keyboard controls that reverse the motion of each hierarchy level independently.
- Collision-based player pushback that accounts for the motion of the contacted ride part.
- A ride assembly built from Unity primitives, custom-colored materials, and imported airplane models, with a fixed ceiling plate and bearing.
- Ceiling geometry at different heights, including raised sections over the ball pit and trampoline area, integrated with the ceiling-mounted ride.
- Overhead lighting and spotlights placed around the playground to illuminate distinct play areas and improve visibility around the ride.

### Reused or Adapted Components

- The indoor playground layout, player character, and movement script from the Environment Effects mini project.
- The third-person camera setup from earlier mini projects and camera obstacle avoidance from the final project.
- An imported low-poly airplane model adapted as a repeated component of the ride.
- Unity built-in `Transform` hierarchy, primitives, colliders, kinematic `Rigidbody`, `CharacterController`, and `Physics.OverlapCapsule`.

## 7. Controls

| Input | Action |
|---|---|
| W / A / S / D | Move player |
| Mouse | Control camera |
| Mouse scroll | Zoom camera |
| C | Toggle first-person / third-person view |
| R | Jump |
| J | Reverse rotor (A) direction |
| K | Reverse arm (B) swing direction |
| L | Reverse plane (C) bobbing direction |
| Esc | Exit play mode |

## 8. Assets and References

The following external assets were used in the project:

- **Kenney — Toy Car Kit**  
  `https://kenney.nl/assets/toy-car-kit`  
  Toy car and track-related models.

- **Unity Asset Store — Claw Machine Free**  
  `https://assetstore.unity.com/packages/3d/props/claw-machine-free-409820`  
  Claw-machine prop.

- **Unity Asset Store — 9t5 Low Poly Playground**  
  `https://assetstore.unity.com/packages/3d/characters/9t5-low-poly-playground-102503`  
  Playground assets.

- **reodesuxz — Low Poly Playground & Park Pack**  
  `https://reodesuxz.itch.io/low-poly-playground-park-pack`  
  Low-poly playground and park props.

- **3DAssets.dev — Trampoline Park and Soft Play**  
  `https://3dassets.dev/packs/trampoline-park-and-soft-play`  
  Trampoline and indoor soft-play models.

- **3DAssets.dev — Nursery and Primary Classroom**  
  `https://3dassets.dev/packs/nursery-and-primary-classroom`  
  Classroom and play-area props.

- **Hooded Adventurer — Quaternius / Poly Pizza**  
  `https://poly.pizza/m/y9KWOVG21R`  
  Player character model reused from the previous mini project.

- **Daniel Dormin — Free Low Poly Airplane**  
  `https://daniel-dormin.itch.io/free-low-poly-airplane`  
  Low-poly airplane model (`plane.fbx`) used for the six planes on the hierarchical ride.

- **Unity Built-in Components**  
  `Transform` hierarchy, primitive meshes, `Light` components, `CharacterController`, colliders, trigger events, `Rigidbody`, `Physics.OverlapCapsule`, and Gizmos.
