# Shadow Escape — 2D Puzzle Platformer (Unity / C#)

A complete scripting foundation for a dark, minimal 2D puzzle-platformer where the
player records and replays "shadow clones" of themselves to solve switch/door puzzles.

This package contains all gameplay C# scripts, organized the way a Unity project
should be. It does **not** include Unity scene files, sprites, or audio clips (binary
assets can't be generated here) — you'll import your own art/audio and wire the
scripts to GameObjects as described below. Every script is commented and built to be
easy to extend (add a Level 4, a new hazard type, etc.) without touching existing code.

---

## 1. Folder structure (import as-is into `Assets/`)

```
Assets/
└── Scripts/
    ├── Player/
    │   ├── PlayerController.cs   → movement, jump, double jump
    │   ├── PlayerHealth.cs       → health, damage, death, respawn
    │   └── ShadowClone.cs        → record & replay shadow clone mechanic
    ├── Environment/
    │   ├── MovingPlatform.cs
    │   ├── PressureSwitch.cs
    │   ├── Door.cs
    │   ├── KeyItem.cs
    │   ├── PlayerInventory.cs
    │   ├── Checkpoint.cs
    │   ├── LevelExit.cs
    │   └── LightFlicker2D.cs     → dynamic lighting flicker (needs URP 2D Light)
    ├── Managers/
    │   ├── GameManager.cs        → pause/death/win flow, persists across scenes
    │   ├── AudioManager.cs       → music + SFX, persists across scenes
    │   ├── SaveData.cs
    │   └── SaveSystem.cs         → JSON save/load
    ├── UI/
    │   ├── UIManager.cs
    │   ├── MainMenuUI.cs
    │   ├── PauseMenuUI.cs
    │   ├── WinLoseUI.cs
    │   └── HealthUI.cs
    └── Camera/
        └── CameraFollow.cs
```

Just copy the whole `Scripts` folder into your Unity project's `Assets` folder.

---

## 2. Project setup

1. **Unity version:** 2021.3 LTS or newer (2D template). For `LightFlicker2D.cs`
   dynamic lighting, install **Universal Render Pipeline (URP)** via Package Manager
   and enable the **2D Renderer**. Without URP, delete/ignore that one script — the
   rest of the game works fine with normal sprites.
2. **Tags** — create these Tags (Edit → Project Settings → Tags and Layers):
   `Player`, `ShadowClone`, `Hazard`, `Pushable`.
3. **Layers** — create a `Ground` layer and assign it to all platform/floor colliders
   (used by `PlayerController`'s ground check).
4. **Scenes** — create 4 scenes: `MainMenu`, `Level1`, `Level2`, `Level3`, and add
   them all to **File → Build Settings → Scenes In Build** in that order.

---

## 3. Wiring the Player

Create a Player GameObject with:
- `SpriteRenderer`, `Animator` (with `Speed`, `IsGrounded`, `VerticalVelocity` float/
  bool params and a `Jump` trigger — wired to your run/idle/jump animation clips)
- `Rigidbody2D` (Freeze Z rotation)
- `Collider2D` (e.g. `CapsuleCollider2D`), tag = `Player`
- An empty child object named `GroundCheck` positioned at the player's feet

Attach these components to the Player GameObject:
- `PlayerController.cs` — drag the `GroundCheck` child into the `Ground Check` field,
  set `Ground Layer` to your `Ground` layer.
- `PlayerHealth.cs`
- `ShadowClone.cs` — assign a **Clone Prefab** (a simple sprite + trigger collider,
  tag = `ShadowClone`, semi-transparent/dark material to look like a shadow). Press
  `E` in-game to spawn a clone that replays your last ~8 seconds of movement.
- `PlayerInventory.cs`

---

## 4. Wiring the environment

| Script | Attach to | Notes |
|---|---|---|
| `MovingPlatform.cs` | Platform GameObject (`Rigidbody2D` kinematic + `Collider2D`) | Assign an array of empty `Transform` waypoints in the Inspector |
| `PressureSwitch.cs` | Switch GameObject (`Collider2D`, Is Trigger) | Wire `OnPressed`/`OnReleased` UnityEvents in the Inspector to e.g. `Door.Open()` |
| `Door.cs` | Door GameObject (`Collider2D`) | Set `Required Key Id` to match a `KeyItem`, or leave blank and drive it entirely from a switch's UnityEvents |
| `KeyItem.cs` | Key pickup (`Collider2D`, Is Trigger) | Set a unique `Key Id` string matching the door |
| `Checkpoint.cs` | Checkpoint GameObject (`Collider2D`, Is Trigger) | Auto-saves progress on touch |
| `LevelExit.cs` | Level exit/goal (`Collider2D`, Is Trigger) | Triggers the win screen |
| `LightFlicker2D.cs` | Any GameObject with a `Light2D` | Torches, lanterns, ambient glow |

---

## 5. Managers (one-time setup, only in the first scene loaded — e.g. `MainMenu`)

Create two empty GameObjects that persist across all scenes via `DontDestroyOnLoad`:

- **GameManager** → attach `GameManager.cs`. Fill in `Level Scenes` array with
  `Level1`, `Level2`, `Level3` and `Main Menu Scene` = `MainMenu`.
- **AudioManager** → attach `AudioManager.cs`. Assign your background music clip and
  all SFX clips (jump, footstep, button click, door open, victory).

Because both are singletons with `DontDestroyOnLoad`, you only place them in the
first scene — they survive into every level automatically.

---

## 6. UI (per-scene Canvas)

Each level scene needs its own `Canvas` with:
- A HUD panel (health bar `Slider` → add `HealthUI.cs` here)
- A Pause panel (Resume / Restart / Main Menu buttons) → add `PauseMenuUI.cs` and
  wire buttons to `OnResumeClicked()`, `OnRestartClicked()`, `OnMainMenuClicked()`
- A Win panel (Next Level / Main Menu buttons) → add `WinLoseUI.cs`, wire to
  `OnNextLevelClicked()`, `OnWinMainMenuClicked()`
- A Lose panel (shown briefly on death, auto-hides on respawn) → same `WinLoseUI.cs`,
  can add an optional `OnRetryClicked()` button
- An empty `UIManager` GameObject → attach `UIManager.cs`, drag in the four panels

`MainMenu` scene needs its own Canvas with `MainMenuUI.cs` attached, wired to
New Game / Continue / Quit buttons.

---

## 7. Camera

Add `CameraFollow.cs` to `Main Camera`, drag the Player transform into `Target`.
Adjust `Smooth Time` for how "floaty" the follow feels; optionally enable `Use Bounds`
and set min/max to clamp the camera within each level's edges.

---

## 8. Building the 3 levels

Each `LevelN` scene should contain: Player, ground/platform tilemap or sprites (on
the `Ground` layer), at least one `MovingPlatform`, one `PressureSwitch` + `Door`
pair, one `KeyItem` + `Door` pair, a `Checkpoint`, and a `LevelExit`. Suggested
difficulty curve:

- **Level 1** — teaches movement, double jump, and a single pressure switch/door.
- **Level 2** — introduces the shadow clone (a switch that must stay pressed while
  the real player proceeds elsewhere) plus a key/door puzzle and moving platforms.
- **Level 3** — combines everything: multiple switches, a clone needed to hold one
  switch while the player uses a second clone/timing trick for another, hazards,
  and a longer platforming gauntlet to the exit.

---

## 9. Extending the game

- **New hazard type:** create a script that calls `PlayerHealth.TakeDamage(x)` on
  trigger — no changes needed elsewhere.
- **New puzzle piece:** anything that should react to a switch just exposes a public
  method and gets wired into the switch's `OnPressed`/`OnReleased` UnityEvents —
  zero code coupling required.
- **New level:** add the scene to Build Settings and to `GameManager`'s
  `Level Scenes` array.
- **Save data:** extend `SaveData.cs` with new fields (e.g. collected shadow-orb
  count) — `SaveSystem` serializes whatever is in that class automatically.

---

## 10. Script-to-GameObject quick reference

| GameObject | Scripts |
|---|---|
| Player | `PlayerController`, `PlayerHealth`, `ShadowClone`, `PlayerInventory` |
| Main Camera | `CameraFollow` |
| GameManager (empty, MainMenu scene) | `GameManager` |
| AudioManager (empty, MainMenu scene) | `AudioManager` |
| Moving platform | `MovingPlatform` |
| Pressure switch | `PressureSwitch` |
| Door | `Door` |
| Key pickup | `KeyItem` |
| Checkpoint | `Checkpoint` |
| Level exit | `LevelExit` |
| Torch/light | `LightFlicker2D` |
| Canvas → UIManager (empty, per level) | `UIManager` |
| Canvas → PauseMenu | `PauseMenuUI` |
| Canvas → WinLosePanel | `WinLoseUI` |
| Canvas → HealthBar | `HealthUI` |
| MainMenu Canvas | `MainMenuUI` |
