# Harbor Vale continuation instructions

Read project/STATUS.md, project/NEXT_TASK.md, project/TASKS.json, docs/12_CONFIRMED_FEATURE_CHOICES.md and project/EXPANSION_HANDOFF.md first. Inspect Git and preserve user changes.

This root is the canonical Unity 6000.6.0f1 project. Scene: Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity. The original planning package references game/ and Godot in historical defaults; those paths/engine defaults do not apply to this map project.

Current authorization: scaled city, Aster Finch model and driveable stock-car prototype. Read project/DRIVING_HANDOFF.md. Career, economy, races, traffic/police and shops require further user direction. Do not present the driving prototype as a complete racing game. Preserve the handling test scene and stock powertrain importer. Runtime bootstrap equips the scene model; do not duplicate wheel physics or overwrite the map to install the controller.

Use one unit per meter. Preserve fictional identities, stable service/event IDs and model dimensions. Use the road source generator and validation described in project/EXPANSION_HANDOFF.md. Rebuilding explicitly replaces the map scene; preserve manual edits. Builder keeps timestamped local backups and creates a fresh generated-mesh folder. Never delete old generated assets without checking scene/prefab dependencies. Do not run legacy exporters over this scene.

Record actual verification, remaining manual checks and exact next steps in progress files after changes. Unity rendering/geometry checks do not prove driving feel or input behavior. Maintain original torque-curve, sequential paid-stage, mileage, history and transaction requirements from the design when gameplay is later authorized.

Git remote is already configured. Push intended source to existing origin/main after checks. Never recreate the repository or include credentials, Unity caches, local Verification/ logs, or scene backups.


## First time-trial expansion
User authorized choosing the next expansion. M12-T01 implements the first time trial and saved race earnings. Read project/TRIAL_HANDOFF.md before any race/economy edits. Preserve frozen historical receipts, integer currency and atomic idempotent transactions. Race earnings are a prototype account, not a fabricated car purchase or full wallet; future integration must migrate earned receipts exactly once. Do not reset a corrupted profile silently. Keep Claude's work confined to its separate workshop-art folder and do not edit that output during gameplay work.


## Paid garage expansion
Latest user continuation authorized bounded M12-T02: first paid Stage 1 Finch tune. Read project/GARAGE_HANDOFF.md before edits. Use imported authored curves and fee; keep cash, receipt and installed stage atomic and idempotent. Preserve actual historical amounts and prototype identity. New production save path is v2 with one-time validated v1 import; never repeatedly import, silently reset paid data, or overwrite unknown newer schemas. Preserve current city/static model/test scene and leave Claude's working art folder/session untouched. Future staged tunes/conversions require their real prerequisites and a recorded migration when needed.

