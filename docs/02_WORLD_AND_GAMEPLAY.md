# World and gameplay blueprint

Status: proposed design; no game implementation or playtesting is implied. Economy values, car definitions, performance classes, rewards, and reputation gates belong to [03_ECONOMY_AND_CARS.md](03_ECONOMY_AND_CARS.md). This document owns road layout intent, event formats, player interactions, and race rules.

Confirmed direction: Windows PC, with semi-realistic driving comparable in accessibility and physical weight to GTA driving, rather than a hardcore driving simulator. That is a feel reference only; this game has its own world, characters, vehicles, and art.

## 1. The player experience

Drive out of a modest garage, discover a local racing scene, choose an event, earn cash, and return with a specific improvement in mind. A better tire package should be felt in the same corner that caused trouble earlier. A newly purchased car should suggest different routes and builds. The collection becomes a record of the player's decisions.

The open world connects these decisions: races, dealers, the garage, landmarks, and rivals all exist in the same place. Keep travel short enough that a player who wants another race can begin one promptly. All three districts are physically explorable from the start; event and inventory gates provide progression without invisible district walls.

The signature feature is **a garage of personal project cars**. Every vehicle has a persistent identity, paint, installed build, mileage, condition and service/repair history. Switching cars preserves them. A starter hatchback can remain a favorite after a faster car is purchased. Worn bargains invite inspection and repair before tuning. No transferable parts inventory or arbitrary healthy-car failure roll is required.

## 2. Scope and map

The vertical slice contains Dockside, three cars, the first four Dockside events, one garage, one dealer interface, and saving. Build it with simple road meshes and placeholder buildings before creating the larger region.

The complete first release contains three connected districts, eight fictional car models, sixteen authored career events, six named rivals, six optional lookout discoveries, ten initial used-car offers and one separate workshop recovery activity. It includes wear, repair records, inspections and mechanical breakdowns. There is no moving civilian traffic, police pursuit, fuel-consumption system, deformable crash damage, pedestrian simulation, dynamic weather, or multiplayer in this release. Parked scenery cars can suggest a populated place. Use one readable daylight setup initially.

Map dimensions are layout targets to validate through driving, not measured performance claims:

| Area | Character and purpose | Initial layout target |
| --- | --- | --- |
| Dockside | Warehouses, cranes, container yards, broad bends; onboarding and forgiving racing | About 600 × 450 m, roughly 2 km of connected road |
| Midtown | Shopfronts, plazas, avenues, tighter blocks; braking and route reading | About 650 × 550 m, roughly 2.5 km of road |
| Ridgeway | Hills, exposed curves, pine silhouettes, scenic overlooks; carrying momentum | About 700 × 600 m, roughly 2.5 km of road |
| Whole region | Short connectors join the districts into a loop | Approximately 1.8 × 1.4 km footprint and 7–9 km of usable road |

These footprints overlap at district boundaries; do not add them as disjoint tiles. Use local roads around 8–12 m wide, main avenues 14–18 m, and hill roads 7–9 m. Provide visible run-off at early corners. Curbs and road seams must not catch wheels. Avoid mandatory narrow alleys, jumps, or elevated junctions in the first routes. A simple skyline, distinctive ground colors, and one landmark per district make navigation readable before decorative detail is added.

Design each district around a loop with two or three cross connections. Reuse that network for several routes, but change turn sequences, direction, starts, and finishes. Never block the only connection to a service during a race. Event barriers and arrows exist only in the active race instance/state and are removed on exit.

### Suggested road topology

Use three district loops joined into a larger regional loop. Dockside's quay road joins Midtown's main boulevard; Midtown's uphill approach reaches Ridgeway; a longer coastal bypass joins Ridgeway back to Dockside. Include at least one alternate connection inside each district so free roam feels like a network rather than a corridor.

For the first full-map greybox, use X east/west and Z north/south, with north toward -Z. Suggested district centers in meters: Dockside `(-400, +350)`, Midtown `(+450, 0)`, Ridgeway `(-200, -400)`. Keep the sea at the western edge; use modest Dockside elevation, a small rise into Midtown and hills up to roughly 120 m in Ridgeway. These are adjustable layout guides, not save-file spawn coordinates. Author and validate the actual road heights, joins and recovery transforms in scene data.

## 3. Driving feel

Aim for accessible, semi-realistic driving with perceptible weight and grip: stable ordinary driving, meaningful braking distances, recoverable slides, suspension movement, and a clear difference between light and heavy cars. Steering response must remain useful on a keyboard; holding a key should smoothly approach the steering target instead of snapping the wheels. Reduce maximum steering angle with speed. Braking, weight transfer, and tire grip should matter more than memorizing button combinations.

Use automatic transmission, forgiving low-speed traction, reverse after braking to a near stop, and an optional handbrake for tighter rotation. Drifting is a driving technique, not a separately scored event format. There is no nitrous in the first release. Give the player three clear feedback signals: tire sound for lost grip, camera/engine response for acceleration, and a brake-light response to deceleration.

Required handling checks before content expansion:

- A stock starter completes every slice route without upgrades.
- A keyboard player can take the onboarding bends without repeated full spins.
- Braking before a corner produces a reliable shorter turn than entering at full throttle.
- Tire, brake, and power upgrades each produce a visible difference on an appropriate test section.
- A crash or rollover always has a reliable recovery path.

Do not call the handling finished because the car moves. Revisit it after real laps with keyboard and controller. V1 includes believable authored torque curves, automatic gearing and distinct boost response from `10_POWERTRAIN_AND_TUNING.md`. Full combustion simulation, manual clutch, player-edited ignition/fuel maps and tire-temperature systems remain outside scope.

## 4. Events and route catalog

Only three event implementations are required:

- **Sprint:** ordered checkpoints, one finish; first valid finisher wins.
- **Circuit:** ordered checkpoints across a specified lap count; first valid finisher wins. The start line also closes the lap, and the final checkpoint must be accepted before that crossing counts.
- **Time trial:** one player, ordered checkpoints, fixed medal times. Use the highest achieved medal for result scoring; medal targets are authored per route and must be calibrated after driving is stable.

Ordinary head-to-head events have the player and three AI drivers. A featured rival sprint has the player and one AI rival. Time trials have no AI. Target approximately 60–180 seconds for a clean attempt; shorten or lengthen routes after playtesting. The lengths below are initial route-layout intentions, not promises about finish times.

| Event ID | Name | Format | Route intention | Featured character |
| --- | --- | --- | --- | --- |
| `evt_dock_01` | First Lap | Time trial | 1.0 km, broad harbor loop; no narrow sections | — |
| `evt_dock_02` | Freight Run | Sprint | 1.3 km, warehouse cross connections and a long braking zone | — |
| `evt_dock_03` | Quay Circuit | Circuit, 2 laps | 0.9 km per lap, two straights and four readable turns | — |
| `evt_dock_04` | Slipstream Challenge | Rival sprint | 1.5 km, mixed quay and warehouse roads | Mika |
| `evt_dock_05` | Drydock Precision | Time trial | 1.4 km, tighter turns that reward tires and braking | — |
| `evt_dock_06` | Dockside Cup | Circuit, 3 laps | 1.0 km per lap, district skills combined | Rook |
| `evt_mid_01` | Boulevard Circuit | Circuit, 2 laps | 1.1 km per lap, sweeping avenues and one tight square | — |
| `evt_mid_02` | Cross City | Sprint | 1.8 km, Dockside connector to the Midtown plaza | — |
| `evt_mid_03` | Green Lights | Time trial | 1.5 km, urban braking rhythm; lights are scenery | — |
| `evt_mid_04` | Block by Block | Rival sprint | 1.7 km, alternating fast avenues and side streets | Sera |
| `evt_mid_05` | Midtown Cup | Circuit, 3 laps | 1.1 km per lap, boulevard and plaza combination | Vale |
| `evt_ridge_01` | Lookout Run | Time trial | 1.5 km, introductory climb with generous run-off | — |
| `evt_ridge_02` | Above the City | Sprint | 2.0 km, Midtown connector to the upper lookout | — |
| `evt_ridge_03` | Plateau Circuit | Circuit, 2 laps | 1.3 km per lap, mixed uphill and downhill corners | — |
| `evt_ridge_04` | Mountain Measure | Rival sprint | 2.2 km, sustained climbing and descending | Tomas |
| `evt_ridge_05` | Summit Invitational | Circuit, 3 laps | 1.3 km per lap, final test of the hill network | Ash |

`evt_dock_01` through `evt_dock_04` comprise the slice. Route assets should have their own stable IDs, such as `route_dock_01`, rather than looking up assets through display names. Rewards, entry gates, field difficulty, and class restrictions are taken from the economy document/data. Do not add hidden prerequisite races in route logic.

## 5. Rivals and campaign

Tell a small story through short garage messages, event introductions, and result reactions. Every named rival has a recognizable paint scheme and driving tendency. Dialogue is text first; voiced cutscenes are unnecessary.

| Rival | Personality | On-track identity | Featured event |
| --- | --- | --- | --- |
| Mika | Friendly mechanic who welcomes the player | Early braking, tidy corner exits | `evt_dock_04` |
| Rook | Dockside organizer, proud of well-built cheap cars | Consistent laps, few mistakes | `evt_dock_06` |
| Sera | Courier who knows the city blocks | Quick direction changes | `evt_mid_04` |
| Vale | Car-club host who prizes clean builds | Strong straight-line pace | `evt_mid_05` |
| Tomas | Patient hill-road regular | Smooth momentum through bends | `evt_ridge_04` |
| Ash | Regional champion with a carefully developed car | Balanced, sustained pace | `evt_ridge_05` |

These tendencies are parameter variations of the same AI controller. They do not imply six bespoke AI implementations. Qualifying through reputation and winning the Summit Invitational completes the campaign. Earlier cups and rival wins remain recorded milestones, but do not become additional unlisted entry requirements. After the ending, keep the save, money, collection, exploration, event replays, and personal bests available. No prestige reset is required.

## 6. First thirty minutes

Timing is approximate and depends on the player:

1. **Arrival:** introduce the garage and starter purchase. Show price, basic handling character, and remaining cash before confirmation. Keep the introduction skippable.
2. **First drive:** guide the player to a nearby event using a route line and a large world marker. Teach steering, braking, map, and recovery with brief prompts that disappear after use.
3. **First result:** show finish time/position, awarded money, reputation gained, and the next practical action. A completed low-placement attempt still moves progression forward under the economy rules.
4. **First upgrade:** suggest comparing tires, brakes, and power without forcing a particular purchase. Show the exact cost, changed stats, and remaining balance.
5. **Choice:** let the player replay the time trial, enter another event, inspect a different car, or freely explore. The tutorial ends once the loop is understood, not once every menu is visited.

The introduction must work with both eligible starter choices and with the player leaving the guided route. Avoid permanently missable rewards or mandatory timed tutorials.

## 7. Free roam and services

The region has one owned garage/workshop and a dealer interface accessed at the garage or a clearly marked forecourt. They can share underlying menus. Dealer offers show the individual vehicle's history and optional inspection; workshop service shows faults, exact repair quotes and receipts. The starter selection guarantees a healthy, verified car at either original starter price. Six lookout markers, two per district, record a discovery badge and show a short location description when the stationary player interacts. They provide optional destinations without introducing a second currency, repeated cash farming, or a photography subsystem.

Show the main road network immediately. Reveal discovered lookout markers and available events; locked events may be visible with their exact unlock condition. The player can set one waypoint from the map. Route navigation should use the road graph, not a straight line through buildings. Fast travel to the owned garage becomes available after visiting it; it is free and limited to free roam while stationary. Starting an event is always a deliberate interaction, never an accidental drive-through trigger.

## 8. Race validity, results, and recovery

Use ordered checkpoint gates spanning the intended road and reasonable shoulders. A gate counts only when the car crosses its plane in the allowed direction and it is the next expected gate. Detect the movement segment between physics samples so high speed cannot skip detection. Crossing a later gate, circling backwards, or revisiting the finish cannot advance progress. Allow reasonable corner cutting if all ordered gates are satisfied; place extra gates where a shortcut would erase the intended route.

Track rank from accepted lap/checkpoint progress, then distance along the current route segment. Resolve finish order using recorded valid finish timestamps. Never infer a victory from proximity to the finish. One run has one results record and at most one settled reward transaction. Pause, retry, reload, and leaving the results screen must not duplicate that reward.

Missing a gate shows a clear marker back to the next required gate. Holding recovery resets to a validated safe spawn: the most recent accepted checkpoint during races, or a recently sampled grounded road position during free roam. Preserve the outgoing travel direction, clear velocity, and avoid overlapping another car. Race recovery includes a tunable three-second stationary countdown while the race clock and other racers continue. This creates the time cost during the race; do not add another postfinish time penalty. Free-roam recovery is immediate and free. Falling outside world bounds invokes the same recovery. Recovery never moves a racer past an unaccepted gate.

Race entry and retry have no fee. Aborting a race gives no race reward and returns to free roam. Cash and purchases from before the race remain intact. AI obeys ordered route progress too; stuck recovery must return it to its last accepted checkpoint and use the same stationary countdown. Do not teleport opponents ahead, award them unearned checkpoints, or silently alter their physical grip to force a close finish. Difficulty changes their driving targets and mistakes.

### Mechanical failure and the workshop

Race entry additionally checks the selected car's mechanical readiness. Warnings, degraded response and limp-home behavior come from component condition and the simple heat model. A hard breakdown during a race ends that attempt as a DNF and cannot grant a finish payout. A previously committed valid result remains settled. A roadside prompt offers a free tow to the workshop; an ordinary recovery reset never repairs components.

The workshop offers an itemized minimum-roadworthy repair and full component service. An unavailable repair budget does not erase the car. When no owned car is roadworthy and cash is below the minimum quote, a short repeatable loaner time trial pays 300 credits per valid completion without reputation or first-clear rewards. It reuses the introductory route and is separate from the sixteen career events. A full minimum quote is at most 1,200 credits, so zero cash needs at most four completed jobs. The loaner cannot be sold, upgraded or carried into career events.

The precise thresholds, quotes, history schema, condition modifiers and recovery contract are in `11_USED_CARS_MAINTENANCE_AND_BREAKDOWNS.md`. Potential PI describes the healthy installed build; current mechanical readiness and output are displayed separately.

## 9. Menus, controls, and accessibility

Default bindings below are proposed starting controls; expose remapping and show prompts for the last input device used. Xbox-style labels are followed by PlayStation equivalents where useful.

| Action | Keyboard | Controller |
| --- | --- | --- |
| Throttle | W / Up | RT / R2 |
| Brake, then reverse near rest | S / Down | LT / L2 |
| Steer | A/D or Left/Right | Left stick |
| Handbrake | Space | A / Cross |
| Interact / open service | E | X / Square |
| Recover | Hold R | Hold B / Circle while driving |
| Cycle near/far chase camera | C | Y / Triangle |
| Look around | Optional mouse look | Right stick, auto-recenter |
| Map | M | View / Touchpad |
| Pause / back | Escape | Menu / Options; B / Circle backs out of menus |
| Garage tab navigation | Q/E while in menus | LB/RB / L1/R1 |

Menu focus consumes its inputs so interacting or changing a tab cannot also throttle or recover a parked car. A recovery hold is accepted only in driving/race states. Keep keyboard throttle/brake smoothing separate from the steering smoothing settings.

The HUD needs speed, automatic gear, minimap/next turn, next checkpoint marker, and context prompts. During a race add position, lap/checkpoint progress, timer, and time-trial target; show money and reputation changes on results. Use a chase camera with modest smoothing, speed-based look-ahead, and collision handling. Offer near/far chase distances, camera shake off, and motion blur off by default. Cockpit interiors are outside scope.

Garage tabs: **Cars**, **Upgrades**, **Dealer**, and **Progress**. Always identify the selected car and current cash. Upgrade and purchase confirmations show the exact transaction and resulting balance. The engine tab compares current and proposed torque/power curves over RPM, explains boost response and shows the next paid stage plus any unmet hardware requirement. Stage skipping is unavailable. Engine path selection clearly explains the V1 commitment before the first stage is bought. Car comparison shows price, performance class, peak power/torque, mass, drivetrain, and a short handling description. Locked items state their unlock requirement. A sale preview shows the exact return and identifies the affected car. V1 paint uses a small free palette; applying a color saves it and has no effect on resale or performance. Track per-car mileage from actual driven distance, excluding reset/fast-travel jumps.

Add workshop/history access from the garage, warning lamps with text labels and a visible roadside/tow action. Used-car comparison must distinguish verified repairs, missing records and measured condition. Show the total purchase plus quoted repair cost before commitment when inspection makes it known. State what a new paid replacement changes; tires, brakes and suspension renew their own condition while engine calibration cannot repair a worn engine core.

Use readable text, high-contrast markers, scalable HUD/text, independent music/effects volumes, remappable controls, and visual indications alongside sound cues. Do not encode checkpoint state, warning severity or rarity by color alone. Every menu must support keyboard and controller focus with a visible selected element. Pause freezes offline gameplay, race clocks, wear and heat. Include a reset-controls action and a confirmation before deleting save data.

## 10. Completion checks

- A new player can buy a starter, leave the garage, finish any slice event, install an upgrade, change cars, save, reload, and continue with matching ownership and balances.
- Every listed event has a driveable route, valid start/finish, reachable gates, a useful recovery spawn per gate, appropriate event signage, and a tested return to free roam.
- The map supports travel among all three districts without a scene dead end or progression wall.
- Normal travel and races do not strand the player after crashes, rollovers, aborts, or missed checkpoints.
- All six rival introductions and the ending trigger once at the proper result; replays remain functional.
- Decorative work begins after route readability, handling, race validity, and recovery pass their checks. Asset/detail budgets are established by profiling the target hardware, rather than by assuming these map dimensions guarantee performance.
