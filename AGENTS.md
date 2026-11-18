# Harbor Vale continuation instructions

Read project/STATUS.md, project/NEXT_TASK.md, project/TASKS.json, docs/12_CONFIRMED_FEATURE_CHOICES.md and project/EXPANSION_HANDOFF.md first. Inspect Git and preserve user changes.

This root is the canonical Unity 6000.6.0f1 project. Scene: Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity. The original planning package references game/ and Godot in historical defaults; those paths/engine defaults do not apply to this map project.

Current authorization: scaled city, Aster Finch model and driveable stock-car prototype. Read project/DRIVING_HANDOFF.md. Career, economy, races, traffic/police and shops require further user direction. Do not present the driving prototype as a complete racing game. Preserve the handling test scene and stock powertrain importer. Runtime bootstrap equips the scene model; do not duplicate wheel physics or overwrite the map to install the controller.

Use one unit per meter. Preserve fictional identities, stable service/event IDs and model dimensions. Use the road source generator and validation described in project/EXPANSION_HANDOFF.md. Rebuilding explicitly replaces the map scene; preserve manual edits. Builder keeps timestamped local backups and creates a fresh generated-mesh folder. Never delete old generated assets without checking scene/prefab dependencies. Do not run legacy exporters over this scene.

Record actual verification, remaining manual checks and exact next steps in progress files after changes. Unity rendering/geometry checks do not prove driving feel or input behavior. Maintain original torque-curve, sequential paid-stage, mileage, history and transaction requirements from the design when gameplay is later authorized.

Git remote is already configured. Push intended source to existing origin/main after checks. Never recreate the repository or include credentials, Unity caches, local Verification/ logs, or scene backups.

