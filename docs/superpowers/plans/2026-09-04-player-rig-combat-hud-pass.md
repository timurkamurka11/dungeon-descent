# Player Rig Combat HUD Pass Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the procedural player with a real KayKit rigged knight, add real locomotion/jump/attack animation, make melee damage reliable, and make health/stamina bars visibly update.

**Architecture:** Keep movement and combat authoritative in existing gameplay components. Import a CC0 rigged knight as presentation only, drive its Animator with Unity Playables and root motion disabled, isolate melee target geometry in a testable helper, and render HUD bar percentages with RectTransform scaling instead of sprite-dependent Image fill.

**Tech Stack:** Unity 6000.3.19f1, URP 17.3, Input System 1.14.2, Unity Playables/Animation, NUnit EditMode tests, GitHub Actions transport/verification.

**Spec:** `docs/superpowers/specs/2026-09-04-player-rig-combat-hud-design.md`

## Global Constraints

- Work from `feature/player-rig-combat-hud-pass`, never from `main`.
- Final delivery target is `development`.
- Do not touch `skinny-to-beast-clicker`.
- Keep exactly one active player visual/Animator and one gameplay CharacterController.
- Root motion remains disabled; CharacterController owns displacement.
- Every new file/folder under Assets must have a Unity `.meta` file.
- KayKit source/license must be recorded in `THIRD_PARTY_ASSETS.md`.
- No fallback to the old procedural hero when the real model is available.

---

### Task 1: Lock asset and animation contract

**Files:**
- Create: `Assets/DungeonDescent/Resources/Models/Hero/KayKit/Knight.fbx`
- Create: `Assets/DungeonDescent/Resources/Models/Hero/KayKit/knight_texture.png`
- Create: `Assets/DungeonDescent/Resources/Models/Hero/KayKit/sword_1handed.fbx`
- Create: `Assets/DungeonDescent/Resources/Models/Hero/KayKit/shield_badge_color.fbx`
- Create matching `.meta` files
- Modify: `THIRD_PARTY_ASSETS.md`

**Interfaces:**
- Produces: importable KayKit hero asset with real skeleton and embedded animation clips.

- [ ] **Step 1: Inspect source FBX animation stack names**

Run a GitHub Actions probe against the public KayKit repository and record the actual imported animation names. Required semantic coverage: Idle, Walk, Run, Jump and at least one melee attack.

- [ ] **Step 2: Copy exact source binaries**

Copy the source Knight FBX, texture, one-handed sword and shield without re-encoding. Verify source and destination byte counts/SHA-256 within the workflow.

- [ ] **Step 3: Add Unity metadata/import policy**

Add deterministic `.meta` files plus an AssetPostprocessor that forces the Knight to import as Humanoid with animation enabled and in-place/root-motion-safe clip settings.

- [ ] **Step 4: Update asset licensing documentation**

Record KayKit Character Pack: Adventurers, author Kay Lousberg/KayKit, source repository/itch page, CC0 1.0, exact files used and purpose.

- [ ] **Step 5: Verify asset contract**

Check that each binary is non-empty, has a corresponding `.meta`, and the model source reports the required animation semantics.

### Task 2: Add failing combat and HUD regression tests

**Files:**
- Create: `Assets/DungeonDescent/Runtime/Combat/MeleeTargeting.cs`
- Create matching `.meta`
- Create: `Assets/DungeonDescent/Runtime/UI/HudValue.cs`
- Create matching `.meta`
- Modify: `Assets/DungeonDescent/Tests/EditMode/DungeonDescentCoreTests.cs`

**Interfaces:**
- Produces: `MeleeTargeting.IsInsideMeleeArc(Vector3 origin, Vector3 forward, Vector3 target, float maxDistance, float minDot)`
- Produces: `HudValue.Normalized(float current, float maximum)`

- [ ] **Step 1: Write failing melee tests**

Add tests proving a target 1.6 m in front is eligible, a target behind is rejected, and a target beyond max reach is rejected.

- [ ] **Step 2: Write failing HUD tests**

Add tests proving 75/100 = .75 and results clamp to [0,1], including max <= 0 => 0.

- [ ] **Step 3: Run RED verifier/test probe**

Before helper implementation, run a source-level regression probe that requires the new APIs and confirm it fails for the missing implementation.

- [ ] **Step 4: Implement minimal pure helpers**

Implement distance/dot geometry and normalized fraction with `Mathf.Clamp01`.

- [ ] **Step 5: Run GREEN static/unit-compatible verification**

Verify signatures and source invariants. Unity EditMode execution remains a final local-editor check if no licensed Unity CI runner is available.

### Task 3: Replace procedural hero with rigged model

**Files:**
- Create: `Assets/DungeonDescent/Runtime/Player/PlayerModelFactory.cs`
- Create matching `.meta`
- Create: `Assets/DungeonDescent/Editor/KayKitModelImportProcessor.cs`
- Create matching `.meta`
- Modify: `Assets/DungeonDescent/Runtime/Core/DungeonGameBootstrap.cs`
- Modify: `Assets/DungeonDescent/Runtime/Player/PlayerAnimationController.cs`

**Interfaces:**
- `PlayerModelFactory.Build(Transform parent) -> RiggedPlayerVisual`
- `PlayerAnimationController.Configure(RiggedPlayerVisual visual)`
- `SetLocomotion(float speed01, bool grounded)`
- `PlayJump()`
- `PlayAttack(bool heavy, int comboIndex)`
- `PlayDodge()`
- `PlayHit()`
- `PlayDeath()`

- [ ] **Step 1: Build one real visual instance**

Load the Knight prefab/model from Resources, instantiate exactly once, set layer recursively, remove imported gameplay colliders, find its Animator, disable root motion, normalize local scale/offset from renderer bounds, and attach weapon/shield to Humanoid hand bones when available.

- [ ] **Step 2: Build Playables animation graph**

Resolve clips by normalized aliases from the inspected FBX names. Use nested mixers for Idle/Walk/Run locomotion and one-shot action playback. Crossfade instead of snapping poses.

- [ ] **Step 3: Switch bootstrap**

Delete the active call path `VisualFactory.BuildHero(go.transform)` from player construction and replace it with `PlayerModelFactory.Build(go.transform)`.

- [ ] **Step 4: Add anti-duplication guard**

If a player visual already exists under the parent, destroy/replace that single visual root before instantiating. Do not permit two SkinnedMeshRenderer hierarchies or two active Animators.

- [ ] **Step 5: Verify source invariants**

Verifier must fail if bootstrap calls `VisualFactory.BuildHero` or if the imported model/license files are missing.

### Task 4: Add jump and animation-aware controls

**Files:**
- Modify: `Assets/DungeonDescent/Runtime/Player/PlayerController.cs`
- Modify: `Assets/DungeonDescent/Runtime/UI/GameUI.cs`

**Interfaces:**
- Space = jump
- Left Ctrl = dodge
- Existing Shift sprint remains unchanged

- [ ] **Step 1: Add jump state**

Add `JumpHeight = 1.35f`; when grounded and Space is pressed, set vertical velocity using `sqrt(JumpHeight * -2 * Physics.gravity.y)` and trigger `PlayJump()`.

- [ ] **Step 2: Move dodge input**

Bind dodge to Left Ctrl and keep its existing stamina/i-frame movement routine.

- [ ] **Step 3: Update controls UI**

Display `SPACE Jump` and `LEFT CTRL Dodge / Roll`.

- [ ] **Step 4: Verify no double movement**

Ensure `Animator.applyRootMotion` remains false and controller movement is the only transform displacement path.

### Task 5: Repair melee hit registration

**Files:**
- Modify: `Assets/DungeonDescent/Runtime/Player/PlayerCombat.cs`
- Modify: `Assets/DungeonDescent/Runtime/Combat/MeleeTargeting.cs`

**Interfaces:**
- Candidate collection via a forward overlap volume.
- Pure target eligibility via `MeleeTargeting.IsInsideMeleeArc`.

- [ ] **Step 1: Replace the undersized point sphere**

Use a forward capsule/box-like overlap spanning torso-to-sword reach, with separate light/heavy reach values.

- [ ] **Step 2: Filter candidates deterministically**

Use the helper against target collider bounds center, reject self hierarchy, deduplicate by `IDamageable`, and apply damage exactly once per attack.

- [ ] **Step 3: Add editor debug visualization**

Draw the effective melee reach/arc in `OnDrawGizmosSelected` so future tuning is visible.

- [ ] **Step 4: Verify damage path**

Static verifier checks that `ReceiveDamage` is reached through the new targeting helper and that old narrow hard-coded overlap radius path is absent.

### Task 6: Repair HUD health/stamina rendering

**Files:**
- Modify: `Assets/DungeonDescent/Runtime/UI/GameUI.cs`
- Modify: `Assets/DungeonDescent/Runtime/UI/HudValue.cs`

**Interfaces:**
- `OnHealth` and `OnStamina` call a bar setter that scales the foreground RectTransform from the left edge.

- [ ] **Step 1: Remove sprite-dependent fill rendering**

Create bars as ordinary Images with a stored left-pivot foreground RectTransform rather than `Image.Type.Filled` without a sprite.

- [ ] **Step 2: Apply normalized fraction**

Use `HudValue.Normalized(current,max)` and set foreground horizontal scale/anchors while preserving full height.

- [ ] **Step 3: Initialize bars after subscription**

Immediately read current `PlayerVitals` when game starts/shows HUD so values are correct even if initial GameEvents fired before UI subscribed.

- [ ] **Step 4: Verify runtime event path**

Keep `PlayerVitals -> GameEvents.PlayerHealthChanged/PlayerStaminaChanged -> GameUI` as the only HUD data flow.

### Task 7: Project verification, review and delivery

**Files:**
- Modify: `tools/verify_project.py`
- Modify: `CHANGELOG.md`

- [ ] **Step 1: Extend verifier**

Require non-empty KayKit hero binaries, all `.meta`, model factory/import processor, license entry, root-motion-disabled integration, no active procedural hero bootstrap, jump mapping, melee helper and HUD scale rendering.

- [ ] **Step 2: Run full static verifier**

Run `python tools/verify_project.py`; expected `VERIFY PASS` with zero errors.

- [ ] **Step 3: Review feature diff**

Check only intended player/combat/HUD/assets/docs changed and no `Library`, `Temp`, `Logs` or unrelated repo files were introduced.

- [ ] **Step 4: Update changelog**

Record real rigged knight, animation pipeline, jump, melee hit fix and HUD fix.

- [ ] **Step 5: Move verified feature to development**

Fast-forward/merge the verified feature branch into `development` and verify the remote development HEAD contains all intended files.
