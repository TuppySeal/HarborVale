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

Publication verified: main commit 18ca9f173b43ee9f5ef3c5beff7ae102272328b1 on https://github.com/TuppySeal/HarborVale. Final Windows development build succeeded with 0 errors (166340233 bytes), copied to Builds/HarborVale-Driving. Source tracked working tree clean; pre-existing untracked PackageManagerSettings.asset preserved. Executables/logs stay local. Evidence: project/DRIVING_GITHUB_VERIFICATION.json.


## 2026-11-19T01:17:01.8518223Z — M12-T01 first playable trial
Added first Dockside time trial on 2,042 m/37 gates with countdown, route markers, timer/results, penalty recovery and replay/abandon/free-roam flow. Separate prototype race earnings/best record/receipt ledger uses integer rewards, atomic replace/backup, exclusive-lock re-read, first-clear once, best-only reputation and frozen historic terms. Fixtures and actual Play Mode passed; final map-abandon camera/input edge case verified. Driving regression remained 13.56 s 0-100/52.7 m braking. Windows scripted smoke passed payout/reload; actual visible GUI screenshots inspected after rejecting gray hidden-window captures. Added built-in ScreenCapture 1.0.0 for development QA, no paid dependency. Final Windows export succeeded, installed alongside older builds. No manual full-lap or medal calibration claimed. Claude supplied generator/docs, with assets pending; assigned one verified workbench separately and left its files untouched. Next: player feedback then bounded garage/economy integration.

Publication verified: https://github.com/TuppySeal/HarborVale main commit 0cf6e56f1b7d57c2ac02b70d0f76b3b1d5563c77. Installed race files and three driving integrations match the tested copy by SHA-256; tracked source clean. Existing untracked PackageManagerSettings.asset preserved. Windows build and test profile/logs remain local; Claude's art output was untouched. Evidence: project/TRIAL_GITHUB_VERIFICATION.json.


## 2026-11-19T01:49:35.4014817Z — M12-T02 paid garage Stage 1
Added G garage with exact 400-credit quote, stock/tuned torque plot, derived crank power, available balance, receipt/install state and guarded modal input. Imported cfg_finch_na_s1 and price from design data. Added schema-2 available/spending/build ledger with read-only one-time v1 import to a separate path, immutable race/purchase receipts, duplicate/stale quote protection, coherent backups and unknown-version rejection. Failed writes leave cash/build unchanged. Native 0-100 comparison: 13.56→12.84 s. Rule/migration/error tests, old trial regression and actual Play Mode passed. Windows purchase and separate-process restart passed; final opaque UI/axis cleanup visually inspected. Installed source/build alongside previous versions without overwriting scene/static prefab/test scene or user PackageManagerSettings.asset. Claude task is already running; no art/session interaction occurred. Next: user feedback and deliberate next scope.


Publication verified: main commit 6cc67d6c187dce4adfe987dd80eec6b0386edd43 on https://github.com/TuppySeal/HarborVale. Installed new garage/integration source matches tested copy by SHA-256. Final Windows build is Builds/HarborVale-Garage; tracked source clean, pre-existing untracked PackageManagerSettings.asset preserved. Reports: project/GARAGE_GITHUB_VERIFICATION.json and artifacts/qa/M12-T02/install-verification.txt.


## 2026-11-19T02:40:03.341Z — M12-T03 and M12-T04
Finished one-location paid Finch gearbox/turbo/intercooler with authored curves/spool and atomic schema3 migration. Native rule/failure/physics, Play Mode, trial regression, Windows purchase/restart and actual UI inspection passed. User then authorized separate sports testing car: added original primitive Aster Apex, AWD, higher grip/brakes, lower CG and speed-squared downforce, stronger same-category curves, sequential free test controls and own city scene/build. Native sports acceleration and Windows prerequisites/driving/recovery/no-career-writes passed. Both exports installed; SHA256 comparison confirms 46 source/assets/meta files and both player executables. Static city/Finch/testpad preserved; user PackageManagerSettings.asset untouched. No external tools/assets installed, no Claude files modified. Manual full-lap/controller/performance review remains.

Publication verified: fe4e007b4abd8341d9e61995b784ce1db9e6cf50 on origin/main. Both installed builds available; tracked source clean and user untracked PackageManagerSettings.asset preserved. Verification record: project/SPORTS_GITHUB_VERIFICATION.json.

## 2026-11-19T03:10:35.404Z — M12-T05 unified launch/test flow
Read Claude garage delivery; source exists but no saved scene/verified Unity project, likely compile issues. Left it untouched and prepared focused finish/verify/export prompt. Implemented unified Finch/Apex hub, per-scene dispatch replacing four one-shot hooks, in-app return with race abandonment/no payout and pending-valid-save guard. Added sports live 0–100/session best per build; recovery/reinstall/map/reverse/air/timeout cancellation. Early recovery attempts caught stale contact readiness; ResetAt now clears GroundedWheels and arming requires 0.35 seconds steady stop/contact state. Failed reports retained, final 47 Windows assertions passed including physical 4.42 s run, duplicate-system checks, paid reload and injected-save-failure guard/retry. Native Finch/sports/trial regressions passed. Actual UI captures inspected; source/export installed and hashes match (19 source/settings assets, 249 Windows files). Older map/prefab/testpad/players and user PackageManagerSettings preserved. No Claude integration, new career content, controller/manual full-lap/performance claim.

Publication verified: aed745ce16c98c54f465c4a4c49024fca2ac685c on origin/main. Unified player installed at Builds/HarborVale-Play; source/export match tested copies and original user PackageManagerSettings remains untracked.
