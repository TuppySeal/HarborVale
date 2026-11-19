# Claude — independent Unity garage preview

Create a separate Unity 6000.6.0f1 project at `C:/Users/Justice/Documents/HarborVale-Claude-Art/GaragePreview`. Use the installed editor at `C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Unity.exe`. Blender is unavailable and unnecessary: use original Unity primitives/procedural meshes.

Do not modify/open for editing the main project `C:/Users/Justice/Documents/Unity/HarborVale`, its scripts/scenes/packages/Git/progress or real player saves. Another AI owns the actual upgrade logic, prices, purchases and persistence. Earlier blocked workbench output stays separate.

The static source Finch is `Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab`: original procedural Unity mesh hatchback, body 4.05 x 1.70 x 1.46 m, separately named body/wheel/window/light parts. Copy it read-only with its mesh/material/meta dependencies and StarterCarSpecification only, or import the prepared visual-only unitypackage when supplied. Never copy gameplay bootstraps/controllers/save scripts.

Create `Assets/ClaudeGarage/Scenes/GaragePreview.unity` with one plausible garage bay, the Finch, lights, workbench, part display, orbit/zoom camera and a single counter/menu for all parts. All parts are sold in one location for now; do not build separate shops.

Selectable visual displays: Stage 1 engine tune, gearbox support, turbo kit, basic/street intercoolers. Make simple original turbo/intercooler meshes, selection highlighting and short descriptions. Approximate mounting views should be labeled previews. Use handoff IDs `engine_stage1`, `gearbox_stage1`, `turbo_stage2`, `intercooler_stage1`.

This is an interaction/art preview. Do not implement authoritative prices, real purchases, performance or saves; clearly mark any example values as preview data. Namespace all scripts `ClaudeGaragePreview`; put authored assets under `Assets/ClaudeGarage`.

Deliver a saved running scene, actual Unity garage/turbo/intercooler screenshots, reusable prefabs, root STATUS.md/NEXT_TASK.md and an asset/interaction manifest. Report performed checks and unverified items. Finish this one garage before more rooms/shops. Do not integrate into the main game; we will review/import later.
