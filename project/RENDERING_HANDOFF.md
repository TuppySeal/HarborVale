# Cleaner rendering and isolated map art — M15

Canonical C:/Users/Justice/Documents/Unity/HarborVale; Unity6000.6.0f1. Latest installed Windows executable Builds/HarborVale-CleanRender/HarborVale.exe. M15-T01 complete. M15-T02/03 assignments prepared, NOT dispatched: Windows Security network-access prompt blocks external app controls. Do not claim helper progress. Domain Bridge was reconnected at user request, but the browser on this same PC is also blocked by the prompt. No firewall rules were changed. Never put connection credentials in Git.

## Rendering changes

Built-in renderer retained. ProjectSettings changes only color space Gamma -> Linear. Runtime city/sports presentation enables 4xMSAA, anisotropic filtering, HDR camera and an original soft tonemap shader; restrained trilight ambient, warm soft sun, 140m cascaded shadows, distance haze and 3km driving range. Map inspection restores 9km range, removes haze and detail culling; returning restores driving settings. Source map is not rebuilt or resaved.

Static batching selects readable MeshRenderers under the authored city root, explicitly excluding StarterCarSpecification/FinchVehicle ancestors and TextMesh. The car is a child of this root: exclusion is essential. Readable CityUnitCube resource replaces unreadable built-in cubes only on eligible static objects. Native player measured 4,249 eligible/4,247 batched renderers; 1,718 facade/door details use unnamed layers28/29 with 750m/240m limits. Keep these layers free or revise the assignment. Road/land/curb surfaces receive shadows but do not cast, clearing observed self-shadow stripes while building/car shadows remain. Sun bias0.08/normal0.6/near1 validated in captures.

Garage/race start TextMesh markers are bounded at42 pixels and fade65-130m during driving; map inspection restores authored labels. No location IDs, colliders, car handling, torque curves, parts fees, save migrations or paid transactions changed. Paid v5 and separate v1 turbo preference remain intact. CityPresentation and tone effect are scene scoped; four WheelColliders and dynamic car exclusion checked.

## Actual verification

Native source/marker rules13; enhanced rendering9; baseline comparison3; AV/camera75; workshop51; chassis54; launch/transitions47 PASS. Build succeeded with0 errors. Installed source and 250 player files SHA-verified. 13 protected source/settings files unchanged. Independent static collision digest:1,844 colliders, 9CEC99D3EEF8C4543A58B5D9559E36A84923FF8C200D023F8AD3FC051B89E657, identical before/after. Dynamic colliders excluded from that geometry digest because the car settles between frames.

RTX5070 Direct3D12,1280x800,60 FPS cap,180 wall-clock samples per view. Street SetPass169 ->116; district205 ->173. Mean frames approximately16.58ms in both builds. This proves lower render-state changes and actual static batch membership, not uncapped FPS gains. Draw-call/batch profiler counters unavailable (-1), so no invented draw-call figure. Final street/district/map screenshots reviewed; road-shadow bands removed and map overview remains usable. Native probes have no runtime errors. Subjective full-lap quality, actual desktop input and other GPUs/resolutions still need human checks.

Repeat in disposable validation project: PresentationVerification.CompareRelease creates Gamma baseline and Linear enhanced builds without regenerating city. Native baseline uses --presentation-off --render-check-dir FRESH_FOLDER; enhanced uses --render-check-dir FRESH_FOLDER. Use fresh isolated folders for --av-camera-check-dir, --workshop-check-dir, --chassis-check-dir, --launch-smoke-dir. Do not target real accounts. QA artifacts under artifacts/qa/M15-T01; logs/test accounts excluded.

## External helper continuation

Prepared exact briefs under project/map-art-coordination. Claude: C:/Users/Justice/Documents/HarborVale-Claude-Art/CityDressing/TASK.md (four roadside props). Antigravity: C:/Users/Justice/Documents/HarborVale-Antigravity-QA/CityArchitecture/TASK.md (three building prototypes). Minimal separate Unity Project folders and read-only layout/current-street references exist there. Existing brake/garage work untouched. Dispatch to new helper sessions after the blocking Windows dialog is dismissed; preserve drafts and prior work. No assignment has been sent yet.

Each helper writes only its own folder, saves native meshes/materials/prefabs, actually runs its builder and renders previews, measures bounds/counts, and exports via AssetDatabase.ExportPackage. No Blender dependency, no downloaded art, no main project/pipeline/scene edits, no runtime car scripts. Independently inspect actual files, package contents and images before any main-map integration. Rebuilding source city would destroy manual edits; read EXPANSION_HANDOFF first and preserve dependencies/backups. Renderer is owned by Codex; helpers only create isolated art assets.

Existing teal checkpoint arches remain prototype navigation geometry, including the near-camera overhead bar visible at spawn; this release does not replace that art. External buildings/roadside dressing are still pending.
