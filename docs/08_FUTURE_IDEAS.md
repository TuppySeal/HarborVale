# Future ideas

This is an idea bank, not an implementation queue. V1 is defined in the scope document. Move an idea into the roadmap only after recording its gameplay purpose, dependencies, acceptance checks and effect on existing saves.

## Strong candidates after V1

| Idea | Why it could help | What it needs |
|---|---|---|
| Profitable project-car trading | Extend V1's inspections, condition and repair records into a separate business | A new market profit/availability model; V1 already supports worn purchases, service and history-preserving buybacks |
| Ambient traffic | More life and driving variety between races | Lane graph, junction rules, spawn/despawn, traffic-safe recovery, performance headroom |
| Drift events | Gives rear-wheel-drive builds another purpose | Scoring based on controlled speed/angle/progress, combo rules, exploit checks |
| Drag events | Makes launch and power builds meaningful | Launch rules, dedicated routes, gearing balance, separate AI behavior |
| Body kits and wheel styles | More attachment to owned cars | Compatible visual variants, preview workflow, clear distinction from performance parts |
| Car delivery jobs | Variety and small reliable earnings | Mission states, loaned-car handling, payout and abandonment rules |
| Extra garages | New collection milestones | Property economy, service locations, save migration, clear convenience benefits |
| Ghost replays | Personal improvement without more AI | Input/transform recording, versioning, storage limits, playback verification |
| Test drives | Lets the player feel a car before buying | Loaned-instance state, fixed test route, clean return and purchase rules |
| More world discoveries | Makes exploration rewarding | Authored destinations, content budget, rewards that preserve progression balance |

## Larger expansions

**Police pursuits:** add pursuit awareness, escalating states, escape rules, AI road behavior and clear consequences. Decide how pursuit vehicles coexist with events and whether money can be lost. Treat this as a major subsystem.

**Multiplayer:** requires a new networking architecture, authority model, synchronization, matchmaking/hosting, collision policy, save ownership and cheating decisions. It is not a small toggle on an offline game. Start with a separately planned prototype if requested.

**Dynamic rain and night:** need tested visibility, headlights, surface grip changes, compatible tire and AI behavior, lighting performance and save state. A fixed presentation works for the initial career.

**Engine swaps and player-edited calibration:** extend V1's authored torque/gearing model with new engine fitment, mounting/drivetrain compatibility, detailed ECU controls and event rebalance. V1 already has compatible induction paths and paid tune stages; freeform boost/ignition/fuel sliders and engine replacements are separate expansions.

**On-foot garage/showroom:** requires a character controller, camera, navigation and interaction system. A well-designed garage interface already serves the ownership loop.

## Small ideas to test cheaply

- A wall of career trophies in the garage presentation scene.
- Add trophy photos and career highlights to V1's existing per-car service, ownership and build history.
- Rival messages that acknowledge the car/build used to beat them.
- A simple photo frame at each lookout after a basic photo mode exists.
- A weekly local challenge generated from existing routes, with no online services required.
- Manual shifting as an optional driving feature once automatic gears are reliable.
- A “favorite car” filter and build nickname without expanding the economy.

## Choosing what to add

Favor an addition that deepens driving, ownership or meaningful spending. Estimate its cost in code, content, UI, testing and save migration. Prototype it in isolation, then decide whether it improves the existing game enough to justify those costs.

If the main request is simply “more game,” additional well-tested routes and distinct cars may provide more value than several unfinished new systems.
