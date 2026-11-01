# Mini Project 4.A.3 — Game Portals

## 1. Overview

This mini project demonstrates a three-portal teleportation system in a 3D shopping mall food court environment using Unity.

The player starts near the elevator and can explore the food court or use portals to travel quickly between different areas. The portals provide convenient shortcuts through the food stalls and dining areas.

Continuous particle effects highlight the portals, while arrival particle effects provide visual feedback after teleportation.

## 2. Scene

**Scene:** `A_GamePortals_Demo.unity`

**Location:** `Assets/_Project/Scenes/Sandbox/MiniProjects_A/`

The scene represents a shopping mall food court with 13 food shops, dining areas, and various environmental decorations. All environment objects are manually placed to create a more complete and organized layout.

## 3. How to Run

1. Open the Unity project.
2. Open `A_GamePortals_Demo.unity`.
3. Press **Play**.
4. Use **W/A/S/D** to move the player.
5. Walk into a portal to teleport to another location.

**Portal connections:** A → B → C → A

## 4. Implemented Features

### Portal System

- Three connected portals with predefined destinations.
- Teleportation between different locations in the food court.
- A shared cooldown to prevent immediate repeated teleportation.
- Continuous particle effects at portal locations.
- Arrival particle effects after teleportation.
- Safe player repositioning by temporarily disabling the `CharacterController`.

### Food Court Environment

- A shopping mall food court containing 13 food shops.
- Food models displayed on the counters of each shop.
- Dining areas with tables and chairs for customers.
- A tray return trolley and additional environmental decorations.
- Imported 3D models combined with Unity built-in primitive objects.
- More than 20 scene objects manually placed and arranged.
- Manual adjustments to object placement to maintain clear walking paths and access to the portals.

### Scene Design

The player starts near the elevator and enters the food court. Since the food court contains many shops and seating areas, three portals are placed at different locations to provide shortcuts.

Players can either walk through the food court normally or use the portals to move more quickly between different areas.

## 5. Scripts

| Script | Purpose |
|---|---|
| `PortalTrigger.cs` | Handles portal teleportation, cooldown, and arrival particle effects. |
| `TestPlayerMovement.cs` | Controls player movement and character rotation. |

## 6. Original and Reused Work

### New Implementation

- Three-portal teleportation system.
- Teleportation cooldown and particle effect logic.
- Portal destinations and placement within the food court.
- Manual food court environment design, including food shops, dining areas, furniture, and decorations.

### Reused or Adapted Components

- Player and camera components from previous coursework.
- External 3D models used for the player, furniture, food, and environmental decorations.
- Unity built-in primitives, colliders, and particle systems.

## 7. Controls

| Input | Action |
|---|---|
| W / A / S / D | Move player |
| Mouse | Control camera |
| Walk into a portal | Teleport |

## 8. Assets and References

The following external assets were used in the project:

- **Kenney — Food Kit**  
  https://kenney.nl/assets/food-kit  
  Food models displayed on shop counters.

- **Kenney — Fantasy Town Kit**  
  https://kenney.nl/assets/fantasy-town-kit  
  Environmental and architectural models.

- **Kenney — Furniture Kit**  
  https://kenney.nl/assets/furniture-kit  
  Furniture models used in the food court.

- **3DAssets.dev — Tray Return Trolley**  
  https://3dassets.dev/assets/airport-terminal-and-ground-operations-tray-return-tro-6d84dbed  
  Tray return trolley model placed in the dining area.

- **Hooded Adventurer — Quaternius / Poly Pizza**  
  https://poly.pizza/m/y9KWOVG21R  
  Player character model.

- **Unity Built-in Components**  
  Built-in 3D primitives, `CharacterController`, colliders, and particle systems.
