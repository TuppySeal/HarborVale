# HarborVale — Windows racing prototype

[**Download the latest Windows build**](https://github.com/TuppySeal/HarborVale/releases/latest)

This repository and its downloads are private. Sign in with a GitHub account that has access.

Download **HarborVale-CityArt-Windows-x64.zip** from the release's Assets list, extract the entire ZIP, then run **HarborVale.exe** inside the extracted HarborVale folder. Keep HarborVale_Data and all DLLs beside it. Close older game versions first. The game runs on a Windows64-bit PC; GitHub does not run it in a browser.

## Try the garage

- Choose **Test Apex · free upgrades**, stop, then press **G**. Install and test parts freely; Apex upgrades reset each visit.
- Choose **Drive Finch · saved progress**, stop near the starting player garage, then press **G** for paid upgrades saved to that PC.

The garage has **17 upgrade-family tabs** with 49 offers, sequential stages/prerequisites and performance comparisons. Small/large turbos, superchargers and twin turbos have separate tabs; prerequisite links jump to the correct family. It includes engine tunes, small/large/twin turbo options, supercharging, cooling, intake/exhaust, tires, brakes, chassis/settings and optional burble/flame calibrations. Compatibility and hardware requirements apply; twins are Apex-only. Turbo audio offers BOV/flutter choices. The dyno shows crank HP, estimated WHP and vehicle weight. Tire squeal uses smoother filtered rubber texture tied to actual road speed and wheel slip, with soft starts and fade-outs.

## Drive

W/S accelerate, brake/reverse; A/D steer; Space handbrake; R recover; C chase/bonnet view; Tab map. M switches automatic/manual; Z/X shift down/up. Hold RMB and move the mouse to look around, or hold J/L for side views and K for rear. Hold W+S while stopped for tune-dependent brake boosting.

## Current map and limits

Cleaner lighting/edge smoothing and a first art pass on17 existing Midtown plots: detailed apartment facades,34 lamps and17 planters. Forecourt props are visual only. Roads, driveways and original collision shapes remain; other districts are still greybox. This is a playable prototype, not a finished racing/ownership game. Saves stay local to each PC; no personal saves are included in the ZIP.

## Unity source

Unity **6000.6.0f1**, built-in renderer. Open **Assets/HarborValeGreybox/Launch/Scenes/HarborVale_Hub.unity** and press Play. Installed local player: **Builds/HarborVale-GarageTabs/HarborVale.exe**. Builds and caches are excluded from Git; downloadable players are attached to GitHub Releases.

Read project/STATUS.md, project/NEXT_TASK.md, project/MAP_ART_HANDOFF.md and project/RELEASES.json for current progress, actual verification and download hashes. Earlier milestone handoffs preserve historical measurements and save migrations. Follow AGENTS.md before changes; do not run legacy scene exporters or reset paid saves.
