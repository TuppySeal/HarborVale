# Player garage and first paid Finch tune

Task **M12-T02** is the user-requested next expansion after First Lap. This is a bounded prototype garage, not the complete ownership/maintenance career.

## Player behavior

Use the latest `Builds/HarborVale-Garage/HarborVale.exe` in the canonical Unity project (`C:/Users/Justice/Documents/Unity/HarborVale`, Unity 6000.6.0f1). Close older previews before switching builds. Unity users can open the existing city scene and press Play.

Drive slowly or stop by the player garage near the initial spawn and press **G**. The workshop shows available credits, exact price, remaining cash, stock/Stage 1 torque curves and derived peak power. Buy with Enter or the install button. Esc/G closes the workshop. Racing/map/driving input cannot overlap the modal; an active race or result must be left before shopping.

Win credits from First Lap, then install the **400-credit Stage 1 naturally aspirated package**. Fee is imported from the existing formula: reference 550 × minimum scaling 0.75, rounded to nearest 50 (half up). The installed package selects `cfg_finch_na_s1`; its torque rises from stock 119 Nm to 125 Nm at 4500 RPM. Derived peak crank power is approximately 65.2 → 70.1 kW. No additional horsepower multiplier or double boost gain exists.

The UI uses the original fictional stock transmission/engine and ECU/intake/exhaust package. Only the first paid stage is implemented in this task. Stages 2/3, forced-induction conversions, mechanic delay/quality, physical parts, dealer purchases, servicing and lifetime odometer/history remain future systems. The Stage 1 NA receipt describes the current package; it must not prohibit later paid conversions.

## Save migration and transaction contract

Production progress is now `%LOCALAPPDATA%/HarborVale/Prototype/race-progress-v2.json`.

When no v2 primary/backup exists, valid `race-progress-v1.json` (or its validated backup) is imported into memory with its original profile ID, race receipts, best record, first-clear history, earnings and reputation. The first v2 transaction atomically writes the migrated ledger plus the new transaction. Old v1 files are retained untouched. Once v2 exists, it is authoritative and is never repeatedly topped up from v1.

This separate path prevents an older executable's v1 fallback logic from overwriting paid upgrades. **Do not continue earning in an older build after switching**: later v1 earnings are retained in the old file but not automatically merged into v2. Any future reconciliation must deduplicate run receipts and first-clear/reputation effects deliberately.

Schema 2 adds `availableCredits`, `engineStage`, `prototypeCarId` and `spending` receipts. Gross `earnings` remains the sum of race receipts; available credits must equal gross earnings minus actual recorded tune spending. Prototype car ID is derived from the stable profile ID and is not a fabricated dealer purchase, VIN, service record or ownership transfer. Future full-career migration must preserve actual paid receipts and explicitly map this prototype identity.

A purchase validates the immediately preceding stage (stock), compatible package, supporting-transmission requirement (0), reputation, quoted fee and funds. It re-reads under an exclusive file lock, then commits debit, receipt and installed stage together. A duplicate transaction returns its original receipt; a new request cannot rebuy Stage 1. Historical paid fees are never recalculated from a later catalog price. The runtime applies the new curve only after the commit succeeds. A failed write does not debit or install.

Backups restore cash and build together. Unknown newer schemas are rejected without falling back and overwriting them. Invalid current and backup data are preserved; the game never silently resets paid progress. Temporary candidates do not count as committed transactions. Closing/restarting, recovery, race reset and abandon retain the paid stage. A stale second instance cannot buy the same stage twice.

## Code and data

- `Garage/Scripts/GarageCatalog.cs`: imported offer and stock/tuned definitions.
- `Garage/Scripts/FinchGarage.cs`: shop interaction, quote, torque graph, install and receipt display; uses the trial's shared progress store.
- `Racing/Scripts/RaceProgressStore.cs`: v1 import, v2 validation, spending ledger and atomic purchases. There is one available-credit account, not independent race/shop wallets.
- `Driving/Scripts/FinchVehicle.cs`: installs an authored definition without rebuilding wheel physics; carries the display stage through resets. `FinchPowertrain.cs` derives peak power from knots and valid interior extrema.
- `FinchInput.cs`: G/Enter keys; HUD shows installed stage. `DocksideTrialDirector` handles the v2 production path, shared modal lock and available-credit display.
- `tools/import_finch_garage.mjs`: reproducibly imports `cars.json`, `upgrades.json`, `powertrains.json` and the D01 access position into `Garage/Resources/FinchGarageCatalog.json`. Do not maintain a separate editable price/curve table.

All paths above are inside `Assets/HarborValeGreybox/` unless prefixed `tools/`. Map/static source prefab and independent handling test scene are preserved.

## Evidence and checks

Isolated verification project: workspace `.work/UnityGarageValidation`; staged source `.work/GarageSource`. Do not run batch verification entry points inside an editor with unsaved user edits.

- `GarageVerification.Run`: insufficient funds, valid v1 import, atomic purchase, untouched v1, duplicate/repeated requests, reload, replay accounting, no reimport, injected failed write, stale quote, simultaneous instances, historical price, coherent backup and unknown schema. Native acceleration comparison measured **stock 13.56 s → Stage 1 12.84 s** from 0–100 km/h using the actual torque curves.
- `GaragePlayModeVerification.Begin`: real Play Mode modal locks, race exclusion, unfunded/funded purchase, runtime curve/HUD installation, duplicate request, reset/recovery/race/abandon preservation and disk reload.
- Existing trial rule/save regression is rerun against schema 2.
- `GaragePlayerBuild.Build`: Windows development export. Explicit `--garage-smoke --garage-smoke-dir <isolated folder>` mode checks purchase and UI; `--garage-reload` uses a separate process to verify restart persistence. These modes never use the real player profile.

Reports and actual visible Windows screenshots belong under `artifacts/qa/M12-T02/`. Test-only rewards come from an isolated valid-finish fixture and do not demonstrate a manually driven lap or seed real user credits. Sustained manual keyboard handling, controller support and full campaign balance remain unverified. Existing editor-only search indexing issue K001 is recorded separately; other gameplay runtime errors fail the checks.

Claude is working on the directly submitted **one verified workbench** task in `Documents/HarborVale-Claude-Art/WorkshopProps`. Leave that session and folder untouched while it runs; integrate actual exported assets only in a later deliberate step.

Final verification complete: fixtures, trial regression, actual Play Mode, Windows purchase and separate-process reload passed. Final opaque modal/graph UI captures were inspected in artifacts/qa/M12-T02. Production output is Builds/HarborVale-Garage/HarborVale.exe; original map/static prefab/test scene were not overwritten. No new dependency. The latest native probes verified a 400-credit purchase left 700 of 1100 test-earned credits, and the next process restored Stage 1 without another debit.

