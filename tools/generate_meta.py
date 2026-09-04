#!/usr/bin/env python3
from pathlib import Path
import hashlib
ROOT=Path(__file__).resolve().parents[1]
ASSETS=ROOT/'Assets'
SPECIAL={str((ASSETS/'DungeonDescent/Scenes/Main.unity').resolve()):'e4f9d452495e427e812923064dddbe29'}
def guid_for(p):
    sp=str(p.resolve())
    if sp in SPECIAL:return SPECIAL[sp]
    rel=p.relative_to(ROOT).as_posix()
    return hashlib.md5(('DUNGEON_DESCENT_V1:'+rel).encode()).hexdigest()
def write_meta(p):
    mp=Path(str(p)+'.meta')
    if mp.exists():return
    g=guid_for(p)
    if p.is_dir():
        text=f'''fileFormatVersion: 2\nguid: {g}\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'''
    else:
        text=f'''fileFormatVersion: 2\nguid: {g}\n'''
    mp.write_text(text)
for p in sorted(ASSETS.rglob('*'),key=lambda x:(len(x.parts),x.as_posix())):
    if p.name.endswith('.meta'):continue
    write_meta(p)
print('generated meta files')
