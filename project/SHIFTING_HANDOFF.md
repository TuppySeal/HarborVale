# Smooth driving and manual paddles — M12-T06

Latest source: canonical Documents/Unity/HarborVale, Unity 6000.6.0f1. Verified authoring copy: .work/UnityBoostValidation. Latest Windows export is installed under Builds/HarborVale-SmoothDrive/HarborVale.exe; prior players are retained. Do not overwrite a running old executable.

## Controls and behavior

- M toggles automatic/manual. Z downshifts, X upshifts; successful paddle requests select manual. The same actions have clickable HUD buttons. No clutch pedal is required. Automatic is the default on a new drive/scene visit.
- Safe downshifts are RPM-checked against road speed (94% of redline). Invalid gear limits, reverse paddles and overlapping shifts are rejected with HUD feedback. Manual otherwise holds gear at the limiter, without an automatic upshift. No damage/stall simulation was added.
- Manual preference survives recovery and hardware installation for the current car. Rolling to a stop selects first while keeping manual mode. An intentional stationary manual upshift can still select second for launch. Preferences are session-only; no progress schema change.
- Automatic downshifts to a road-speed-matched gear on deceleration and uses strong-demand kickdown after throttle lift or when lugging. RPM margin, torque benefit and shift cooldown prevent hunting. Loaded wheel slip can trigger a sensible upshift above a minimum road-speed threshold; continuous full throttle does not immediately kick back down.
- RPM and delivered wheel torque use the newly selected ratio in the same step. Shifts cut power through the first 70% of their authored duration and smoothly reconnect through the final 30%. Paid gearbox packages retain their shorter actual duration.
- Keyboard throttle ramps in around 0.18 s; brakes around 0.10 s. Direction-opposed input and control locks cut drive immediately. Modest closed-throttle engine braking uses the off-boost curve; rolling resistance remains separate. Existing engine-loop pitch follows RPM smoothly. Steering/grip/camera tuning and authored torque curves/prices are unchanged.

Shift controls are blocked by the garage/countdown lock and map input lock. Keyboard uses the existing focused-window input path. Physical keyboard/mouse driving feel and gamepad hardware have not been manually verified; do not claim controller support.

## Evidence

Shifting/Editor/ShiftVerification.cs uses the actual former gearbox in Editor/LegacyFinchPowertrain.cs as a reproducer. In a synthetic top-speed-to-30-km/h fixture, after 0.6 s the old code was still in fourth (Finch) or fifth (Apex), while the revised gearbox selected first safely. These are synthetic decision fixtures, not measured physical stopping distances.

Native rigidbody/WheelCollider tests on a temporary flat straight verify high-speed braking, lower-gear selection and reacceleration. Current measurements: Finch stock 0–100 13.46 s, 30–100 after braking 12.02 s; Apex upgraded 0–100 4.32 s, 30–100 after braking 3.62 s. Current paid Finch Stage1/basic turbo/street-cooler 0–100: 12.72/10.48/10.28 s. These supersede previous-release acceleration baselines; no flat horsepower boost was added.

Rule checks include RPM safety, shift overlap/limits/reverse rejection, bounded partial clutch engagement, manual holding, standstill assist, loaded-slip upshift without hunting, coast stability, throttle/brake blending, preference preservation and modal locks. Paid-parts/transaction/spool/migration fixtures and trial-rule regressions are retained. Windows probes exercise live manual acceleration, unsafe downshift, safe upshift, automatic kickdown and real map/control locks; the existing unified-player probe checks paid saves, sports isolation, scene transitions and timing.

Reports/captures: artifacts/qa/M12-T06. Development-only --shift-smoke-dir tests sports mode without opening a career store. The separate --launch-smoke-dir probe seeds an explicit fresh isolated profile only. Never seed user progress. Keep actual native reports and UI inspection separate from manual driving feel.

No Claude assets/project files were modified or integrated. Its garage/audio previews stay separate. Full-lap feel, road seams, camera preferences, long-run frame pacing and controller work remain future review items.
