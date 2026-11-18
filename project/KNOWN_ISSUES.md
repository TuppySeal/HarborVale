# Known issues and open risks

## Implementation defects

None recorded yet because game implementation has not started. This does not imply the unimplemented game has passed any checks.

## Setup information

- `godot` was not present in the planning session's PATH lookup. Installation elsewhere was not investigated. M00-T01 must establish the actual environment.

## Design assumptions to validate

| ID | Assumption / risk | Validation task |
|---|---|---|
| R001 | Custom raycast handling can reach the requested GTA-like feel | M01 handling gate and M06 slice playtests |
| R002 | A compact 7–9 km road network supports sixteen enjoyable route variants | M04/M07 driving and route review |
| R003 | A city without moving traffic still feels inviting | M06 feedback; consider traffic only after core completion |
| R004 | Best-result reputation's early skill gate is understandable | M06 tutorial and M09 novice progression report |
| R005 | Payouts and prices produce satisfying gradual growth | M09 measured balance scenarios |
| R006 | A reference PC can sustain the initial 1080p/60 fps target | M06/M09 profiling; record hardware and quality settings |
| R007 | Appropriate car/world/audio assets fit the production budget | M00 asset register, M08 provenance and integration |
| R008 | Torque curves, gearing and boost response make different builds perceptibly different while retaining accessible driving | M01-T05 engine prototype and M05-T05 dyno/purchase integration |
| R009 | Sequential fees and supporting-part prerequisites preserve reasonable progression costs | M05/M09 engine-build examples and novice progression report |
| R010 | Used reports and quoted repairs make worn purchases understandable | M04-T04/M05-T06 listing, inspection and service checks |
| R011 | Wear, heat and breakdowns remain fair through normal healthy play | M02-T05 fixtures and M06/M09 playtests |
| R012 | Zero-cash recovery and buyback preserve the career without economic exploits | M05-T06 and release regression, including all-broken saves |

These are planning risks, not invented runtime failures. Do not label an implementation task blocked solely because one of these questions needs its scheduled experiment.

## Defect entry format for future use

```text
ID / severity / status:
Affected task and build:
Expected:
Observed:
Reproduction steps:
Evidence path:
Workaround:
Next investigation:
Resolved by / verification:
```

## K001 / low / open — Unity editor search indexing
Affected: isolated Unity 6000.6.0f1 verification editor. At startup UnityEditor.Search.SearchDatabase.EnumerateAll/GetDefaultSearchDatabase throws ArgumentOutOfRangeException. Reproduced across batch editor launches; no game-runtime stack. Play Mode test records this editor-only issue separately; all other runtime errors still fail. Windows player does not include editor search code. Evidence: artifacts/qa/M11-T03/playmode-results.txt; local .work/UnityDrivingValidation/driving-playmode.log. Next investigation: Unity editor search indexing configuration or a version-matched upstream fix; do not silently upgrade the pinned engine.

