# Vision and scope

## The pitch

**Redline: From Rust to Riches** is a singleplayer open-world racing career game. You start with a cheap car and build financial success through races, smart purchases and useful upgrades. You become attached to your cars because they keep their builds, appearance, results and resale value.

The setting is Harbor Vale, a fictional coastal city with an industrial waterfront, an urban center and hills above the bay. Racing crews provide a light career story. There is no requirement to tell the story through cinematic cutscenes or walking missions.

The title, names, visual style, specific cars and balance values are proposed defaults. User-confirmed requirements are listed in the README and decision log.

## Player experience

The first satisfying moment should be driving the starter car confidently. The next should be installing a part and recognizing what it changed. The long-term pull is deciding what to build, what to keep and what to buy next.

Five design pillars:

1. **Driving is enjoyable between races.** Roads have recognizable landmarks, elevation, different corner types and places to test acceleration and grip.
2. **Cars have identities.** Weight, drivetrain, wheelbase, engine delivery and grip create different driving experiences. A more expensive car need not win every route.
3. **Money creates choices.** Buying a part delays the next car, while a better build helps earn more. Prices and sale quotes are visible before committing.
4. **Progress is understandable.** The map states event eligibility, a dealership states unlock requirements, and a result screen explains every reward.
5. **A smaller finished game comes first.** A strong compact world and verified career loop are the release target.

## Handling target

The user's comparison is GTA-like semi-realism. Interpret it as believable speed and weight, forgiving steering, strong car-to-car differences and entertaining slides. Braking distance matters; tires have a grip limit; the car leans and transfers weight. A first-time player can drive with keyboard or controller without understanding racing engineering.

Use automatic shifting initially, analog controller triggers, smoothed keyboard steering and a readable chase camera. Allow a handbrake to help rotate the car. Mild optional stability assistance and countersteering assistance can improve accessibility. Any assistance settings are applied consistently and do not alter reward eligibility in this offline game.

The user also wants credible upgrades. Give every engine an RPM/torque curve and let induction choice change its delivery. A turbo's boost response differs from a positive-displacement blower or a naturally aspirated engine. Every tune stage is a separate paid step with defined supporting hardware. Engine behavior, gearing, wheel force and the garage's power graph use the same data; `10_POWERTRAIN_AND_TUNING.md` defines that model.

Include the component-condition, wear, repair-history and breakdown systems requested by the user. `11_USED_CARS_MAINTENANCE_AND_BREAKDOWNS.md` defines readable warnings, service quotes, inspections and recovery. Full tire-temperature, fuel-mixture and clutch simulation remain outside the driving target.

Prototype acceptance targets are specified in the technical blueprint and testing plan. Handling needs actual playtests; invented performance numbers in a design table cannot establish driving quality.

## Core loops

### Minute to minute

Drive toward a marker, explore a side road, start a race, read the next checkpoint, brake, turn, pass opponents, finish and collect a clearly explained reward.

### Session to session

Choose an event suitable for the current car. Earn money and improve your best result. Return to the garage. Compare an upgrade with saving for another car. Save, reload and find that your money and build are intact.

### Career

Rookie events introduce the waterfront. Club events reward better builds and route knowledge. Pro events favor cars with specialized strengths. Elite events culminate in a final circuit challenge in Ridgeway. Reputation comes from improving the best result in distinct events, so progression encourages trying the authored content.

### Collection

Keep a beloved starter, acquire another body style, create a grip build or a power build, repaint it, inspect its value and switch cars at the garage. V1's garage capacity accommodates all eight unique cars. Do not introduce garage-slot purchases in this version.

## What “wealth” means

Display three separate values:

- **Cash:** spendable currency.
- **Garage value:** current condition/history-adjusted sell quotes, with current-ownership upgrade investment counted once. Historical repair spending is not extra asset value.
- **Net worth:** cash plus garage value.

Buying a car trades liquid cash for a depreciating asset. Buying parts improves driving performance and adds some resale value; it need not instantly increase net worth. Racing provides the main source of financial growth. Explain that distinction in the first purchase preview rather than displaying an unexplained falling wealth number.

Reselling an unchanged car or repeatedly fitting the same part cannot generate cash. A cheaper worn car can be a useful purchase when inspection and repairs make its all-in cost attractive. V1's paid-price-based resale keeps immediate buy/repair/sell arbitrage unprofitable. A fuller speculative trading business remains a separate expansion.

## Three scope levels

| Area | Handling prototype | Playable slice | Complete V1 |
|---|---|---|---|
| World | Test pad, straight, curves, ramp | Dockside greybox | Three connected districts |
| Cars | One tunable placeholder | Three data-driven cars | Eight distinct fictional cars |
| Events | One debug route | First four Dockside events | Sixteen authored events |
| Career | None | Early rewards, rep and first rival | Four tiers, six rivals, final victory |
| Garage | Debug stats | Buy, install, switch, sell | Same mechanics with complete inventory |
| Persistence | Settings as needed | Full slice progression | Versioned complete progression |
| Presentation | Debug HUD | Clear functional UI and basic sound | Consistent art, sound and accessible UI |

Do not label a handling demo as the slice. The slice requires the entire earn/spend/save/reload loop. Do not label the slice as V1; V1 requires the full authored world and career, plus release verification.

## Release boundary

V1 contains:

- One offline career save plus a recoverable backup and independent settings.
- A connected compact world, with race and free-roam states in that same environment.
- Sprint, circuit and time-trial event rules. Rival races use those rules rather than a fourth bespoke system.
- Eight unique car models, one owned instance of each model at most, with persistent upgrades and paint.
- Six upgrade categories with sequential paid stages, compatible engine build paths, supporting hardware and torque/power previews, as defined by the economy and powertrain data.
- Persistent used-car listings with mileage, known/unknown history, inspection reports and retained repair/build records.
- Six mechanical condition components, deterministic wear, warning signs, bounded performance effects, breakdowns and paid service.
- Free towing and one repeatable emergency workshop loaner activity to earn minimum repair money when every owned car needs service.
- Six rivals represented through portraits, short text and events.
- A complete career with a final win condition and continued free play.
- Keyboard and controller support, remapping, pause, volume controls, camera comfort controls and legible UI.
- A Windows build that runs without the editor and preserves saves through ordinary restarts.

V1 omits ambient moving traffic, police chases, multiplayer, on-foot gameplay, fuel costs, destructible body deformation, full engine swaps, dynamic weather, dynamic day/night, photo mode, in-game radio stations and user-generated content. Component wear and paid mechanical repairs are included; a detailed crash-destruction simulator is not required.

Choose a fixed late-afternoon lighting setup for the first polished world. Parked cars and distant visual motion can imply a city without adding traffic AI. The sparse population is a deliberate scope decision to revisit after the racing loop works.

## Story and emotional arc

You inherit an old workshop lease, not a free car collection. A local mechanic introduces the garage. Your starter is visibly humble, but it is yours. Rivals begin skeptical and gradually treat you as a serious competitor.

Each rival introduces a driving lesson and a region of the racing scene: clean braking, line choice, managing power, consistency, elevation and combining skills. Short messages before and after key events are enough. The final event earns recognition and a workshop celebration; the user's collection remains available afterward.

Do not assume the player must beat the campaign to enjoy the game. Clear earnings, recognizable upgrades and accessible free driving should be satisfying early.

## Success criteria

The slice succeeds when a new tester can, without developer instructions:

1. Start a career and buy the starter.
2. Drive to and finish an event.
3. Explain how cash and reputation differ.
4. Buy a useful upgrade and feel a measurable change.
5. Save, close, reopen and continue with the same car, cash and progress.
6. Recover from a crash or missed checkpoint without losing the career.
7. Inspect a worn listing, understand its repair needs, service an owned car and recover from a zero-cash breakdown through the workshop activity.

V1 succeeds when that loop remains coherent through all sixteen events, every unlock is reachable, the last rival can be beaten with earnable cars and upgrades, and the exported build passes the release checklist.

Campaign duration is a **tuning target of roughly 4–8 hours** for a typical first completion, including travel, purchases and retries. This is a hypothesis to measure in playtests, not a promise. Completionists can continue collecting and replaying beyond that.

## Changing the plan

Tune numerical values as playtests justify. Preserve stable car, upgrade and event IDs so saves and tests remain understandable. Record substantial changes to engine, scope, save formats or progression in `project/DECISIONS.md`.

If a feature threatens the slice, reduce presentation complexity or move optional content to the backlog. Preserve the core race → earn → upgrade/buy → save loop.
