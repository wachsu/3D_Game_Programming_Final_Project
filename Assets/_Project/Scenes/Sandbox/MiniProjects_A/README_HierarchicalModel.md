# Mini Project 4.A.1 — Hierarchical Model

## 1. Overview

This mini project demonstrates a hierarchical model in Unity: a ceiling-mounted toy plane ride for the indoor toy-floor playground. The ride is built as a three-level transform hierarchy (**A → B → C**), where each level has its own motion and inherits the motion of its parents.

- **A — Rotor:** the central column rotates around the vertical (Y) axis.
- **B — Arms:** six arms attached to the rotor through joints swing up and down around their local X axis, a different axis from A.
- **C — Planes:** a toy plane hangs from the end of each arm and bobs periodically (sine function) along the Y axis of its parent arm B, in B's local frame.

The ride has two joints (rotor → arm, arm → plane), and the full model is made of more than five components.

The player can walk around the ride, change the direction of each level with the keyboard, and gets pushed away when touching the moving parts. The ride is designed as a potential attraction on the toy floor in the final shopping mall game.

## 2. Scene

**Scene:** `A_HierarchicalModel_Demo.unity`

**Location:** `Assets/_Project/Scenes/Sandbox/MiniProjects_A/`

The scene reuses the indoor playground from the Environment Effects mini project (ball pit, trampoline, and curved toy-car track) and adds the plane ride hanging from the ceiling. The player can walk under and around the ride to see how the three levels of motion combine.

## 3. How to Run

1. Open the Unity project.
2. Open `A_HierarchicalModel_Demo.unity`.
3. Press **Play**.
4. Use **W/A/S/D** to move and the **mouse** to control the camera.
5. Walk to the plane ride and watch the rotor, arms, and planes move together.
6. Press **J**, **K**, or **L** to reverse the motion of level A, B, or C.
7. Walk into the moving arms or planes to be pushed away by the ride.

## 4. Implemented Features

### Hierarchical Structure

```
PlaneRide                     (PlaneRideAnimator, PlaneRidePusher)
├── Static                    CeilingPlate, Bearing (fixed to the ceiling)
└── A_Rotor                   A: rotates around Y
    ├── Column, Canopy, ArmHub, BottomBall, CanopyFlag_0–5
    └── ArmMount_0–5          fixed 60° offsets around the rotor
        └── B_Arm_i           B: swings around local X
            ├── Joint, Beam
            └── C_Plane_i     C: bobs along B's local Y
                ├── HangerRod
                └── Plane_Model
                    └── Helice  propeller spins around local Z
```

- Each child is positioned and rotated in its parent's **local space**, so moving a parent automatically moves all of its children.
- `ArmMount_i` objects separate the fixed 60° placement of each arm from the arm's own swinging rotation, so the arm only needs to animate one axis.

### Level A — Rotor

- The rotor rotates continuously around its local Y axis at **40°/s**.
- The angle is wrapped with `Mathf.Repeat` to stay within 0–360°.
- Rotating the rotor carries all six arms and planes around the center.

### Level B — Arms

- Each arm swings around its local X axis between **−8°** and **+8°** at **18°/s**, reversing direction at each limit.
- Arms start at different angles and alternate swing directions, so neighboring arms move out of phase.

### Level C — Planes

- Each plane bobs along its parent arm's local Y axis using a sine wave (amplitude **0.15**, frequency **0.8 Hz**).
- A phase offset of 2π/6 per plane makes the six planes move in a wave pattern instead of together.
- The propeller of each plane model spins around its local Z axis as an additional decorative level.

### Interactive Direction Control

- **J** reverses the rotor direction (A).
- **K** reverses the swing direction of all arms (B).
- **L** reverses the bobbing direction of the planes (C).
- Each change is reported in the Console, and the other levels keep moving, showing that each level's motion is independent.

### Player Interaction — Push Away

- The rotor uses a kinematic `Rigidbody`, and the column, arms, and planes have colliders.
- Every frame, an overlap capsule matching the player's `CharacterController` checks for contact with the ride's colliders.
- The velocity of each touched part is estimated from the change of its bounds center, and the player is separated from the part and pushed in its moving direction.
- The push fades out over a short duration, so the player slides away smoothly instead of being stuck inside the moving parts.

### Camera

- The third-person camera from the previous mini projects is reused.
- Camera obstacle avoidance keeps the camera from passing through walls and the ceiling when the player walks around the ride.

## 5. Scripts

**Location:** `Assets/_Project/Scripts/MiniProjects/A/HierarchicalModel/`

| Script | Purpose |
|---|---|
| `PlaneRideAnimator.cs` | Animates the three hierarchy levels (rotor rotation, arm swing, plane bobbing), spins the propellers, and handles the J/K/L direction controls. |
| `PlaneRidePusher.cs` | Detects contact between the player and the moving ride parts and pushes the player away based on the parts' velocity. |

**Reused scripts:**

| Script | Purpose |
|---|---|
| `EnvironmentPlayerMovement.cs` | Player movement, jumping, and animation from the Environment Effects mini project. |
| `CameraFollow.cs` | Third-person camera that follows the player. |
| `CameraObstacleAvoidance.cs` | Moves the camera closer when walls or ceilings block the view. |

## 6. Original and Reused Work

### New Implementation

- A three-level hierarchical plane ride (rotor → arms → planes) with separate motion at each level.
- Rotor rotation, arm swinging with angle limits, and sine-wave plane bobbing with phase offsets.
- Keyboard controls to reverse each level's motion independently.
- Collision-based pushing that moves the player away from the moving ride parts.
- The ride model, built from Unity primitives (cylinders, spheres, cubes) with custom colored materials and an imported plane model.

### Reused or Adapted Components

- Indoor playground scene, player character, and movement script from the Environment Effects mini project.
- Third-person camera setup from the previous mini projects, with camera obstacle avoidance from the final project.
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

- **Daniel Dormin — Free Low Poly Airplane**  
  https://daniel-dormin.itch.io/free-low-poly-airplane  
  Low-poly airplane model used for the planes on the ride (`plane.fbx`).

- **Hooded Adventurer — Quaternius / Poly Pizza**  
  https://poly.pizza/m/y9KWOVG21R  
  Player character model reused from the previous mini projects.

- **Environment Effects playground assets**  
  The playground props from the Environment Effects mini project (Kenney Toy Car Kit, Claw Machine Free, 9t5 Low Poly Playground, Low Poly Playground & Park Pack, Trampoline Park and Soft Play, Nursery and Primary Classroom) are reused as the surrounding scene. See `README_EnvironmentEffects.md` for links.

- **Unity Built-in Components**  
  `Transform` hierarchy, primitive meshes, colliders, kinematic `Rigidbody`, `CharacterController`, and physics overlap queries.
