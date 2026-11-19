# Unified launch and live sports timing — M12-T05

Unity 6000.6.0f1, same built-in renderer/packages. Latest local player: `C:/Users/Justice/Documents/Unity/HarborVale/Builds/HarborVale-Play/HarborVale.exe`. Earlier TurboParts/SportsTest executables are preserved.

Start at HarborVale_Hub. Choose Finch for existing First Lap/saved paid parts, or Apex for free temporary sports upgrades and live 0–100 testing. The bottom-right Main menu button changes modes in the same process. During a race it explicitly says End run / main menu: that abandons without payout. A valid finish with a pending save error blocks menu departure until retry or the existing explicit forfeit flow resolves it.

Scenes are listed in ProjectSettings/EditorBuildSettings.asset: new Launch/Scenes/HarborVale_Hub.unity, existing Scenes/HarborVale_CityMap.unity and existing SportsTest/Scenes/HarborVale_SportsTest.unity. No existing city, static Finch prefab or handling scene was regenerated. Opening either driving scene directly in Unity still works.

HarborValeLaunch registers one SceneManager.sceneLoaded handler at runtime startup. It invokes scene-specific FinchDrivingBootstrap.StartDriving, DocksideTrialDirector.Hook, FinchGarage.Hook and ApexTestDrive.Hook. Their prior one-time AfterSceneLoad attributes were removed to prevent double initialization. Every return creates one scene-owned car/four wheels/camera/audio listener/trial/garage as appropriate; only the launcher survives scene changes. Do not restore the old one-time attributes alongside this dispatcher.

SportsAccelerationHUD tracks session-only best times separately for five configurations. Stop below 0.5 km/h with at least two grounded wheels for 0.35 seconds, click Arm test, then accelerate. Time begins with positive throttle and ends at an interpolated 100 km/h crossing between fixed-step speed samples. Road slope/route affects results; this is not a certified flat-track dyno or campaign record. No credits/reputation are awarded and no timing results are saved.

Recovery increments FinchVehicle.ResetSequence and clears stale GroundedWheels. The timer also watches the actual engine object, not just the stage number, so reinstalling the same package cancels a stale run. Changed build/recovery, map/input lock, reverse, more than 0.3 seconds airborne and a 60-second timeout cancel active tests. Best times and free sports parts reset on leaving the sports scene. Existing Finch v3 persistence/migrations are unchanged.

Verification: pure timer boundary fixtures; Windows menu/transition/purchase/reload checks (47 assertions), actual 4.42-second upgraded Apex live run, recovery/reinstall cancellation, no sports writes to isolated career account, unpaid-valid-finish save-failure/retry guard, two visits to each driving mode and duplicate controller/audio/contact checks. Existing Finch turbo/transaction/physics, sports physics and trial-rule regressions passed. Windows export succeeded with zero errors. Actual native screenshots inspected. Physical mouse-click usability, controllers, long full-lap feel, performance profiling and campaign balance remain manual review items.

Two early player attempts failed when the test tried to arm immediately after recovery while the chassis was settling. Those reports remain under attempt-1.txt/attempt-2.txt. Final readiness now requires sustained stop/contact state; final player-smoke.txt passes. No failing attempt is represented as a completed test.

Evidence: artifacts/qa/M12-T05. Authoring/verification: .work/UnityBoostValidation. Reproduce build through HarborValeGreybox.Launch.Editor.LaunchBuild.Build; opt-in Windows verification uses --launch-smoke-dir followed by a new absolute output directory. It seeds only an isolated account and injects only a test store failure. Never run the fixture over real user progress or reuse an existing fixture directory expecting a fresh account.

Claude's initial garage scripts were read only. They have no delivered saved scene yet and contain likely compile errors; the standalone preview remains unintegrated. Follow-up assignment is CLAUDE_GARAGE_FINISH_PROMPT.md. No Claude output was modified.
