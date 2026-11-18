# Current status — driveable stock Aster Finch

Canonical Unity project: C:/Users/Justice/Documents/Unity/HarborVale, Unity 6000.6.0f1. Open Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity and press Play. The starter now drives with four-wheel suspension, stock RPM-dependent torque, automatic five-speed FWD transmission, progressive steering, braking/reverse, handbrake, recovery, chase/bonnet cameras, HUD and procedural engine audio.

Controls: W/S throttle, brake and reverse; A/D steering; Space handbrake; R recover; C camera; Tab map inspection/return. Arrow keys also work. Input is ignored while unfocused, with a parking handbrake, and Alt/Ctrl combinations do not trigger camera/map/recovery shortcuts.

M11-T01, M11-T02 and bounded M11-T03 are complete. The original broader 42-task gameplay roadmap remains unstarted; this does not claim stock/upgraded handling acceptance, races, economy, police, traffic or ownership persistence are complete. The expanded city remains 24 roads, 39 junctions, about 29.5 km and 550 ordinary building placeholders.

Verification: isolated native physics checks measured stock 0-100 km/h 13.56 s and 100-0 braking 4.10 s / 52.7 m. Reverse, gears/RPM, steering upright, rear handbrake, collision barrier, recovery, 8 cm curb, ramp landing and real city road contact/acceleration passed. Actual Play Mode integration passed bootstrap, acceleration/braking, HUD/audio initialization, recovery and map/driving-camera switching. Windows development export succeeded; live player HUD and physical Tab return-to-driving were visually checked. Further UI testing stopped when live user input was detected. Sustained physical keyboard driving feel and controller support are not verified; user feedback remains necessary.

Known editor-only Unity SearchDatabase indexing exception is recorded separately from runtime results. Source/data/evidence are under Assets/HarborValeGreybox/Driving/, tools/import_finch_driving_data.mjs and artifacts/qa/M11-T03/. Durable continuation guide: project/DRIVING_HANDOFF.md. The earlier workspace game/ is a historical map, not the canonical project.

Latest Windows output: Builds/HarborVale-Driving/HarborVale.exe in the Unity project. The earlier Builds/HarborVale preview was left open if still running; relaunch the latest output for the focus/overlay fixes. Private source remote: https://github.com/TuppySeal/HarborVale.
