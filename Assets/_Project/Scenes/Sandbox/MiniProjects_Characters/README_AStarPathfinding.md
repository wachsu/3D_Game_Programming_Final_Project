# Mini Project 4.B.1 — A* Path Finding

## 1. Overview

This mini project demonstrates A* path finding on a grid in Unity.

Click anywhere on the floor and the character finds a path around the obstacles to that point, shows it with small spheres, and walks there with a walk animation. A new target can be clicked at any time, even while walking; the character turns smoothly toward the new path without stopping or jumping.

The search follows the course slides (05_3DGP_PathFinding): 4-direction movement, Manhattan distance as h(n), and the OPEN/CLOSED loop that reopens a node when a better f(n) is found. The raw grid path can then be turned into a straight path and a smooth path (Catmull-Rom), the three results compared on slide 165.

## 2. Scene

**Scene:** `B1_AStarPathfinding.unity`

Located in `Assets/_Project/Scenes/Sandbox/MiniProjects_Characters/`.

## 3. How to Run

1. Open the Unity project (Unity 6000.3.23f1).
2. Open `B1_AStarPathfinding.unity`.
3. Press **Play**.
4. **Left-click on the floor** in the Game view. Yellow spheres show the path and the character walks along it.
5. Click somewhere else while the character is walking to change the target.
6. To compare the three kinds of path, select **Player** and toggle **Straighten** and **Smooth** in the `PathFollower` component, then click the same target again:
   - both off: typical A* path (4-direction staircase)
   - Straighten only: straight path
   - both on (default): smooth path
7. To see the blocked cells, open the **Scene** view during Play (Gizmos on): blocked cells are drawn in red.

## 4. Implemented Features

### Grid (`AStarGrid`, `GridNode`)
- The 40 × 30 m floor is split into 0.5 m cells (80 × 60 = 4800 cells).
- A cell is blocked when a capsule the size of the character (radius 0.3 m, height 1.7 m) placed on it touches the `Obstacle` layer (`Physics.CheckCapsule`). This leaves 172 blocked cells, so paths keep the character's body away from obstacles.
- `NodeFromWorld` converts a world position to its cell; `GetNeighbours` returns the walkable cells up, down, left, and right.
- Gizmos draw the grid bounds and the blocked cells in the Scene view.

### A* Search (`AStarPathfinder`)
- `Heuristic`: Manhattan distance in meters, which never overestimates for 4-direction movement (admissible), so the path found is the shortest one.
- `FindPath`: the OPEN/CLOSED pseudocode from the slides. Each step takes the OPEN node with the lowest f(n) = g(n) + h(n), moves it to CLOSED, and updates its neighbours; a neighbour already in OPEN or CLOSED is updated only if the new f(n) is better. Returns the goal node, or `null` when there is no solution (for example, clicking inside a pillar).
- `BuildPath`: follows the `Parent` links back from the goal and reverses them into a start-to-goal list.

### Straight Path (`AStarPathfinder.StraightenPath`, `AStarGrid.IsLineWalkable`)
- Slide 88: connect nodes if there are no obstacles between them.
- `IsLineWalkable` samples a line every quarter cell and checks that each sample's cell is walkable.
- `StraightenPath` looks as far ahead as it can from the current point and keeps a point only where the next one is out of sight, so only the turning points remain.

### Smooth Path (`AStarPathfinder.SmoothPath`)
- Slide 166: Catmull-Rom spline. Each segment p2 → p3 uses its neighbours p1 and p4 as control points (the end points are repeated at the start and end of the path), and is sampled every 0.25 m.
- If any part of the curve crosses a blocked cell, the straight path is used instead.

### Mouse Target (`MouseTargetPicker`)
- A left click casts a ray from the camera through the mouse position onto the `Floor` layer only, and sends the hit point to the character.

### Path Spheres (`PathMarkers`)
- A small sphere is placed every 1 m along the path, so the number of spheres is path length / spacing + 1 and grows with the path length.
- Spheres are placed by distance along the path, so the spacing stays even on straight and smooth paths.
- Old spheres are removed whenever a new path is set, and all spheres are removed when the character arrives.

### Character Movement (`PathFollower`)
- Moves with a `CharacterController` a small step every frame toward the next path point, so the position always changes continuously.
- Accelerates from standing and brakes with a constant deceleration (v = √(2as)) so it stops at the target without sliding.
- Turns toward the walking direction at up to 360° per second (`Quaternion.RotateTowards`), so the orientation changes smoothly.
- Sends its speed to the Animator, whose Blend Tree changes from Idle to Walk while moving and back to Idle on arrival.
- A new target recomputes the path from the character's current position and keeps the current speed, so it changes direction without stopping. A click with no solution is ignored and the character keeps its current path.

### Debug Tool (`PathTester`)
- Draws the path between two scene objects with Gizmos and recomputes when either moves. Used during development to check the search; disabled in the final scene.

## 5. Requirement Checklist (Project Exercises PDF, 4.B.1)

| Requirement | How it is met |
|---|---|
| An environment with obstacles | 13 obstacles on the `Obstacle` layer; 172 blocked grid cells |
| Use the mouse to set a target point on the floor | `MouseTargetPicker`: left-click raycast onto the `Floor` layer |
| Compute a path and show it with small spheres; the number of spheres depends on the path length | A* search, then `PathMarkers` places one sphere per 1 m of path |
| The character moves continuously from its position to the target and is animated while walking | `PathFollower` moves with `CharacterController.Move` every frame and drives the Idle/Walk Blend Tree |
| A new target can be set at any time while walking | `SetDestination` recomputes the path from the current position on every click |
| Orientation changes smoothly; no jumping from one position to another | Rotation limited to 360°/s; position changes by speed × deltaTime each frame |

## 6. Scripts

All scripts are in `Assets/_Project/Scripts/Characters/Pathfinding/`.

| Script | Purpose |
|---|---|
| `GridNode.cs` | One grid cell: index, world position, walkable flag, and the A* data g, h, f, and parent. |
| `AStarGrid.cs` | Builds the grid, converts world positions to cells, finds neighbours, checks lines, draws Gizmos. |
| `AStarPathfinder.cs` | Heuristic, A* search, path retracing, straight path, and Catmull-Rom smooth path. |
| `MouseTargetPicker.cs` | Mouse click on the floor sets the character's target. |
| `PathMarkers.cs` | Places spheres along the path, one every 1 m. |
| `PathFollower.cs` | Moves, turns, and animates the character along the path. |
| `PathTester.cs` | Development tool: draws the path between two points with Gizmos. |

## 7. Original and Reused Work

### New Implementation
- The grid, the A* search, path retracing, straight path, and smooth path, all following the course slides.
- Mouse target picking, path spheres, and the path-following character controller.
- The `PathMarker` sphere prefab and its material.

### Reused from Mini Project 4.A.2
- The sandbox environment and its 13 obstacles (the scene was copied from `A2_CharacterNavigation.unity`).
- `Player.prefab`, its Animator Controller (Idle/Walk/Run Blend Tree), and the Cinemachine camera. In this scene `PlayerMotor` (keyboard control) is disabled and replaced by `PathFollower`, and the mouse-orbit camera input is disabled so the mouse can be used for clicking; the camera keeps a fixed 45° view.
- The gravity handling and Animator speed code in `PathFollower` follow the same approach as `PlayerMotor`.

### Reused or External Components
- Character models and animation clips: Quaternius Animated Men Pack (CC0).
- Cinemachine 3 and the Input System packages.
- Unity built-in 3D primitives for the obstacles and spheres.

## 8. Controls

| Input | Action |
|---|---|
| Left click on the floor | Set the target; the character walks there |
| Left click again while walking | Change the target |

## 9. Assets and References

- **Course slides:** 05_3DGP_PathFinding (A* pseudocode, Manhattan heuristic, straight path, Catmull-Rom smooth path).
- **Quaternius — Animated Men Pack (CC0):** https://quaternius.com/packs/animatedmen.html
- **Unity Cinemachine 3:** https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/index.html
- **Unity Input System:** https://docs.unity3d.com/Packages/com.unity.inputsystem@1.20/manual/index.html

## 10. Notes

This is an independent mini project demo scene and can be tested separately from the final level scenes. The changes to the Player (PathFollower added, PlayerMotor disabled) are scene overrides in `B1_AStarPathfinding.unity` only; `Player.prefab` itself is unchanged.

The scripts were written with step-by-step guidance from Claude (Anthropic), using fill-in-the-blank templates based on the course slides.
