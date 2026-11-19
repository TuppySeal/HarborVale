# Confirmed choices — November 18, 2026

**Later instruction:** the user authorized a Unity city greybox with boxes and labels. The user subsequently authorized the scaled city, starter model and stock starter-car driving (M11-T01 through M11-T03). The user then authorized selecting the next expansion: M12-T01 adds the first time trial and saved prototype race earnings. Later user requests also authorized paid parts (M12-T02–T03), a separate Apex sports test (M12-T04), and a unified launch/live timing flow (M12-T05). These implementation requests supersede the planning-only hold below; unimplemented career features still require deliberate scoped work. See `game/README.md` and `project/STATUS.md`.

**Planning only. Do not build the game or begin engine setup.** These answers override conflicting earlier defaults. The 42-task roadmap and numeric data remain a baseline awaiting reconciliation with this expansion.

| Question | Confirmed choice |
|---|---|
| 1 Shops | Each has a small website inside the game. |
| 2 Repair options | Temporary fixes, used parts, new parts and upgraded parts. |
| 3 Bad work | Used parts passed off as new and shoddy craftsmanship can cause problems. |
| 4 Warranties | Available except at the lowest-end shops. |
| 5 Diagnosis | Misdiagnoses and second opinions. |
| 6 Time | Delayed repairs with an extra payment for instant completion. |
| 7 DIY | Included; exact interaction depth still to design. |
| 8 Maintenance | Major systems rather than every small consumable. |
| 9 Driving habits | No extra cold-start/redline/launch/shifting-technique wear penalties. |
| 10 Fuel | Consumption, grades and tune-specific requirements. |
| 11 Damage | Visible crash damage. |
| 12 Test drives | Included before purchase. |
| 13 Sellers | Dishonesty, concealed faults and suspicious records with discoverable clues. |
| 14 Bargaining | Negotiate based on evidence and condition. |
| 15 Buying channels | Dealers, private sellers, auctions, salvage yards and abandoned projects. |
| 16 Conversions | Paid induction-path changes, engine swaps and drivetrain conversions. |
| 17 Tuning | Preset stages and adjustable settings. |
| 18 Activities | Choose activities that fit the ownership/racing loop. |
| 19 World | Traffic and police. Weather/day-night remain undecided. |
| 20 Stakes | Relaxed ownership with real consequences: pink slips, spinouts and wrecks. |

## Proposed design interpretation

Shop websites are offline native in-game phone/laptop pages, not hosted public websites. Show services, specialties, reputation, parts choices, quotes, completion time, express fees, warranty and job status. Visible versus discoverable reputation was not explicitly chosen. Propose a skilled cheap independent, an unreliable corner-cutting garage, a dependable general shop and a performance specialist. Price alone does not determine quality.

Separate skill, honesty, part provenance and part condition. An honestly sold used part is different from a used part falsely invoiced as new. Defects have specific causes: leaking installation, bad alignment or an unsuitable calibration. Persist a work order's quality outcome so reloads do not reroll it. A second opinion can reveal a wrong diagnosis; covered workmanship/parts faults are corrected under the stated warranty. Express payment changes time, not automatically quality.

Normal jobs take active in-game time; paid express completion is instant. No real-money purchase is involved. Propose simple workshop menus and short hands-on DIY tasks for major assemblies. Mechanic deliveries, recovery jobs, project restoration and car meets fit naturally; a full shop-management business is not yet committed.

Retain permanent mileage, service records and major-component wear. Disable the earlier driving-style abuse multipliers: no extra penalty for cold starts, redlining, launches or shifting technique. Normal distance, neglected faults, bad repairs and collisions still matter. Fuel needs stations, prices, grades and compatible tune rules; emergency loaners must include fuel.

Visible damage can start with authored dents/broken-panel states rather than full soft-body physics. Mechanical and cosmetic work have separate quotes. Test drives use the actual candidate's build/condition; liability must be disclosed. Every buying channel preserves physical vehicle identity and history.

Paid conversions now supersede permanent engine-path locks. Specify compatibility, labor, retained/traded hardware and which stages/calibrations must be repurchased. Adjustable settings remain inside the installed hardware's supported range. Avoid free conversion resets or erased history.

Traffic and police need road/lane behavior, pursuits, fines/impound and race integration. Propose controlled roads for sanctioned races and traffic/police exposure for illegal events; exact enforcement severity remains to design.

Pink-slip races are optional, deliberately confirmed bets on specific cars. Both stakes must genuinely exist. Transfers preserve condition/build/history and commit atomically. Losing a last car must be possible when knowingly staked, superseding the minimum-one-owned-car invariant. A free loaner/job path must recover from zero-car ownership without reversing the loss. Routine ownership stays affordable; severe losses come from understandable wrecks or explicit bets. Insurance, total-loss thresholds and rematches remain design questions.

## Before implementation

Reconcile older documents/data for shop quality and delayed jobs, fuel, visible damage, conversions, traffic/police, pink-slip transfers and zero-car recovery. Preserve earlier task IDs/statuses and add appropriate tasks. Baseline validation does not validate this expanded scope. Continue planning until the user requests building.


