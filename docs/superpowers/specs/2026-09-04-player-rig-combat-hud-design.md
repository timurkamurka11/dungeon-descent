# Player Rig, Combat and HUD Pass — Design

## Goal
Replace the current procedural player visual with one real rigged and animated knight, make melee hits reliable, make the health/stamina HUD visibly reflect runtime values, and add a real jump while preserving the existing CharacterController-driven movement and combat loop.

## Scope
This pass changes only the player presentation/gameplay integration and related regression tests. Enemy visuals remain on the current pipeline for this pass.

## Hero asset
Use KayKit Character Pack: Adventurers — `Knight.fbx`, `knight_texture.png`, and a one-handed sword/shield from the same pack. The pack is CC0 1.0, commercially usable, Unity-compatible, fully rigged and animated. Exact source and license are recorded in `THIRD_PARTY_ASSETS.md`.

The imported knight becomes the only active player visual. `VisualFactory.BuildHero` is not instantiated for the player after this pass. The player root retains exactly one gameplay `CharacterController`; the imported hierarchy supplies only renderers, bones and one `Animator`. Imported colliders are disabled/removed at runtime so there is no double collision body.

## Animation architecture
`PlayerAnimationController` owns a Playables-based animation graph connected to the imported model `Animator`. Movement remains code-driven; `Animator.applyRootMotion = false` so imported clips cannot move the gameplay root or cause duplicate displacement/foot-position drift.

Clips are resolved by normalized name aliases from the FBX. Required semantic states are:
- Idle
- Walk
- Run
- Jump
- Roll/Dodge
- Light Attack
- Heavy Attack or a second melee attack
- Hit, if present
- Death, if present

Locomotion crossfades between idle/walk/run. Jump, roll and attack play as temporary actions and blend back into locomotion. If a secondary optional clip is absent, the controller uses the closest available melee/hit clip; required Idle/Walk/Run/Jump/Attack must exist or the model factory logs a clear error and refuses the rig rather than silently returning to the old procedural hero.

## Input
- WASD: move
- Left Shift: sprint
- Space: jump
- Left Ctrl: dodge/roll
- LMB: light attack combo
- F: heavy attack
- RMB: block

Jump uses `CharacterController` vertical velocity with a configurable jump height. The animation is visual only; gravity and landing remain owned by `PlayerController`.

## Combat reliability
The current single small overlap sphere is replaced with a forward melee volume that covers the sword reach from the player torso forward. Target eligibility is split into a pure geometry helper so it can be unit-tested without physics. Physics still gathers candidate colliders, but each unique `IDamageable` can be hit only once per attack.

The attack volume must:
- reject the player hierarchy;
- reject targets behind the player outside the allowed arc;
- accept ordinary enemy capsule colliders at expected melee distances;
- use a larger reach for heavy attacks;
- provide a debug gizmo in editor builds for future tuning.

## HUD reliability
The current bars use `Image.Type.Filled` with no sprite, which makes `fillAmount` unreliable visually. The pass changes bar rendering to a left-anchored foreground RectTransform whose horizontal scale is driven by a clamped normalized value. Health and stamina events continue to come from `GameEvents`; only their visual representation changes.

## Testing
Add EditMode tests for:
- normalized HUD fraction clamps and computes correctly;
- melee geometry accepts a target in front at melee range;
- melee geometry rejects targets behind/out of range;
- health model event still reports changed health.

Extend `tools/verify_project.py` to require the real hero asset, its `.meta`, the new model factory, the KayKit license/source entry, and to reject any active player bootstrap call to `VisualFactory.BuildHero`.

## Delivery
All work is isolated on `feature/player-rig-combat-hud-pass`, verified there, then moved to `development`. The user continues to update only with `git pull origin development`.
