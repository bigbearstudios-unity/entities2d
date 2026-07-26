# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- `Tests/` assembly with coverage for `InputBuffer`, `InputState`, `UnityActionMapping` type flags, `CoyoteTime`, `EffectController`'s effect lookup, and `EntityController.FindReflectedFields<T>()`.
- Coyote time wired into `PlayerMovementController`'s jump logic — a jump input shortly after walking off a ledge (not after a voluntary jump) is now honoured.
- `InputBuffer` — buffering policy extracted out of `UnityInputBufferedButtonAction` into its own composable class.
- Reflection results in `EntityController.FindReflectedFields<T>()` are now cached per (concrete type, target type), avoiding a repeated reflection walk on every `Awake()`.
- `MissingActionMappingException` — descriptive exception thrown by `PlayerUnityInputController.Movement`/`Axis`/`Button` when a mapping isn't configured.
- `PlayerUnityInputController._Editor_AddAxisMapping()` editor helper, mirroring the existing Movement/Button helpers.
- Inspector: setup warnings (missing Player Input, missing actions asset, no mappings, mappings pointing at nonexistent actions) and a "populate from Input Actions" section to add unmapped actions with one click.
- Execution-order rationale documented on `InputController`, `StateController`, and `AttributeController`, including a caveat that `DefaultExecutionOrder` isn't inherited by concrete subclasses.

### Changed

- `PlayerUnityInputController.HasMovementMapping` now checks `IsMovementType` instead of `IsButtonType`.
- `UnityInputAxisAction.HasXMovement`/`HasYMovement` now detect movement in both directions of each axis, not just one.
- `EntityController.FindReflectedFields<T>()` now uses `IsAssignableFrom` instead of `IsSubclassOf` (matches exact-type and interface-implementing fields), skips unassigned (`null`) fields, and no longer double-counts fields inherited from base classes.
- `StateController.AddState()` now defers each `EntityState.Start()` call until every state has been registered, and applies the initial `setState` transition only after all of them have run.
- `AnimationController.AnimationComplete` removed in favour of the existing `AnimationStateEnded` event-flag pattern; `GroundedStaggerRecoveryState`, `GroundedFlinchState`, and `GroundedDeathState` (project-level) updated accordingly.
- `PlayerMovementController.TogglePlatformCollision` now ignores collision against only the specific platform colliders overlapped at toggle-time (via `Physics2D.IgnoreCollision`) instead of the whole layer (via `Physics2D.IgnoreLayerCollision`), and always restores them via `try`/`finally`.
- `CoyoteTime`'s internal "timer not started" check now uses an explicit flag instead of a `float.MinValue` sentinel.
- `ApplyMovement()` no longer accepts `fallThroughPlatforms`/`horizontalAcceleration` — both were previously accepted but never read.
- `BBUnity Pools`: `ObjectPool.FindInScene` now returns `null` when no pool is found instead of throwing, matching what `EffectController`'s auto-create fallback expected.

### Fixed

- `EffectController.InstantiateEffect` no longer throws `KeyNotFoundException` for an unregistered effect name; logs a diagnostic instead. Null effect name/prefab entries in the inspector list now also log a useful message instead of a silent no-op or an empty error.
- `EffectController` now logs when it auto-creates a new "Effect Pool" (previously silent, and previously unreachable at all due to the `ObjectPool.FindInScene` bug above).
- `PlayerUnityInputController.Movement`/`Axis`/`Button` no longer throw a raw `KeyNotFoundException` on a missing mapping.
- Fixed a compile error in `CharacterUnityInputControllerInspector` caused by the project's auto-generated `PlayerInput` class (global namespace) shadowing `UnityEngine.InputSystem.PlayerInput`.
- Renamed `WasAirBorne` → `WasAirborne`, `toogleBackAfter` → `toggleBackAfter`, and the `"Hoizontal Movement"` inspector header → `"Horizontal Movement"`.
- Removed a stale `StateController` TODO describing a type-safety gap that isn't reachable through this package's public API.
