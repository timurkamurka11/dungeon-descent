# THIRD-PARTY ASSETS

## KayKit Character Pack: Adventurers

- **Asset:** KayKit - Character Pack: Adventurers
- **Author:** Kay Lousberg / KayKit
- **Source repository:** `https://github.com/KayKit-Game-Assets/KayKit-Character-Pack-Adventures-1.0`
- **Original project page:** `https://kaylousberg.itch.io/kaykit-adventurers`
- **License:** CC0 1.0 Universal — free for personal and commercial use, no attribution required.
- **License file:** `LICENSE.txt` in the source repository.
- **Files used in DUNGEON DESCENT:**
  - `Characters/fbx/Knight.fbx` -> `Assets/DungeonDescent/Resources/Models/Hero/KayKit/Knight.fbx`
  - `Characters/fbx/knight_texture.png` -> `Assets/DungeonDescent/Resources/Models/Hero/KayKit/knight_texture.png`
- **Source verification:** Knight FBX is Kaydara FBX 7400, 20,659,324 bytes, SHA-256 `b25e35cc7eaaa3c7c103f10a4f41fde1cbd81936fede9aa9ae4efbd29d40ba1b` at integration time.
- **In-game use:** primary player character, embedded one-handed sword and badge shield, humanoid skeleton and embedded animation set.
- **Animation verification:** source FBX contains 76 animations, including `Idle`, `Walking_A`, `Running_A`, `Jump_Full_Short`, `Dodge_Forward`, `Hit_A`, `Death_A`, `Blocking`, and the `1H_Melee_Attack_*` set.
- **Modifications/integration:** imported as Humanoid by `KayKitModelImportProcessor`; root motion disabled; runtime URP Lit material uses the supplied KayKit texture; unused embedded weapon/shield variants are hidden so only the one-handed sword and badge shield remain active.

## Project-owned visual and audio assets

All files under:

- `Assets/DungeonDescent/Art/Textures`
- `Assets/DungeonDescent/Art/Icons`
- `Assets/DungeonDescent/Art/Audio`

were created specifically for DUNGEON DESCENT as project-owned generated source material. Environment meshes and current non-player creature meshes are produced by the project-owned `ProceduralMeshFactory` and composed by `VisualFactory` / `DungeonWorldBuilder`.

The duplicated project-owned files under `Assets/DungeonDescent/Resources` are runtime-loadable copies of those same source assets.

## Unity packages

The project depends on official Unity packages declared in `Packages/manifest.json` (URP, Input System, Cinemachine, AI Navigation, Test Framework, UGUI and standard Unity modules). These are software dependencies installed through the Unity Package Manager; no content from commercial games or unverified asset mirrors is bundled.

## Future asset rule

Any later external content must be added here with: exact asset name, author, source URL, license, modification notes and in-game usage before it is committed.
