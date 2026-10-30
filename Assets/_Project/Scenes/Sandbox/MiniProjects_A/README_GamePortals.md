# Mini Project 4.A.3 — Game Portals

## 1. Overview

This mini project demonstrates a three-portal teleportation system in a procedurally generated 3D food court environment using Unity.

The player can move around the scene, enter portals, and teleport between three locations. Particle effects provide visual feedback for the portals and teleportation.

## 2. Scene

**Scene:** `A_GamePortals_Demo.unity`

The scene is located in the Sandbox folder under `Assets/_Project/Scenes/`.

## 3. How to Run

1. Open the Unity project.
2. Find and open `A_GamePortals_Demo.unity`.
3. Press **Play**.
4. Use **W/A/S/D** to move the player.
5. Move into a portal to teleport to another location.

**Portal connections:** A → B → C → A

## 4. Implemented Features

### Portal System
- Three connected portals.
- Teleportation between predefined destinations.
- A shared cooldown to prevent immediate repeated teleportation.
- Continuous particle effects at portal locations.
- Arrival particle effects after teleportation.
- Safe player repositioning by temporarily disabling the CharacterController.

### Procedural Environment
- Food court environment with five food shops.
- Tables and chairs generated using nested loops.
- Shop structures assembled from Unity primitive objects.
- Food models displayed on shop counters using prefabs.
- Portal clearance checks to avoid placing tables too close to portals.

## 5. Scripts

| Script | Purpose |
|---|---|
| `PortalTrigger.cs` | Handles portal teleportation, cooldown, and arrival particle effects. |
| `FoodCourtGenerator.cs` | Generates food shops, dining tables, chairs, and food displays. |
| `TestPlayerMovement.cs` | Controls player movement and character rotation. |

## 6. Original and Reused Work

### New Implementation
- Three-portal teleportation system.
- Teleportation cooldown and particle effect logic.
- Procedural food court generation and object placement.
- Food prefab arrangement and portal clearance checks.

### Reused or Adapted Components
- Player model and camera components from previous coursework.
- External food models from Kenney Food Kit.

## 7. Controls

| Input | Action |
|---|---|
| W / A / S / D | Move player |
| Mouse | Control camera |
| Walk into a portal | Teleport |

## 8. Assets and References

- **Kenney — Food Kit:** https://kenney.nl/assets/food-kit
- Unity built-in 3D primitives and particle systems.
- Player and camera components adapted from previous coursework.

## 9. Notes

This is an independent Mini Project demo scene. It can be tested separately from the final group game scenes.