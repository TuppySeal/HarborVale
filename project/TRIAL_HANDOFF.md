# First Lap — Dockside time trial

Current task: **M12-T01**. User authorized choosing the next game expansion. The first time trial gives the existing driving a repeatable activity and starts saved race earnings; a garage spending/ownership system is not yet implemented.

Canonical project remains `C:/Users/Justice/Documents/Unity/HarborVale`, Unity **6000.6.0f1**. Never regenerate the map to install this feature. Runtime bootstrap attaches the trial to the unchanged city scene.

## Player flow

At the nearby FIRST LAP arch, stop or slow below 8 km/h and press **E**. A three-second countdown holds the car, then releases it. Follow the teal road line and pass the gold checkpoint arch; the next arch is teal. Direction prompts and braking hints supplement the route. Complete all **37 directional checkpoints** around the **2,042 m loop** to finish.

**R** returns to a passed route checkpoint and adds three seconds. It never advances checkpoint progress. **Esc** abandons without payment. Map inspection and losing application focus do not stop an active race clock. Results show the medal, time, payout, reputation and best time. E retries; Esc returns to free roam. A failed save offers retry; explicitly leaving without payout is available instead of silently discarding it.

Provisional targets: gold 1:55, silver 2:25, bronze 3:05. These require player-driving calibration; scripted test crossings do not establish medal pacing.

## Rewards and persistence

Amounts are imported from `design-data/events.json` and `economy.json`, using integer basis-point multipliers and integer credits:

| Result | Repeat credits | Best reputation target |
|---|---:|---:|
| Gold | 700 | 50 |
| Silver | 560 | 40 |
| Bronze | 455 | 30 |
| Valid slower finish | 350 | 20 |
| Abandon / invalid | 0 | 0 |

The first valid completion adds **400 credits once**. Reputation grants only the improvement over the previous best. Credits, receipt and best record commit in one transaction. Each run has a unique ID; duplicate settlement after reload cannot pay twice. Receipt terms preserve historical reward/medal rules when future tuning changes.

**Race earnings are a separate prototype account starting at zero**, not an invented starter-purchase receipt or the full career wallet. The already-present Finch remains a prototype car; no purchase, sale, wear/history, upgrades or maintenance are claimed. Future wallet/ownership integration must explicitly migrate earned receipts exactly once without inventing mileage, service or purchase history. Do not create a second independent conflicting price table.

Real profile: `%LOCALAPPDATA%/HarborVale/Prototype/race-progress-v1.json`, shared by Editor and Windows builds. It is deliberately independent of Unity product-name folders. `.bak` stores the last valid prior transaction; `.tmp` is an incomplete candidate and is never treated as committed earnings. An exclusive `.lock` prevents two running game instances from overwriting transactions; settlement re-reads current progress under that lock. When primary data is corrupt, only a validated backup can load, with a visible notice. If neither save is valid, racing is disabled while free driving and the existing files remain available. No silent profile reset.

This is a single-event version-1 prototype ledger. Adding other races, purchases or catalog/schema changes requires a deliberate migration preserving earned receipts and actual historical amounts. The receipt ledger checks integrity/consistency; it is not a server-side anti-cheat system.

## Code/data boundaries

- `Assets/HarborValeGreybox/Racing/Scripts/TrialSession.cs`: countdown, ordered directional swept-gate crossings, time, finish/abandon and recovery penalty rules. Rejects height/width misses, backwards crossings, long teleports and non-finite vehicle positions.
- `RaceProgressStore.cs`: validated atomic file transactions, frozen receipt terms, best records, reload, backup and duplicate/concurrent settlement protection.
- `DocksideTrialDirector.cs`: car/UI/world-marker integration, route line, countdown input lock, results and recovery adapter. It does not implement vehicle motion.
- `TrialPlayerSmokeProbe.cs`: explicit development-only `--trial-smoke --trial-smoke-dir <isolated folder>` mode for scripted Windows UI/ledger testing. Normal play never starts the probe or uses its test profile.
- `tools/import_first_trial.mjs`: reproducibly authors the road-following loop and imports reward rules into `Racing/Resources/FirstDocksideTrial.json`. Route roads: RD02 G→H, RD04 H→C, RD01 C→B, RD03 B→G, then RD02 back to the start near the player garage.
- Small existing driving integration edits: `FinchVehicle` exposes a countdown lock and recovery-policy interface; `FinchInput` maps E; HUD blocks map toggling while countdown/results lock the car. Free driving behavior otherwise remains available.

Checkpoint the layout importer after map changes and verify its closed route/spacing report. Never hand-edit independent reward values in UI/controllers.

## Verification workflow

Use an isolated project copy for batch tests so active user scenes/builds remain untouched. Current working copy: workspace `.work/UnityRaceValidation`; staged source: `.work/RaceSource`.

1. `TrialVerification.Run`: rule, first-clear/replay/improvement, idempotence, stale-instance, history-retuning, injected failed-write, corrupt-backup and invalid-save tests. Evidence in `Verification/Trial/rule-results.txt`.
2. `TrialPlayModeVerification.Begin`: actual Play Mode countdown/car lock, recovery/clock/map, scripted ordered gate crossings, result settlement, duplicate result and free-roam/abandon checks. Uses an isolated profile; does not drive a full lap.
3. `TrialPlayerBuild.Build`: Windows development export. Development player smoke mode captures the actual offer/race/results GUI and checks the isolated payout reloading from disk. Scripted crossings are not a manual full-lap test.
4. Copy verified evidence into `artifacts/qa/M12-T01/`, inspect actual PNGs, update ledger/progress and push the existing private remote.

Known Unity editor SearchDatabase indexing issue K001 remains separately recorded; all other gameplay runtime exceptions fail checks. Driving physics results from M11-T03 remain historical; the new race integration must not claim the broader stock/upgraded handling milestone is complete.

Claude's independent prop pack belongs only in `C:/Users/Justice/Documents/HarborVale-Claude-Art/WorkshopProps`; do not modify or import that folder during this gameplay task. See `project/CLAUDE_ART_HANDOFF.md`.

References consulted: [Unity 6000.6 LineRenderer](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/linerenderer), [.NET File.Replace](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.replace?view=netframework-4.8.1).

Final local Windows output: C:/Users/Justice/Documents/Unity/HarborVale/Builds/HarborVale-FirstLap/HarborVale.exe. Built-in ScreenCapture module 1.0.0 supplies the runtime QA capture API. A visible player is required for useful UI screenshots; the initial hidden-window images were gray and rejected. Verified actual UI captures are in artifacts/qa/M12-T01, alongside a note explaining scripted crossings and the final map-abandon fix. Last export succeeded with zero errors; existing driving regression stayed unchanged. No user profile was preloaded with test rewards.

