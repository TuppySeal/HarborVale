# Harbor Vale — driving prototype

Open the project in **Unity 6000.6.0f1**, open `Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity`, then press **Play**. The Aster Finch starts on the nearby road, ready to drive.

| Controls | Action |
|---|---|
| W / S, Up / Down | Accelerate, brake, reverse |
| A / D, Left / Right | Steer |
| Space | Handbrake |
| R | Recover on road |
| C | Chase / bonnet camera |
| Tab | Map inspection / return to driving |

Local Windows output: `Builds/HarborVale-Driving/HarborVale.exe` (build output is not stored in Git). Close an older preview and relaunch this output for the latest changes.

The stock Finch has physical suspension and tires, authored RPM-dependent torque, automatic gears, brakes/reverse and progressive steering. Includes HUD and original procedural engine audio. The city retains 24 curved roads, three labeled districts and meter-scale building placeholders. This is a driving prototype; racing/career, upgrades, traffic/police and persistence remain future work.

Measured native physics: 0–100 km/h 13.56 s; 100–0 braking 4.10 s / 52.7 m. Physics and actual Play Mode integration checks passed. Sustained physical keyboard driving feel still needs user feedback. See `artifacts/qa/M11-T03/` for reports and the observed Windows-player preview.

Read `project/STATUS.md`, `project/NEXT_TASK.md`, `project/TASKS.json` and **`project/DRIVING_HANDOFF.md`** before continuing. Layout/model history remains in `project/EXPANSION_HANDOFF.md`. Preserve the independent `Finch_HandlingTest.unity` scene. Original geometry/audio, Unity built-in shaders/font and built-in modules; no paid controller or third-party car asset.
