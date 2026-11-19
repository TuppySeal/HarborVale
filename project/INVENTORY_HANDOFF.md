# HarborVale garage, engine inventory and Tempest

Canonical Unity project: `C:/Users/Justice/Documents/Unity/HarborVale`, Unity 6000.6.0f1. Checked source and player are installed. Run `Builds/HarborVale-GarageInventory/HarborVale.exe`. Published verified Windows ZIP at https://github.com/TuppySeal/HarborVale/releases/latest (tag test-2026-11-19-garage-inventory). All251 player files verified inside the252-entry ZIP; uploaded asset size and SHA256 independently verified by GitHub. See RELEASES.json / INVENTORY_GITHUB_VERIFICATION.json.

## User flow

Drive to the player garage (D01), stop, press **G**. The new board shows owned cars, a rotating model preview, real HP/estimated WHP/mass, chassis stages and stored engine assemblies. Select a car and choose **Upgrade parts** for the existing categorized staged-parts shop. **Buy another car** opens the H showroom. Physical garage has three open bays; up to two other owned cars are displayed without duplicate driving physics.

Drag an engine handle from a car into inventory to remove it. Drag a stored assembly onto a car, or one car's engine directly onto another car. A confirmation shows fitting labor and compatibility. Receiver's old engine goes to storage; donor chassis remains owned. Prepare gearbox/brakes/tires before removing a car's engine. Engineless cars cannot drive, upgrade or race; another ready owned car or the recovery loaner is selected. Inventory currently contains whole engines with installed engine hardware, not individually removable loose turbos/brakes/etc.

Four new fictional purchasable models have explicit authored torque curves, gearing, mass, drive layout, geometry and staged calibrations: Coda CS (2.0 I4 RWD coupe, 4,500cr), Vector Six (2.8 I6 AWD sedan, 8,000cr), Bastion GT (4.6 V8 RWD muscle, 12,000cr) and Tempest R (4.0 twin-turbo V8 AWD, about 1,100 crank HP, 95,000cr). The launch menu also has **Test Tempest / free**. Apex and Tempest tests are temporary and do not grant paid ownership or alter paid saves.

## Save and swap rules

Production schema/path is v9. Optional v8 draft migration and released v7/v6/older paths are validated, imported once and left read-only. Never reset corrupt/unknown newer schemas. Old receipts and original Finch/Apex catalog remain immutable. Engine registry seeds only genuinely fitted assemblies; no invented returned historical parts. New purchases create one real engine identity. Atomic idempotent transactions commit money, engine links, power snapshots and car selection together. Installed engines follow pink-slip car ownership; loose engines stay with their owner.

Engine snapshot contains induction, tune, cooling, intake, exhaust and sound stages. Chassis, gearbox, tires, brakes, suspension and drivetrain remain with the receiver. Hybrid powertrain uses donor torque/RPM/boost, receiver gearing/drivetrain efficiency and engine-mass difference. Sound firing follows actual 4/6/8-cylinder donor. Frozen fitment rules revision1: same-model 400cr, cross I4 1,200cr, I6/V8 2,000cr, Tempest 3,500cr. Finch supports compact I4 packages only. High torque requires staged receiver gearbox support. Tempest cross-fit only Vector/Bastion with gearbox3, brakes2 and tires2. Factory Tempest tune stages use its factory twin turbo; aftermarket induction conversion paths are blocked.

## Source and art

`tools/import_vehicle_lineup.mjs` imports `design-data/vehicle_lineup.json` into Lineup resources. `LineupAssets.Build` creates persistent ordinary-car/garage resources; TempestImport normalizes FBX axes and four wheel pivots. Do not rebuild the source map to install these systems. Original D01 placeholder is replaced at runtime, foundation and source scene remain intact. Finch defaults, source static prefab, handling scene and stock importer are preserved.

Claude created the original Tempest in Blender 5.3 under `Documents/HarborVale-Claude-Art/TempestSportsCar`; do not modify that folder during gameplay work. 76,126 triangles, 9 meshes, 13 simple materials, 4.58 x 1.965 x 1.165m, wheelbase2.76m. Helper verified Blender FBX re-import; main integration separately verified Unity pivots/model. Editable source/generator are copied outside Assets to `art-source/Tempest` to avoid Unity's automatic .blend import. Curved body is a prototype with some rough fender edges; no engine-bay interior. Ordinary cars remain distinct primitive greybox models.

## AI recovery

RoadPilot responds to significant collision impacts by reacquiring nearby road samples, bounded reverse/steer/forward maneuvers and upright torque for tipped cars. Persistent flipped/stuck rivals use existing penalized passed-gate recovery; traffic recycling remains distant/unseen. Ordinary bump test rejoins road and drives again without teleporting or skipping race gates. This is prototype traffic, without overtaking, signals or police.

## Verification and exact continuation

Editor entry point: `HarborValeGreybox.Racing.Editor.InventoryVerification.Release`. Current results: 38 save/hybrid/recovery/FBX checks, four actual 37-gate city laps with zero assisted recoveries, 33 mixer/camera, 29 turbo and 9 city-art checks. Native first pass exposed InputSystem/IMGUI pointer mismatch; using Event.mousePosition outside scroll groups fixed physical stored-engine drag to the selected left tile. Native mouse run passed35 checks including paid swap, reload, free Tempest and save isolation. Final native pass passed43 checks and additionally verifies parked cars, transplanted pink-slip title loss and actual V8 listener recording. An initial final-pass assertion incorrectly counted the newly created rival engine as duplication; baseline now captures engine count after opponent creation.

Use a development player with `--inventory-check-dir <empty isolated folder>`, optionally `--inventory-interactive` for physical mouse verification. Test-only fixture earnings are confined to that override; never include profiles/logs/cache files in Git or release. Full evidence and remaining manual checks are recorded in `artifacts/qa/M17-T02/install-verification.json` after installation. Native screenshots prove rendering and tested inputs, not subjective handling/sound quality. Human driving, small-screen readability, audio listening, collision edge cases and traffic/difficulty/performance review remain useful.

Final audio recording: peak0.4021, RMS0.1131, zero clipped samples. All251 installed player files match the tested build. Protected original source matches prior Git commit; user PackageManagerSettings.asset remains untracked and untouched. Source and checked Windows ZIP are published; see RELEASES.json. Next: human driving/audio/GUI/collision review, then deliberately choose future loose-parts/service/world scope. Never reuse or overwrite an old release tag.
