# THIRD-PARTY ASSETS

## Current status

**No third-party visual, audio, model, texture, animation or music asset is included in this vertical slice.**

All files under:

- `Assets/DungeonDescent/Art/Textures`
- `Assets/DungeonDescent/Art/Icons`
- `Assets/DungeonDescent/Art/Audio`

were created specifically for DUNGEON DESCENT as project-owned generated source material. Runtime character, monster, weapon, prop and environment meshes are produced by the project-owned `ProceduralMeshFactory` and composed by `VisualFactory` / `DungeonWorldBuilder`.

The duplicated files under `Assets/DungeonDescent/Resources` are runtime-loadable copies of those same project-owned source assets, not separate third-party materials.

## Unity packages

The project depends on official Unity packages declared in `Packages/manifest.json` (URP, Input System, Cinemachine, AI Navigation, Test Framework, UGUI and standard Unity modules). These are software dependencies installed through the Unity Package Manager; no content from commercial games or unverified asset mirrors is bundled.

## Future asset rule

Any later external content must be added here with: exact asset name, author, source URL, license, modification notes and in-game usage before it is committed.
