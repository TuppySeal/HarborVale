# HarborVale — one launch menu and live sports tests

Canonical Unity: C:/Users/Justice/Documents/Unity/HarborVale, pinned 6000.6.0f1. Completed bounded tasks M11-T01–T03 and M12-T01–T05. The complete planned career is not built.

Latest Windows player: Builds/HarborVale-Play/HarborVale.exe. Starts at HarborVale_Hub: choose saved Finch racing/parts or free Apex sports tests. Bottom-right Main menu returns between modes; an active race is explicitly abandoned without rewards, while a valid result with failed payment save blocks accidental departure. Existing scene/static prefab/handling scene and older players are preserved.

Finch retains First Lap earnings and one-location paid Stage1 (400), gearbox (250), turbo/basic intercooler (1900), street intercooler (400). Production v3 progress and read-only one-time v2/v1 migrations unchanged. Sports starts stock each visit, offers stronger free sequential parts, and has no career services or writes. See BOOST_HANDOFF.md and SPORTS_TEST_HANDOFF.md.

Apex now has an armable 0–100 timer and session best per configuration. Stop below 0.5 km/h and let wheel contacts settle, click Arm test, then accelerate. Recovery/reinstall, input/map lock, reverse or prolonged airborne state cancel stale tests. Road slope affects results; no rewards/records or timer persistence. A fully upgraded live city fixture measured 4.42 s; controlled native physics baseline remains 4.44 s. See LAUNCH_HANDOFF.md.

Evidence M12-T05: timer boundary fixtures; actual Windows mode menu, 47 transition/purchase/reload/timing/recovery/save-error assertions; Finch/sports/trial regressions; zero-error Windows export; rendered screenshot inspection; 19 installed source/scene/meta/build-settings files and all 249 installed Windows export files verified by SHA256. Two earlier recovery-readiness test failures are preserved and resolved by sustained stop/contact readiness. No manual physical-click, controller, full-lap feel or performance-profile claim. K001 editor indexing remains recorded.

Claude supplied initial garage scripts, without a generated/verified scene. Read-only inspection found likely compile errors. Garage remains separate and unintegrated; follow-up prompt: CLAUDE_GARAGE_FINISH_PROMPT.md. No Claude project files were modified.

Detailed completed/todo: FEATURE_CHECKLIST.md. Previous implementations and verification records remain in their handoffs; publication verification for this update follows after push. Existing user PackageManagerSettings.asset remains untracked and untouched.

Implementation uploaded and remote-verified at aed745ce16c98c54f465c4a4c49024fca2ac685c. See LAUNCH_GITHUB_VERIFICATION.json.
