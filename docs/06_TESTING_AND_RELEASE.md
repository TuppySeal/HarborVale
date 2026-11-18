# Testing and release

This is the verification plan for implementation. No game tests or driving playtests have been run in the initial design session. Validation of the written package is a separate activity.

## Evidence rules

Every implementation task records its actual command or play procedure, the result, and relevant artifacts. Store evidence under `artifacts/qa/<task-id>/`. Record engine version, content revision and hardware for handling/performance reports.

Use automated checks for deterministic rules and manual play for feel, visual clarity and device behavior. A check that could not run is `not_run`, with a reason and the exact next action. It is never a pass.

## Automated coverage

| Area | Required checks |
|---|---|
| Content | Unique IDs; valid car→powertrain/path/package references; finite physics values/ordered RPM; positive ratios/radius; sequential fees/prerequisites; six component conditions in 0–10,000; four slice/ten V1 offers including the two healthy starters; sixteen career events plus one separate workshop job; correct scope membership |
| Stat derivation | Stock and every compatible path/stage; healthy curve-derived power/PI; condition/heat derating recomputed as a separate layer; repeated derivation/reload never compounds upgrades or permanently damages the base curve |
| Torque and units | Nm/RPM/kW conversion; curve interpolation/endpoints; zero/invalid RPM and speed guards; gear/final-drive/efficiency/wheel-radius conversion; shared grip/traction force cap |
| Induction | NA throttle response; turbo RPM/load threshold separate from time-dependent spool; loaded/unloaded throttle steps; boost release/reapplication; prompt positive-displacement response; parasitic loss counted once in net curves |
| Transmission | Actual drive-force cut during each shift; shorter upgraded shift duration changes acceleration; no torque multiplication twice; stable automatic shift boundaries; no visual-only shift improvements |
| Dyno preview | Current/proposed torque and kW samples match canonical build derivation; peak labels use the same calculation; steady-state full-load preview is distinguished from live transient boost and per-gear wheel force |
| Currency | Integer math; exact affordability boundary; insufficient funds; invalid amounts; overflow/range checks |
| Transactions | Buy once; reject duplicate model/stale listing revision; cancel/stale/repeated requests; reject skipped/installed stages, incompatible paths, missing transmission or required service; atomic buy/inspect/service/tune/replacement records; never require service of the old component being replaced; safe sell active/last-car rules |
| Tune spending | Catalog MSRP scales upgrades; engine full next-step fee; other-category fee = next catalog stage total minus current installed catalog stage total, including preinstalled builds; never subtract current-ownership investment; atomic payment/stage/receipt; no free renewal |
| Resale and buyback | Current actual purchase basis plus only new ownership upgrade investment; no double-counting preinstalled build; buyback `max(last_paid, resale + 500)`; same vehicle/build/condition/history/path; acquisition receipt validates installed stages; no profitable trade cycle |
| Used inspection | Persistent listing and real-vehicle IDs/revision; known versus unknown records; report matches actual inspection snapshot; no free refresh/reroll; purchased car retains repairs/accidents/mileage; sold listing cannot be purchased twice |
| Wear and heat | Deterministic six-component updates; only actual driven distance/load; fractional residuals survive reload; no offline/paused/teleport wear; no healthy-car random failures; severe failure follows warning; reset never repairs/cools for free |
| Service and breakdown | Exact quote/target; affordability boundary; paid receipt separate from upgrade investment; atomic rollback; mechanical roadworthiness; pre-finish hard component failure (≤1,000) gives DNF; settled earlier rewards remain intact |
| Heat-only protection | 9,500 drive cut keeps race clock running; persisted latch releases only below 7,500; free live engine-off cooling; no offline/Pause/reset/tow cooling; heat alone requires neither paid repair nor emergency loaner |
| Paid component replacement | Next tire/brake/suspension stage renews only its mapped component to 10,000; other condition/heat prerequisites apply; engine/transmission/weight preserve core condition; duplicate/rejected/preview operations never renew; stage/payment/condition/history commit together |
| Emergency recovery | Free tow with zero cash; all-owned-cars-broken profile loads; temporary loaner job pays exactly 300 once/run; no reputation/first-clear/owned-car wear/resale; minimum roadworthy fix ≤1,200; at most four jobs recover from zero |
| Rewards | First clear once; repeat cash rules; best-result rep delta; worse result zero extra rep; duplicate run settlement zero extra reward |
| Progression | Every tier reachable; every required event reachable; at least one eligible earnable build at each gate; final win sets completion once |
| Race rules | Ordered gates; direction/misses/lap closure; finish early; retry ID; pause; abort; reset penalty; reject mechanically unroadworthy entry without a new PI cap; breakdown/quit cannot fake a finish |
| Saves | Round trip of paths/stages/paid steps, condition/thermal/wear residuals, mileage, vehicle history, listing/report revisions and workshop settlements; derived curves/PI match; migrated fixture; invalid/newer data; backup/interrupted-write recovery; no history washing |
| Settings | Rebind persists; restore defaults; dead-zone bounds; invalid display preference falls back safely |

Use independent expected examples from the economy document for financial tests. A test that calls the same price function twice does not independently verify the formula.

Give SaveService an injected test directory under `user://test_runs/<run-id>/`. Never point automated fixtures at the real career files. Clean up only the verified test directory. Keep each fixture's initial state and expected result inspectable.

Physics regression checks use tolerances and measured ranges. Fixed input replay is useful for comparison but is not assumed to reproduce identical rigid-body states on every machine.

## Powertrain fixtures and measurements

Use `10_POWERTRAIN_AND_TUNING.md` and `design-data/powertrains.json` as the model and numeric source. M01-T05 uses injected fixtures before the shop exists; M05-T05 repeats relevant cases through real sequential purchases. Do not replace torque curves with a peak-power multiplier or claim that displaying a dyno proves the force model.

Independent calculation checks include:

- A 200 Nm sample at 3,000 RPM produces approximately 62.83 kW using `kW = Nm × RPM / 9549.2966`. Interpolating 100 Nm at 2,000 RPM and 200 Nm at 4,000 RPM gives 150 Nm at 3,000 RPM. Check the specified endpoint, idle and redline behavior explicitly.
- With 200 Nm net engine torque, gear ratio 2, final drive 4, efficiency 0.9 and wheel radius 0.3 m, the requested total longitudinal wheel force is 4,800 N before traction limiting. Distributing that force to driven contacts must not multiply it by wheel count; combined cornering/acceleration remains inside the tire-force limit.
- Zero road speed is valid. Invalid radius, ratios, nonfinite input or malformed curves produce a safe validation failure. Never derive launch force by dividing power by zero speed. Changing mph/km/h display must not change physics.
- Peak kW and PI follow the canonical interpolation/peak-search rule, including possible maxima between authored RPM points. Selecting a different curve with the same peak can retain a similar headline PI while changing useful torque across the operating range.

For response measurements, hold mass, tires, gear, road, starting speed/RPM, input sequence and assistance constant. Record actual engine RPM, throttle/load, target/current boost, net engine torque, selected gear, shift state, requested wheel force, traction-limited force and speed against time. State tolerances and fixture versions; do not infer exact cross-machine rigid-body determinism.

1. **Curve shape:** compare two fixtures with the same peak kW but different low/midrange torque. A fixed-gear roll-on and the same corner exit must show explainable differences in force/speed histories. Avoid a traction-saturated test that masks both curves.
2. **Turbo threshold versus lag:** run loaded throttle steps below and above the authored operating threshold. High enough RPM enables boost but does not make full boost instantaneous; remaining at low RPM cannot bypass the threshold by waiting. At adequate RPM compare low-load and high-load steps, then release/reapply throttle and inspect boost decay/recovery. A simple RPM-only switch fails this check.
3. **Supercharger character:** compare a positive-displacement blower with an otherwise controlled turbo fixture. Blower torque follows throttle promptly without an invented turbo-style spool delay. Its published net torque already includes parasitic drive loss, so the controller must not subtract it again. Supporting hardware is included in the selected engine package; do not apply its effects twice.
4. **Gearing and shifts:** compare logged RPM/force to ratios and radius; then measure stock and upgraded transmission shift interruptions. A faster stage must reduce lost drive time and change the measured run. Faster gauge animation or audio alone does not pass. Include low-speed launch, redline/upshift, braking/downshift and wheel-airborne cases.
5. **Dyno truthfulness:** compare preview samples/peaks to the selected build's steady-state net torque/kW. A turbo dyno is the authored full-load, settled-boost curve; instantaneous live output can be lower while spooling. Label engine output separately from wheel force or wheel power, and keep the current/proposed comparison stable after rebuild, selection changes and reload.

Record plots or tables and a short driving capture under the relevant task's evidence directory. Repeat purchased-build measurements after real garage integration; stage-3 fixture checks in the slice do not unlock stage 3 in the production slice. M07 validates actual V1 stage-3 purchases and every supported per-car path.

## Sequential tune transaction checks

Run these cases for engine, tires, suspension, brakes, weight and transmission. Stock→1→2→3 is a paid sequence; a reputation unlock never permits skipping a missing stage. At engine stage 1 select only a path supported by that car, then keep the path locked for V1. Treat naturally aspirated, factory-boosted and conversion packages according to their own data rather than assuming every car supports every induction type.

- Compare each quoted next-stage charge with canonical reference fees/scaling. Engine steps have full paid-step fees. Other categories charge the difference between the next and current **installed-stage catalog totals**, including on a used/preinstalled build; never subtract current-ownership upgrade investment. Persist the actual debit for current-ownership accounting, not a recomputed future price. Stage 0 has catalog total zero.
- Reject stock→2, stock→3 and 1→3, an installed-stage repayment, a path change after stage 1, insufficient reputation/funds, an unmet `required_transmission_stage`, or required condition service not performed. Check exact affordability and prerequisite boundaries. A paid new tire/brake/suspension stage replaces only its mapped component; exclude that old component from pre-replacement service checks. No rejection charges credits, installs hardware, changes condition/stage or appends a receipt. Engine/transmission/weight stages preserve core wear.
- Buy supporting transmission stages separately when required, then buy the engine step. Verify the quoted hardware/tune description, derived curve, wallet, stage and receipt all agree. Cancel, double-click, replay a stale request, or inject a failed save: the previous committed build and money must remain usable without partial installation.
- Save/reload a mixed-category build, rebuild repeatedly, sell an eligible extra car and reacquire it. Curves/PI/condition and the engine path stay unchanged. The new purchase basis includes its installed build; current-ownership upgrade investment resets to zero while lifetime receipts remain. Resale adds only subsequent ownership upgrade spending once. Its acquisition receipt proves installed stages, so the next sequential upgrade remains possible without repaying old stages. Catalog changes never rewrite old spending or create refundable duplicate tunes.

These are deterministic transaction checks. Any assertion about stronger acceleration, turbo feel, or useful shifting additionally needs the measured driving evidence above.

## Used cars, condition, history and workshop recovery

Use `11_USED_CARS_MAINTENANCE_AND_BREAKDOWNS.md`, `design-data/maintenance.json` and `used_car_listings.json` as the source for thresholds, wear rates, inspection fees, service quotes and valuation. M02-T05 proves the pure rules/save fixtures; M04-T04 integrates listings/reports; M05-T06 integrates runtime wear, servicing, failure and the separate workshop activity. These additions do not require deformation physics or a random parts-failure simulator.

### Identity and inspection

1. Verify the original healthy starter offers remain at 4,000/5,000 credits. The four slice/ten V1 vehicle offers include those two guaranteed healthy starters; they are not additional inventory. Enforce one owned car per model across all offers.
2. For each used offer compare its persistent real-vehicle ID, listing ID/revision, mileage, condition and disclosed records before/after reopen, travel, reset and reload. The same listing must never roll a new condition or record set. Unknown history is explicitly unknown, never silently clean.
3. Purchase an inspection using the canonical quote; verify its report reflects the inspected revision and known/unknown fields. Inspect twice, cancel, submit a stale request or inject a failed save: apply the specified repeat-report rule and never double-charge or publish an uncommitted report.
4. Buy using the displayed revision. The exact vehicle, mileage, records, condition and saved report transfer with payment; consume the listing atomically. Reject stale revisions/duplicate models. After repair/reload retain old records and append the new service. Sell/rebuy through the dealer and verify the same real-vehicle ID, installed build, heat/condition and locked path remain. Quote buyback as `max(last_paid, resale + 500)` under canonical ownership rules. New purchase price becomes the acquisition basis and current-ownership upgrade investment starts at zero; lifetime history is retained. An acquisition receipt validates preinstalled stages without requiring free installs or fictitious past ownership payments.
5. On a reacquired car with a preinstalled stage 2 tire/brake/suspension/transmission build, quote only the stage-3 catalog total minus the stage-2 catalog total. Zero current-ownership upgrade investment must not cause a full-stage-total charge. Engine stage 3 remains its full authored next-step fee. Verify resale after that purchase adds only its new paid difference/engine step, never old installed investment a second time. Several buyback cycles cannot generate credits, reset condition or unlock a different engine path.

### Deterministic mechanics and service

- Test all six components at healthy, warning, derating, unroadworthy and failed boundaries using the canonical basis-point thresholds. No healthy car receives a random severe failure; warnings precede the authored severe-failure path. Keep mileage and service history separate from the amount known to the buyer.
- Replay a fixed driven-distance/load trace. Compare continuous execution with checkpoint/reload execution, including fractional wear residuals and thermal/overstress accumulators. They must agree within the model's documented quantization. Many short sessions cannot avoid wear by discarding each fraction. No RNG draw is refreshed to reroll a failure.
- Verify real road travel accrues mileage/wear, while fast travel/reset/teleport displacement does not. Offline time adds none; pause freezes mechanics and clocks. Reset/reload must not heal a worn component, erase an accident, clear a pending failure, or provide free thermal recovery inconsistent with the canonical cooling rules.
- Measure temporary engine/brake/tire/transmission/suspension effects as specified. Recompute from the healthy upgraded configuration plus current condition each time; do not repeatedly scale an already derated curve. Repairing restores the defined operating capability without secretly adding a stronger tune or resetting unrelated history.
- Compare inspection, repair and upgrade quotes for the same new/used model. Upgrade scaling uses catalog MSRP; discounted actual purchase price belongs to the purchase/resale ledger. Service fees use their own formula. Paid service appends its receipt/spending separately from upgrade investment. A paid upgrade replacement records its component/history change against that stage receipt without a second repair invoice or duplicated investment.
- Test exact repair affordability, cancel, double-click, stale quote, insufficient funds and save failure. Debit, repaired condition and append-only receipt commit together. Required pre-installation service is explicit. Engine/transmission installs require the canonical engine/cooling/transmission core thresholds (8,000 initially) and preserve those condition values; weight reduction heals nothing. All upgrades still enforce other applicable component/heat limits.
- Exercise the authored new-part mapping: a paid next stage of tires, brakes or suspension renews only that respective component to 10,000, while unrelated components must satisfy their threshold (4,500 initially) and heat stays below 8,000. Do not demand repairing the old part that is being replaced. Test both sides of every threshold, a worn replacement target with otherwise valid components, and an unhealthy unrelated component that must still block purchase. Duplicate-stage attempts, previews and reloads never renew anything. Inject save failure and verify payment, stage, renewal and history all roll back together. Later paid servicing may replace the part again while retaining its installed performance stage and historical stage fees.

### Breakdown, race state and zero-cash escape

Test roadworthiness before career entry separately from PI guidance: a mechanically healthy low-PI car follows the existing event rules, while an unroadworthy car is refused with the service reason. Force a warned hard component failure (condition ≤1,000) during countdown/driving/racing using fixtures. Before a valid finish this produces DNF/no finish reward, clears race state safely and offers free tow. Pause/quit/retry/reload cannot transform that run into a finish or wipe the failure/history. An earlier durably settled valid finish remains valid; later failure cannot revoke it.

Test temporary heat-only protection separately. At thermal load 9,500, cut positive drive while keeping the race clock running; the player can stop the engine, cool in live time and resume, or abandon. Persist the protection latch through reload and release it only below 7,500. Engine-off cooling works in the unpaused world, normal garage and roadside UI. Explicit Escape/Pause and offline time freeze cooling; reset and tow provide no cooling themselves. Heat alone never forces a paid repair, paid inspection or loaner job. Verify this free cooling path from zero credits without altering component condition, mileage or history.

Use an isolated save with **zero credits and every owned car unroadworthy**:

1. Load successfully into the recoverable workshop flow. Free tow remains available; no entry fee or roadworthy owned car is needed to access emergency recovery.
2. Start the separate workshop loaner time trial. Its temporary vehicle is not added to owned inventory, cannot be sold/upgraded/used for career entry, and does not replace the saved career active-car identity. It produces no owned-car mileage, wear or resale value.
3. Finish validly and receive exactly 300 credits, no reputation and no career first-clear bonus. Reopen results, replay the same settlement, pause/quit/reload: that job run pays at most once. A genuine retry/new job gets a new run ID. An abandoned/invalid run pays nothing.
4. From zero, at most four valid jobs fund the canonical minimum fix (capped at 1,200 credits) on at least one owned car. Apply that quote and verify the car passes mechanical entry checks. A full restoration may cost more; it is not required to escape the softlock.
5. The loaner route never grants career reputation, unlock flags, a purchasable model, or permission to enter higher career races. Workshop-job income remains distinct from the sixteen career events and their event best-result/first-clear tables.

Repeat with the lowest-condition/multiple-car cases, a corrupted-primary recovery and a partially completed job. Failure to recover from zero cash, an inaccessible job, or a last-car rule that traps the player is a release blocker.

## Manual handling course

Use one saved test scene and fixed starting conditions:

1. Level straight: measure 0–100 km/h and speed after a fixed distance.
2. Braking lane: brake from 100 km/h at the same marker; measure stopping distance.
3. Constant-radius circle: compare stock and upgraded tires at stable input.
4. S-bend: compare light and heavy cars with keyboard and controller.
5. Low curb and road seam: check grounded stability and suspension recovery.
6. Ramp and slope: check landing, reverse, braking downhill and reset.
7. Handbrake turn: verify a recoverable slide and camera readability.

Repeat meaningful cases after physics, road-collision, mass or upgrade changes. Do not rerun every test for a text-only UI change.

The starter should be controllable by a novice, and tire/brake/power purchases should each change an appropriate measurement. Record a short capture and a table of results. Ask for user feedback when available; routine progress does not require an extra approval gate.

## Slice acceptance script

Start with a fresh test profile:

1. Start career, inspect the healthy starter choices, buy one at its original fixed price and check remaining cash/condition.
2. Leave the garage, follow navigation and complete `evt_dock_01`.
3. Verify cash, best result, first-clear flag and reputation against the reference data.
4. Reopen results or retry a settlement and verify there is no second payout.
5. Buy an affordable upgrade. Drive the same section and inspect the changed behavior. Select an allowed engine path at stage 1; compare its current/proposed torque and kW preview and record the exact paid step.
6. Complete all four Dockside slice events, including a missed gate and a recovery.
7. Inspect all four slice used offers; use isolated profiles to exercise conflicting same-model offers. Earn enough to buy the other slice models, switch among them, and preserve vehicle identities, builds, condition, mileage and records.
8. Sell an eligible extra car. Confirm the quote, cash and active-car fallback.
9. Exercise the slice's stage-2 sequence, an attempted skip/repayment, a path change, and a missing transmission prerequisite. Verify rejected transactions leave wallet/build/receipts unchanged. Save, close the process and reload; compare ownership, selected path, stages, investments, derived curves and PI.
10. Service a worn car and verify its appended receipt/history after reload. Separately buy a new tire/brake/suspension stage and verify only its mapped condition renews; engine/transmission/weight do not heal core wear. Copy the profile, truncate its primary save and verify recovery without affecting the original. Test hard component failure/DNF, resumable heat-only protection/free cooling, and the zero-cash/all-broken loaner-job escape above.
11. Pause, abort, lose, reconnect a controller and revisit the garage. Confirm that controls, camera and timers recover correctly.
12. Run the exported build outside the editor and repeat a race, used inspection/purchase, service, workshop-job settlement, sequential tune, dyno and measured acceleration/shift check. Confirm all package data is present.

Keep a checklist with `pass`, `fail` or `not_run`, not an unsupported blanket claim that the slice works.

## Campaign balance measurement

Use two reports: an experienced tester progressing efficiently and a new tester learning normally. Record time to first race/upgrade, sequential tune/support purchases, inspections/servicing, next car, reputation tiers and finale. Track travel, failures, repairs and menus separately. Verify warned wear creates understandable ownership choices rather than requiring recovery-job grinding during ordinary healthy play. Check that useful NA builds remain viable and boosted fees/prerequisites buy distinct character rather than automatic victory.

Validate the progression graph by simulation before playtesting it. The simulation proves numerical reachability under stated results; it does not prove AI difficulty, route quality or real campaign duration. Check both best-result and modest-result scenarios. Replays earn cash but should not be assumed to create unlimited reputation.

The 4–8 hour campaign target is provisional. Adjust payouts, pacing, route length or difficulty from observed runs. Keep the value of the next purchase understandable; avoid requiring dozens of identical low-tier replays for a mandatory upgrade.

## Performance measurement

Initial target: **smooth 60 fps at 1080p on a recorded reference Windows PC**, using a documented quality preset. This is a development target, not a hardware guarantee.

Capture repeatable free-roam and race routes with four cars, district transitions, the garage, the busiest street view and a long hill view. Record CPU/GPU frame times, 95th-percentile frame time, hitch count, memory and load duration where tools permit. A 60 fps target corresponds to roughly 16.7 ms per frame; averages alone can hide poor pacing.

Use observed bottlenecks to set model, shadow, texture and draw-call budgets. Profile before adding streaming or rewriting systems. Keep a lower-quality preset, cap expensive distant effects and avoid unnecessary per-frame allocations in vehicle and AI loops.

## Accessibility and input verification

- Complete all required menus using only keyboard, then controller.
- Confirm visible focus, correct back behavior and useful default selections.
- Check at 1280×720 and 1920×1080, including text scaling and the longest labels.
- Verify high-contrast checkpoint cues remain understandable without color alone.
- Test camera shake off, motion blur off, music muted and speed-unit changes.
- Verify remapping and prompt updates, controller dead zones, reconnect and alternate input device switching.
- Confirm race cues remain visible when sound is unavailable.

## Release-blocking defects

Crashes, lost valid saves, partial purchases/services, duplicate payouts/charges, skipped paid stages, an unenforced engine-path lock, erased vehicle history, free condition restoration, negative currency, lost career ownership, campaign/recovery softlocks, unfinishable events, persistent broken controls and missing export assets block release. Having only unroadworthy owned cars is valid when the free tow/workshop/minimum-fix path remains available. A temporary loaner is not ownership. Torque/induction/transmission upgrades existing only in displays/sound fail scope. Zero-car onboarding is valid only before the first purchase and loads into the starter dealer.

Record lower-impact defects with severity, reproduction, affected build and workaround. Closing a defect requires evidence of the corrected path. Do not hide unresolved issues by resetting the test profile.

## Mileage and service-distance checks

Verify 1,609.344 m displays as 1 mile or 1.609344 km while stored distance, wear and physics remain identical. Resetting the trip changes neither lifetime odometer nor fractional wear, service markers, history or current condition. Repairs/new components preserve lifetime mileage; transfer and reacquisition retain the same vehicle's odometer. A parked owned car gains no mileage while the loaner is driven.

Use per-component history fixtures: missing engine service remains null/Unknown; an inspection at today's odometer does not become an engine service; a new tire/brake/suspension package updates only its own service marker; minimum repair remains labeled as minimum repair. Multiple receipts at the same odometer are valid when sequence increases. Decreasing readings or a receipt beyond the current odometer produce a discrepancy/validation failure rather than a fabricated clean record.

Switch units, reset trips and reopen reports around check-interval boundaries. Check reminders must not charge money, alter power/wear/resale or block an otherwise roadworthy car. Acquiring a car can start a new inspection schedule while preserving its actual unknown/old service history. Include a high-mileage healthy fixture and a lower-mileage worn fixture: condition determines live performance, with no extra raw-odometer power penalty.

## Release package

The local release candidate contains the Windows executable/runtime files, version/build identifier, controls, vehicle-inspection/service/recovery instructions, save recovery, credits, asset notices and known issues. Keep source/build instructions separately. Record a checksum of the export.

Run it from a clean directory outside the source tree without the editor. Test a fresh career, an existing supported career, settings, a race, a purchase and a reload. Build export filters must include runtime JSON or resources produced by the content import pipeline.

Producing a local build does not publish it. Uploading to a store, making a public page or paying for distribution is a separate action when the user requests it.
