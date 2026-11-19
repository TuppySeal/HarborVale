# Current status — earn, buy Stage 1, drive the upgraded Finch

Canonical Unity project: C:/Users/Justice/Documents/Unity/HarborVale, Unity 6000.6.0f1. Open the existing HarborVale_CityMap scene and press Play. Latest Windows player: Builds/HarborVale-Garage/HarborVale.exe. Close older previews before switching; old executables were preserved.

Completed bounded prototype tasks: M11-T01–T03, M12-T01 and M12-T02. The first loop now supports driving the meter-scale city, racing First Lap, earning credits, opening the nearby player garage with G and buying the authored Stage 1 NA Finch package for 400 credits. The garage quotes price/remaining cash, plots stock/tuned torque and derives peak power. Purchase debit, receipt and installed stage save atomically. Stage/HUD/torque survive resets, recovery, race retries and full application restart.

Controls: W/S throttle/brake/reverse; A/D steering; Space handbrake; E start/retry trial; G garage; Enter buy in garage; Esc/G close garage; R recover (+3 seconds in a race); C camera; Tab map. Garage requires slow/stopped car near D01 and blocks racing/map/driving until closed.

Stage 1 uses cfg_finch_na_s1, not a flat horsepower bonus. At 4500 RPM torque is 119→125 Nm; derived peak crank power ~65.2→70.1 kW. Measured native 0-100 km/h: stock 13.56 s, tuned 12.84 s. Only first engine stage is implemented; stages 2/3, conversions, other parts, dealer ownership and maintenance/history remain future work. The prototype does not fabricate a starter purchase or service history.

Progress uses LocalAppData/HarborVale/Prototype/race-progress-v2.json. Valid v1 earned receipts/records import once when no v2 exists; old files remain untouched. Available credits equal gross earned minus historical tune spending. Once v2 exists, later old-build earnings are not automatically reimported; use the latest build. A stale/duplicate purchase cannot double charge; failed writes do not debit/install; historical fees remain fixed; unknown newer schemas fail without overwriting progress.

Evidence: catalog import, purchase/migration/failure/backup/idempotence fixtures, actual Play Mode modal/curve/recovery tests, old race-rule regression, Windows export, real purchase and separate-process restart, and visually inspected final UI screenshots under artifacts/qa/M12-T02. No user save was seeded by probes. Manual sustained keyboard feel, controller support and full campaign balance remain review items. Known editor-only issue K001 remains recorded separately.

Read project/GARAGE_HANDOFF.md for source/migration/reproduction details. Current city/static model and known handling test scene are preserved; historical game/ and the original 42-task Godot baseline are not the current Unity implementation queue. Current user next-step request authorized this bounded garage expansion beyond the intervening map-only hold; no full-game completion is claimed.

Claude is working on the directly submitted verified-workbench task in its separate art folder. No Claude files/session were modified during this garage work. Actual art integration remains a later step after export verification.
