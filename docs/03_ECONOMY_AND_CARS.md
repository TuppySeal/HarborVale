# Economy, cars, upgrades, and progression

Status: proposed design baseline, not an implemented or playtested economy. All prices are fictional in-game credits. The canonical numeric reference is `design-data/economy.json`, `cars.json`, `upgrades.json`, `powertrains.json`, `maintenance.json`, `used_car_listings.json`, and `events.json`. If a value changes, update those files and this document in the same task. [10_POWERTRAIN_AND_TUNING.md](10_POWERTRAIN_AND_TUNING.md) owns engine configurations, torque curves, aspiration response, supporting hardware, and the drive-force model. [11_USED_CARS_MAINTENANCE_AND_BREAKDOWNS.md](11_USED_CARS_MAINTENANCE_AND_BREAKDOWNS.md) owns condition, records, inspection, wear, failure, and service definitions.

## 1. The player promise

Start with 8,000 credits, buy a modest car, and turn racing success into a collection that visibly grows. Every completed race pays something. Better driving increases the payout; improvements to the car make more demanding events practical. The important decision is whether to improve a car you enjoy, save for a different car, or keep both and build a collection.

Progress has two separate measures:

- **Credits** buy cars and parts. They can always be earned by replaying unlocked races.
- **Reputation** unlocks events and dealership stock. Each event awards reputation for the player's best result so far, preventing one easy race from unlocking the entire game.

Race entry and fuel are free. Condition-based maintenance and repairs cost credits. There are no loans, daily timers, or purchase rerolls. With a roadworthy car, an eligible career race remains a way to earn credits. If every owned car is unroadworthy and the player cannot afford the cheapest minimum repair, a free tow and a workshop loaner activity provide an explicit recovery path without negative money or forced sales.

## 2. Concrete progression

| Tier | Minimum total reputation | What changes |
| --- | ---: | --- |
| Rookie | 0 | Choose a starter; enter early Dockside events; install stage 1 parts. |
| Club | 150 | Stage 2 parts, first hot hatch, more Dockside and Midtown events. |
| Pro | 500 | Stage 3 parts, performance cars, Ridgeway events. |
| Elite | 1,100 | Final dealership car and championship finale become available. |

All districts are physically explorable immediately. Reputation gates event entry and inventory, with the exact requirement visible in the map and dealership. Event recommended performance is guidance, not a hard performance-score restriction. A car must also pass the mechanical roadworthiness checks in the maintenance design; the menu explains an unsafe component, active breakdown, or cooldown requirement explicitly. Story flags do not silently lock races.

The vertical slice contains the first three cars and `evt_dock_01` through `evt_dock_04`. It therefore demonstrates a starter choice, upgrades, Club reputation, a desirable next car, repeatable earnings, and a rival. It must not expose purchase buttons for cars whose assets and gameplay are absent.

## 3. Starting state and car catalog

Before the initial purchase, the player has 8,000 credits and no owned car. The initial dealer offers a guaranteed healthy Finch for 4,000 and Courier for 5,000, both fully verified with all six mechanical conditions at 10,000/10,000. Their included inspection and clean accident history prevent an unavoidable starter trap. The purchase transaction creates the first owned-car instance, selects it, debits its actual listing price, and saves together. After that, every active save must contain at least one car.

The catalog table is the **reference MSRP**, used for upgrade price scaling and comparison. Actual used listings may cost less and have their own persistent vehicle ID, mileage, disclosures, repair records, component condition, and inspection state. Buying transfers that exact vehicle into ownership; it does not reset its odometer, faults, history, or already installed parts. One owned instance per model permits choosing an alternative listing for that model, while rejecting a second simultaneously owned copy. Consumed initial listings stay consumed across saves. A sold car can return through the explicit buyback rule below with the same vehicle ID and state; reloading or revisiting the dealer cannot produce a fresh healthy copy or reroll its history.

Before committing, show listing price, catalog MSRP, mileage, disclosed repair/accident history, known condition, inspection availability, and known repair costs plus a conservative bound for unknowns. Viewing seller disclosures and existing records is free. The optional 100-credit mechanic inspection reveals the actual underlying condition before purchase and saves that report; it never generates or changes the car's faults. Reopening the same paid report is free. An owned car's minimum roadworthiness quote is free and authoritative, including when the player cannot afford inspection. A low sticker price can therefore be a deliberate project-car choice with a visible service budget.

The table's `kW` column is approximate stock peak crankshaft power **derived from the stock torque curve** in `powertrains.json`, not an independently editable field in `cars.json`. Grip is a dimensionless tuning coefficient; braking is target peak deceleration in m/s²; chassis is a 0–100 response rating. These are prototype targets, not promises of simulated real-world performance. Top speed is an approximate target before upgrades, not a speed limiter or the sole measure of speed. PI is computed from the formula below rather than stored as a second editable truth.

| Catalog ID | Fictional car / role | Price | Rep | Drive | kW | kg | Grip | Brake | Chassis | Approx. km/h | Shift seconds |
| --- | --- | ---: | ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `car_finch` | Aster Finch; compact starter, easy placement | 4,000 | 0 | FWD | 65 | 1,050 | 0.95 | 7.2 | 50 | 155 | 0.45 |
| `car_courier` | Brant Courier; inexpensive RWD coupe | 5,000 | 0 | RWD | 85 | 1,220 | 0.96 | 7.0 | 48 | 175 | 0.48 |
| `car_lynx` | Cinder Lynx; first hot hatch | 12,000 | 150 | FWD | 130 | 1,180 | 1.00 | 8.0 | 60 | 210 | 0.35 |
| `car_switch` | Nera Switch; stable AWD all-rounder | 20,000 | 250 | AWD | 155 | 1,400 | 1.02 | 8.2 | 62 | 215 | 0.32 |
| `car_sable` | Vanta Sable; RWD sports coupe | 32,000 | 500 | RWD | 200 | 1,370 | 1.03 | 8.6 | 66 | 240 | 0.30 |
| `car_quarry` | Talon Quarry; AWD hill specialist | 48,000 | 650 | AWD | 230 | 1,460 | 1.07 | 8.8 | 68 | 245 | 0.28 |
| `car_vela` | Orin Vela; lightweight high-speed coupe | 72,000 | 900 | RWD | 300 | 1,360 | 1.08 | 9.3 | 76 | 280 | 0.24 |
| `car_apex` | Spectra Apex; aspirational AWD flagship | 110,000 | 1,100 | AWD | 400 | 1,450 | 1.12 | 10.5 | 82 | 300 | 0.20 |

All eight cars need recognizably different silhouettes and driving character; one mesh with a different price does not satisfy eight cars. Slice cars may use coherent placeholder models while handling is verified. Use fictional badges and licensed or self-created assets. Car catalog entries are definitions; an owned car is an instance with its own stable ID, paint, upgrades, and purchase investment. V1 permits one instance per catalog ID to keep collection UI and save logic small.

## 4. Upgrade system

All six categories have stock and three **sequential paid stages**. The only valid purchase is the next stage in that category: stock → 1 → 2 → 3. Reputation alone does not permit skipping a stage. Each purchase has its own confirmation, wallet debit, installed-state change, and saved receipt. Stage unlock reputation remains 0 / 150 / 500. A player may stop at any stage; there is no pressure to buy every category.

Engine upgrades select a compatible, authored hardware-and-tuning configuration. Other categories use absolute offsets or multipliers against stock values, rather than compounding previously modified statistics. Tires stage 3 sets grip to stock × 1.12; it does not multiply all three tire effects. Categories combine with each other. Engine peak power comes from the installed torque curve, never a generic power multiplier or a manually edited horsepower number.

| Category | Stage 1 | Stage 2 | Stage 3 | Reference price rule |
| --- | --- | --- | --- | --- |
| Engine | First paid preparation/tune package | Next compatible hardware/tune package | Final supported internals/cooling/tune package | Each step has an individual paid price in its engine path. |
| Tires | Grip × 1.04 | × 1.08 | × 1.12 | Cumulative reference totals: 450 / 950 / 1,800 |
| Suspension | Chassis + 4 | + 8 | + 12 | Cumulative reference totals: 350 / 800 / 1,500 |
| Brakes | Deceleration × 1.06 | × 1.12 | × 1.18 | Cumulative reference totals: 250 / 600 / 1,100 |
| Weight reduction | Mass × 0.97 | × 0.94 | × 0.90 | Cumulative reference totals: 300 / 700 / 1,400 |
| Transmission | Shift time × 0.88 | × 0.72 | × 0.55 | Cumulative reference totals: 350 / 850 / 1,650 |

### Engine paths and meaningful hardware

Each car references a stock powertrain and a list of compatible paths. The Finch has a modest naturally aspirated path; the Courier and Sable support naturally aspirated, turbo-conversion, and positive-displacement-supercharger paths. The Vela supports naturally aspirated tuning or a turbo conversion. Factory-boosted cars have their own supported packages. Do not fit every kit to every car or treat a turbo as a cosmetic badge.

At engine stage 1, the player chooses a path and sees its complete stage progression, prices, response characteristics, and transmission requirements. That choice is saved and locked in V1. A naturally aspirated car's stage 1 configuration is the same across its compatible paths; a conversion path's turbo or supercharger arrives at stage 2, after that conversion package is actually purchased. Choosing a future turbo path grants no free boost at stage 1. Explain the locked choice before payment.

- **Naturally aspirated tuning:** modest gains through the usable RPM range, immediate throttle response, and an authored safe redline. Small tuning gains remain small.
- **Turbo path:** an authored boosted torque curve, a meaningful spool region, and transient boost response. More peak power can require keeping the engine in its useful range.
- **Positive-displacement supercharger:** stronger low/mid-range response with the package's net torque curve accounting for parasitic load. It does not inherit turbo lag or receive an additional free boost multiplier.
- **Factory turbo/twin-turbo:** staged packages suit the already-boosted engine rather than purchasing another copy of its stock hardware.

Each engine step bundles its listed required supporting hardware and calibration: the relevant intake/exhaust/fueling, conversion hardware, cooling, or strengthened internals. Those items are included in that step's paid price and do not appear as a second invoice, freely sellable part, or separate resale bonus. Transmission stages remain separately purchased prerequisites where the powertrain definition requires them. For example, conversion stage 2 requires transmission stage 1 and conversion stage 3 requires transmission stage 2. Check the specific package; do not silently install a prerequisite or substitute a weaker curve.

Reference engine **step prices**, before car scaling:

| Path family | Stage 1 paid step | Stage 2 paid step | Stage 3 paid step | Sum of reference steps |
| --- | ---: | ---: | ---: | ---: |
| Naturally aspirated | 550 | 800 | 1,600 | 2,950 |
| Factory turbo | 550 | 1,500 | 3,000 | 5,050 |
| Aftermarket turbo / positive-displacement conversion | 550 | 2,500 | 4,500 | 7,550 |
| Apex factory twin-turbo | 550 | 1,750 | 3,500 | 5,800 |

The path stages in `powertrains.json` are the authoritative price and compatibility source. This summary does not replace per-car package data. Each step is scaled and rounded separately, so rounding a path's combined reference sum is incorrect. Prices are fictional game balance, not quotations for real parts.

### Payment and installed investment

Use the shared scaling for both pricing modes:

```text
price_multiplier = clamp(sqrt(car_definition.purchase_price / 8000), 0.75, 3.0)
rounded_scaled_price(reference) = floor(reference * price_multiplier / 50 + 0.5) * 50

require requested_stage == current_stage + 1
require reputation >= stage_reputation_required[requested_stage]
require compatible_engine_path_and_supporting_stages_when_applicable
require maintenance_readiness_for_installation

engine_amount_due = rounded_scaled_price(path.next_stage.paid_reference_price)
current_installed_stage_total = 0 if current_stage == 0
                              else rounded_scaled_price(category.current_stage.reference_total_price)
other_amount_due = rounded_scaled_price(category.next_stage.reference_total_price)
                 - current_installed_stage_total

new_historical_category_investment = old_historical_category_investment + amount_due
```

For engine stages, never subtract earlier engine payments from a later step's price: every stage is separately paid. For other categories, each sequential step pays only the difference between the next and current installed stage's published cumulative totals. This preserves the existing stock-to-stage-3 prices and correctly credits hardware already included in a bought-back car. Do not subtract current-ownership historical investment to calculate that quote: an acquired preinstalled stage may have zero new investment by this owner. Historical category investment starts at zero on acquisition and records only actual new debits for resale. Reject a negative quote or inconsistent migrated stage data rather than crediting the wallet during an upgrade.

Here `car_definition.purchase_price` always means the catalog MSRP, even when the owned car came from a discounted listing. Never substitute `purchase_price_paid`: buying a worn car cheaply must not make every later tuning package artificially cheap. For example, every Courier uses the 5,000-credit catalog basis for the same 450/2,000/3,550 turbo steps, regardless of its listing price. Installation requires the maintenance readiness thresholds and no active thermal warning.

A paid next-stage tires, brakes, or suspension installation replaces its own physical component set and renews **only that component** to 10,000, with no prior repair charge for the component being replaced. Other components must satisfy the authored readiness checks. Engine, transmission, and weight upgrades preserve component conditions; engine/transmission installation additionally requires healthy core engine/cooling/transmission condition. A bundled intercooler or forged-internals package is not a free overhaul of the cooling system or worn engine block. No upgrade erases repair/accident history or heals unrelated components. `maintenance.json` owns the exact category-to-component renewal mapping and readiness thresholds.

Example: Finch engine steps cost 400, 600, and 1,200 credits after scaling/rounding, for 2,200 paid in total. Its stage 3 engine prerequisite also requires the separately paid transmission stage specified by its path. Finch tires cost 350 for stage 1, another 350 to reach the 700-credit stage 2 total, then another 650 to reach the 1,350-credit stage 3 total. Trying to buy either stage 3 from stock must fail without changing cash or parts.

The Courier's turbo-conversion engine steps cost 450, 2,000, and 3,550 credits, for 6,000 of engine investment. Transmission stage 1 costs another 300 before engine stage 2; reaching transmission stage 2 costs another 350 before engine stage 3. A stage 3 conversion therefore requires at least 6,650 of actual paid engine/transmission steps, beyond buying the car. Engine stage 3 alone is not a 3,550-credit shortcut from stock.

For full-build budgeting, every category at stage 3 costs 7,850 in installed upgrades on the Finch, 8,200 on a naturally aspirated Courier, or 11,850 on a converted Courier. These totals sum every engine step and each other category's final cumulative total, including the paid transmission. They exclude the original car purchase and optional paint. Recompute them from package data when balancing; never assume one universal engine stage 3 cost.

Display each due amount, already paid investment, remaining prerequisite, affected torque curve or other statistics, estimated PI change, and wallet after purchase before confirming. The engine preview shows low/mid/high-RPM torque, peak derived power, redline, and response, not only a single power bar. Tire and brake purchases must be perceptible in repeatable corner/braking trials. Tuning values can be adjusted during playtests while these identities stay distinct.

There is no separate tradable part inventory, downgrade, path swapping, or engine swapping in V1. Repairs are workshop service transactions that restore defined components, not loose inventory items. A repeated or stale purchase request must not charge again, install another copy, or compound the curve. Different builds remain useful because their drivetrains, torque delivery, grip, mass, cost, and maintenance condition differ.

## 5. Performance estimate

Use the installed build's **healthy potential** values in this shared heuristic. Label it **Potential PI (healthy)**. Engine power is a derived value:

```text
power_kw_at_rpm = installed_steady_wot_torque_nm(rpm) * rpm / 9549.2966
effective_power_kw = maximum(power_kw_at_rpm over installed idle..redline range)
power_to_weight = effective_power_kw * 1000 / effective_mass_kg
raw_pi = 100
       + 2.2 * (power_to_weight - 60)
       + 250 * (effective_grip - 0.85)
       + 1.2 * (effective_chassis - 40)
       + 6 * (effective_brake_deceleration - 6)
       + 30 * (0.45 - effective_shift_seconds)
PI = clamp(floor(raw_pi + 0.5), 100, 999)
```

The torque curve is interpolated according to the powertrain document. Derive peak power consistently: a maximum can occur inside an interpolated interval, not only at a curve knot. Do not independently author `power_kw` in the car catalog, apply an old stage multiplier, or add boost a second time to a curve that already includes steady-state boost.

Class labels: D 100–249; C 250–399; B 400–549; A 550–699; S 700–999. These labels explain the garage, not additional event restrictions. PI estimates healthy build capability; torque delivery, spool response, gearing, road layout, drivetrain, condition, and driver skill still affect results. Show mechanical readiness, current derating, and faults separately. A worn car can have strong potential PI while delivering reduced torque or grip and being ineligible to race. Apply bounded runtime condition modifiers without mutating the authored healthy torque curve or installed upgrade stage.

The default garage dyno compares the current healthy installed configuration with the proposed healthy configuration. An optional current-condition overlay must be labeled clearly. Equal-PI engines need not feel identical. Changing a displayed number alone does not implement a part: the vehicle physics must consume the same configuration and the defined condition modifiers.

Computed balance checkpoints from the authored curves and current prices:

| Car / engine path | Stock PI | Stage 3 engine peak kW | PI with all six categories at stage 3 | Total paid upgrades for that full build |
| --- | ---: | ---: | ---: | ---: |
| Finch / naturally aspirated | 149 | 86.4 | 270 | 7,850 |
| Courier / naturally aspirated | 163 | 113.0 | 294 | 8,200 |
| Courier / turbo conversion | 163 | 162.5 | 393 | 11,850 |
| Courier / supercharger conversion | 163 | 154.5 | 377 | 11,850 |
| Lynx / factory turbo | 285 | 188.5 | 493 | 15,250 |
| Switch / factory turbo | 296 | 226.2 | 507 | 19,700 |
| Sable / naturally aspirated | 386 | 258.7 | 584 | 20,800 |
| Sable / turbo conversion | 386 | 367.6 | 779 | 30,000 |
| Sable / supercharger conversion | 386 | 336.9 | 724 | 30,000 |
| Quarry / factory turbo | 426 | 337.6 | 704 | 30,600 |
| Vela / naturally aspirated | 578 | 369.2 | 819 | 31,200 |
| Vela / turbo conversion | 578 | 510.9 | 999 | 45,000 |
| Apex / factory twin-turbo | 728 | 590.2 | 999 | 39,750 |

Full-build costs exclude buying the car, inspection, and service; they sum all paid sequential engine steps plus the other five categories' final cumulative totals. They include transmission payments and therefore satisfy those packages' supporting-stage requirements. All PI values assume healthy components. They are budgeting examples, not a permission to buy a final-stage bundle. PI 999 is a display ceiling: it does not make the Vela and Apex identical in actual acceleration or response. A fully upgraded starter remains below the stock flagship, while an expensive conversion can carry a mid-tier favorite into later races. These computed values validate arithmetic only; driving and progression still require playtesting.

## 6. Race payouts and reputation

All event rewards are authored data. Normal sprint/circuit events have three AI opponents plus the player. Rival sprints have one opponent. Time trials have gold, silver, and bronze targets defined with the route after a real baseline run; they are not fabricated unverified times in planning data.

| Result | Credit multiplier | Reputation multiplier |
| --- | ---: | ---: |
| First / gold | 1.00 | 1.00 |
| Second / silver | 0.80 | 0.80 |
| Third / bronze | 0.65 | 0.60 |
| Fourth / completed below bronze | 0.50 | 0.40 |
| Abandon / DNF / invalid checkpoint sequence | 0 | 0 |

A rival loss is second place, using the same second-place factors. Any valid completed finish can claim the first-clear bonus, even if the player did not win. Rival story defeat requires first place, but completing a loss still pays. A time trial has no in-game timeout by default; a valid slow finish earns the last result band.

```text
credits_earned = floor(base_reward * result_credit_multiplier)
              + (first_valid_finish ? first_clear_bonus : 0)
new_best_rep = floor(max_reputation * result_rep_multiplier)
rep_awarded = max(0, new_best_rep - saved_best_rep_for_event)
saved_best_rep_for_event = max(saved_best_rep_for_event, new_best_rep)
```

Credits are replayable. First-clear bonuses and reputation milestones are not. Example: third in a 100-reputation race awards 60; later winning awards another 40, not another 100. Event entry snapshots opponent settings and player car; installation or selling during the race is disallowed.

| Event ID | Type | Rep gate | Recommended PI | Base win / gold | First-clear bonus | Maximum rep |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| `evt_dock_01` | time_trial | 0 | 145 | 700 | 400 | 50 |
| `evt_dock_02` | sprint | 0 | 155 | 900 | 500 | 60 |
| `evt_dock_03` | circuit | 0 | 170 | 1,000 | 600 | 70 |
| `evt_dock_04` | rival sprint | 120 | 185 | 1,400 | 800 | 90 |
| `evt_dock_05` | time_trial | 150 | 220 | 1,700 | 900 | 100 |
| `evt_dock_06` | circuit | 250 | 260 | 2,200 | 1,200 | 140 |
| `evt_mid_01` | circuit | 150 | 240 | 1,700 | 900 | 100 |
| `evt_mid_02` | sprint | 250 | 290 | 2,100 | 1,000 | 110 |
| `evt_mid_03` | time_trial | 350 | 320 | 2,500 | 1,200 | 130 |
| `evt_mid_04` | rival sprint | 450 | 360 | 3,000 | 1,500 | 160 |
| `evt_mid_05` | circuit | 650 | 430 | 3,800 | 1,800 | 200 |
| `evt_ridge_01` | time_trial | 500 | 360 | 2,800 | 1,400 | 150 |
| `evt_ridge_02` | sprint | 650 | 420 | 3,400 | 1,600 | 170 |
| `evt_ridge_03` | circuit | 800 | 480 | 4,200 | 2,000 | 200 |
| `evt_ridge_04` | rival sprint | 950 | 550 | 5,000 | 2,500 | 230 |
| `evt_ridge_05` | circuit finale | 1,100 | 650 | 6,500 | 4,000 | 300 |

There are no prerequisite event IDs in V1: all `prerequisite_ids` arrays are empty. The authored route document owns route names, checkpoints, laps, and rivals; this table owns reward and entry values. Circuit events with a named rival remain standard four-car races.

Gate reachability with top results: Dockside 01–03 produce 180 rep, enough for Club and Dockside 04. Those four produce 270, opening Dockside 06 and Midtown 02. Dockside 05 and 06 then produce 510 total, enough for Pro. Midtown 01–04 add 500, reaching 1,010; Ridgeway 01 adds 150, reaching 1,160 and Elite. These are examples, not required event order. All 16 events together award at most 2,260 reputation. A player getting weaker results can revisit races to improve best results; the UI must explain that relationship.

The opening intentionally includes a modest skill gate: bronze/third across the first three events totals 108 reputation, below the rival's 120. Improving Dockside 03 from bronze to silver adds 14, reaching 122; completing the rival even in second then reaches Club. All-silver results can numerically reach every event. Repeated identical results still earn credits for better parts, but do not add reputation. When the player stalls, the map should identify an achievable best-result improvement rather than merely say “earn more reputation.” Final campaign victory still requires winning the finale.

## 7. Ownership, selling, and displayed wealth

Keep credits and collection wealth distinct:

```text
paid_basis = floor(0.70 * purchase_price_paid)
           + floor(0.50 * sum(installed_category_investments))
condition_bps = 4000 + floor(6000 * sum(six_component_conditions) / 60000)
history_bps = 10000 if no_recorded_accident
              9500 if minor_repaired_incident_is_worst_history
              8500 if major_or_structural_history_is_worst
resale(car) = floor(paid_basis * condition_bps * history_bps / 100000000)
garage_value = sum(resale(car) for every currently owned car)
net_worth = wallet + garage_value
```

Conditions are integer basis points on 0–10,000. Both valuation factors are at most 1; a healthy car with clean history retains the old paid-basis quote exactly. The history penalty reflects the worst persistent incident severity and cannot be removed by buying an inspection or completing a service. `maintenance.json` owns these factor definitions. Reject invalid or out-of-range condition instead of manufacturing a quote from it.

Use the instance's recorded actual listing price paid, never catalog MSRP or a future listing price, for the base resale. Installed investment means the sum of actual new upgrade payments during **current ownership**, not lifetime payments from earlier ownership cycles. Engine investment includes every newly paid sequential engine step; supporting engine hardware is already included in those payments. Paid transmission prerequisites belong to transmission investment and are counted there once. Preinstalled used-car hardware is included in its actual purchase price; it receives no fabricated historical upgrade investment for this ownership cycle. Do not separately value a turbo, a tune, previous stages, or bundled hardware on top of that sum.

Inspection and service spending are recorded separately in the car's history and spending summary and contribute **zero additional upgrade investment or separate resale value**. A repair can improve the condition factor and driving readiness but never push the quote above `paid_basis`. Paint has no resale value. A sale removes the car and credits its resale in one transaction. Do not separately add original cost, service receipts, installed parts, or lifetime earnings to net worth. Spending money may reduce net worth through transaction loss; the garage remains a satisfying visible collection rather than an ever-increasing score.

The player cannot sell their last car. Selling the active car selects another owned car in the garage; if the replacement is not roadworthy, remain in workshop/recovery UI instead of spawning an unusable car into an active race. V1 has no paid garage slot expansion or capacity purchase. Buying a second simultaneously owned instance of the same model is disabled. Trading cycles lose credits: a fully healthy clean-history Finch bought for 4,000 has a 2,800 quote; installing 350-credit tires adds 175 to its healthy paid basis. Wear or incident history can reduce the actual quote.

A sold car can be bought back at `max(last_actual_purchase_price, current_resale_quote + 500)`. The dealer retains that exact car's stable vehicle ID, mileage, current conditions, failures, locked engine path, installed stages, and lifetime service/tuning/inspection history. It does not respawn a pristine catalog car. On reacquisition, the actual buyback debit becomes the new purchase-price basis and current-ownership upgrade investments reset to zero: the acquired hardware is now included in that price. An acquisition receipt confirms existing stages and their history without charging the new owner again for those installed stages. Future purchases still allow only the next stage. Lifetime receipts remain inspectable but are not counted twice as current investment. All ten initial offers are stock; preinstalled-stage acquisition first matters when a player buys back their own developed car.

Display actual purchase price, upgrade investment, separate maintenance/inspection spending, current condition and history percentages, and the current resale quote. Show garage value as the sum of those current quotes and net worth as wallet plus garage value. A repair can increase a depressed current quote while still costing more overall than selling was worth; V1 does not promise profitable flipping. Since resale never exceeds 70% of the actual paid purchase plus 50% of paid upgrades, buying, repairing, upgrading, and selling cannot create a net profit even if repair restores every condition to perfect.

Concrete used-car valuation: `listing_courier_002` costs 3,500, has no paid player upgrades, and has minor accident history. Its six conditions total 25,500, giving condition factor 65.50% and history factor 95%. Paid basis is 2,450; the current resale quote is 1,524. The 300-credit cooling/brake minimum repair raises total condition to 31,200 and the quote to 1,657, an increase of 133. The repair expense is not an investment bonus, and the minor-history factor remains. Full mechanical restoration would still cap this car's quote at 2,327, below the purchase price even before inspection and service costs. These values are arithmetic examples drawn from the authored listing, not real vehicle-market prices.

## 8. Maintenance budget and recovery income

The maintenance design defines six components: engine, cooling, transmission, brakes, tires, and suspension. A component below 2,000 condition points is not roadworthy. Minimum service repairs **all components below 2,000**, including those still above the 1,000 hard-failure threshold, to 4,500. It never repairs only already immobilized components while leaving an unsafe 1,001–1,999 gap. The fixed minimum prices are 300 / 150 / 250 / 150 / 200 / 150 respectively; repairing all six therefore costs at most 1,200. Full restoration costs 600 / 300 / 500 / 300 / 400 / 300 and restores selected components to 10,000. All prices are fictional balancing values and unscaled by MSRP. Exact wear, repair-readiness, cooling, and failure rules remain in the maintenance document.

Minimum service is an affordable route back onto the road; tuning readiness can require healthier components than roadworthiness. Show minimum repair and full restoration as distinct quotes with components, resulting condition, total cost, and remaining wallet. A service transaction debits once, updates the relevant conditions/failure state and records the repair at the actual mileage together. It never pays cash to the player, installs a free tuning stage, or resets unrelated component wear. A paid service can replace worn tires/brakes/suspension later while preserving their installed tuning stage. Optional inspection does not repair anything. Engine-off cooling and towing to the workshop are free; a temporary heat-only restriction never requires buying a repair or paying to inspect a healthy cooling system.

Worked maintenance budget: reserve 1,000 credits **in addition to a listing's purchase price**. Pay 100 for its optional pre-purchase inspection; the reserve becomes 900. After buying at the separately displayed listing price, repair cooling at 1,500 condition and brakes at 1,800 condition for 150 each. Both rise to 4,500; the maintenance reserve becomes 600. Other components remain as purchased, and the 300 of service spending goes into service history rather than the resale upgrade total. The player can instead inspect a guaranteed healthy starter with its already included report and avoid these initial inspection/repair costs.

When no owned car is roadworthy, compare wallet against the cheapest authoritative minimum-service quote across owned cars:

1. Free tow always reaches workshop UI. Getting a minimum quote never costs credits.
2. If wallet covers that quote, offer the paid minimum repair directly. If a car is merely hot, cool it with the engine off for free instead.
3. If wallet is smaller, offer `act_workshop_recovery`, a workshop emergency time trial using a healthy temporary loaner. A valid finish pays 300, with no reputation, first-clear bonus, medal-dependent reduction, entry fee, or loaner wear. It uses ordered gates and a unique settled run ID. Abandoning/invalid runs pay zero and may be retried for free.
4. Recheck eligibility before every run. Once wallet covers the minimum repair, stop offering new emergency runs and show that repair. The job is separate from the sixteen career events and their reputation ledger; it never becomes a seventeenth championship event. The loaner is not added to ownership and cannot be sold, tuned, or used in career races.

With zero credits, the maximum 1,200 minimum quote needs four valid 300-credit jobs. If one component needs a 150-credit fix, one job suffices and leaves 150 after repair. More generally the required successful runs are `ceil(max(0, cheapest_quote - wallet) / 300)`, at most four. This is a recovery bound, not a claim that novice play never needs repairs. Repeated runs cannot settle twice, and no repair creates debt. A player who deliberately delays a repair can still navigate workshop menus and view history without being stranded.

## 9. First-session example

1. Start at 8,000; buy the 4,000-credit Finch: wallet 4,000.
2. Install stage 1 tires (350), brakes (200), and the Finch's first naturally aspirated engine package (400): wallet 3,050. This is a paid stock → 1 engine step using its authored stage 1 curve; later engine stages remain uninstalled and unpaid.
3. Gold in Dockside 01: earn 700 + 400, wallet 4,150, rep 50.
4. Win Dockside 02: earn 900 + 500, wallet 5,550, rep 110.
5. Win Dockside 03: earn 1,000 + 600, wallet 7,150, rep 180. Club, stage 2, and the Lynx become visible and unlocked; the Lynx is still a savings goal.
6. Decide whether to invest further, run the rival, explore Midtown, or repeat enjoyable events to reach 12,000. Repeating a perfect early result adds credits while reputation stays unchanged.

This example uses the guaranteed healthy starter, its included inspection, and a short clean opening run that does not cross a service threshold. It therefore preserves the original 7,150-credit result without hiding a compulsory inspection or repair fee. Later maintenance is condition-based and must be included in measured progression pacing.

Pacing targets for playtesting: first meaningful upgrade in the opening session; four introductory events in roughly 20–35 minutes including free roam and menus; first next-tier car in roughly 35–60 minutes; first finale in roughly 4–8 hours; broader collection/upgrades over 6–10 hours or more. These are hypotheses. Measure actual driving time, median placement, failures, event repeats, and purchase timing before increasing costs or payouts. Sixteen authored events plus repeats do not guarantee this amount of entertainment; add route variants only after the driving loop is enjoyable.

## 10. Transaction safety and acceptance checks

All mutations validate current authoritative state, then commit once. Each valid race completion uses a unique run ID. Persist wallet, event best result, first-clear flag, claimed run ID, unlock-derived state, and session position together. Returning to the results screen or loading a save must not claim the same run twice. Save failures must retain the last valid on-disk save and must not grant a second payout on retry.

Required meaningful checks when implemented:

- Start, buy either starter, save, relaunch: same instance, wallet, active car, and upgrades.
- Reject stock → 2, stock → 3, and stage 1 → 3 in all six categories without charging or installing anything. Pay each allowed step once; the same transaction ID and stale state cannot pay/install again.
- Install engine 1 → 2 → 3: wallet debits equal the sum of individually rounded package step prices; historical engine investment equals those actual debits, with no subtraction of earlier engine payments from later prices.
- Install other categories 1 → 2 → 3: the paid differences sum to their final rounded cumulative totals. Check exact-half price rounding and deterministic quotes after reload.
- Reject an incompatible or changed engine path, insufficient reputation, missing transmission prerequisite, insufficient funds, or an already-installed stage without any partial mutation.
- Tires change physical cornering behavior; brakes change stopping distance; installed engine curves change timed acceleration and low/mid/high-RPM torque under the same input. Turbo spool and supercharger response follow their own authored configurations.
- Recompute/load a build repeatedly: no compounded curves, duplicate boost, free supporting stages, or additional hardware resale value. Derive displayed peak power and PI from the same installed curve the controller consumes.
- Third then first in one event: total rep equals that event's maximum; repeated first places add no rep or first-clear bonus.
- Reopen/reload results with the same run ID: no second payout; a newly completed replay does pay its repeat reward.
- Losing or slowly finishing pays the promised amount; abandoning and skipped checkpoints do not.
- Spend to zero with a roadworthy car: an eligible career event remains playable without an entry fee. With all cars unroadworthy: tow and minimum quote are free, and eligible loaner jobs can fund at least one repair in at most four valid finishes.
- Buy–upgrade–sell: no wealth creation and no upgrade value counted twice; selling the last car fails without mutation.
- Buy a discounted used listing: vehicle ID, mileage, installed build, hidden state, disclosures and history transfer unchanged; its actual paid price becomes the resale basis while upgrade quotes still use catalog MSRP.
- Inspect, save, reload, revisit, or buy back after sale: no listing reroll, duplicated vehicle, lost receipt, refreshed healthy copy, or extra charge for reopening the same saved report. Buyback preserves the exact build and condition, uses its defined price, and resets only current-ownership investment bookkeeping.
- Repair cooling/brakes at 1,001–1,999 condition: a positive minimum quote exists and service makes both roadworthy. Inspection cannot be a prerequisite for free tow, authoritative owned-car minimum quote, or emergency eligibility.
- Service restores only quoted conditions and records mileage/cost once; it adds nothing to paid upgrade investment. Engine/transmission/weight stages preserve all condition. A paid next tires/brakes/suspension stage renews only its own component, with one saved receipt; repeating that stage cannot heal or charge again.
- Acquire a preinstalled stage with zero new-owner upgrade investment, then buy its next stage: quote equals the published next-minus-installed-stage total, and resale investment increases by only the actual new debit. Existing engine paths and installed stages remain locked/sequential.
- The healthy paid-basis quote, condition/history discounts, and final rounding match independent examples; no factor exceeds 1, and repair/sale cannot exceed the car's paid basis.
- Potential PI stays stable when a build's mechanical condition changes; actual output, readiness, derating, and a labeled current-condition overlay reflect that change separately.
- Every rep gate is reachable through existing events; all eight stock and upgraded configurations produce finite positive values.

No automated checks have been run against game code because this packet contains a design, not a game implementation.
