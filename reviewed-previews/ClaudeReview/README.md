# Reviewed standalone previews

Open this folder as a separate Unity 6000.6.0f1 project. The parent main game does not import these assets because they are outside its Assets folder. This is reviewed/repaired source, not an integration of garage/audio/dashboard features into gameplay.

Scenes: Assets/ClaudeGarage/Scenes/GaragePreview.unity, Assets/ClaudeSFX/Scenes/SFXDemo.unity, Assets/ClaudeDashboards/Scenes/DashboardPreview.unity.

Garage: real static Finch, four part models, one counter, mock fitments and camera controls. Conditional effects: true Finch/Apex static models, simulated/replayed state gates, native procedural DSP audio; supercharger hardware is a preview. Dashboards: distinct Finch/Apex styles, explicit demo telemetry, trip integrates declared speed, odometer unconnected. No saved game mileage is invented.

Inputs credited: Claude's initial procedural turbo/intercooler geometry and preview concepts, plus HarborVale's original static Finch and Apex visual geometry. Codex repaired/replaced broken project wiring, prefab loading, camera, UI/dependencies, parenting/material persistence, turbo geometry and condition-gated audio/visual implementation. Original Claude projects preserved. No downloaded/paid models or brands.

Verification: actual Editor Play Mode for garage; native Windows execution for all three; screenshots/captured walkthroughs, including native generated DSP mix for effects (48 kHz stereo, 21.077 s, RMS .07332, peak .3920). State gates, no-body-physics and root-immutability checks passed. Audio aesthetic realism, physical hardware interaction and main-game integration are not claimed. These are prototype visuals/synthetic audio, not final production art.

Previews use native Unity modules (audio, particles, physics, image conversion, IMGUI, screen capture). No TMP/UGUI or third-party runtime needed. Encoding used an already-installed FFmpeg 7.0.2 for review movies; the Unity runtime does not depend on FFmpeg. See parent artifacts/qa/M13-T01, M13-T02 and M13-T03 for results. Main gameplay remains separately owned.
