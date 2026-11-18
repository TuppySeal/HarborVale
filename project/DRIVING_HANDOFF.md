# Stock Finch driving prototype

Canonical Unity project: `C:/Users/Justice/Documents/Unity/HarborVale`, Unity **6000.6.0f1**. The user authorized making the starter car driveable. This is the stock driving prototype, not the completed racing/career game.

## How to play

Open `Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity` and press **Play**. The runtime equips the existing Finch and starts it on the nearby road.

| Control | Action |
|---|---|
| W / Up | Accelerate forward; brake if reversing |
| S / Down | Brake; reverse after slowing |
| A / D or Left / Right | Steer |
| Space | Rear handbrake |
| R | Recover upright on the nearest road |
| C | Chase / bonnet camera |
| Tab | Park and inspect the map; press again to drive |

In inspection mode, previous map controls remain available: 1–4 district views, 5–6 car views, F fly, WASD move, Q/E height, L labels. Return to driving before using vehicle controls. Click the Game view if Unity is not receiving keyboard input.

## Implementation boundaries

- `Assets/HarborValeGreybox/Driving/Scripts/FinchVehicle.cs`: Rigidbody motion, four WheelColliders, progressive steering, brakes, handbrake, anti-roll and wheel visualization. FWD distributes total axle torque across the two front wheels. Tire grip remains physical; there is no direct velocity/speed clamp or forced yaw steering.
- `FinchPowertrain.cs`: independent per-car engine RPM and automatic five-speed gearbox, shift torque cuts, low-speed launch coupling and reverse. Stock crank torque comes from the eight authored RPM knots, with gear/final-drive/efficiency applied once. No flat horsepower upgrade multiplier or boosted engine implementation.
- `FinchChaseCamera.cs`: smooth perspective chase and bonnet view, scenery occlusion checks, speed-based field of view.
- `FinchDrivingBootstrap.cs`: equips the existing scene model after scene load, without rewriting the map or static source prefab. City road recovery uses imported centerline anchors; the test scene returns to its own starting point.
- `FinchDrivingHUD.cs`: speed, RPM, gear, control hints and an original procedural four-cylinder audio loop. Labels are hidden during driving and return for map inspection.
- `FinchInput.cs`: keyboard polling with legacy/new Input System conditional support. This project uses its existing legacy input configuration. Gamepad support has not been implemented or verified.

Reusable prefab: `Assets/HarborValeGreybox/Driving/AsterFinch_Driveable.prefab`. The original visual prefab is retained. Physics contacts and Rigidbody are assembled at runtime so the static model remains independently editable.

Handling test scene: `Assets/HarborValeGreybox/Driving/Scenes/Finch_HandlingTest.unity`. Preserve this scene through future handling changes. It contains a long marked pad, collision barrier, 8 cm curb and ramp.

## Source data and reproduction

Run `node tools/import_finch_driving_data.mjs` from the Unity repository root after changing the Finch stock specification or road layout. It reproducibly imports `design-data/powertrains.json`, `cars.json` and `harbor_vale_expanded_map.json` into `Driving/Resources/`. Do not manually maintain another torque or gear-ratio table.

The physical tire radius is 0.295 m to match the authored model; the old planning gearbox radius was 0.29 m. All other stock engine knots, gear ratios, shift duration, mass and brake specification come from design data. Imported road anchors are spaced approximately 36 m apart. Recovery changes transient pose/velocity/gear state only; no mileage/history/career data system exists yet.

Built-in Unity modules added: `com.unity.modules.vehicles` and `com.unity.modules.audio`, both version `1.0.0`. No paid assets or third-party controller package.

## Verification and continuation

`FinchDrivingVerification.Run` creates/preserves an independent handling test scene and runs manual-step Unity physics checks. Run it in an isolated copy: it replaces the active test scene and creates test objects. `FinchPlayModeVerification.Begin` runs scripted integration checks in actual Play Mode and exits its editor instance. Do not invoke these batch entry points inside an editor containing unsaved user changes. `FinchPlayerBuild.Build` exports a development Windows player to local `Verification/Driving/Windows/`.

Measured physical checks: stationary four-wheel contact; stock 0–100 km/h **13.56 s**; 100–0 km/h **4.10 s / 52.7 m**; reverse; automatic shifts and bounded RPM; steering upright; rear handbrake response; collision barrier; reset; curb crossing; ramp landing; chase camera; city road spawn/acceleration. Scripted actual Play Mode checks cover runtime bootstrap, acceleration/braking, HUD/audio initialization, recovery and returning from map inspection.

Actual reports/renders: `artifacts/qa/M11-T03/`. These physics/integration tests do not replace subjective handling feedback. The 13.56 s acceleration is slightly outside the earlier 10–13 s tuning hypothesis and is recorded honestly; tune only after feedback. Stock/upgraded comparisons, controller support, tire-surface differentiation, skid effects and full performance profiling remain future work. No upgrade purchases, fuel/maintenance/history, races, police or traffic were added.

Known editor issue: this Unity version's SearchDatabase startup indexing throws an editor-only `ArgumentOutOfRangeException` in the isolated verification project. Its stack is recorded separately in Play Mode evidence and is excluded only from the runtime-controller exception gate. It is not shipped in the Windows player. Other runtime exceptions still fail verification.

Version-matched references: [WheelCollider](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/wheelcollider), [suspension](https://docs.unity.com/en-us/engine/6000.6/manual/physics-section/physics-overview/collision-section/collider-shapes/wheel-colliders/suspension), [GetWorldPose](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/wheelcollider/getworldpose), [Rigidbody](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/rigidbody).

Windows player: Builds/HarborVale-Driving/HarborVale.exe. An earlier preview may still be running from Builds/HarborVale; close it and launch the latest build when ready. A live player HUD/camera screenshot and physical Tab return-to-driving were checked; further input stopped on detected user activity. Sustained physical keyboard feel, final focus-loss behavior and gamepad remain manual review items. Final keyboard-focus/overlay code passed scripted Play Mode checks and the exported player build. Logs/executables remain local, outside Git.

