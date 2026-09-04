# DUNGEON DESCENT Vertical Slice Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a self-contained Unity 6.3 URP third-person dark-fantasy dungeon crawler vertical slice covering Safe Room, descent, three dungeon floors, elite encounter, Crypt Warden boss, reward, extraction, permanent upgrade, save/load, UI, audio, VFX, and replayable seeded layout.

**Architecture:** A small persistent runtime core coordinates independently testable player, combat, enemies, dungeon, loot/progression, save, UI, audio, and presentation modules. A deterministic procedural art/world layer builds authored modular rooms and stylized meshes from committed source code and committed PBR-style textures/audio so the project has no placeholder dependencies or manual asset restoration steps. Unity Editor automation bakes the runtime-ready scene, materials, animator assets, layers, and Build Settings on first import.

**Tech Stack:** Unity 6000.3.19f1, URP 17.3.x, Input System 1.14.2, Cinemachine 3.1.5, AI Navigation 2.x, C#.

**Spec:** `docs/superpowers/specs/2026-09-04-dungeon-descent-master-spec.md`

## Global Constraints

- New standalone repository/project; never reuse Skinny-to-Beast / Gym Tapper repository.
- No final primitive placeholders, empty scenes, placeholder UI, temporary sounds, or missing visual states.
- Keep Unity `.meta` files in version control; ignore Library, Temp, Logs, Obj, Builds, UserSettings.
- Entry loop must be Safe Room → Descent → Floor 1 → Floor 2 → Floor 3 → Boss → Reward → Safe Room → Upgrade → Save/Continue.
- Use modular focused classes; no giant 1000-line MonoBehaviour god classes.
- External assets require license/source entries in `THIRD_PARTY_ASSETS.md`; original generated assets are documented separately.
- Final deliverable must be a complete ZIP or pushed GitHub state; this session must always produce a ZIP fallback.

---

### Task 1: Project shell, deterministic source assets, and verification harness
**Files:** `Packages/manifest.json`, `ProjectSettings/*`, `Assets/DungeonDescent/Tests/EditMode/*`, `tools/verify_project.py`, generated texture/audio source files.
**Produces:** Valid Unity project shell, package versions, test assemblies, asset/documentation contract.
- [ ] Write failing/static verification rules for required project structure, banned placeholder names, `.meta` coverage, package versions, and documentation.
- [ ] Add Unity project/package settings and deterministic generated visual/audio source files.
- [ ] Run static verifier and fix structural failures.
- [ ] Commit `chore: create Unity project foundation`.

### Task 2: Core runtime, save data, events, audio state machine
**Files:** `Runtime/Core/*`, `Runtime/Save/*`, `Runtime/Audio/*`, tests.
**Produces:** Bootstrap lifecycle, versioned JSON save/load with corruption fallback, event hub, music-state crossfade API.
- [ ] Define tests for save defaults/versioning/corrupt-data fallback and music state transitions.
- [ ] Implement focused runtime services and bootstrap wiring.
- [ ] Run static verification.
- [ ] Commit `feat: add core runtime and save architecture`.

### Task 3: Player locomotion, camera, interaction, health/stamina/combat
**Files:** `Runtime/Player/*`, `Runtime/Combat/*`, tests.
**Produces:** Third-person movement, sprint, dodge, lock-on, light/heavy combo, block, potion, interaction, hit windows, camera collision.
- [ ] Define tests for stamina costs/regeneration, health/death, one-hit-per-swing, combo timing.
- [ ] Implement player/controller/combat modules and Cinemachine-compatible camera target.
- [ ] Run static verification.
- [ ] Commit `feat: implement player locomotion and melee combat`.

### Task 4: Procedural art kit and authored world builder
**Files:** `Runtime/Presentation/*`, `Runtime/World/*`, Editor setup.
**Produces:** Custom mesh-based dark knight, rat/skeleton/crawler/cultist/warden visuals; stone/wood/metal/cloth materials; Safe Room, descent stair, three themed floors and boss arena with no Cube/Capsule placeholders.
- [ ] Define structural tests for required room/floor markers and visual factories.
- [ ] Implement mesh factory, material library, environment modules, lighting/fog/VFX setup.
- [ ] Run static verification.
- [ ] Commit `feat: build safe room and dungeon world`.

### Task 5: Enemy architecture, encounters, elite, boss
**Files:** `Runtime/Enemies/*`, `Runtime/Boss/*`, tests.
**Produces:** Shared AI states, rat/skeleton/crawler/cultist archetypes, elite modifiers, two-phase Crypt Warden.
- [ ] Define state transition/damage tests.
- [ ] Implement navigation/combat/telegraph/stagger/death logic.
- [ ] Run static verification.
- [ ] Commit `feat: add enemies elite encounter and crypt warden`.

### Task 6: Loot, inventory, extraction, permanent progression
**Files:** `Runtime/Loot/*`, `Runtime/Progression/*`, `Runtime/Interactables/*`, tests.
**Produces:** Chests, rarity, essence/gold/items, extraction decision, upgrade table, persistent stats.
- [ ] Define loot and upgrade tests.
- [ ] Implement runtime systems and interactables.
- [ ] Run static verification.
- [ ] Commit `feat: add loot extraction and permanent upgrades`.

### Task 7: Main menu, HUD, pause/settings, feedback and polish
**Files:** `Runtime/UI/*`, presentation/audio integration.
**Produces:** Main menu, HUD, boss bar, prompt, pause/settings, damage/low-HP feedback, music state transitions, hit VFX, camera impulses.
- [ ] Define required UI/action coverage tests.
- [ ] Implement polished runtime UI and feedback.
- [ ] Run static verification.
- [ ] Commit `polish: add UI audio VFX and combat feedback`.

### Task 8: Editor auto-setup, acceptance checks, docs and delivery
**Files:** `Editor/DungeonProjectAutoSetup.cs`, `README.md`, `GAME_DESIGN.md`, `ARCHITECTURE.md`, `THIRD_PARTY_ASSETS.md`, `CHANGELOG.md`.
**Produces:** One-open scene/material/controller/build-settings setup, documented controls/workflow, delivery archive.
- [ ] Implement idempotent Editor setup and acceptance marker checks.
- [ ] Run full static verifier and repository hygiene checks.
- [ ] Create ZIP excluding generated Unity cache folders and `.git`.
- [ ] Commit `chore: finalize playable vertical slice delivery`.
