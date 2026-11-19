# HarborVale — smoother automatic and paddle-style manual

Canonical Unity: C:/Users/Justice/Documents/Unity/HarborVale, pinned 6000.6.0f1. Completed bounded prototype tasks M11-T01–T03 and M12-T01–T06. Full career/ownership/maintenance remains planned.

Latest Windows player: Builds/HarborVale-SmoothDrive/HarborVale.exe. Close old game windows before using this release. The same hub selects saved Finch or free Apex. Prior Play/TurboParts/SportsTest builds and original city/static Finch/handling scene are retained.

Automatic now road-matches lower gears while slowing and kicks down on renewed strong demand or lugging, with hysteresis/cooldown. Loaded-slip upshifts avoid lingering on the limiter. Smooth clutch re-engagement, modest engine braking and throttle/brake/audio-pitch ramps reduce abrupt transitions. Authored torque curves, prices, grip and camera tuning unchanged.

M toggles auto/manual; Z down/X up, with clickable HUD controls. Successful paddle use selects manual; unsafe over-rev, reverse, bounds and overlapping shifts reject visibly. Manual holds gears, selects first when rolling to a stop, and allows intentional second-gear stationary launch. Mode survives recovery/hardware changes within a drive and defaults to auto on a new scene visit. Shop/countdown/map input locks block shifting. See SHIFTING_HANDOFF.md.

Finch v3 paid parts/earnings/migrations unchanged. One-location Stage1 400 → gearbox 250 → turbo/basic cooler 1900 → street cooler 400. Sports free parts/session timing remain separate from career. No save schema or account edits for this feature. Read BOOST_HANDOFF.md, SPORTS_TEST_HANDOFF.md and LAUNCH_HANDOFF.md for retained systems.

Evidence M12-T06: 104 native rule/physics assertions, 19 native Windows manual/automatic checks, 47 unified-player purchase/save/transition/timing checks, parts/trial regressions and zero-error export. Actual UI captures inspected. Installed 12 source/meta files and all 249 player/data files match tested copy by SHA256. Synthetic reproduction selected first safely after slowdown where legacy Finch/Apex remained in fourth/fifth after 0.6 s. Current native 0–100: Finch stock 13.46, Stage1 12.72, turbo 10.48, cooler 10.28; Apex upgraded 4.32 s (city timer fixture 4.31). Earlier handoff timings describe earlier releases.

Objective checks do not prove subjective full-lap smoothness. Physical keyboard/mouse feel, controller hardware, listening, road-seam/camera preferences and frame-pacing remain review items. Existing editor-only K001 recorded; K002 automatic hesitation resolved. No Claude previews were modified/integrated. FEATURE_CHECKLIST.md separates completed from future work. Publication record follows after upload.

Implementation uploaded and remote-verified at 758fa45fe6f4e2e1e4b37da26fea61f0d371483a. See SHIFTING_GITHUB_VERIFICATION.json.
