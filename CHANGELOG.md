# CHANGELOG

## 0.1.1 — Rigged Player / Combat / HUD Pass — 2026-09-04

- Replaced the active procedural player visual with the CC0 KayKit Adventurers Knight rig.
- Added Humanoid import policy and Playables-driven Idle, Walk, Run, Jump, Dodge, Block, Hit, Death and one-handed sword attack animation integration with root motion disabled.
- Added Space jump and moved dodge/roll to Left Ctrl.
- Reworked melee hit registration from a small point sphere into a forward sword-reach capsule volume with testable arc/range filtering and unique-target damage.
- Reworked HUD health/stamina/boss bars to use left-anchored visual scaling, fixing bars that remained visually full while values changed.
- Added regression coverage for HUD normalization, melee geometry and health changed events.
- Documented KayKit source, exact model SHA-256 and CC0 1.0 license in `THIRD_PARTY_ASSETS.md`.

## 0.1.0 — Vertical Slice Foundation — 2026-09-04

- Created Unity 6000.3.19f1 URP project structure and Unity-safe Git ignore rules.
- Added Input System, Cinemachine 3, AI Navigation and URP package dependencies.
- Added versioned save architecture, persistent progression and run-held extraction currency.
- Added responsive third-person locomotion, sprint, stamina, dodge, light combo, heavy attack, block, lock-on, interaction and potion systems.
- Added Cinemachine third-person follow rig.
- Added project-owned procedural mesh library and generated PBR-style texture/audio source set.
- Added original hero, Grave Rat, Hollow Skeleton, Crypt Crawler, Cultist and Crypt Warden visual compositions.
- Added Safe Room, physical dungeon Descent, Old Catacombs, Flooded Depths, Forgotten Temple and Boss Arena.
- Added encounter locking, chest rewards, permanent upgrade table, rest/save point and extraction altar.
- Added shared enemy state machine, specialized fast/ranged enemy behaviors, elite and two-phase Crypt Warden boss.
- Added main menu, HUD, boss health, pause, controls, graphics/audio/gameplay settings, permanent upgrades and death presentation.
- Added automatic URP/import/config setup and ScriptableObject data definitions.
- Added deterministic EditMode core tests and repository acceptance verifier.
- Added complete `.meta` coverage for project-owned Unity Assets.
