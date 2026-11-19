# Current status — first playable time trial and saved race progress

Canonical Unity project: C:/Users/Justice/Documents/Unity/HarborVale, Unity 6000.6.0f1. Scene Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity. Latest Windows build: Builds/HarborVale-FirstLap/HarborVale.exe. Older previews were preserved.

M11-T01–T03 and M12-T01 are complete within their prototype scopes. The starter drives through the expanded 24-road city. First Lap is now a playable 2,042 m Dockside time trial with 37 directional ordered checkpoints, countdown/input lock, marked road route, navigation/braking hints, time/medal results and replay/abandon/free-roam flow.

Press E while stopped/slow at the nearby FIRST LAP marker. W/S accelerate/brake/reverse, A/D steer, Space handbrake, R checkpoint recovery (+3 seconds), C camera, Tab map. Esc abandons a run; E retries from results; Esc returns to free roam. Race time continues in map/focus changes. Finishing/abandoning from map view restores the result keyboard/camera controls.

Valid gold pays 700 credits, silver 560, bronze 455, slower finish 350; first valid completion adds 400 once. Reputation grants only best-result improvement. Saved race earnings begin at zero and are explicitly separate from a future full career wallet/vehicle acquisition system. No starter purchase/history, upgrades, maintenance, traffic, police or AI opponents are claimed. Historical reward terms/records survive later tuning. Profile location: LocalAppData/HarborVale/Prototype/race-progress-v1.json.

Evidence in artifacts/qa/M12-T01: route validation; race/ledger fixtures; actual Play Mode checkpoint/result/recovery/map checks; unchanged driving regression; successful Windows export; isolated Windows-player payout/reload checks; visually inspected actual offer/racing/results UI PNGs. Scripted gate crossings are not a manual driven lap. Gold/silver/bronze targets remain provisional and need player feedback. Known editor-only search issue K001 remains separately recorded; all other runtime errors fail checks.

Continuation: project/TRIAL_HANDOFF.md. Original 42-task Godot-era baseline remains historical/unstarted; modern Unity additions have not completed the full career milestone. Do not overwrite the current city or independent handling test scene. Next sensible step: user route/medal feedback, then a deliberately scoped garage/paid-upgrade transaction loop that migrates prototype earnings without duplicating receipts. Await user direction for the next feature.

Claude's output remains isolated in Documents/HarborVale-Claude-Art/WorkshopProps. Its report currently supplies a generator/docs; FBX/blend/previews remain pending generation. The next small assignment is one verified workbench; do not import incomplete claims as assets. See project/CLAUDE_ART_HANDOFF.md.
