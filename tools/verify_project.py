#!/usr/bin/env python3
from pathlib import Path
import json, re, sys
ROOT=Path(__file__).resolve().parents[1]
errors=[]

def req(path):
    p=ROOT/path
    if not p.exists(): errors.append(f"missing: {path}")
    return p

def contains(path, needle):
    p=req(path)
    if p.exists() and needle not in p.read_text(errors='ignore'):
        errors.append(f"{path}: missing {needle!r}")

for p in [
    'Packages/manifest.json','ProjectSettings/ProjectVersion.txt','README.md','GAME_DESIGN.md','ARCHITECTURE.md','THIRD_PARTY_ASSETS.md','CHANGELOG.md',
    'Assets/DungeonDescent/Runtime/Core/DungeonGameBootstrap.cs',
    'Assets/DungeonDescent/Runtime/Save/SaveManager.cs',
    'Assets/DungeonDescent/Runtime/Player/PlayerController.cs',
    'Assets/DungeonDescent/Runtime/Player/PlayerCombat.cs',
    'Assets/DungeonDescent/Runtime/World/DungeonWorldBuilder.cs',
    'Assets/DungeonDescent/Runtime/Enemies/EnemyBrain.cs',
    'Assets/DungeonDescent/Runtime/Boss/CryptWardenController.cs',
    'Assets/DungeonDescent/Runtime/UI/GameUI.cs',
    'Assets/DungeonDescent/Editor/DungeonProjectAutoSetup.cs',
    'Assets/DungeonDescent/Tests/EditMode/DungeonDescentCoreTests.cs',
]: req(p)

manifest=req('Packages/manifest.json')
if manifest.exists():
    try:
        deps=json.loads(manifest.read_text())['dependencies']
        expected={
            'com.unity.render-pipelines.universal':'17.3.0',
            'com.unity.inputsystem':'1.14.2',
            'com.unity.cinemachine':'3.1.5',
            'com.unity.ai.navigation':'2.0.9',
        }
        for k,v in expected.items():
            if deps.get(k)!=v: errors.append(f"manifest dependency {k} expected {v}, got {deps.get(k)}")
    except Exception as e: errors.append(f"manifest invalid: {e}")

contains('ProjectSettings/ProjectVersion.txt','6000.3.19f1')
contains('.gitignore','[Ll]ibrary/')
contains('.gitignore','.worktrees/')

# Required source visual/audio assets — no network/manual restoration.
for name in ['stone_albedo.png','stone_normal.png','wood_albedo.png','metal_albedo.png','cloth_albedo.png','moss_albedo.png']:
    req('Assets/DungeonDescent/Art/Textures/'+name)
for name in ['safe_room.wav','exploration.wav','combat.wav','boss.wav','fireplace.wav','dungeon_wind.wav','sword_swing.wav','sword_impact.wav','door_creak.wav','loot.wav','heal.wav']:
    req('Assets/DungeonDescent/Art/Audio/'+name)

# Ban explicit placeholder artifacts in project-owned runtime/art files.
banned=[r'\bCapsule\b',r'\bPlaceholder\b',r'placeholder_ui',r'TODO:',r'TBD']
for base in [ROOT/'Assets/DungeonDescent/Runtime', ROOT/'Assets/DungeonDescent/Art']:
    if not base.exists(): continue
    for p in base.rglob('*'):
        if p.suffix.lower() not in {'.cs','.md','.txt','.json','.uxml','.uss'}: continue
        text=p.read_text(errors='ignore')
        for pat in banned:
            if re.search(pat,text,re.I): errors.append(f"banned marker {pat} in {p.relative_to(ROOT)}")

# Every committed Assets file/folder should have a Unity meta file, except meta itself.
assets=ROOT/'Assets'
if assets.exists():
    for p in assets.rglob('*'):
        if p.name.endswith('.meta'): continue
        meta=Path(str(p)+'.meta')
        if not meta.exists(): errors.append(f"missing meta: {meta.relative_to(ROOT)}")

# Ensure gameplay loop vocabulary is actually present in world/bootstrap sources.
if (ROOT/'Assets/DungeonDescent/Runtime/World/DungeonWorldBuilder.cs').exists():
    text=(ROOT/'Assets/DungeonDescent/Runtime/World/DungeonWorldBuilder.cs').read_text(errors='ignore')
    for needle in ['Safe Room','Descent','Old Catacombs','Flooded Depths','Forgotten Temple','Boss Arena']:
        if needle not in text: errors.append(f"world builder missing marker: {needle}")


# Static C# sanity and architecture guardrails.
for p in (ROOT/'Assets/DungeonDescent').rglob('*.cs'):
    text=p.read_text(errors='ignore')
    stripped=re.sub(r'@?"(?:\\.|[^"\\])*"','""',text)
    stripped=re.sub(r'//.*','',stripped)
    for left,right in [('{','}'),('(',')'),('[',']')]:
        if stripped.count(left)!=stripped.count(right):
            errors.append(f"delimiter mismatch {left}{right}: {p.relative_to(ROOT)}")
    if 'GameObject.CreatePrimitive' in text:
        errors.append(f"Unity primitive visual creation is forbidden: {p.relative_to(ROOT)}")
    if re.search(r'\bGameObject\.Find\s*\(', text):
        errors.append(f"GameObject.Find usage is forbidden: {p.relative_to(ROOT)}")

# Main scene must contain the real bootstrap component and its stable script GUID.
scene=req('Assets/DungeonDescent/Scenes/Main.unity')
boot=req('Assets/DungeonDescent/Runtime/Core/DungeonGameBootstrap.cs.meta')
if scene.exists() and boot.exists():
    match=re.search(r'^guid:\s*([0-9a-f]{32})',boot.read_text(),re.M)
    if not match or match.group(1) not in scene.read_text(errors='ignore'):
        errors.append('Main.unity does not reference DungeonGameBootstrap script GUID')

# Build settings must target the tracked Main scene GUID.
main_meta=req('Assets/DungeonDescent/Scenes/Main.unity.meta')
build_settings=req('ProjectSettings/EditorBuildSettings.asset')
if main_meta.exists() and build_settings.exists():
    gm=re.search(r'^guid:\s*([0-9a-f]{32})',main_meta.read_text(),re.M)
    if not gm or gm.group(1) not in build_settings.read_text(errors='ignore'):
        errors.append('EditorBuildSettings Main scene GUID mismatch')

if errors:
    print('VERIFY FAIL')
    for e in errors: print(' -',e)
    sys.exit(1)
print('VERIFY PASS')
