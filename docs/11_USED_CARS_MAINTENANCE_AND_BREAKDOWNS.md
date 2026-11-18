# Used cars, maintenance and breakdowns

## Purpose and scope

Buying a used car now includes checking its repair history, judging its condition, and budgeting for service. Worn or abused cars can lose performance and eventually break down. A properly maintained healthy car does not fail because of an invisible random roll.

`design-data/maintenance.json` owns the proposed condition, wear, service, breakdown and recovery rules. `design-data/used_car_listings.json` owns ten initial vehicle offers: four in the slice and six added for V1. These are fictional game values requiring implementation and playtesting, not real repair prices or reliability predictions. The release still has eight car models and sixteen career events; the workshop recovery activity is separate from that event count.

The initial dealer guarantees a fully inspected, recently serviced Finch for 4,000 and Courier for 5,000. Both start with all six modeled components at 10,000/10,000. Starting cash remains 8,000. The slice also contains a worn Courier alternative and a healthy Lynx. A second, worn Sable joins the remaining five model offers in V1. There is no procedural stock reroll or hidden mandatory starter repair bill.

## Buying and checking a car

Each listing represents an actual persistent vehicle, not a temporary offer that generates a different car when purchased. Its page shows asking price, odometer, known owner count, disclosed accidents, supplied service receipts, inspection status, installed build, and known/unknown condition fields. The one-owned-car-per-model restriction still applies; the alternative Courier and Sable offers are choices, not additional garage slots.

Viewing history and seller disclosures is free. Every offer permits a mechanic inspection. `inspection_report_included` means a current complete report is already supplied; false means the optional **100-credit pre-purchase inspection** can be bought to reveal all six actual component values for that vehicle's current condition revision. Save the inspection result; leaving, reloading or changing menus never rerolls it. Inspection cannot reconstruct missing historical records. “No recorded accident, incomplete history” must not become “accident free.” A documented repaired incident remains visible.

Before inspection, display exact values only for `known_condition_components`. Label the rest unknown and show known repair needs plus a conservative possible-cost range, rather than pretending to know an exact total. Do not expose the hidden truth fields through an ordinary comparison screen. Both guaranteed starters already include their complete inspection, so paying again is unnecessary.

After purchase, the owner's health screen and free workshop assessment expose the modeled current condition and exact service quotes. The optional inspection fee must never block diagnosis, an essential repair, or recovery income. Confirming purchase transfers the listed vehicle ID, history, odometer, condition, build and path into ownership in the same saved transaction as the debit.

## Vehicle identity, history and ownership accounting

Keep three concepts separate:

- **Catalog model:** shared specifications and MSRP, such as `car_courier`. MSRP remains the basis for upgrade-price scaling.
- **Physical vehicle:** persistent `vehicle_id`, odometer and fractional distance, six conditions and wear fractions, heat, installed build/path, inspection revisions, and append-only history.
- **Current ownership period:** acquisition receipt, actual purchase price paid, and actual upgrade payments made during this ownership period. These determine the paid resale basis.

Every service, repair, upgrade/replacement, acquisition, sale and major incident appends an idempotent receipt with a unique record ID, sequence, odometer, type, description, applicable components, and actual cost. Record incident severity where relevant. Sequence increases and odometer never decreases. Preexisting missing records stay missing; ordinary service does not erase prior incidents or manufacture a clean past.

Inspection and service spend are operating expenses, separate from upgrade investment. Lifetime receipts remain available after selling and reacquiring a vehicle; their historical costs are not credited again as current ownership investment.

A sold car becomes a dealer buyback listing with the **same vehicle, condition, mileage, installed stages and locked engine path**. Its asking price is `max(last_purchase_price_paid, sale_quote + 500)`. Buying it back creates a new ownership period: its existing build is included in the new purchase price, and current ownership upgrade investment starts at zero. Previously installed stages still satisfy progression prerequisites. For nonengine upgrades, the next payment is the rounded published cumulative price of the next stage minus that of the installed stage, not minus current ownership investment. The next engine step still costs its full individually priced payment.

## Mileage, trip distance and service intervals

Each vehicle has a permanent lifetime odometer. Show it prominently in listings, comparisons, the garage and its history page. Let the player choose miles or kilometers; keep the same underlying meters with `1 mile = 1,609.344 m` and `1 km = 1,000 m`. Round only the displayed number. An 82,000 km car is approximately 50,952 miles, not a different vehicle or condition calculation.

Track distance driven during the current ownership period and a separately resettable trip counter. Store the acquisition odometer and trip baseline/residual; derive the displayed distances from actual movement. Resetting the trip changes only the trip. Repairs, a rebuilt engine, new tires, upgrades, resale and reacquisition never reset the chassis odometer or erase earlier records. A temporary workshop loaner adds no distance to parked owned cars.

For each component, show **last documented service mileage**, work performed, condition after that work when recorded, and distance since it. Derive the latest service from actual scoped maintenance/repair/replacement receipts. Missing service mileage is `null` and displays “Unknown”; never invent zero or the current odometer. A recent minimum repair is not a full rebuild. Inspection can measure present condition but cannot manufacture a service record. Replacing brakes updates their service marker, not the engine's.

Also show an advisory **next component check** based on distance since the latest scoped assessment/service or the current acquisition baseline, whichever is newest. Proposed game check intervals are engine 250 km, cooling/transmission 300 km, brakes 100 km, tires 80 km and suspension 200 km. These are deliberately compressed game reminders, not real vehicle maintenance specifications. Use `maintenance.json` for their values.

The acquisition/assessment baseline starts a gameplay check schedule without falsely claiming that a historical service happened today. A free owned-car assessment can acknowledge a check and record the observed condition; it does not reset last-service mileage, heal parts or clear a condition warning. A due reminder adds no fee, wear multiplier, power penalty or race restriction. Existing component condition remains authoritative.

Mileage helps judge price and history rather than directly subtracting horsepower. A well-serviced high-mileage car can drive better than a neglected lower-mileage example. Authored asking prices and the existing condition/history valuation cover that difference; do not add another raw-mileage depreciation multiplier. Label history as documented, incomplete or inconsistent. A receipt beyond the claimed odometer or a decreasing reading is a data/report discrepancy, never something to silently rewrite as clean history. V1 has no odometer-tampering gameplay; invalid authored records fail validation.

## Six conditions and their effects

Use integer conditions from zero to 10,000: engine, cooling, transmission, brakes, tires and suspension. Display percentages or readable bands. Healthy installed-build **potential PI** remains unchanged; show present condition, readiness and live derating separately. Apply condition modifiers after healthy engine/build derivation without rewriting authored torque curves.

| Condition | Meaning and action |
| --- | --- |
| 6,000–10,000 | Normal operation; no service warning |
| Below 6,000 | Service-soon notice and quote |
| Below 3,000 | Urgent warning identifying the affected component |
| Below 2,000 | No new career race; an existing moving car may limp home above hard-failure level |
| 1,000 or below | Deterministic hard failure; stop attempting the race and offer free tow |

Normal performance returns at condition 4,500. Below it, interpolate the affected multiplier toward the component's configured minimum: engine torque 0.55, transmission drive 0.65, brake deceleration 0.35, tire grip 0.70, and suspension response 0.70 at zero condition. Transmission shift latency can rise toward 1.8 times its healthy value. Hard-failure behavior overrides these interpolation values. Cooling primarily affects the heat model.

At 1,001–1,999, limit continued driving to a 25 km/h limp-home target and disallow starting another career event. At a hard failure, disable positive throttle while retaining steering, coasting and available residual braking. Explain the actual cause: engine will not run, cooling protection cut, transmission cannot deliver drive, unsafe brakes, flat tire, or unsafe suspension. A severe tire fault must change grip/rolling behavior visibly enough to accompany its warning; detailed tire deformation is unnecessary.

An in-progress race with hard mechanical failure ends as DNF with no race reward. A condition warning or entry restriction does not itself erase already earned money. Recovery/reset does not repair components or remove failure flags. Free towing is always available.

## Wear, impacts and heat

Wear follows **actual driven meters**, not wall-clock time, menu visits, or an offline timer. Base loss per kilometer is engine 4 points, cooling 5, transmission 6, brakes 18, tires 30 and suspension 8. These intentionally slow rates let healthy starters complete many introductory races. Worn project offers can need attention immediately because their disclosed starting conditions are low.

For each component, accumulate `distance_m / 1000 × base_rate × stress` into a fractional pending loss. Subtract whole condition points and retain the remaining fraction. Persist `wear_fraction_by_component`, `odometer_fraction_m`, odometer, condition and heat together. Flooring each physics frame would erase small wear; clearing fractions on load would reward reloads. Teleports, tow and recovery add no driven distance and restore no health.

Engine/cooling/transmission stress scales mildly with installed healthy peak power relative to that same engine's stock peak, capped at 1.35. Normalizing this way and including supporting hardware in the upgrade bundles keeps an expensive well-built car from being automatically unreliable. Sustained near-redline operation, hard braking, driven-wheel slip and high heat add the documented component-specific multipliers, capped at 3 overall. Stationary revving has no distance wear in this deliberately limited model.

Real collision impulses can apply small fixed component losses. Group contacts in a physics step, distinguish ordinary suspension support from an actual impact, and enforce the two-second duplicate-contact cooldown. Minor impacts begin at an impulse/mass equivalent of 4 m/s; major impacts at 8 m/s. Append one incident receipt for the accepted impact episode. A single major impact cannot instantly destroy a fully healthy engine. No body deformation or complex damage simulation is required.

Use one bounded `thermal_load` gauge, not a fluid simulation. Its target is `clamp(2000 + 4000 × normalized_load + 1.8 × max(0, 6000 - cooling_condition), 0, 10000)`. With healthy cooling of at least 6,000, full-load target heat is only 6,000. At a still-roadworthy cooling condition of 4,000, full-load target is 9,600, so sustained load can reach protection. A minimum-repaired cooling system at 4,500 targets 8,700: it can warn but does not inevitably cut under unchanged conditions. Heat warns at 8,000, gradually derates torque, and cuts positive drive at 9,500; protection releases below 7,500. These are fictional tuning thresholds. This temporary protection cut is distinct from a component failure.

Persist heat and its protection latch: once cut, protection stays active until heat drops below 7,500 even if the player reloads. A temporary heat-only cut leaves a race's clock running; the player can cool the engine in live time and resume or abandon. A hard component failure still produces immediate DNF. No cooling is gained by Pause.

While the engine is off in the **live unpaused world, garage or roadside screen**, heat cools toward zero with the configured 40-second time constant. Normal garage parking is not Pause: its visible heat gauge continues updating with the engine off, even while service menus are open. Explicit Escape/Pause and offline time freeze that update. Loading, tow and reset provide no instant cooling. Offer a clear free “stop engine and cool down” action; heat alone never requires payment or creates a permanent recovery-income gate. A paid cooling repair/service can reset heat. Service transactions themselves are instant, with no workshop waiting timer.

Save wear and odometer in every payout, purchase, repair and other profile transaction, plus bounded free-roam checkpoints. A payout cannot be saved while discarding the wear incurred earning it. Pause/offline time creates neither wear nor cooling; no timed daily maintenance is involved.

## Quotes, repairs and a 1,000-credit example

The free minimum-roadworthy quote includes **every component below 2,000**, including the 1,001–1,999 range. Repair those components to 4,500 and leave all others unchanged. Full restoration is selectable per component and restores it to 10,000. Do not charge for an already perfect component or silently include unrelated work.

| Component | Minimum-roadworthy repair | Full component restoration |
| --- | ---: | ---: |
| Engine | 300 | 600 |
| Cooling | 150 | 300 |
| Transmission | 250 | 500 |
| Brakes | 150 | 300 |
| Tires | 200 | 400 |
| Suspension | 150 | 300 |
| All six, worst case | **1,200** | **2,400** |

These fixed gameplay prices do not scale with car MSRP. A quote states the target conditions, exact charge, resulting cash and remaining issues. Bind it to the component condition revision, incremented when condition changes or a paid replacement occurs; a changing heat gauge alone does not invalidate its fixed repair price. Revalidate actual transaction preconditions on confirmation. A repair preserves installed performance stages, path and history. Reset the serviced component's fractional wear accumulator; leave unrelated components unchanged. Commit the charge, condition change and receipt together, once.

Example: a buyer reserves **1,000 credits beyond the worn Courier's 3,500 asking price**. The optional 100 pre-purchase inspection leaves 900 of that reserve; the vehicle price is funded separately. Its actual cooling condition is 1,500 and brakes 1,800; other components exceed 2,000. Minimum repairs cost 150 + 150 = 300, restore cooling/brakes to 4,500, and leave **600**. Fully restoring just those two instead costs 600, leaving 300. A complete six-component restoration is a separately quoted 2,400 and is unaffordable on this budget. The history records what was actually done.

## Upgrades and repairs interact explicitly

A paid next-stage tire, brake or suspension purchase installs new hardware and restores **only its corresponding component** to 10,000. Exempt that replaced component from the installation-readiness check; other components must be at least 4,500 and heat below 8,000. Record the renewal in the upgrade receipt. Replaying the same purchase cannot renew it again. Later paid service can replace/service it while preserving the installed stage.

Engine and transmission upgrades preserve their core conditions and require engine, cooling and transmission at least 8,000 before installation; other components need 4,500. Weight reduction preserves every condition. Engine-package intercooling/forged hardware does not magically overhaul the existing coolant system or a neglected engine. Preview, reload, car switching, reset and fast travel never perform repairs.

## Free towing and workshop recovery income

Tow the car to the garage for zero credits, preserving condition, heat and mileage. The vehicle remains owned and can remain selected while parked, even if broken. Returning from a failed race does not require it to be driveable.

Offer `act_workshop_recovery` only when **all owned cars are mechanically unroadworthy and cash is below the cheapest minimum-roadworthy repair quote across them**. Diagnosis and quote are free. Heat-only restrictions have a free cooling path first. A mechanically roadworthy owned car or enough cash for that cheapest fix disables new recovery-job starts.

The activity reuses `route_dock_01` as an untimed-to-complete time trial with a temporary stock, healthy Finch loaner. A valid finish pays **300 credits**, no reputation and no first-clear bonus. Abandoning pays zero. The loaner is not owned, receives no wear, cannot be sold or upgraded, and adds no mileage or wear to parked owned cars. Return to the garage afterward. Use a unique activity run ID and idempotent settlement just like a race; eligibility is checked when starting, and the next start rechecks the updated wallet.

The worst minimum repair is 1,200; four valid jobs from zero pay exactly 1,200. Thus even every component failed and zero cash has a finite recovery path. Ordinary career races pay more, keeping this an emergency tool rather than the preferred economy loop.

## Resale and completion checks

Apply condition and historical-accident factors to the existing paid resale basis: `condition_bps = 4000 + floor(6000 × sum(six_conditions) / 60000)`. History factor is 10,000 with no recorded incident, 9,500 for a minor incident, or 8,500 for a major incident; preserve the worst recorded severity. Quote `floor(paid_basis × condition_bps × history_bps / 100000000)`. Service spend adds no investment. Both factors are at most one, keeping repair/sale cycles below the paid purchase-plus-upgrade basis. Repairing mechanical condition never removes the history factor.

Implementation must verify listing/inspection persistence; receipt chronology; distance/fraction/heat round trips; no free healing; each deterministic failure; repairs and replacement-stage renewal; the 1,000-credit example; the four-job zero-cash recovery bound; no duplicate job payout; and buyback preserving history/build/path without double-counting prior-owner investment. These are required future checks, not claims that game code has already passed them.
