# DUNGEON DESCENT — GAME DESIGN

## Core fantasy

The player owns a warm, lived-in refuge directly above a hostile ancient dungeon. The emotional loop is the contrast between safety and depth: prepare at home, physically open a heavy gate, descend farther into cold darkness, risk unextracted resources, defeat stronger threats, return home and permanently grow stronger.

Central question: **How far down can I descend this time?**

## Vertical slice route

### Safe Room
Warm fireplace, timber, stone, workbench, storage chest, rest/save point, rune-lit dungeon gate and low-key home ambience. Permanent upgrades are only available while not inside an active run.

### Descent
A long physical stair sequence. Warm light falls away, cold torches appear, chains and dust increase, and the audio state crosses from fireplace ambience into dungeon wind/exploration.

### Floor 1 — The Old Catacombs
Sarcophagi, worn masonry, moss, torches and first combat encounters. Grave Rats teach tracking fast low targets; Hollow Skeletons teach readable melee telegraphs; Crypt Crawlers add pressure. A mini-arena can seal until cleared.

### Floor 2 — The Flooded Depths
Raised causeways between water channels, cold blue lighting, broken pipes and wet atmosphere. Encounters mix faster pressure with an Elite Skeleton and reward a larger chest.

### Floor 3 — The Forgotten Temple
Monumental columns, ritual dais, ancient azure runes and Cultists with ranged magic. The space deliberately feels older and larger than the catacombs above.

### Boss — The Crypt Warden
A large armored ancient knight in a dedicated sanctum. Phase 1 uses readable melee pressure; below 50% health the Warden accelerates, hits harder and gains an aggressive charge. Defeat exposes the run extraction decision/reward.

## Combat

Player actions: walk, jog, sprint, dodge with a short invulnerability window, three-step light combo, heavy attack, block, lock-on, potion and physical interaction. Stamina gates sprint, dodge, heavy attacks and blocking.

Damage is applied only during attack resolution windows. A single swing tracks already-hit damageables to prevent repeated damage from overlapping colliders.

## Resources and progression

Essence and Gold obtained during an active run remain run-held until successful extraction. Death discards them. Extracted currency becomes permanent save data.

Permanent upgrades:

- Vitality: +15 Max Health
- Endurance: +10 Max Stamina
- Flask Belt: +1 potion capacity, capped
- Runic Edge: +5 weapon damage per level

## Presentation principles

The visual target is stylized-realistic dark fantasy rather than photorealism or low-poly tutorial visuals. Project-owned procedural geometry is composed into recognizable architectural pieces, character silhouettes, monsters, props, weapons and environmental storytelling. PBR-style materials use generated stone, normal, wood, iron, cloth and moss texture sources.

Audio has stateful Safe Room, Exploration, Combat and Boss music plus fireplace, underground wind, door, weapon, loot and healing sounds. Music changes crossfade rather than restarting abruptly.
