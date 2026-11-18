# Session log

Append concise checkpoints. Do not replace earlier history with a new narrative.

## 2026-11-18T03:10:31Z — Initial planning package

- User direction: Windows PC, open-world racing, GTA-like semi-realistic handling, parts/car upgrades, gradual wealth and AI continuation across context limits.
- Created: game design documents, reference economy/car/upgrade/event/scope data, 37-task roadmap, builder prompts and handoff records.
- Implementation: not started. No Godot project, runtime checks or playable build.
- Reviews: separate economy, world/gameplay and implementation-roadmap reviews identified and resolved conflicting scope, pause, recovery, initial-save and content-filter rules.
- Design validation: 12 check groups passed for reference data, economy examples, numerical gate reachability, task dependencies and local links; see `project/BLUEPRINT_VALIDATION.md`. No game runtime was tested.
- Next: M00-T01. Locate and verify the engine, record the environment and then create the minimal project.
- Git: no commit created as part of planning.

## 2026-11-18T03:16:25Z — Final design consistency check

- Resolved review findings: six upgrade categories everywhere, reputation-only entry gates, zero-car onboarding saves, unrestricted race pause, three-second recovery countdown, slice content manifest, preserved purchase/upgrade investment, persistent discoveries/narrative flags, and consistent campaign targets.
- Added: repeatable `tools/validate_blueprint.mjs`; all 12 check groups passed, including gold/silver numerical reachability and documented bronze skill gates.
- Implementation remains 0 of 37 tasks complete. Next action remains M00-T01.

## 2026-11-18T03:30:18Z — Believable powertrains and sequential paid tuning

- User refinement: upgrades should resemble real-life delivery, including turbo/supercharger torque curves and paid stages that progressively develop a car.
- Added `docs/10_POWERTRAIN_AND_TUNING.md` and `design-data/powertrains.json`: eight stock engines, thirteen compatible engine paths and forty-seven complete configurations. Removed independently editable peak power from car definitions.
- Replaced flat engine multipliers with net crank torque/RPM curves, derived kW, dynamic turbo availability, prompt PD supercharger response, meaningful gears and shift interruptions. Shared curve-derived power feeds physics, preview and PI.
- All six categories require the immediately previous paid stage. Engine packages have individual step prices, included supporting hardware and explicit transmission prerequisites. Save contracts preserve selected paths and historical payments; resale counts each investment once.
- Updated design, technical architecture, economics, presentation, prompts, decisions and QA. Preserved all earlier task IDs and unfinished states; added M01-T05 and M05-T05. Ledger now has 39 tasks, all not_started.
- Verification: 18 design-package check groups passed, including independent unit/force calculations, interior peak search, turbo threshold consistency, sequential cost examples and all thirteen full-build PI/budgets. No game runtime tests were run.
- Next task remains M00-T01. A builder should use the revised package and retain these rules while implementing the first prototype.

## 2026-11-18T03:54:12Z — Used cars, repair records and breakdowns

- User refinement: inspect repair history when buying cars and allow cars to break down.
- Added `docs/11_USED_CARS_MAINTENANCE_AND_BREAKDOWNS.md`, `design-data/maintenance.json` and `design-data/used_car_listings.json`. Ten initial persistent vehicles/four slice offers include the two original healthy starters. Every offer supports optional inspection; missing records remain unknown.
- Defined six component conditions, accumulated distance wear, persisted heat/protection, clear warnings and hard failures. Paid service preserves history/build; tire/brake/suspension stage purchases renew only their own new hardware. Temporary heat-only cuts can cool in live race time; hard component failure gives DNF.
- Added persistent buybacks: same identity, build/path, condition and history. Current-ownership payments are separate from lifetime receipts. Used purchase price affects resale basis; catalog MSRP still prices upgrades. Nonengine next-stage fees use installed-stage entitlement, avoiding double charges on acquired builds.
- Added free tow/diagnosis and `act_workshop_recovery`: 300 credits per valid loaner run, no reputation or owned inventory/wear, available only for all-unroadworthy ownership with insufficient minimum-repair cash. Maximum minimum quote 1,200 gives a four-job recovery bound.
- Preserved all 39 prior task IDs/states and added M02-T05, M04-T04 and M05-T06. There are now 42 implementation tasks, all not_started.
- Verification: 24 design-package check groups passed, including unique vehicle/listing/history IDs, chronology, all 64 unsafe-component combinations, repaired/before valuations, exact repair budgets, preinstalled-stage pricing and useful heat thresholds. No runtime game behavior is claimed.
- Next task remains M00-T01. The revised archive replaces the earlier handoff package.

## November 18, 2026 — Feature questionnaire answers

Recorded all twenty answers in docs/12_CONFIRMED_FEATURE_CHOICES.md. Planning-only instruction now guards AGENTS, status and next action. No implementation tasks started. Expanded scope supersedes earlier no-fuel/no-traffic/no-police/permanent-path-lock/last-car defaults; reconciliation remains planning work.

## November 18, 2026 — Authorized Unity map greybox

User requested Unity map boxes with all elements labeled. This overrides the prior planning-only hold for the map task. Found Unity6000.0.18f1 on D:. Authored native scene, source layout, annotations/camera, location index and coordinate preview. Local C# compilation passed. Actual editor invocation failed for missing license, so M11-T01 remains blocked on activation/import/render validation. Earlier42tasks untouched. Continue from MAP_HANDOFF.md.

## November18 — Unity license resolved and map rendered

User signed in; editor recognized license. Ran actual Unity compilation, scene generation/reopen, material and camera checks and four renders. Corrected label sizing based on rendered images. Finalscene HarborVale_CityMap.unity; M11-T01 done. Other42tasks unstarted; nextuserreview, no automaticgameplayexpansion.

## November18 — Unity folder import and GitHub requested

Prepared isolated GUID-preserving Assets/HarborValeGreybox import and namespaced/input-compatible scripts; compilation against Unity6000.6passed for both input modes. Destination choice and GitHub repository choice requested, not answered yet. No user Unity project modified; no repository created/pushed. GitHub integration discovered but not confirmed connected. Continue from project/UNITY_IMPORT_AND_GITHUB_HANDOFF.md.

## 2026-11-18T19:06:05.4323531Z — Unity project and GitHub delivery
Created the requested private TuppySeal/HarborVale repository and pushed 112 source files from C:/Users/Justice/Documents/Unity/HarborVale. Verified private=true and local/remote main commit 2fe07e3296c17081479eb19a2664b94d860fd7f3. Destination Unity import/render verification had passed; manual input check remains unperformed. Local Verification/ evidence preserved outside the upload. Updated handoff and connection records. Next: user reviews map; gameplay scope remains unchanged.

## 2026-11-18T19:28:30.8578321Z — M11-T02 scaled city/model completed
Expanded to 24 roads/39 junctions/29.5 km with meter-scale lanes, sidewalks, foundations, driveways and 550 building placeholders. Authored original Aster Finch hatchback prefab. Fixed spline overshoot, one unintended crossing, missing font assignment, car-view obstruction and plaza/road overlap during verification. Node geometry validation passed; Unity 6000.6 saved/reopened/rendered, checked visual dimensions and passed 288 road-collider raycasts. Actual evidence in artifacts/qa/M11-T02. User review/manual input and future driving work remain outside completion claim. Prepared existing GitHub repository update and included reproducible authoring plus design/handoff documents.

Publication verified: https://github.com/TuppySeal/HarborVale main commit 19837b69dde6c49d79b5767b8000dff9b9d9ce0c. All saved-scene asset dependencies are included in Git. Tracked working tree clean; pre-existing untracked ProjectSettings/PackageManagerSettings.asset preserved. Older unused mesh generations kept locally and excluded from Git; no source assets were deleted. Unity-generated serialization whitespace warnings were observed during Git diff checking and left untouched.


## 2026-11-18T19:53:13.8552163Z — M11-T03 stock Finch driving
Implemented runtime WheelCollider/Rigidbody FWD controller, stock torque/RPM gearbox, progressive steering, brakes/reverse/handbrake, camera/HUD/audio and nearest-road recovery. Added reproducible stock/recovery data importer and separate driveable prefab/handling test scene. Native physics checks passed (0-100 13.56 s; 100-0 4.10 s/52.7 m), followed by actual Play Mode integration checks and successful Windows export. Live Windows UI showed HUD/camera and physical Tab camera return; stopped app input when live user input was detected. Final focus/Alt shortcut guard and map-overlay cleanup passed repeated Play Mode integration. Physical sustained driving feel remains for user review. Unity editor search exception classified and recorded as K001. Existing map scene/static prefab/user PackageManagerSettings.asset preserved. Latest Windows build uses Builds/HarborVale-Driving to preserve the running earlier preview.
