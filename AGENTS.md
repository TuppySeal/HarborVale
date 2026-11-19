# Harbor Vale continuation instructions

Read project/STATUS.md, project/NEXT_TASK.md, project/TASKS.json, docs/12_CONFIRMED_FEATURE_CHOICES.md and project/EXPANSION_HANDOFF.md first. Inspect Git and preserve user changes.

This root is the canonical Unity 6000.6.0f1 project. Scene: Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity. The original planning package references game/ and Godot in historical defaults; those paths/engine defaults do not apply to this map project.

Historical stock-car authorization (superseded for the later bounded tasks below): scaled city, Aster Finch model and driveable stock-car prototype. Read project/DRIVING_HANDOFF.md. Career, economy, races, traffic/police and shops require further user direction. Do not present the driving prototype as a complete racing game. Preserve the handling test scene and stock powertrain importer. Runtime bootstrap equips the scene model; do not duplicate wheel physics or overwrite the map to install the controller.

Use one unit per meter. Preserve fictional identities, stable service/event IDs and model dimensions. Use the road source generator and validation described in project/EXPANSION_HANDOFF.md. Rebuilding explicitly replaces the map scene; preserve manual edits. Builder keeps timestamped local backups and creates a fresh generated-mesh folder. Never delete old generated assets without checking scene/prefab dependencies. Do not run legacy exporters over this scene.

Record actual verification, remaining manual checks and exact next steps in progress files after changes. Unity rendering/geometry checks do not prove driving feel or input behavior. Maintain original torque-curve, sequential paid-stage, mileage, history and transaction requirements from the design when gameplay is later authorized.

Git remote is already configured. Push intended source to existing origin/main after checks. Never recreate the repository or include credentials, Unity caches, local Verification/ logs, or scene backups.


## First time-trial expansion
User authorized choosing the next expansion. M12-T01 implements the first time trial and saved race earnings. Read project/TRIAL_HANDOFF.md before any race/economy edits. Preserve frozen historical receipts, integer currency and atomic idempotent transactions. Race earnings are a prototype account, not a fabricated car purchase or full wallet; future integration must migrate earned receipts exactly once. Do not reset a corrupted profile silently. Keep Claude's work confined to its separate workshop-art folder and do not edit that output during gameplay work.


## Paid garage expansion
Latest user continuation authorized bounded M12-T02: first paid Stage 1 Finch tune. Read project/GARAGE_HANDOFF.md before edits. Use imported authored curves and fee; keep cash, receipt and installed stage atomic and idempotent. Preserve actual historical amounts and prototype identity. New production save path is v2 with one-time validated v1 import; never repeatedly import, silently reset paid data, or overwrite unknown newer schemas. Preserve current city/static model/test scene and leave Claude's working art folder/session untouched. Future staged tunes/conversions require their real prerequisites and a recorded migration when needed.


## Latest authorized parts and sports tests
M12-T03 and M12-T04 complete. Read BOOST_HANDOFF.md and SPORTS_TEST_HANDOFF.md in project/. One-location paid Stage1/gearbox/turbo/cooler uses v3 with read-only one-time v2/v1 migration. Apex SportsTest is separate free temporary testing, never career transactions/ownership. Preserve Finch handling defaults, source city/static prefab/handling scene and Claude separate project. No full career expansion is implied. Source balance is imported from design-data; run boost importer instead of old base importer alone.

## Unified launch and sports timing
M12-T05 complete. Read project/LAUNCH_HANDOFF.md. HarborVale_Hub selects Finch or Apex; launch dispatcher initializes systems on each scene load. Do not re-add the old one-time bootstrap attributes. Keep pending valid race-save guard and no-payout abandonment on menu return. Sports timer/configuration best is session-only, separate from career data. Claude initial garage remains unverified/unintegrated; its finish prompt is in project/CLAUDE_GARAGE_FINISH_PROMPT.md.

## Smooth transmission/manual controls
M12-T06 complete. Read project/SHIFTING_HANDOFF.md. M automatic/manual, Z down/X up; manual preference is per drive, preserved through recovery/parts but not a save schema field. Keep road-speed RPM safety, kickdown hysteresis, modal locks and authored torque/price invariants. Native/window checks do not substitute for subjective full-lap/controller verification. Latest player Builds/HarborVale-SmoothDrive.

## Staged workshop release
M14-T02 complete; read project/STAGED_WORKSHOP_HANDOFF.md first for parts/save edits. Latest player Builds/HarborVale-CustomWorkshop. Runtime ExpandedWorkshop owns the G interface for paid Finch and free Apex; retain legacy four-offer APIs for historical tests. Source catalog design-data/expanded_workshop.json, importer tools/import_expanded_workshop.mjs. Production v4 path imports validated v3/v2/v1 once read-only; preserve receipts and prototype IDs. New built-engine stages supersede only the previous bounded Stage2 power ceiling; stock and original four fees/curves stay exact. Twin kits Apex-only, induction/sound choices exclusive, shared upgrades retained. Dedicated child DSP source prevents HUD audio startup conflict. Do not infer heat/fade, physical hardware inventory, odometer/history or full career from staged parts. Preserve original city/static model/handling scene and Claude separate art.


## Latest chassis and staged launch expansion
M14-T03 complete. Read project/CHASSIS_HANDOFF.md before edits. Latest player Builds/HarborVale-Chassis. 49 offers with paid chassis hardware, free bounded parked setup receipts and v5 one-time validated migration from v4/v3/v2/v1. Preserve historical amounts, original fees/curves, free Apex isolation and all protected scene/prefab assets. W+S stopped first-gear brake staging carries finite actual rev/spool state; countdown brakes block early release. Burnout effects must follow grounded powered contact slip. WHP is estimated, mass includes aero hardware. Human handling/listening review remains; no thermal/wear/full-career scope implied.


## Latest turbo audio expansion
M14-T04 complete. Read project/TURBO_AUDIO_HANDOFF.md. Latest player Builds/HarborVale-TurboAudio. Paid state/catalog remain v5/49offers. BOV/flutter is a free cosmetic preference stored separately; Apex temporary. Release follows actual boosted driver lift, never key-only/NA/Roots effects. Preserve real state gates, atomic settings error handling and audio-thread safety. Procedural sound is not real-car recording or physical surge/valve simulation.


## Latest driving presentation/camera polish
M14-T05 complete. Read project/AV_CAMERA_HANDOFF.md. Latest player Builds/HarborVale-PolishedDrive. RMB/J/L/K view controls must stay separate from vehicle steering and release cursor on modal/focus/disable/scene exit. Preserve visual-only wheel interpolation, real audio/slip/load gates, finite sample-rate ramps and scene fade. Paid v5/catalog49 and separate v1 turbo preference unchanged. Full-lap subjective listening and physical-input checks remain manual; no further gameplay expansion implied.
