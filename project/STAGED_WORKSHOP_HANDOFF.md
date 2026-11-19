# Staged workshop — M14-T02

Unity 6000.6.0f1. Canonical C:/Users/Justice/Documents/Unity/HarborVale. Latest Windows player: Builds/HarborVale-CustomWorkshop/HarborVale.exe. Prior Workshop/SmoothDrive players retained. Close older players; use this release after v4 migration because older executables continue writing their older separate saves.

Stop below 5 km/h and press G. Finch requires the player garage near spawn; Apex can open its free test workshop while stopped. Same searchable, categorized interface for both cars. Click a stage to inspect its full prerequisite path, price and complete-build torque/power/shift/brake comparison. Enter or Install buys the selected visible part. Escape first leaves search, then Escape/G returns to driving. Empty searches cannot buy a hidden prior selection. Apex has no account or cash counter; parts reset each visit. Acceleration session records are keyed by the full build, with old five-package compatibility preserved.

37 offers: three NA engine stages; three gearbox stages; three stages each of small turbo, large turbo, Roots supercharger and Apex-only twins; three cooling stages; three brake stages; three tire stages; two intake and two exhaust stages; road calibration and two stages each of burble/flame. A separate paid removal service returns to NA. Stage1 induction choices are full kits; changing family or returning to a previous kit is a new full-kit payment, with no refund/spare inventory. Subsequent active-family stages are sequential. Shared cooling/gearbox/brakes/tires/intake/exhaust payments persist through conversions. Cooling plumbing compatible with the replacement kit is included. NA2/3 affect NA curves; paid NA3 built-engine hardware supports induction Stage3 without stacking the NA power curve on top of boost.

Authoritative source design-data/expanded_workshop.json, imported using tools/import_expanded_workshop.mjs to Workshop/Resources/ExpandedWorkshop.json. Complete per-car net torque arrays, off-boost arrays, RPM onset/spool, matched cooling calibration, intake/exhaust RPM contributions, brake targets, shift and tire modifiers. Small turbo1/basic and cooling1 preserve original authored curves exactly; original four fees remain 400/250/1900/400. Higher newly authorized built-engine stages extend the old bounded Stage2-only Finch power ceiling; runtime authoring ceiling 145 kW Finch / 450 kW Apex. Stock car/handling parameters are unchanged at zero upgrades. Tire3 increases actual longitudinal/lateral friction by 20%; this does not add downforce or engine power.

Sound choices exclusive. Burble stages lose 2%/4% net torque/power; flame stages lose 4%/6%; road restores normal curves. Loss applies exactly once to resolved full and off-boost curves, never compounds on reload. Stage2 sound has a longer/louder synthetic pop and larger/brighter temporary flame. A dedicated child owns one source/filter to avoid the old HUD sound startup race. Engine/turbo/SC/tire effects read real driving telemetry; SC whine requires installed roots hardware, turbo load/boost/ground, squeal actual wheel slip. Overrun effects respond to high-RPM pedal lift with a cooldown, independent of the smooth throttle actuator decay, with no shift-only false trigger. Flame geometry/light is visual only. Audio is procedural prototype synthesis; subjective realism/listening is unverified.

Production save: LocalAppData/HarborVale/Prototype/race-progress-v4.json. First load with no v4 file validates and materializes v3 then v2 then v1 import, read-only for old file contents; later loads never reimport. Preserve profile/prototype IDs, frozen historical receipt credits and earnings. Each stage/choice/payment writes atomically; replay of immutable old and new receipts must match the installed WorkshopBuild and integer balance. Unknown version5/corrupt paid state rejects without reset; validated backups remain supported. No physical car purchase/history/odometer or complete career wallet is invented.

Verification: 177 new native configuration/save/rule checks; legacy garage 18, boost 27, trial 25, shifting/physics 104. Native Windows workshop 51 checks, including paid multi-stage SC/tires/sound, actual SC whine and high-RPM lift pop, free Apex twins, repeated selection and byte-unchanged paid ledger; 47 existing scene/timing/save-failure checks. Actual 1280x720 and1600x900 screenshots inspected. Initial audio conflict was caught by transition test, then fixed using a dedicated child DSP source and runtime-error monitoring. Failed intermediate runs remain local; final evidence under final-player and final-transitions.

Brake target upgrades apply real increasing front torque: 724.8 / 835.6 / 926.2 / 1016.8 Nm in the native fixture. Stopping distances were equal (36.87m from100 to2 km/h) on stock tires in that fixture, so no shorter single-stop distance claim is made. No brake fade/heat/ABS or cooling heat-soak simulation exists. Physical keyboard/wheel/controller/full-lap feel, listening and sustained performance still need human review. Claude brake artwork remains separate and is not used as proof of brake gameplay. Original city/static Finch/handling scene SHA256 preserved; tested source and all 249 installed player files verified by SHA256.

## Authored price list (credits)

| Offer | Price | Required offer IDs |
|---|---:|---|
| Engine / NA Stage 1 | 400 | None |
| Engine / NA Stage 2 | 850 | na_1 |
| Engine / NA Stage 3 | 1650 | na_2, intake_1, exhaust_1 |
| Gearbox Stage 1 | 250 | na_1 |
| Gearbox Stage 2 | 650 | gearbox_1 |
| Gearbox Stage 3 | 1150 | gearbox_2 |
| Small turbo Stage 1 | 1900 | na_1, gearbox_1 |
| Small turbo Stage 2 | 1200 | induction_small_1 |
| Small turbo Stage 3 | 1800 | induction_small_2, na_3, gearbox_2, cooling_2 |
| Large turbo Stage 1 | 2400 | na_1, gearbox_1 |
| Large turbo Stage 2 | 1500 | induction_large_1 |
| Large turbo Stage 3 | 2250 | induction_large_2, na_3, gearbox_2, cooling_2 |
| Supercharger Stage 1 | 2650 | na_1, gearbox_1 |
| Supercharger Stage 2 | 1500 | induction_roots_1 |
| Supercharger Stage 3 | 2250 | induction_roots_2, na_3, gearbox_2, cooling_2 |
| Twin turbo Stage 1 | 3750 | Apex only |
| Twin turbo Stage 2 | 1950 | Apex only |
| Twin turbo Stage 3 | 2700 | Apex only |
| Charge cooling Stage 1 | 400 | na_1, gearbox_1 |
| Charge cooling Stage 2 | 750 | cooling_1 |
| Charge cooling Stage 3 | 1300 | cooling_2 |
| Street brakes | 300 | None |
| Sport brakes | 600 | brakes_1 |
| Track brakes | 1050 | brakes_2 |
| Intake Stage 1 | 300 | na_1 |
| Intake Stage 2 | 600 | intake_1 |
| Exhaust Stage 1 | 450 | na_1 |
| Exhaust Stage 2 | 900 | exhaust_1 |
| Road exhaust calibration | 100 | None |
| Burble tune Stage 1 | 300 | na_1, exhaust_1 |
| Burble tune Stage 2 | 600 | sound_burble_1, exhaust_2 |
| Flame tune Stage 1 | 500 | na_1, exhaust_1 |
| Flame tune Stage 2 | 1000 | sound_flame_1, exhaust_2 |
| Return to naturally aspirated | 750 | na_1 |
| Street tires | 250 | None |
| Sport tires | 550 | tires_1 |
| Track tires | 1000 | tires_2 |
