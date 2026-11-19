# Finish the existing garage preview

Continue only in `C:\Users\Justice\Documents\HarborVale-Claude-Art\GaragePreview`. Another AI is working on the main game's launch menu, driving, live performance tests, purchases and saves. Do not modify the main HarborVale Unity project or Git repository.

Your next task is to finish and verify the garage you already started. Do not add more features or documentation until it actually runs in Unity.

1. Make this a valid Unity **6000.6.0f1** built-in-renderer project, including correct ProjectSettings and package configuration. The installed editor is `C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe`.
2. Compile in the actual editor and fix every compile error. Two issues found in your delivered source: SceneGenerator passes `NewSceneSetup.DefaultGameObjects` as NewScene's second argument, where `NewSceneMode.Single` is expected; UpgradeUIManager uses `Button` without importing `UnityEngine.UI`. Put editor-only generation code in an Editor folder and ensure it cannot break a player build. Check actual UI/TMP dependencies and required references rather than assuming they are installed or wired.
3. Save the real scene at `Assets/ClaudeGarage/Scenes/GaragePreview.unity`. Make the camera, menu buttons and part display work without manually assigning missing references. Reopening must not create duplicate scene objects.
4. You copied far more HarborVale assets than needed. Do not copy anything else from the main game. Keep only the static Finch visual and required dependencies in the deliverable; exclude driving/racing/garage/save/sports/launch code. A prepared visual-only package is available at `C:\Users\Justice\Documents\Codex\2026-11-17\help-me-brainstorm-an-adventure-game\deliverables\ClaudeGarageVisualKit\AsterFinch_VisualOnly.unitypackage`. Inspect dependencies before removing anything in your own project; never delete or change main-project files.
5. Test Play Mode: visible car/workshop, orbit and zoom, menu open/close, and selection of `engine_stage1`, `gearbox_stage1`, `turbo_stage2`, `intercooler_stage1`. Show a basic intercooler with the turbo kit and a distinct upgraded street intercooler. Use one parts counter. All money/install states remain labeled preview mocks; do not build real transactions, saves or performance logic.
6. Capture actual Unity screenshots showing the garage, turbo, intercooler and readable menu. Export an editable `.unitypackage` with your authored assets and only required static dependencies. Confirm the exported file exists and report its exact path.

Update STATUS.md and NEXT_TASK.md with actual results, exact errors and remaining work. Correct the old date and remove unsupported percentages/production-ready/compiled claims. A script file is not a verified scene. Do not hand scene generation or testing back to me as manual instructions when you can perform it. If genuinely blocked, report the exact failed action and error, preserve the work and state what remains unverified.

Stop after this garage is verified and packaged. Do not integrate it into the main game.
