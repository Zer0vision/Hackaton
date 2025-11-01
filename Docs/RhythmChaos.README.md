# Rhythm Chaos: Shooting on Beat + Chaos Meter (Unity)

This PR adds four scripts: `Bullet`, `Weapon2D`, `ChaosMeter`, `RhythmHooks` and a small patch guideline for your `RhythmJudge.cs`.

## How to wire in Unity (short)
1. Create a `Bullet` prefab (Rigidbody2D + small Collider2D set to Trigger). Add `Bullet.cs`.
2. On the Player add `Weapon2D`, assign `muzzle` (empty Transform at the gun barrel) and the Bullet prefab.
3. In Canvas add `Image` with **Type: Filled**, **Vertical**, **Origin: Bottom**. Assign it to `ChaosMeter.barFill`.
4. Drop `RhythmHooks` anywhere and set references to your `RhythmJudge` object, `Weapon2D` and `ChaosMeter`.
5. In `RhythmJudge.cs` raise `OnHit`/`OnMiss` where verdict is made.

See the PR description for full, step‑by‑step instructions.
