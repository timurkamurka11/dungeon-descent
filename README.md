# DUNGEON DESCENT

A self-contained Unity 6 dark-fantasy third-person dungeon crawler vertical slice.

## Unity version

- Unity **6000.3.19f1**
- Universal Render Pipeline **17.3.0**
- Input System **1.14.2**
- Cinemachine **3.1.5**
- AI Navigation **2.0.9**

## Open and run

1. Add this folder in Unity Hub using Unity 6000.3.19f1.
2. Unity imports packages and runs `DungeonProjectAutoSetup` automatically. The setup creates/assigns the URP pipeline asset and configures generated textures.
3. Open `Assets/DungeonDescent/Scenes/Main.unity` if it is not already open.
4. Press Play.

The entry scene contains the `DungeonGameBootstrap` component. Runtime construction is deliberate: the authored world is generated from project-owned custom mesh builders, PBR materials, textures, audio, encounters and data definitions. No manual prefab reconstruction or asset download is required.

## Playable loop

`Main Menu → Safe Room → Dungeon Door → physical Descent → Old Catacombs → Flooded Depths → Forgotten Temple → Crypt Warden → Extraction → Safe Room → Permanent Upgrade → next run`

Death before extraction discards run-held Essence/Gold. Permanent progression remains saved.

## Controls

- `WASD` movement
- `Left Shift` sprint
- `Space` dodge / evade
- `LMB` light combo
- `F` heavy attack
- `RMB` block
- `Q` lock-on / release
- `E` interact
- `R` healing potion
- `Esc` pause

## Project structure

- `Assets/DungeonDescent/Runtime/Core` — bootstrap, session and events
- `Assets/DungeonDescent/Runtime/Player` — locomotion, combat, interaction, potions, lifecycle
- `Assets/DungeonDescent/Runtime/Camera` — Cinemachine third-person rig
- `Assets/DungeonDescent/Runtime/World` — authored dungeon construction, rooms, encounters, zones
- `Assets/DungeonDescent/Runtime/Enemies` — shared enemy state machine and archetypes
- `Assets/DungeonDescent/Runtime/Boss` — Crypt Warden two-phase boss
- `Assets/DungeonDescent/Runtime/Presentation` — custom meshes, materials and character/monster visuals
- `Assets/DungeonDescent/Runtime/UI` — main menu, HUD, pause, settings and upgrades
- `Assets/DungeonDescent/Runtime/Save` — versioned JSON save
- `Assets/DungeonDescent/Art` — project-owned textures, icon, ambience, music and SFX
- `Assets/DungeonDescent/Resources` — runtime-loadable copies of generated source art/audio
- `Assets/DungeonDescent/Tests/EditMode` — deterministic core tests
- `Assets/DungeonDescent/Editor` — automatic URP/import/config setup
- `tools/verify_project.py` — repository acceptance verifier

## Save location

The save is JSON in Unity `Application.persistentDataPath`. Writes use a temporary file and replacement flow, with corrupt-save fallback handled by `SaveManager`.

## Verification

Run outside Unity:

```bash
python tools/verify_project.py
```

Inside Unity, run the EditMode tests in Test Runner. The full gameplay acceptance pass should be performed in Play Mode after Unity finishes its first package import.

## Git workflow

Unity cache directories are ignored. All project-authored `Assets` files and their `.meta` files are tracked so GUID relationships remain stable.
