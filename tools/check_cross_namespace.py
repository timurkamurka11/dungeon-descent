#!/usr/bin/env python3
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
errors = []

for source in (ROOT / 'Assets' / 'DungeonDescent').rglob('*.cs'):
    text = source.read_text(errors='ignore')
    if 'PlayerVitals' not in text:
        continue
    if 'namespace DungeonDescent.Combat' in text:
        continue
    if 'using DungeonDescent.Combat;' in text or 'DungeonDescent.Combat.PlayerVitals' in text:
        continue
    errors.append(f"{source.relative_to(ROOT)} references PlayerVitals without importing DungeonDescent.Combat")

if errors:
    print('PLAYER_VITALS_NAMESPACE_CHECK=FAIL')
    for error in errors:
        print(' -', error)
    sys.exit(1)

print('PLAYER_VITALS_NAMESPACE_CHECK=PASS')
