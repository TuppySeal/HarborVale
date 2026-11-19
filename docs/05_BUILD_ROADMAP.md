# Build roadmap

Current implementation track: Unity 6000.6.0f1, user-authorized M11 map/model/driving prototypes and M12-T01 first time trial. The Godot-era queue below is a historical design baseline; read the live ledger and confirmed choices before selecting work. Added prototype tasks do not imply completion of the full career milestones.

This is an implementation queue, not a report of completed work. Every task starts `not_started`; the design documents do not constitute a running game. The first task is `M00-T01`. Use the status and handoff files named in `README.md` as the live record after implementation begins.

Baseline: Windows PC, Godot 4 with GDScript, offline singleplayer, and GTA-like semi-realistic driving with accessible weight and grip. Use the custom RigidBody3D/raycast approach in `04_TECHNICAL_BLUEPRINT.md`; this is not a hardcore simulation. Verify and pin the exact engine release during setup. If the owner changes platform or engine decisions, revise the affected tasks before building them.

The vertical slice contains one Dockside district, three car models, four career events (`evt_dock_01` through `evt_dock_04`), a garage/dealer, four used-car offers, inspections, servicing/wear, one separate workshop recovery job, upgrades, and persistent progression. V1 expands this into three connected districts, eight car models, sixteen career events, ten used-car offers, four reputation tiers, and six rivals. The four slice/ten V1 offers include the two healthy original starters at 4,000/5,000 credits. The workshop job is separate from the sixteen career events. Traffic, police, multiplayer, fuel management, deformable crash damage, and dynamic weather are outside V1.

Powertrain behavior follows `10_POWERTRAIN_AND_TUNING.md` and `design-data/powertrains.json`: RPM-dependent torque, meaningful naturally aspirated/turbo/supercharger character, working gearing and shifts, and paid sequential tune stages with supporting hardware. Condition, used-car history and maintenance follow `11_USED_CARS_MAINTENANCE_AND_BREAKDOWNS.md` and `design-data/maintenance.json` / `used_car_listings.json`. These are functional driving/ownership systems. The roadmap contains **42 tasks across 11 milestones**; added tasks preserve all earlier IDs.

## How an AI should execute this queue

1. Read `AGENTS.md`, `project/STATUS.md`, `project/NEXT_TASK.md`, and `project/TASKS.json`. Inspect the actual repository and run the recorded baseline check before changing code. A previous chat's claim is not verification.
2. Select the first incomplete task whose dependencies are verified. Keep one task active. Split a task into documented subtasks if it will not fit into one focused session; preserve its existing ID.
3. Implement the smallest complete behavior. Use placeholders until a milestone explicitly calls for final assets. Run the task's meaningful checks and record commands, results, and any manual observations.
4. Update `project/TASKS.json`, `STATUS.md`, `NEXT_TASK.md`, and `SESSION_LOG.md` together; add decisions or known issues to their corresponding files. Record exact files changed, what works, what fails, the next task ID, and the first command to run next time. A failed check remains a known failure, not a passed task.
5. Before context is nearly exhausted, stop starting new changes. Leave the repository buildable when possible; otherwise record the precise unfinished edit and reproduction steps. Preserve user changes. A commit is useful when Git is configured, but a commit alone is not a handoff.

Each milestone ends in a playable or inspectable artifact. Do not begin world expansion until the driving gate passes. Do not begin V1 expansion until the complete slice passes. Placeholder art is acceptable at these gates; incomplete core behavior is not.

## M00 — Establish a reproducible project

- **M00-T01 — Verify engine and environment.** Confirm the chosen engine release, renderer, supported Windows export target, installed editor, and export tooling. Record exact versions and launch commands. **Accept:** another session can identify the executable and open an empty project using the documented command. Missing tools are recorded with the specific installation step required; never report an unrun launch as successful.
- **M00-T02 — Create the minimal project and smoke scene.** Add the agreed folder structure, input actions, version-control exclusions, and a small scene containing a ground plane, camera, and visible marker. **Accept:** editor and standalone development launch show the marker, with no script/import errors. Avoid final assets and menus.
- **M00-T03 — Establish records and a repeatable check.** Create the implementation test entry point, asset provenance log, and concise environment instructions. Carry forward this task inventory into the canonical queue without marking unimplemented tasks done. **Accept:** a clean launch/import check can run and return a truthful pass/fail result; the asset log has fields for source URL, author, license, modifications, attribution, and intended use.

**Exit artifact:** a launchable empty project, pinned environment notes, and an executable baseline check.

## M01 — Prove that driving is enjoyable

- **M01-T01 — Build a graybox driving test ground.** Include a straight, wide and tight turns, braking markers, a slope, a curb, and safe reset positions. **Accept:** the scale and route allow repeated acceleration, braking, cornering, and recovery without leaving the test area.
- **M01-T02 — Implement the first car controller.** Implement the technical design's suspension/contact model, acceleration, braking/reverse, speed-sensitive steering, grip, and handbrake. Use one placeholder car and explicit tuning data. **Accept:** the car accelerates, stops, reverses, and completes ten consecutive laps without numerical explosions, persistent unintended hopping, or falling through the road. Log physics settings used.
- **M01-T03 — Add chase camera, input, and recovery.** Support keyboard and a standard controller, adjustable steering sensitivity, camera obstruction handling, and a reset action. **Accept:** reset returns an inverted or stuck car to a safe road transform with cleared unsafe velocity; camera motion remains readable during braking and reversing. Test actual controller hardware when available and record when unavailable.
- **M01-T04 — Review and freeze the initial handling baseline.** Compare two or three tuning presets on the same route and retain one. Expose a small debug overlay for speed, grounded wheels, and relevant forces. **Accept:** record a repeatable run demonstrating controllable braking, corners, slopes, and reset without instability; record owner feedback if provided. Automated checks cover invalid tuning inputs and repeated reset stability. The gate is the recorded handling acceptance evidence, not waiting for a new permission request.
- **M01-T05 — Prove RPM torque and induction response.** After the unchanged base-handling gate, inject data fixtures without building a shop. Implement torque interpolation, `kW = Nm × RPM / 9549.2966`, RPM/gear/final-drive/wheel-radius conversion, drivetrain efficiency, grip-limited drive force, and real shift interruptions. Compare naturally aspirated, factory-turbo, conversion-turbo, and positive-displacement-supercharger fixtures. **Accept:** logged fixed-gear roll-ons and corner exits distinguish equal-peak-kW curves; turbo boost threshold and load/RPM-dependent spool lag are separate; throttle release decays boost; a blower responds promptly with its parasitic loss already included in net torque. Zero/invalid RPM or speed cannot create a divide-by-zero/force spike. Faster transmission stages shorten an actual drive-force cut, not only the animation. Preserve the handling baseline and record new measurements rather than inventing performance claims.

**Exit artifact:** a playable driving prototype, recorded base-handling decision, and verified powertrain fixtures/measurements. These gates have priority over adding cars or scenery.

## M02 — Establish data, state, and safe persistence

- **M02-T01 — Add validated content definitions.** Define stable IDs and schemas for cars, powertrains, engine paths/packages, six upgrade categories, maintenance/components, used listings, events, workshop job and campaign gates using the canonical data. Cars reference `powertrain_id`; derive peak power from the selected net torque curve rather than retaining a competing scalar power upgrade. Validate curve samples, units, ratios, path compatibility, fees/prerequisites, vehicle/listing identity, duplicate IDs, and impossible prerequisites. Use the release-scope manifest without demanding future assets or duplicating balances. **Accept:** valid fixtures load; malformed fixtures report actionable errors; the slice exposes only its three models, four career events, four used offers, separate workshop job, supported paths and two upgrade stages.
- **M02-T02 — Implement game states and service boundaries.** Implement boot/menu, starter selection, free roam, garage/dealer, race preparation, countdown, racing, results, and pause according to the technical design. Use the handling-test world and stub service/result scenes until the real scenes arrive in M04/M05. Separate persistent profile state from transient scene/physics state. **Accept:** a scripted transition walkthrough rejects illegal transitions, prevents duplicate input handling, routes a zero-car onboarding profile to the starter dealer, and returns to free roam without spawning duplicate players or cameras.
- **M02-T03 — Implement versioned saves with recovery.** Save wallet, owned-car instances, selected engine path, installed stages, actual paid-step receipts/historical investments, active car, progress, and safe location. Derived curves/power/PI are recomputed; transient boost/shift state resets safely. Validate before writing, write to a temporary file, use the platform-appropriate replacement strategy, and preserve a known-good backup. **Accept:** new/load/reload preserves path locks, paid steps and resale basis without compounding power; malformed data, a truncated primary, interrupted writes and an older supported schema either recover correctly or show a clear recovery choice. Never overwrite the only valid save while recovering it.
- **M02-T04 — Build transaction and settlement rules.** Make purchases, sales, upgrades, and event rewards validate their full preconditions before applying a single committed profile update. Give each race attempt a stable settlement identity. **Accept:** insufficient funds, a skipped stage in any of the six categories, a repeated installed tune, incompatible/changed engine path, missing transmission prerequisite, stale ownership, cancellation, duplicate settlement, and simulated write failure leave no partial wallet/build/receipt change. Loading then replaying results cannot grant a second reward. Define retry/recovery behavior explicitly.
- **M02-T05 — Model condition, wear and append-only vehicle records.** Use fixtures before UI integration. Add six integer condition values (0–10,000): engine, cooling, transmission, brakes, tires and suspension. Preserve real vehicle identity, mileage, known/unknown fields, inspections, repairs, accidents, condition/heat and wear residuals. Separate lifetime records from current-ownership purchase/upgrade accounting; acquisition receipts validate any preinstalled stages. Implement deterministic wear and warned severe-failure state, deriving derating over the healthy build. **Accept:** fixed inputs reproduce wear; only actual driving counts; pause halts simulation. Reload/reset never mutates the base curve, heals parts, erases records or rerolls failure. Atomic service receipts stay separate from upgrade investment. All-broken ownership is valid and recoverable.

**Exit artifact:** validated data loading, persistent condition/history, and a profile that survives tested failure cases. Do not introduce live market or online services.

## M03 — Complete one race loop

- **M03-T01 — Implement a reusable route and checkpoint system.** Store ordered gates, direction, laps, respawn transforms, and route metadata separately from scenery. **Accept:** forward traversal advances once; reverse crossings, skipped checkpoints, overlapping gate volumes, and crossing the finish early cannot fake race completion. For a fast movement segment crossing several gates, sort intersections in travel order and accept only the expected valid sequence. Legitimate shortcuts remain valid only when all required gates are crossed.
- **M03-T02 — Implement countdown, timing, results, and exit.** Add one time trial through the full state flow. **Accept:** false starts are blocked; unroadworthy cars cannot enter; pause freezes clocks and mechanics; retry creates a new attempt; abandoning or pre-finish hard component failure (condition ≤1,000) gives DNF/no finish award; a valid finish settles once. A temporary heat-only cut keeps the race clock running and permits free live engine-off cooling. Quit/reload cannot turn a hard failure into a finish, heal condition, or remove an earlier durably settled reward. Reset cannot erase elapsed time or bypass required gates.
- **M03-T03 — Add opponent driving and placement.** Implement route-following rivals using the same car limits and the technical design's recovery rules. Rank by valid route progress and finish time. **Accept:** opponents finish repeated runs on straights and corners, recover from a simple obstruction, and do not receive unexplained speed or teleport advantages. Record any hidden recovery behavior.
- **M03-T04 — Add the other slice race types.** Extend the shared event framework to all four slice events defined in the world design. **Accept:** start/finish/retry/abandon work for each type; positions or scores match a controlled sample; event eligibility and results read their IDs from content data. Each race has one verified clean run and one recovery/missed-gate run.

**Exit artifact:** the four slice event behaviors working on temporary routes, with trustworthy outcomes and rewards.

## M04 — Make Dockside a usable open world

- **M04-T01 — Build the Dockside road graybox.** Implement its connected roads, garage/dealer locations, four event starts, and clear world boundaries from the world plan. **Accept:** all destinations are reachable in the starter car; the player cannot become permanently trapped; marked route lengths and cornering tests match the intended driving pace.
- **M04-T02 — Add navigation and discoverable interactions.** Add a readable world map, destination selection, route guidance, event prompts, and consistent garage/dealer interaction distances. **Accept:** a new player can find the first event and return to the garage without developer instructions; UI cannot trigger while inappropriate race states are active.
- **M04-T03 — Integrate routes and world recovery.** Move the four events onto authored Dockside roads and define road-aware recovery, boundary handling, garage arrival, and any unlocked travel behavior. **Accept:** resets near intersections, water/boundaries, and event gates select safe transforms; travel is blocked during a live race; loading into a changed or invalid world position recovers safely.
- **M04-T04 — Add used-car inspection and persistent listings.** Integrate four authored slice offers including the two healthy starters, persistent listing/real-vehicle IDs and listing revisions, disclosed known/unknown history and the canonical inspection report/fee. Buying remains integrated in M05; this task can use preview fixtures. **Accept:** inspection reveals the recorded snapshot/revision and stays saved; unknown history is never described as clean. Cancel, reopen, travel and reload do not refresh condition, erase repair/accident records or reroll offers. A repaired car retains its prior history. One-owned-car-per-model eligibility applies to all offers; missing/duplicate IDs fail validation.

**Exit artifact:** explorable Dockside, functioning activities, and persistent inspectable used listings. Keep geometry replaceable and art secondary.

## M05 — Close the ownership and wealth loop

- **M05-T01 — Implement the dealer and owned cars.** Integrate three slice models, healthy original starters, used purchases/buybacks, persistent real-vehicle IDs, confirmation, selection and garage spawn. **Accept:** healthy starters retain their fixed prices. A used purchase checks its displayed revision and atomically transfers that exact vehicle/build/condition/history, debits cash and consumes the listing. Dealer buyback preserves identity/path lock; reacquisition records a new actual purchase basis including the installed build, resets current-ownership upgrade investment to zero and retains lifetime records. Its acquisition receipt validates preinstalled stages without pretending a tune was skipped. Reject duplicate models/stale purchases; reload preserves all records.
- **M05-T02 — Implement upgrades and performance classes.** Connect six categories to paid sequential stages without skipping/repayment; choose and lock the engine path at stage 1. Use canonical fee rules scaled by catalog MSRP, never used purchase price. **Accept:** quote, prerequisites, debit, stage and receipt agree. New tire/brake/suspension stages renew only their mapped component to 10,000 in the paid replacement transaction; do not require repairing that old component first. Other-component/heat checks still apply. Engine/transmission packages require healthy core components and preserve their wear; weight reduction renews nothing. Reject missing hardware/service atomically. Healthy curves/PI recompute without bonuses compounding; derating remains separate. Preview/reload/rejected or duplicate stages never renew condition.
- **M05-T03 — Implement resale and recovery income.** Implement condition-aware resale/buyback and safe selling; verify workshop accounting with fixtures until M05-T06. **Accept:** valuation uses actual current-ownership purchase basis plus only new ownership upgrade spending; installed build value is never counted twice after reacquisition. Repairs remain operating expenses. Dealer buyback costs `max(last_paid, resale + 500)` under canonical rules; repeated sell/rebuy cannot profit, heal, reset a locked path or erase history. For a preinstalled nonengine stage, the next fee is next catalog stage total minus current installed catalog stage total, never minus reset ownership investment. Selling active/last car remains safe.
- **M05-T04 — Connect progression and garage presentation.** Display wallet, collection value, car/build details, eligible events, and next unlocks. **Accept:** an end-to-end sample earns money, buys a part, wins a newly accessible event, buys a second car, and reloads with the same progression. Race rewards, upgrade costs, and tier gates must come from the canonical data, not duplicated UI constants.
- **M05-T05 — Verify sequential engine builds and dyno preview.** Compare current/proposed net torque and kW curves, path, hardware, next-step fee and prerequisites. Label preview as steady-state full-load output; live spool/shifting stays dynamic. **Accept:** slice purchases exercise stages 1/2; fixtures reject a 1→3 skip and repeated/skipped installs in all categories. Missing prerequisites, stale quotes, locked path changes and failed saves leave money/build untouched. Acquisition receipts permit preinstalled stages without resetting the path; only current-ownership new spending adds upgrade resale value. Paid tunes change measured roll-ons/corner exits; sound, gauges or PI alone do not pass. Reload preserves healthy curves/PI and receipts.
- **M05-T06 — Integrate servicing, breakdowns and workshop recovery.** Connect condition warnings, deterministic wear/heat, derating, roadworthiness, free tow and service transactions. Add the separate loaner time trial paying 300 once per valid run, no reputation/first-clear bonus or owned inventory/resale/wear. **Accept:** warned hard component failure (≤1,000) before finish produces DNF; heat-only protection at 9,500 instead keeps the race clock running and permits free live engine-off cooling. Persist its latch until below 7,500; offline/Pause/reset/tow never supply cooling. Heat alone requires neither paid repair nor a loaner job. Reload preserves wear/history. Service debit/receipt commit together without upgrade value. At zero cash/all cars unroadworthy, free tow and at most four jobs fund the capped 1,200 minimum fix. The loaner never replaces career ownership/active identity or unlocks career races; settlement is idempotent.

**Exit artifact:** the complete slice economy, inspectable ownership history, servicing/breakdowns, useful dyno comparisons, and a verified escape from the zero-cash/all-broken state.

## M06 — Validate the complete vertical slice

- **M06-T01 — Add the first-session tutorial.** Implement the short, skippable introduction using real controls, events, and purchases. Explain condition warnings, known/unknown used history, inspection, service quotes and free tow/workshop recovery without forcing a risky purchase. **Accept:** a fresh player buys a healthy starter, reaches a race, receives payment, installs an upgrade, and identifies a next goal; skipped/completed prompts remain so after reload.
- **M06-T02 — Run slice acceptance and repair failures.** Play from a fresh save through all four career events, all three models and the four used offers; repeat with an existing save. Test service/history persistence, paid mapped-component replacement, all-broken recovery, workshop-job duplicate settlement, hard component failure/DNF versus resumable heat-only cuts, sequential tunes, induction/shift behavior, dyno, pause/reset/abort, corrupt-primary recovery and controller reconnect. **Accept:** every required path passes; unavailable required checks leave the task incomplete/blocked. No save loss, history washing, free renewal, unusable recovery path, broken controls, cosmetic-only engine upgrades or progression blocker remains.
- **M06-T03 — Package and review the slice.** Produce a development export, record build/version and known issues, and measure a representative race on the agreed reference machine. **Accept:** the export launches outside the editor and completes the core loop. Record feedback if available and the evidence that the slice gate passes. Optimize or reduce scope before expansion if frame pacing or handling is unacceptable.

**Exit artifact:** a reviewable slice build and acceptance report. V1 expansion depends on this gate.

## M07 — Expand to the complete V1 campaign

- **M07-T01 — Add Midtown and Ridgeway.** Extend the connected road network, district landmarks, event-entry markers, navigation, and recovery coverage. **Accept:** travel works across district boundaries without reputation walls; event gates cannot strand the player; the reference-machine measurement is repeated after adding geometry.
- **M07-T02 — Add the remaining five cars and twelve events.** Reach eight V1 models, ten used offers and sixteen career events: Dockside six, Midtown five, Ridgeway five. Keep the single workshop job outside that count. Enable stage 3 and all authored paths. **Accept:** content validation passes; all models' condition/service prices and supported builds pass checks; all ten offers preserve distinct real-vehicle records; every car drives a route; every career event receives start/finish, reward, roadworthiness and reset checks. No placeholder route is mislabeled finished.
- **M07-T03 — Add rivals, progression gates, and completion.** Integrate all six rivals and the four career tiers with their authored events, brief dialogue, clear next goals, final challenge, and postcampaign repeat play. **Accept:** the campaign dependency graph has a valid path from a fresh save to completion; the final unlock occurs once; repeat events remain available with the specified repeat rewards.

**Exit artifact:** a complete graybox campaign with no missing required content.

## M08 — Integrate presentation and usable settings

- **M08-T01 — Replace placeholder visuals responsibly.** Integrate licensed car/world assets, district identity, materials, lighting, signs, and restrained effects. **Accept:** every external asset has a completed provenance record and compatible redistribution terms; scale/collision and silhouette readability pass driving checks; imported art does not alter race validity or remove reset markers.
- **M08-T02 — Add audio and feedback.** Implement engine/load/speed response, tire and impact feedback, menu sounds, race cues, and music controls. **Accept:** audio levels can be adjusted independently; repeated events do not leak sounds; race-critical information is also visible; licensed music and sound attributions are complete.
- **M08-T03 — Finish HUD, menus, and settings.** Implement the specified navigation hierarchy, readable text, control rebinding, sensitivity/deadzones, camera options, accessibility options, and graphics/audio settings. **Accept:** keyboard and controller can complete all required flows without a mouse; settings persist; safe defaults recover from invalid display settings; prompts match current bindings.

**Exit artifact:** a cohesive playable V1 with presentation and settings integrated into the existing loop.

## M09 — Balance, performance, and campaign QA

- **M09-T01 — Measure and tune progression.** Run documented novice and experienced-player scenarios using the same starting profile. Record race durations, earnings, first upgrade/second-car milestones, unlock pace, and viable builds. **Accept:** observed results are compared with the economy targets; changes update the canonical data and examples; no mandatory unrepeatable reward is needed to escape a progression dead end.
- **M09-T02 — Profile and improve frame pacing.** Measure CPU, GPU, memory, loading, and frame times on the recorded reference configuration across busy streets, hill views, and races. Change one evidenced bottleneck at a time. **Accept:** meet the documented target or record an agreed scope/quality reduction; repeat the same capture route after each meaningful optimization. Avoid claiming performance for untested hardware.
- **M09-T03 — Run full regression and accessibility checks.** Exercise fresh/old saves, the complete campaign, event boundaries, all cars, upgrade classes, controller/keyboard flows, readable HUD, settings, and recovery. **Accept:** no release-blocking crash, save loss, transaction exploit, softlock, or unfinishable event remains; remaining cosmetic issues have exact reproductions and recorded priority.

**Exit artifact:** measured balance/performance notes and a completed regression report.

## M10 — Produce a reproducible release candidate

- **M10-T01 — Freeze content and prepare export.** Assign a version, lock tested engine/dependencies, remove debug-only entry points from the release flow, finalize credits/licenses, and document save locations and supported migrations. **Accept:** release packaging includes required runtime files and notices; development-only controls cannot grant money or skip career gates in the normal build.
- **M10-T02 — Test a clean release installation.** Launch the packaged game from outside the source tree, using a clean profile and an upgraded supported profile. Complete a race, a purchase, an upgrade, exit/reload, settings changes, and a cross-district trip. **Accept:** no editor, developer paths, or missing imported assets are required; the version/build and checksums identify the artifact reviewed.
- **M10-T03 — Package final handoff and known issues.** Include source, build instructions, the tested export, licenses, controls, save/recovery instructions, and concise known issues. **Accept:** the recorded build procedure produces a working package and all required tasks are verified. Publishing or uploading is a separate owner-directed action, not implied by creating a release candidate.

**Exit artifact:** a reproducible local release candidate and truthful completion record.

## Task inventory

Dependencies are task IDs separated by commas. A dependency is satisfied only when its acceptance checks are recorded as passed. `-` means no dependency. This table is the initial inventory; the live task queue becomes the status authority during implementation.

| ID | Title | Dependencies | Initial status |
|---|---|---|---|
| M00-T01 | Verify engine and environment | - | not_started |
| M00-T02 | Create minimal project and smoke scene | M00-T01 | not_started |
| M00-T03 | Establish records and repeatable check | M00-T02 | not_started |
| M01-T01 | Build driving test ground | M00-T03 | not_started |
| M01-T02 | Implement first car controller | M01-T01 | not_started |
| M01-T03 | Add camera input and recovery | M01-T02 | not_started |
| M01-T04 | Review handling baseline | M01-T03 | not_started |
| M01-T05 | Prove RPM torque and induction response | M01-T04 | not_started |
| M02-T01 | Add validated content definitions | M01-T05 | not_started |
| M02-T02 | Implement game states and boundaries | M02-T01 | not_started |
| M02-T03 | Implement versioned saves and recovery | M02-T02 | not_started |
| M02-T04 | Build transaction and settlement rules | M02-T03 | not_started |
| M02-T05 | Model condition, wear and append-only vehicle records | M02-T04 | not_started |
| M03-T01 | Implement routes and checkpoints | M02-T05 | not_started |
| M03-T02 | Implement timed race loop | M03-T01 | not_started |
| M03-T03 | Add opponents and placement | M03-T02 | not_started |
| M03-T04 | Add remaining slice race types | M03-T03 | not_started |
| M04-T01 | Build Dockside road graybox | M03-T04 | not_started |
| M04-T02 | Add navigation and interactions | M04-T01 | not_started |
| M04-T03 | Integrate routes and world recovery | M04-T02 | not_started |
| M04-T04 | Add used-car inspection and persistent listings | M04-T03 | not_started |
| M05-T01 | Implement dealer and owned cars | M04-T04 | not_started |
| M05-T02 | Implement upgrades and classes | M05-T01 | not_started |
| M05-T03 | Implement resale and recovery income | M05-T02 | not_started |
| M05-T04 | Connect progression and garage UI | M05-T03 | not_started |
| M05-T05 | Verify sequential engine builds and dyno preview | M05-T04 | not_started |
| M05-T06 | Integrate servicing, breakdowns and workshop recovery | M05-T05 | not_started |
| M06-T01 | Add first-session tutorial | M05-T06 | not_started |
| M06-T02 | Validate slice and repair failures | M06-T01 | not_started |
| M06-T03 | Package and review slice | M06-T02 | not_started |
| M07-T01 | Add Midtown and Ridgeway | M06-T03 | not_started |
| M07-T02 | Add remaining cars and events | M07-T01 | not_started |
| M07-T03 | Add rivals and campaign completion | M07-T02 | not_started |
| M08-T01 | Integrate visual assets | M07-T03 | not_started |
| M08-T02 | Integrate audio and feedback | M08-T01 | not_started |
| M08-T03 | Finish menus HUD and settings | M08-T02 | not_started |
| M09-T01 | Measure and tune progression | M08-T03 | not_started |
| M09-T02 | Profile and improve frame pacing | M09-T01 | not_started |
| M09-T03 | Run full regression | M09-T02 | not_started |
| M10-T01 | Prepare release export | M09-T03 | not_started |
| M10-T02 | Test clean release installation | M10-T01 | not_started |
| M10-T03 | Package final handoff | M10-T02 | not_started |

| M11-T01 | Author and verify labeled Unity city greybox | - | done |
| M11-T02 | Expand and scale curved street map with starter-car model | M11-T01 | done |
| M11-T03 | Driveable stock Aster Finch in Unity city | M11-T02 | done |
| M12-T01 | First Dockside time trial with saved race earnings | M11-T03 | done |
| M12-T02 | Garage purchase and persistent Stage 1 Finch tune | M12-T01 | done |
| M12-T03 | Functional Finch turbo, gearbox support and intercooler upgrades | M12-T02 | done |
| M12-T04 | Sports test car with grip, downforce and sequential test upgrades | M12-T03 | done |
| M12-T05 | Unified launch menu and live sports acceleration testing | M12-T04 | done |
| M12-T06 | Smooth automatic downshifts and optional paddle-style manual transmission | M12-T05 | done |
| M13-T01 | Verify and repair isolated Claude garage preview | M12-T06 | done |
| M13-T02 | Verify isolated conditional audio and visual effects demo | M13-T01 | done |
| M13-T03 | Verify distinct Finch and Apex dashboard previews | M13-T02 | done |
| M14-T01 | Polished easy-to-use one-shop garage interface | M13-T03 | done |
| M14-T02 | Expanded induction, brake and optional burble/flame tune catalogue | M14-T01 | done |

### M11-T01 — User-requested Unity map prototype

Author native editable boxes, roads, labeled districts/locations, event starts and inspection cameras in Unity. Preserve source layout and stable IDs. Accept when the source compiles, map data is structurally valid, and the actual scene imports/renders in Unity for inspection. A missing editor license leaves the editor check blocked; source/coordinate previews are not Unity rendering evidence. Record the exact editor path and next action. Do not add gameplay systems during this task.

## Mileage acceptance within existing tasks

- **M02-T05:** permanent odometer and fractional distance survive save/reload, repairs and ownership transfers; acquisition distance and trip reset remain separate. Derive component service mileage only from scoped work, allowing unknown values.
- **M04-T04:** used comparisons show miles/kilometers, documented/unknown history and service mileage. Verify exact unit conversion and flag inconsistent records without claiming missing history is clean.
- **M05-T06:** component checks are advisory mileage reminders. Inspections acknowledge checks without inventing service or healing parts. Only actual scoped service/replacement updates service mileage; a brake replacement does not reset engine/chassis distance. No additional task ID or price multiplier is introduced.

## Release-blocking criteria

Treat crashes, lost valid saves, duplicate payouts/charges, negative balances, erased vehicle history, ownership lost after first purchase, required races that cannot finish, persistent broken controls and campaign/recovery softlocks as blockers. Owning only unroadworthy cars is valid if the free tow/loaner-job/minimum-fix path remains available; the temporary loaner is never an owned car. Zero-car onboarding remains valid before the first purchase. Cosmetic defects need reproducible known-issue entries. Never close a task because context is running out, because code exists, or because a later task might repair it.


### M11-T02 — Scaled city expansion and starter-car visual

User authorized November 18: expand the map, use believable dimensions, curve roads beyond the initial right-angle layout, and add the starter car model. Accept when continuous curved roads and shared junctions import/render, dimensions and grades pass geometry checks, stable service/event IDs persist, and a reusable fictional Aster Finch prefab is visible at garage scale. Preserve the prior scene before rebuilding. Actual editor rendering is required; manual driving is outside this model/map task. Record remaining input/playtest limitations.



### M11-T03 — Driveable stock Finch

User authorized driving November 18. Add physical stock-car suspension/traction, RPM-dependent authored stock torque curve, automatic gears, throttle/brake/reverse, steering, handbrake, chase camera, HUD and safe recovery. Preserve the city and an independent handling test scene. Verify acceleration, braking, reverse, turns, handbrake, resting stability, collisions/recovery, city road contacts and rendering; manually check keyboard/camera if available. Record measured evidence and unperformed checks. This does not complete the larger stock/upgraded-handling milestone or authorize economy/racing systems.


### M12-T01 — First playable Dockside time trial
Accept when evt_dock_01 starts near the player, counts down with locked controls, requires ordered directional swept-gate crossings, tracks time, displays medal/results and credits valid rewards from design data. Early finish, missed/wrong-direction checkpoints, teleport/recovery, abort and duplicate settlement cannot award undeserved credits. Recovery restores a passed route checkpoint and adds 3 seconds; clock continues during map/focus changes. Save earnings/receipt/best-time/reputation atomically; replay first-clear and best-result reputation cannot duplicate; primary corruption can recover a validated backup. No silent erase when all saves are invalid. Validate rule fixtures, reload/error paths, actual Play Mode integration and Windows export. User route/handling/medal pacing remains a review item; no AI rivals or complete career economy are claimed.



### M12-T02 — First paid Finch garage tune
Accept when a nearby garage opens only while stopped outside a race, quotes the design-data-derived Stage 1 fee and before/after torque curve, rejects insufficient funds/repeats/stale quotes, and atomically saves debit/receipt/installed tune. Valid v1 race receipts/earnings import once into a separate v2 profile; old files remain intact and later reloads do not duplicate funds. Failed save does not install a tune or debit cash. Reload/reset/recovery preserve the installed curve. Physics comparisons demonstrate a measurable stock-versus-Stage1 acceleration difference using authored curves. Check race purchases/modal locking, result flow, actual Windows UI/export, and leave real player profiles untouched by tests.


### M12-T03 — Functional turbo/intercooler parts
Accept when one garage sells real gearbox support, a compatible turbo Stage-2 package and a street intercooler with paid prerequisites. Imported authored steady/off-boost curves and exponential spool drive torque without double boost multiplication. Gearbox support reduces actual shift interruption; intercooler selects a complete matched curve that preserves off-boost behavior. Historical fees, Stage-1 payments and all race progress migrate to v3 atomically; rejects missing hardware, skips, duplicate/stale/insufficient purchases and failed-save installation. Native tests compare NA, turbo and upgraded-cooler power/acceleration/roll-on, per-car spool and shift/throttle decay. Verify actual Play Mode, Windows UI and restart. Supply completed/to-do checklist and preserve independent Claude project boundaries.



### M12-T04 — Sports test car
Separate city test scene with original sports visual, higher grip/brakes, AWD and speed-squared downforce. Same four sequential upgrades with stronger authored torque curves and per-car spool, free only in the test scene with no career transactions. Verify actual physics acceleration, grip/aero settings, prerequisites, recovery and Windows rendering/export. Preserve Finch settings and player saves.

### M12-T05 — Unified testing flow
One Windows entry menu selects regular saved Finch or free Apex sports tests and can return without duplicate controllers/physics/garage/trials. Returning from an active race abandons without payout; unresolved valid finish-save errors block leaving. Sports mode never changes career progress. Add live stopped-start 0–100 timer, current-configuration session best, and cancellation on recovery, changed build, prolonged air or map input lock. Verify repeat scene transitions, paid build reload, timer boundaries and actual Windows rendering/export. No ownership/garage-art integration implied.

### M12-T06 — Smoother gearbox and driving response
Automatic matches gears to road speed when slowing, uses safe throttle kickdown and avoids hunting. Manual paddle-style M/Z/X controls reject unsafe over-rev downshifts, reverse shifts, overlapping shifts and garage/map/countdown inputs. Show current mode/gear and rejection feedback; recovery/upgrades preserve selected mode, with first-gear stop assist. Smooth keyboard throttle/brake and shift re-engagement without altering authored torque curves, paid hardware or saves. Verify original sluggish high-gear scenario, safe shift boundaries, throttle/shift continuity, real native decel/reaccel and Windows integration for Finch/Apex; retain failed checks and manual-feel limits.

### M13-T01 — Garage verification before integration
Separate corrected copy, preserve original Claude and main game. Compile, generate/reopen an editable scene, static real Finch, working four-part selection with actual procedural meshes, one counter, preview-only installs, camera orbit/zoom/reset and readable UI. Capture actual rendered walkthrough/screenshots; export minimal source/dependency package and state verification limits. No gameplay integration.

### M13-T02 — Conditional effects verification
Only after garage verification. Distinct true Finch/Apex visuals, turbo gated by hardware/load/boost, tire squeal by grounded slip/speed, supercharger preview-only hardware gate, exhaust conditional preview. Visual-only animation never writes body/wheels/physics. Test gating, switching and clean scene startup, capture actual video with native mixed audio and provide measured audio/sample evidence and editable source/package. No untested listening/realism claim or game integration.

### M13-T03 — Dashboard preview review
Review latest Claude gauge scripts in a preserved copy. Distinct Finch/Apex visual layouts, readable speed/RPM scales/redline, distance units and explicit demo telemetry labels. Fix time-only phantom mileage; simulated trip integrates declared speed, stationary does not advance. Real odometer remains unconnected until a future gameplay persistence adapter exists. Verify native visual switch/needle bounds and export source; no fake saved game mileage or main integration claim.

### M14-T01 — Garage usability/design first
User explicitly prioritizes an easy and attractive GUI. Clear category navigation, readable part cards, current-build context, visible credits and one detail/quote area combining price, requirements, next action and performance tradeoffs. Unavailable parts explain why and what is needed. Responsive supported layouts, stable modal input and actual purchase/save behavior; native screenshots required. One shop remains.

### M14-T02 — Expanded compatible parts and tunes
After review/interface, add small/large turbo choices, supercharger alternatives and appropriate twin-turbo setups, real brake upgrades and different calibrated tunes. Burble/flame presets are optional with a disclosed small real power loss. Complete authored torque curves and hardware prerequisites, exclusive induction choices rather than accidental stacking, actual brake effects, state-gated audio/visuals and atomic migration preserving historical paid receipts. Verify persistent builds and measured behavior; no invented service/ownership history.


### M14-T03 — Chassis, wheelspin, dyno and staging (done)
Three sequential paid LSD/suspension/gearing/aero stages, parked setup, physical driven-wheel burnout, HP/estimated WHP/weight and W+S tune-dependent staging. Native rules and player checks passed; see project/CHASSIS_HANDOFF.md.


### M14-T04 — Selectable turbo spool/release audio (done)
State-driven airflow, BOV/flutter, free garage audio preferences/previews, native pulls/audio and paid-state isolation. See project/TURBO_AUDIO_HANDOFF.md.


### M14-T05 — Driving audio, effects and camera polish (done)
Sample-smoothed existing mix, preview/scene fades, wheel/flare/skid animation cleanup, RMB and J/L/K view controls. See project/AV_CAMERA_HANDOFF.md.


### M15 — Cleaner rendering and isolated map art
M15-T01 done: Linear/4xMSAA/tonemap, shadow cleanup and measured static batching on unchanged map. M15-T02 Claude roadside props and M15-T03 Antigravity building prototypes prepared but awaiting UI dispatch. See project/RENDERING_HANDOFF.md.


### M15-T02–T04 — Reviewed external art and first Midtown pass (done)
Three building prototypes and four corrected props reviewed;17 existing plots dressed with facades,34 lamps and17 planters while preserving physical map. See project/MAP_ART_HANDOFF.md.
