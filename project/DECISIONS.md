# Requirements and decisions

Initial design date: 2026-11-17, America/Los_Angeles.

`confirmed` means the user supplied the requirement. `proposed default` means this package chooses it so implementation can start; the user can revise it. New user direction takes precedence over earlier defaults.

| ID | State | Decision | Reason / consequence |
|---|---|---|---|
| D001 | confirmed | Windows PC first | Target a native desktop build and keyboard/controller input. |
| D002 | confirmed | Open-world racing | Free-roam roads connect races, garage, dealership and exploration. |
| D003 | confirmed | GTA-like semi-realistic handling; not a hardcore sim | Prioritize believable weight, readable grip and forgiving controls. |
| D004 | confirmed | Parts upgrades, car purchases and gradual wealth progression | The core loop is race → earn → upgrade/buy → enter better events. |
| D005 | confirmed | Steps and durable AI handoffs | Maintain a task ledger, evidence, current status and exact next action in files. |
| D006 | proposed default | Godot 4 standard build + GDScript | Readable project files and a lightweight starting toolchain; pin actual version in M00. |
| D007 | proposed default | Custom RigidBody3D/raycast controller, handling gate first | Allows explicit weight/grip tuning; prove it before content expansion. |
| D008 | proposed default | Offline singleplayer V1 | Keep scope focused on the requested career loop. |
| D009 | proposed default | Three districts, eight cars, sixteen events, six rivals | A bounded full release; slice is one district, three cars and four events. |
| D010 | proposed default, expanded by D023–D025 | Six upgrade categories, three stages each | Engine, tires, suspension, brakes, weight and transmission. Slice exposes stages 1–2. Engine stages now use compatible torque-curve packages. |
| D011 | proposed default | Reputation gates events/inventory; districts remain physically open | Make exploration accessible while preserving career progression. |
| D012 | proposed default | PI is advice, not an event-entry cap | Encourage different builds; no hidden class restriction. |
| D013 | proposed default | Best-result reputation; replayable money; one first-clear bonus | Try different events and improve results while keeping reliable earnings. |
| D014 | superseded in part by D029–D032 | No race entry fees or fuel bills; paid maintenance now included | Zero-cash/all-broken recovery uses free tow, diagnosis and a workshop loaner job. |
| D015 | proposed default | One owned instance per car model; cannot sell the last car | Simple collection UI and preserved basic playability. |
| D016 | proposed default | Wealth = cash + current car resale values | Avoid counting upgrade investment twice; spending can reduce immediate net worth. |
| D017 | proposed default | Race recovery pauses the recovered car for three seconds while the shared race continues | Creates a fair time cost without conflicting postfinish penalties. |
| D018 | proposed default | Explicit persisted initial-purchase state | Zero cars is legal only before the first starter purchase; no free roam until purchase commits. |
| D019 | proposed default | Fixed daylight, stylized grounded art, no moving traffic in V1 | Validate driving and career before expensive city simulation. |
| D020 | proposed default | Campaign completion by winning evt_ridge_05; no prerequisite wins for event entry | Reputation gates are visible and sufficient; postgame preserves the collection. |
| D021 | proposed default | Campaign 4–8 hours and collection 6–10+ hours are tuning hypotheses | Measure actual pacing; do not pad the game to fulfill an invented duration. |
| D022 | proposed default | Price/stat source is design-data/, imported by a reproducible runtime pipeline | Avoid code/UI copies drifting from balance data. |
| D023 | confirmed | Upgrades should resemble real-life power delivery, including torque curves and turbo/supercharger behavior | Supersedes the original flat engine-power multipliers. The accessible driving target remains unchanged. |
| D024 | confirmed | Pay for each stage on the way to maximum performance | No skipping directly to a later stage. Store the installed stage and each payment. |
| D025 | proposed default, clarified by D035 | Compatible authored NA, turbo and positive-displacement supercharger paths; choose at stage 1 and lock on that physical vehicle | Reselling/reacquiring the same vehicle preserves the path and installed build. |
| D026 | proposed default, clarified by D035 | Engine stages have individually priced package steps; chassis stages retain cumulative schedules with mandatory sequential purchases | Next chassis fee is next-stage catalog total minus current installed-stage catalog total, independent of current-ownership investment. |
| D027 | proposed default | Net crank torque curves drive wheel force, displayed power and PI; turbo spool is transient per-vehicle state | Avoids cosmetic horsepower bars, duplicate boost gains, duplicate blower-loss deductions and shared boost state across cars. |
| D028 | proposed default, expanded by D029–D037 | Supporting engine hardware is included in the paid package; transmission and mechanical readiness are prerequisites | Higher output requires an appropriate build. The maintenance model now includes component wear, paid repair and bounded heat. |
| D029 | confirmed | Used-car repair/service history and the ability to break down | Supersedes earlier exclusions of mechanical failures and repair bills. |
| D030 | proposed default | Six condition components with deterministic driven-distance wear, warning bands and component failures | A healthy maintained car has no hidden random failure roll; pause/offline time adds no wear. |
| D031 | proposed default | Ten persistent initial offers, four in the slice, with two guaranteed healthy original-price starters | Inspection creates meaningful used-car choices while preserving the opening purchase budget. |
| D032 | proposed default | Free tow/owned diagnosis; paid quoted repairs; one emergency workshop loaner activity | Pays 300 per valid run; maximum minimum-roadworthy quote 1,200 gives a four-job bound from zero cash. Sixteen career events remain a separate count. |
| D033 | proposed default | Condition and worst-recorded accident discount resale; service/inspection costs are operating expenses | No erased history, duplicated investment or repair-flip arbitrage. Potential PI remains a healthy-build rating. |
| D034 | proposed default | Paid tire/brake/suspension stages renew their own replaced component; engine/transmission/weight do not overhaul the core | Makes new hardware behave plausibly; repeated transactions and resets cannot heal parts. |
| D035 | proposed default | Lifetime vehicle/build/history separate from ownership-period spending; same-vehicle dealer buybacks | Buyback is max(last paid price, resale + 500). Existing stages are included in reacquisition price, current upgrade investment restarts at zero, lifetime receipts persist. |
| D036 | proposed default | Roadworthy threshold 2,000; hard failure at 1,000; minimum repair restores unsafe components to 4,500 | Upgrades have stricter disclosed readiness; permanent all-broken ownership remains recoverable. |
| D037 | proposed default | Bounded persisted heat model with free engine-off cooling in live garage/roadside screens | Supports warnings/protection without full thermodynamics, offline timers or a paid heat-only escape. |
| D038 | proposed default | Heat-only protection keeps a race's clock running and allows cooling; hard component failure ends the attempt as DNF | Persist the heat-protection latch through reload until its release threshold is met. |

## How to add a decision

Append a new entry with its date, requirement/default status, reason, affected files and migration implications. For an actual change, reference the superseded decision. Do not silently change the engine, world size, reward rules or save contracts in an implementation task.

Routine tuning values can change together in the reference data, examples and tests. A balance update affecting existing saves must explicitly define how recorded paid prices, installed investments and stage quotes remain valid; preserve actual historical investment and prevent negative-price refunds or arbitrage.

## D041 — User-selected Unity city greybox

Confirmed: use Unity and start making a boxed/labeled city map. Overrides planning-only hold, proposed Godot and handling-first order for this bounded task. Use installed6000.0.18f1; broader game implementation is not part of this request. Native scene authoring is permitted while editor verification awaits license activation.

## D042 — Meter-scale city expansion and Finch model

Confirmed November 18: expand the Unity map and replace the oversized rectilinear street plan with a connected curved network. New road widths: 7.2 m two lanes, 14 m four lanes; sidewalks 3 m where urban. Use original fictional Aster Finch as the starter visual, 4.05 x 1.70 x 1.46 m, 2.45 m wheelbase; these dimensions are authored defaults, not licensed vehicle measurements. Native procedural meshes and prefab, no third-party model dependency. Canonical destination is Documents/Unity/HarborVale on installed Unity 6000.6.0f1; prior project remains a preserved earlier revision. No handling/career implementation implied.


## D043 — User-authorized stock Finch driving

Confirmed November 18: make the starter car driveable now. Use Unity 6000.6 WheelColliders/Rigidbody, original Finch stock torque samples/ratios and 1,050 kg mass. Runtime bootstrap equips the existing static scene model without overwriting the open map scene; preserve static source prefab and create a separate driveable prefab/test scene. Keyboard primary; gamepad remains unverified. No paid tune stages, economy, traffic or police implemented in this bounded step.


## D044 — First complete time trial as next expansion
User authorized choosing the next development step. Implement evt_dock_01 as a closed Dockside time trial on existing roads, with countdown, directional ordered checkpoints, marked route, results, first-clear/replay rewards, best time and saved race earnings. Race earnings are a separate prototype account starting at zero, not an invented purchase/wallet/owned-car history; use design-data reward amounts and result multipliers. No spending, starter acquisition, condition/history or upgrades are claimed. R returns behind the next unpassed checkpoint with a 3-second penalty; map inspection/focus loss do not stop a running race clock. Atomic validated saves commit earnings, receipt and best record together; duplicate run settlement pays once. Disconnected sessions are abandoned without rewards. Native game/profile paths must be documented for future migration. Claude is assigned isolated workshop art outside the Unity repository; do not edit its output folder.

