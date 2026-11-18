# Harbor Vale map revision 2

Canonical Unity project: `C:/Users/Justice/Documents/Unity/HarborVale`, Unity **6000.6.0f1**. Private remote: https://github.com/TuppySeal/HarborVale. The workspace `game/` is a preserved earlier map and must not be treated as the newest revision.

## Current authorized scope

Expand/scale the city and add a starter-car visual. Full handling, racing, economy, traffic, police and shop simulation remain future work. The car is a static visual prefab with a box collider and specifications; no driving controller is claimed.

## Scale and contents

- One Unity unit = one meter. Terrain approximately 3.44 x 3 km; roads occupy approximately 2.7 x 2.3 km.
- 24 named roads, 39 shared junctions, about 29.5 km of road centerline. Two-lane carriageways 7.2 m; four-lane boulevard 14 m; urban sidewalks 3 m. Road samples are at most 4.1 m apart. Minimum sampled centerline bend radius 37.8 m; maximum longitudinal grade 11.75%.
- 550 ordinary building placeholders plus 45 stable service, market, meet, landmark, lookout and event markers. Original gameplay location IDs are preserved. Building doors are 2.1 m tall, typical story spacing 3.2 m. Level forecourts/foundations and access driveways support scale inspection.
- Starter: original fictional **Aster Finch**, body 4.05 x 1.70 x 1.46 m; wheelbase 2.45 m; tire diameter 590 mm. Mass specification 1,050 kg matches existing design data; it is not yet a physics implementation. Wheel pivots, body with arch cutouts, opaque dark glazing, mirrors, lamps, grille, bumpers, handles, seats and rims are separate labeled children. Mirrors/plates extend beyond nominal body dimensions. A 1.75 m human reference stands nearby.

## Important paths in the Unity repository

- Scene: `Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity`
- Prefab: `Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab`
- Layout source: `design-data/harbor_vale_expanded_map.json`; copied import snapshot under `Assets/HarborValeGreybox/Expansion/`.
- Layout authoring: `tools/create_expanded_map.mjs`; geometry validation: `tools/validate_expanded_map.mjs`.
- Native geometry/prefab builder: `Assets/HarborValeGreybox/Editor/HarborValeExpansionBuilder.cs`.
- Controls: Play mode, 1 city, 2 docks, 3 midtown, 4 ridge, **5 car**, **6 street**, F fly, RMB look, WASD move, Q/E height, Shift faster, L labels. Inspection controls only.

## Continuation procedure

1. Read progress, check Git and preserve user edits. Inspect the saved scene before regenerating it. Never invoke the old v1 map exporter over the canonical scene.
2. For layout changes, run `node tools/create_expanded_map.mjs`, then `node tools/validate_expanded_map.mjs` from the repository root. Generator uses v1 layout only to retain original location metadata; v2 generator is authoritative for the expanded layout. Copy generated v2 JSON into `Assets/HarborValeGreybox/Expansion/harbor_vale_expanded_map.json`.
3. Preserve the current scene before choosing **Harbor Vale > Rebuild expanded map**. This explicitly replaces that scene and creates a fresh mesh-generation folder; it does not preserve manual geometry edits. Check for open Unity sessions before batch editing. Earlier generated meshes are retained to protect references; never delete them without checking dependencies.
4. Use `HarborValeGreybox.Editor.HarborValeExpansionBuilder.ValidateAndRender` to reopen/check the saved scene without regenerating. It checks materials, car visual bounds/collider dimensions, road collision raycasts and renders four camera views.
5. Inspect actual renders and update progress/evidence. Commit intended source and push the existing `origin/main`. Do not rerun the GitHub repository-creation script.

## Evidence and limits

`Verification/Expansion/` in the Unity project contains actual Unity renders and checks. The original scene is preserved there as `HarborVale_CityMap_BeforeExpansion.unity`; the pre-expansion Git commit is also recoverable. Logs and backups remain local and excluded from Git. Shareable check reports/previews are in `artifacts/qa/M11-T02/`.

This is an editable greybox with an original low-poly car model. Buildings have simple solid placeholder interiors; the coastline is a rectangular water boundary, junction dressing and traffic signals are not final, and neither driving feel nor performance has been playtested. Curved geometry/connectivity checks do not substitute for a driving test. No third-party car asset or licensed brand was used. Unity primitives, original procedural meshes and Unity's built-in font/shaders supply the visuals.

Version-matched API references consulted: [Mesh.SetTriangles](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/mesh/settriangles), [MeshCollider](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/meshcollider), [PrefabUtility](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityeditor/prefabutility).
