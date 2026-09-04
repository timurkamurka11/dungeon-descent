# DUNGEON DESCENT — ARCHITECTURE

## Runtime composition

`DungeonGameBootstrap` is the single entry composition root. It ensures the persistent session/audio services, builds the authored world, creates the player, configures the Cinemachine rig and creates the UGUI presentation. The scene therefore has a stable entry object while detailed content is assembled from project-owned modular constructors.

## Major boundaries

### Core
`GameSession` owns current save data and run state. `GameEvents` publishes coarse UI/gameplay signals without systems performing per-frame object searches for unrelated services.

### Save
`SaveData` is a versioned serializable schema. `SaveManager` is responsible only for persistence, migration/fallback and file replacement.

### Player
`PlayerController` owns CharacterController locomotion. `PlayerCombat` owns attack/block input and hit resolution. `PlayerVitals` wraps pure `HealthModel` and `StaminaModel`. `PlayerInteraction`, `PlayerConsumables`, `PlayerLockOn` and `PlayerLifecycle` remain separate responsibilities.

### Camera
`ThirdPersonCameraRig` creates a Cinemachine 3 third-person camera, drives a pitch/yaw tracking target and optionally biases toward a lock-on target.

### Presentation
`ProceduralMeshFactory` owns reusable project-created meshes. `MaterialLibrary` owns URP Lit materials. `VisualFactory` composes those into the hero, four enemy silhouettes and boss visuals. Standard Unity geometry objects are not used as final visual models.

### World
`DungeonWorldBuilder` creates the Safe Room, physical Descent, three authored floors and Boss Arena. `ZoneTrigger` changes music/ambience/floor state. `EncounterController` owns combat-room spawning and seal release. `NavMeshSurface` is built after authored collision geometry exists.

### Enemies
`EnemyBrain` implements shared states: Idle, Patrol, Suspicious, Investigate, Chase, Attack, Recover, Stagger, Return and Dead. Specialized rat/cultist behaviors derive from the shared implementation. `EnemyHealth` owns damage state. `EnemyFactory` maps archetypes to visual/behavior parameter sets.

### Boss
`CryptWardenController` extends the shared enemy brain, publishes boss UI state, transitions at 50% HP and records run completion on death.

### UI
`GameUI` builds and owns main menu, HUD, pause, controls, settings, upgrade and death surfaces. It listens to `GameEvents` instead of polling health/currency components.

## Data-driven layer

The project defines ScriptableObject types for weapons, enemies, items, loot tables, biomes, encounters, upgrades and audio. `DungeonProjectAutoSetup` creates initial config assets on first Unity import, so configuration can migrate from code defaults into editable data without redesigning runtime boundaries.

## Rendering and import setup

The editor setup creates a Universal Render Pipeline asset, initializes built-in Universal Renderer data, assigns it in `GraphicsSettings` and `QualitySettings`, switches the generated stone normal map to NormalMap import type and applies compressed high-quality texture settings.

## Runtime reset strategy

The geometric world and encounters are children of `DungeonWorldBuilder`. Death/extraction returns the player to Safe Room and rebuilds this runtime content tree, resetting encounter state without rebuilding persistent services or save state.
