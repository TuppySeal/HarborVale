# Art, audio and content production

## Art direction

Use a stylized, grounded coastal city: readable shapes, believable proportions, painted metal, weathered concrete and clear road surfaces. Cars should feel desirable without requiring photorealistic interiors or licensed brands. Fixed late-afternoon light gives warm buildings, readable shadows and strong car silhouettes.

District identity:

| District | Shapes and palette | Landmark | Road readability |
|---|---|---|---|
| Dockside | Large warehouse blocks, warm rust, teal water, yellow safety paint | Gantry crane | Broad bends, generous shoulders, clear container barriers |
| Midtown | Taller simple facades, brick, pale stone, restrained shop colors | Clock/sign tower at the plaza | Contrasting sidewalks, visible junctions, repeated block rhythm |
| Ridgeway | Pine silhouettes, rock walls, dry grass, cool distant hills | Radio mast beside the summit lookout | Strong outer edge, bend signs, run-off and guardrails |

The map, characters, cars, logos and sounds are original or appropriately sourced. A reference to another game's driving feel is not an asset specification.

## Asset order

1. Greybox primitives with correct dimensions and collision.
2. One complete car art pipeline with four separate wheels and a working paint material.
3. A small modular Dockside kit, garage exterior and clear race signage.
4. Remaining slice cars, functional UI and basic audio.
5. The complete road network and event routes.
6. Remaining car bodies, district kits, rivals and finishing effects.

Do not delay handling or saving because a final car model is unavailable. Every placeholder should have an intended replacement or a deliberate decision to keep it.

## Car production brief

Eight definitions come from `03_ECONOMY_AND_CARS.md`. Match the visual body style to the data and handling identity. Each car needs:

- A recognizable exterior body, simplified underside and glass.
- Four separate wheel meshes with usable pivots; shared wheel sets are acceptable.
- A paint material supporting an inexpensive palette selection.
- Brake-light and headlight materials; lights need not cast expensive dynamic shadows.
- Chassis and wheel mounting data, consistent real-world scale and an approved collision proxy.
- A dealer thumbnail, garage framing and a short handling description.

Use separate visual and physics hierarchies. Upgrading physics must not resize wheels or move suspension mounts accidentally. V1 upgrades may be represented by stat changes and a few reusable wheel/brake visual variants. Full modelled exhausts, engine bays and body kits are optional later art.

Initial **budget hypotheses**, to be measured on the reference PC: roughly 15k–35k triangles for a close car including wheels, lower detail for distant scenery cars, mostly 1k textures with selected 2k hero textures, and shared atlases/materials for road props. These are planning envelopes, not guarantees that a scene will meet performance targets.

For import, prefer GLB/glTF with separate wheel nodes and material names. Godot's official format guide recommends glTF 2.0 and supports `.glb` and `.gltf`; direct `.blend` import requires Blender on the importing machine. [Official 3D import formats](https://docs.godotengine.org/en/stable/tutorials/assets_pipeline/importing_3d_scenes/available_formats.html).

Keep the source file or reproducible generation instructions when available. Document how to orient the model so the controller uses -Z forward. Check actual tire-ground contact visually; imported scale errors are gameplay errors.

## World kit

Required reusable pieces:

- Straight road, bend, T-junction, crossroads, curb, sidewalk, slope and guardrail.
- Road markings, barriers, race arrows, start/finish assets and service signs.
- Two or three warehouse shells, container, fence, crane silhouette and loading props.
- A few Midtown facade variants, plaza paving, awnings and street furniture.
- Rock chunks, simple trees, grass patches, lookout railings and a radio mast.
- Garage exterior/interior presentation area and a dealer forecourt.
- Distant skyline, water surface and out-of-bounds silhouettes.

Reuse materials, trim sheets and modular dimensions. Road collision is a separate reviewed layer. Decorative poles, curbs and guardrails must not produce invisible snags on intended racing lines.

No pedestrians, moving civilian traffic or enterable shops are required. The city can suggest activity through parked vehicles, lit signs, distant audio and small noninteractive visual motion.

## Screen inventory

| Screen | Required content |
|---|---|
| Main menu | Continue, new career, settings, credits, quit, version |
| Starter/dealer | Car preview, price, handling summary, unlock condition, resulting cash |
| Used-car report | Individual vehicle ID, odometer, owner/history disclosures, verified repairs, unknown fields, inspection purchase, inspected condition and all-in cost |
| Garage cars | Owned cars, selected build, active car, cash, collection value, net worth |
| Upgrade shop | Sequential stages, selected engine path, torque/kW graphs over RPM, response description, included hardware, unmet requirements, exact next-step quote, confirm/cancel |
| Sale preview | Car being sold, exact quote, resulting cash, active-car fallback |
| Workshop | Six-component condition, fault causes, minimum/full service scope, exact quote, paid receipt, upgrade-versus-service explanation |
| Roadside/recovery | Warning/failure explanation, free tow, working-car switch, free cooling or eligible loaner job, no hidden recovery bill |
| Map | Road network, garage, events, unlock explanations, selected waypoint, discoveries |
| Event preview | Mode, route, expected field, eligibility, rewards, start/back |
| Driving HUD | Speed/gear, navigation, context prompt, recovery instruction when needed |
| Race HUD | Position/laps or medal target, timer, next checkpoint, penalty feedback |
| Results | Position/time, old/new personal best, itemized cash and rep, save status, retry/exit |
| Pause/settings | Resume, settings, abandon where relevant, controls, volume, display, camera |
| Recovery/error | Actionable message, retry or recover option, no silent loss of career |

Mileage details include lifetime miles/kilometers, distance during current ownership, a resettable trip, last documented service per component and next check reminders. Keep “last inspected” separate from “last serviced,” show unknown entries honestly, and flag inconsistent readings. Mileage and speed use the player's selected unit presentation without changing stored distance or physics.

Mouse support is welcome, while keyboard and controller focus must work everywhere. Use a restrained palette, high-contrast type, visible focus rings and labels alongside icons. Money values and important prompts should remain legible over bright scenery.

Do not expose internal task IDs, schema versions or debug stack traces in ordinary player screens. Build identifiers belong in credits/settings; developer diagnostics belong in logs or the debug overlay.

## Audio inventory

Minimum slice audio: engine idle/load loop, tire slip, braking/handbrake feedback, a few impacts, confirm/back/error UI sounds, countdown/start/finish cues and quiet environmental ambience.

V1 can use three or four engine sound families mixed and pitched within reasonable limits, rather than eight completely bespoke recordings. Engine load and RPM should influence sound; avoid deriving every sound solely from speed. Tire sound responds to slip, and impacts use strength with cooldowns to prevent rapid repeated noise.

Turbo whistle tracks actual modeled spool/load, a short lift sound follows appropriate throttle release, and a positive-displacement blower uses an RPM-related whine. Naturally aspirated paths retain their own induction character. These sounds follow the powertrain state and never substitute for a physical torque change. In the garage, show net crank power and torque with clear units; label a steady-state curve preview separately from a timed acceleration measurement.

Maintenance feedback adds labeled warning lamps, severity text, restrained smoke/steam at a severe heat fault, worn-part sound cues and a clear breakdown/tow prompt. Visual deformation, fire and dramatic explosions are not needed. Every critical mechanical warning also has a visible explanation. A service-history page uses a readable chronological list with mileage, work performed and paid amount.

Use independent master, engine/effects and music volumes. Music is optional and must not hide tire/braking cues. Begin with a small licensed loop set or original ambient tracks rather than a radio station system. Visual countdown and result cues remain available with sound muted.

## Narrative content

For each of the six rivals: name, portrait or stylized avatar, paint palette, two-line introduction, one pre-race message, win response and loss response. Use the existing rival roster in the world document.

Additional text: skippable arrival scene, garage introduction, brief contextual control prompts, upgrade explanations, unlock messages, six lookout descriptions and a final celebration. No branching dialogue system or voiced cutscenes are necessary.

## Asset provenance record

M00-T03 creates `project/ASSET_REGISTER.csv` with:

```text
asset_id,file_paths,kind,source_url,creator,license,license_file,
changes,required_attribution,redistribution_review,status
```

For generated assets, record the tool, generation date and relevant source/reference restrictions where known. For self-created assets, identify the creator and intended project usage. Keep license/attribution files with the asset bundle. An asset that has not been reviewed can remain a local placeholder but cannot silently enter the release package.

Use assets already available in the project or suitable free resources first. No paid package is assumed by this blueprint. Record any proposed purchase and its purpose before it becomes a dependency.

## Presentation acceptance

Car sizes, collision and wheel positions remain correct after replacing placeholders. Roads read clearly at racing speed. Every needed service and event can be found. UI has visible focus and fits at supported resolutions. Audio communicates load and slip without clipping. All release assets have provenance and any required notices. Presentation changes must preserve the already-tested race and save behavior.
