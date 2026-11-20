# HarborVale — Windows racing prototype

Latest: **Builds/HarborVale-CityStakes/HarborVale.exe**. G at garage -> **Remove / refit parts**. H -> **8-car paid circuit** (3000cr entry;1000per rival beaten). v10 paid saves migrate read-only. See project/COMPONENTS_HANDOFF.md. Highways/police are the next expansion.

[**Download the latest Windows build**](https://github.com/TuppySeal/HarborVale/releases/latest)

This repository and its downloads are private. Sign in with a GitHub account that has access.

Download **HarborVale-GarageInventory-Windows-x64.zip** from the release's Assets list, extract the entire ZIP, then run **HarborVale.exe** inside the extracted HarborVale folder. Keep HarborVale_Data and all DLLs beside it. Close older game versions first. The game runs on a Windows64-bit PC; GitHub does not run it in a browser.

## AI rivals and traffic

Choose **Drive Finch**, stop and press **H** for **Rivals & Fleet**. Regular physical traffic appears automatically. Cash races against three AI drivers pay650/1000/1500credits for a win or150/200/250for a valid second-place finish; no entry fee. Follow all37 checkpoints around the Dockside loop.

Optional **pink slips permanently stake the selected car and its upgrades**, with a separate agreement before starting. Winning transfers the actual rival car/build; losing, abandoning or closing midrace transfers yours. Ties keep both titles. A free stock loaner and1800credit new replacement path recover from losing your last car. Select owned/won cars inH after returning to driving; paid upgrades stay with each car. Existing progress imports read-only into v7. Traffic is a prototype without signals/overtaking/police; current rivals/fleet are Finch variants.

## Try the garage

- Choose **Test Apex · free upgrades**, stop, then press **G**. Install and test parts freely; Apex upgrades reset each visit.
- Choose **Drive Finch · saved progress**, stop near the starting player garage, then press **G** for paid upgrades saved to that PC.

The garage has **18 upgrade-family tabs** with 55 offers, sequential stages/prerequisites and performance comparisons. Small/large turbos, superchargers and twin turbos have separate tabs; prerequisite links jump to the correct family. It includes engine tunes, small/large/twin turbo options, supercharging, cooling, intake/exhaust, tires, brakes, chassis/settings and optional burble/flame calibrations. Compatibility and hardware requirements apply; twins are Apex-only. Simple rotating visuals preview each selected part. Tires include separate road/drift/slick compounds with buy-once/free-refit ownership. Paid FWD/RWD/AWD conversions change driven wheels, mass and estimated WHP; compatible stages stay fitted. Slicks provide higher dry grip, while drift tires give more gradual rear breakaway. Turbo audio offers BOV/flutter choices. The dyno shows crank HP, estimated WHP and vehicle weight. Tire squeal uses smoother filtered rubber texture tied to actual road speed and wheel slip, with soft starts and fade-outs.

## Drive

W/S accelerate, brake/reverse; A/D steer; Space handbrake; R recover; C chase/bonnet view; Tab map. M switches automatic/manual; Z/X shift down/up. Hold RMB and move the mouse to look around, or hold J/L for side views and K for rear. Hold W+S while stopped for tune-dependent brake boosting.

## Current map and limits

Cleaner lighting/edge smoothing and a first art pass on17 existing Midtown plots: detailed apartment facades,34 lamps and17 planters. Forecourt props are visual only. Roads, driveways and original collision shapes remain; other districts are still greybox. This is a playable prototype, not a finished racing/ownership game. Saves stay local to each PC; no personal saves are included in the ZIP.

## Unity source

Unity **6000.6.0f1**, built-in renderer. Open **Assets/HarborValeGreybox/Launch/Scenes/HarborVale_Hub.unity** and press Play. Installed local player: **Builds/HarborVale-GarageInventory/HarborVale.exe**. Builds and caches are excluded from Git; downloadable players are attached to GitHub Releases.

Read project/STATUS.md, project/NEXT_TASK.md, project/RIVALS_HANDOFF.md and project/RELEASES.json for current progress, actual verification and download hashes. Earlier milestone handoffs preserve historical measurements and save migrations. Follow AGENTS.md before changes; do not run legacy scene exporters or reset paid saves.


## Owned garage and engine swaps

Run the latest player, choose Drive Finch, stop at PLAYER GARAGE and press **G**. The board shows owned cars, a rotating model and stored engine assemblies. **Upgrade parts** opens the staged shop; **Buy another car** opens the showroom. Drag engine handles into storage or onto another car, then confirm compatible mounts/ECU labor. Receiver keeps its gearbox, brakes, tires, suspension and drivetrain. Empty chassis cannot drive/race.

Four new models: Coda CS I4 coupe, Vector Six I6 AWD sedan, Bastion GT V8 muscle and original Blender Tempest R twin-turbo V8 (~1,100HP). Main menu **Test Tempest / free** is isolated from paid saves. Engines keep installed power upgrades and follow the car in pink slips; loose engines stay owned. Read [inventory handoff](project/INVENTORY_HANDOFF.md).


## Smarter rivals

Latest local player Builds/HarborVale-RaceAI/HarborVale.exe. H races use full-throttle acceleration, corner/braking planning and clear-lane passing. New rookie/club/tuned limits65/85/100kmh; power/tires stay real. Read project/RACE_AI_HANDOFF.md.
