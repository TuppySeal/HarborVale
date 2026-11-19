# Aster Apex sports test

Authorized separately from the career car. Unity 6000.6.0f1, built-in renderer, original primitive coupe with existing original Finch wheel geometry. No downloaded models or Blender dependency.

Scene: Assets/HarborValeGreybox/SportsTest/Scenes/HarborVale_SportsTest.unity. This copies the city geometry under a different scene name. Existing career race/garage bootstraps only run in HarborVale_CityMap, so the sports scene has no account, paid transactions, rewards or career records. All five test configurations are free and temporary. Restart returns to stock. It is not a newly owned career car.

Same driving controls: W/S, A/D, Space, R recovery, C camera, Tab map. Stop below 5 km/h and click the bottom test-parts buttons. Install Stock → Stage 1 → Gearbox → Turbo/basic intercooler → Street intercooler in order. Earlier configurations can be selected for comparison, then later steps must be reapplied in order.

1250 kg; AWD with 45% front torque; forward grip 1.70, lateral grip 1.65; downforce = 2.2 × speed in m/s squared in newtons (about 1698 N at 100 km/h), drag coefficient-area 0.80 m²; lower CG, stronger springs/antiroll/brakes. These are fictional prototype handling parameters, not a simulation validation. Default Finch values remain unchanged. Downforce requires two grounded contacts and acts along vehicle-down; no airborne road magnet or yaw correction.

Authored RPM curves live in design-data/sports_test.json; import using tools/import_sports_test.mjs with destination Resources directory. Stock 300 Nm at 4500 RPM; Stage 1 335; turbo 480; street cooler 510. Turbo uses existing per-car spool and complete off-boost/steady curves. The street cooler improves the matched steady curve; there is no separate thermal simulation. Gearbox shortens shifts 0.360 → 0.2808 seconds. No flat horsepower gain or second boost multiplier.

Native physics measurements: stock 6.10 s, Stage 1 5.56 s, gearbox 5.42 s, turbo 4.54 s, street cooler 4.44 s for 0–100 km/h. These are automated straight-line tests on the handling pad, not a manual full-lap judgment. Peak crank power respectively 164.2, 188.3, 188.3, 269.7 and 288.9 kW.

Verification source: SportsTest/Editor/ApexVerification.cs; runtime opt-in Windows probe ApexPlayerProbe accepts --sports-smoke-dir followed by an absolute isolated output directory. Probe checks sequential upgrades, moving-install lock, actual loaded driving/spool, recovery and byte-unchanged career primary files, and captures real player screenshots. Do not claim rendered/player verification until artifacts/qa/M12-T04/player-smoke.txt passes.

Verified authoring copy is .work/UnityBoostValidation. Source and Windows export are installed in the canonical project; Windows smoke and visual inspection passed. Preserve the canonical city scene, static Finch prefab and handling test scene when copying source. The standalone sports scene has its own GUID.
