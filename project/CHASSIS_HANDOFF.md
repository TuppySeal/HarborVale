# Chassis, dyno and staged launch — M14-T03

Unity 6000.6.0f1. Canonical C:/Users/Justice/Documents/Unity/HarborVale. Latest Windows executable: Builds/HarborVale-Chassis/HarborVale.exe. Prior CustomWorkshop player is preserved. Close older players; after v5 migration, older executables still write their separate older save and will not sync later earnings.

## Controls and workshop

Stop below 5 km/h near the city player garage and press G. Apex's separate test workshop opens while stopped and is free/temporary. There are now 49 offers, including three paid sequential stages each of limited-slip differential, suspension, gearing and aero. All previous 37 offers, original four fees, net torque curves, paid history, fictional IDs and vehicle identities remain. See STAGED_WORKSHOP_HANDOFF.md for induction, engine, tire, cooling, sound and conversion rules.

Open Chassis setup at the bottom of the shop. Hardware unlocks settings; applying adjustments is free. Sliders edit a draft until Save setup. Reset draft resets the draft settings, without removing purchased hardware. Back to parts discards uncommitted editing when reopened. Invalid or non-descending ratios reject before physics/save. UI shows locked controls, real ratios, installed stages, mass and current/preview performance. Setup is parked only; Enter cannot buy a hidden part while editing.

Differential stages provide maximum torque-bias ratios 2:1 / 3:1 / 4:1; the 0–100% lock slider controls transfer response. Torque is conserved and biased to the slower wheel; this is a simplified traction aid, not a full clutch/friction differential simulation. Stock is an open equal-torque split. Suspension Stage1 changes real springs/dampers/anti-roll; Stage2 unlocks ride height -30..+10mm, springs/damping 80..120%, anti-roll 70..130%; Stage3 has firmer base hardware. Gearing Stage1 unlocks final drive +/-15%; Stage2 +/-20% final drive and +/-10% individual ratios; Stage3 +/-25% and +/-20%. Wing setting 0–100% changes real speed-squared downforce and drag. Aero stages add 8 / 14 / 22kg plus visual splitter/wing; visuals never add wheel physics or colliders. Stock Apex wing is hidden while an aftermarket kit is installed and restored otherwise.

## Brake boosting and wheelspin

While stopped in first gear, hold W + S (accelerator + brake). HUD displays STAGING and the target RPM. Keep W held and release S to launch. Target is calibration-dependent: Finch NA 3400/3800/4000/4200 by engine stage; small turbo 3600/3900/4200, large turbo 4000/4200/4400, Roots 3500/3800/4100, twins 3800/4100/4400. Apex targets add 200RPM. The current target is visible; no separate launch purchase is required. Paddle shifts reject while staging. Gas lift, handbrake, focus loss and modal locks cancel holding. After lifting gas, release/repress S before normal reverse resumes.

Staging requires front grounded contacts, speed below 2.5km/h and first gear. It feeds partial clutch preload through the existing torque curve while brakes hold, builds limited turbo spool from the authored load fraction, and carries actual rev/spool state through a finite 0.5s release. No free launch-force multiplier. During the race countdown staging is allowed behind full brakes; releasing before green cannot launch through the countdown lock. Rolling brake boosting, clutch/torque-converter thermodynamics, wear and heat penalties are not implemented.

Driven-wheel RPM now actually back-drives the crank and limiter without the old artificial road-RPM cap. Manual downshift rejection uses actual crank and road-speed projections. Excess tire speed and grounded powered contact slip cause wheelspin, tire squeal, smoke and transient rubber trails. No key-triggered burnout or tire destruction. Track tires and gentler throttle reduce spin; upgrades do not guarantee zero wheelspin. Reset clears trails, particles and staging. Built-in particle module added; no external asset dependencies. Visuals/effects do not write wheel physics.

## Dyno and saves

Dyno defaults to crank HP plus estimated WHP; Torque toggles back to Nm. HP = peak kW * 1.34102209; estimated WHP = crank HP * authored drivetrain efficiency. This is a curve-derived estimate, not a roller measurement. Actual vehicle mass includes aero hardware. Chassis adjustments cannot invent crank power.

Production save: LocalAppData/HarborVale/Prototype/race-progress-v5.json. With no v5 file, load v4 then v3/v2/v1 only once, validate immutable paid receipts/cached build and materialize v5. Old file contents remain unchanged. Paid stage transactions retain workshop_v4 identity/fees. Free adjustment receipts have kind=setup, partId=chassis_setup, configId=setup_v1, pathId=workshop_v5, credits=0 and canonical setupJson. Setup, balance, build and receipt are atomic/idempotent. Unknown schema6+ or corrupt paid state rejects; never silently reset. Actual prior-release v4 multi-stage paid fixture migrated and source remained byte-unchanged. Apex settings exist only during its test visit and cannot touch the paid Finch ledger.

## Verification and next checks

Native editor rules: chassis 23, brake boost 12, workshop 225, retained garage 18, boost 27, trial 25, shifting 104. Native Windows chassis 54, existing workshop 51 and transitions 47 passed. Final player startup/runtime-error monitoring passed. Captures inspected for staged HUD, HP/WHP/kg dyno, locked/adjustable UI at 720/900 heights, actual contact smoke and visible aero. Build succeeded with zero errors. Original city/static Finch/handling scene hashes unchanged; all 249 installed player files and 46 source/data files SHA-verified. QA artifacts omit test account files and runtime logs.

Measured six-second high-power Finch launch: stock tires peak excess 13.46m/s, slip integral20.69, burnout1.70s, 81.1km/h; track tires9.29m/s, integral16.11, burnout1.08s,86.4km/h; 45% throttle1.92m/s, integral6.39, no burnout,53.7km/h. Same large-turbo/track-tire three-second fixture: ordinary18.31m/41.1km/h, staged20.00m/42.5km/h. These are controlled prototype fixtures, not universal real-world predictions. Existing stock/Stage1 acceleration remained13.46s/12.72s. Human full-lap driving feel, actual keyboard/controller operation and listening remain manual checks. No full career, authoritative odometer/service history, brake fade, tire wear or thermal simulation is claimed. Claude/Antigravity original projects and artwork remain untouched and unintegrated.

To repeat tests use BrakeBoostVerification.Release in a disposable Unity copy, then ChassisPlayerProbe --chassis-check-dir, WorkshopPlayerProbe --workshop-check-dir and launch --launch-smoke-dir. The historical migration test additionally needs the prior-release local v4 fixture at artifacts/qa/M14-T02/final-player/isolated-v4.json (relative to the validation copy layout); saved test accounts are intentionally not published. Never run probes against a real paid account. Next step is human review of launches, mixed braking/reverse, countdown staging and chassis handling before more gameplay expansion.

## New authored prices (credits)

| Offer | Price | Required offer IDs |
|---|---:|---|
| Limited-slip differential Stage 1 | 550 | None |
| Limited-slip differential Stage 2 | 1000 | differential_1 |
| Limited-slip differential Stage 3 | 1600 | differential_2 |
| Suspension Stage 1 | 500 | None |
| Suspension Stage 2 | 1050 | suspension_1 |
| Suspension Stage 3 | 1800 | suspension_2 |
| Adjustable gearing Stage 1 | 400 | None |
| Adjustable gearing Stage 2 | 850 | gearing_1 |
| Adjustable gearing Stage 3 | 1450 | gearing_2 |
| Aero package Stage 1 | 500 | None |
| Aero package Stage 2 | 1150 | aero_1 |
| Aero package Stage 3 | 1900 | aero_2 |
