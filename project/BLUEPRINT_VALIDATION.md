# Blueprint validation

Checked: 2026-11-18T18:15:37.042Z

Result: **PASS — 26 design-package check groups**.

Command: `node tools/validate_blueprint.mjs --write-report`

This validates the written reference data, task structure and document links. It does not test a playable game, actual handling, saves, AI drivers, performance or an exported build.

- Pass: Reference JSON parses and content IDs/counts agree.
- Pass: Starter choices are affordable and all stock physics values are positive.
- Pass: Event modes, district counts, AI fields and rival/finale references agree.
- Pass: Slice and V1 release manifests reference the right definitions.
- Pass: Torque interpolation, power units, interior peak search and wheel-force examples agree.
- Pass: All authored powertrains, compatible packages, supporting stages and gearbox values are valid.
- Pass: Turbo curves respect threshold, bounded off-boost output and positive spool time constants.
- Pass: Conversion choices retain the same paid naturally aspirated first-stage preparation.
- Pass: Independent sequential-fee and required-transmission budget examples match.
- Pass: All thirteen path performance and complete-build cost examples agree.
- Pass: Used offers, persistent identities, inspection fields and repair histories are consistent.
- Pass: Maintenance thresholds, paid replacement mapping and noninvestment expenses agree.
- Pass: All 64 unsafe-component subsets have a finite repair and at most four recovery jobs.
- Pass: Used-car inspection/repair budgets, condition/history resale and buybacks match examples.
- Pass: Fractional wear persists mathematically and useful heat thresholds are reachable.
- Pass: Preinstalled used stages price by installed entitlement without double-counting ownership spending.
- Pass: Mileage units and advisory check intervals do not add a second condition or price penalty.
- Pass: Component service mileage uses scoped work and leaves missing service history unknown.
- Pass: All categories require sequential payments and stage totals remain well-defined.
- Pass: Performance examples and upgraded configurations agree with the formula.
- Pass: Gold and silver results can numerically unlock every event.
- Pass: Opening bronze/worst-result skill gates are explicit and recoverable by improvement.
- Pass: First-session arithmetic and maximum first-clear rewards agree.
- Pass: Car and upgrade resale loops lose credits for every defined configuration.
- Pass: Task ledger matches all roadmap IDs and has a valid acyclic dependency graph.
- Pass: Local Markdown links resolve and no stale START_HERE reference remains.

## Numerical progression scenarios

| Fixed result band | Reachable events | Reputation |
|---|---:|---:|
| gold | 16 | 2260 |
| silver | 16 | 1808 |
| bronze | 3 | 108 |
| worst valid finish | 3 | 72 |

Bronze-only and worst-finish scenarios intentionally need an improved best result before unlocking the next event. Credits remain replayable. Numerical reachability does not prove that a player can beat the AI or that pacing is enjoyable.

## Performance-score examples

| Car | Stock PI | All stage 3 PI |
|---|---:|---:|
| car_finch | 149 | 270 |
| car_lynx | 285 | 493 |
| car_apex | 728 | 999 |

## Sequential engine payment examples

Finch engine steps: 400 + 600 + 1,200 = 2,200 credits. Courier turbo engine steps: 450 + 2,000 + 3,550 = 6,000 credits. Its required transmission stage 2 costs 650 in total, so that engine/transmission project costs 6,650 beyond buying the car.

## Complete-build reference examples

| Engine path | Stock PI | All stage 3 PI | All stage 3 upgrades, credits |
|---|---:|---:|---:|
| path_finch_na | 149 | 270 | 7850 |
| path_courier_na | 163 | 294 | 8200 |
| path_courier_turbo | 163 | 393 | 11850 |
| path_courier_supercharger | 163 | 377 | 11850 |
| path_lynx_turbo | 285 | 493 | 15250 |
| path_switch_turbo | 296 | 507 | 19700 |
| path_sable_na | 386 | 584 | 20800 |
| path_sable_turbo | 386 | 779 | 30000 |
| path_sable_supercharger | 386 | 724 | 30000 |
| path_quarry_turbo | 426 | 704 | 30600 |
| path_vela_na | 578 | 819 | 31200 |
| path_vela_turbo | 578 | 999 | 45000 |
| path_apex_twin_turbo | 728 | 999 | 39750 |

## Used cars and recovery

Ten initial physical vehicles and four slice offers have unique IDs, bounded condition and chronological records. Guaranteed starters preserve their original price and healthy condition. A worn Courier has a 300-credit minimum quote, a resale quote of 1,524 before repair and 1,657 afterward. A 1,000-credit reserve leaves 600 after its 100-credit inspection and 300-credit fix.

All 64 unsafe-component combinations have a valid minimum repair of at most 1,200 credits. Four completed 300-credit workshop jobs therefore fund the worst minimum repair from zero. Buyback/repair valuation, purchased-component renewal and preinstalled-stage price references were checked as data/calculation contracts.

These checks cover authored torque/maintenance data and independent calculation examples. Actual wear, breakdowns, workshop jobs, torque response, transaction rejection and save recovery still require the game implementation and its tests.

All financial and physics values remain proposed tuning baselines. Runtime implementation tasks are still tracked independently in `TASKS.json`.
