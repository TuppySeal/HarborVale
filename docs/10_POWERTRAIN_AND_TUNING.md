# Powertrain and staged tuning

## Purpose and source of truth

This specification adds believable engine delivery to the GTA-like, semi-realistic driving target. A turbo build should reward staying in its useful RPM range; a positive-displacement supercharger should deliver a stronger immediate response; a naturally aspirated build should reward clean revs and momentum. These are different driving experiences, not three names for the same horsepower multiplier.

All numbers in `design-data/powertrains.json` are **fictional proposed game tuning**, not measured manufacturer specifications. They require driving and balance validation. The package contains eight stock engines and thirteen upgrade paths, with three paid stages per path. There is no implemented controller or tested acceleration result implied here.

`cars.json` references `powertrain_id` and no longer stores an editable `power_kw`. Net crank torque versus RPM is authoritative. Power, the comparison graph, and the PI input are derived from that same curve. `upgrades.json` owns other upgrade categories and shared price scaling. This document and `powertrains.json` own engine packages, their curves, individual payment steps, included hardware, and supporting-transmission requirements.

## Engine identities

Approximate peak kW below is calculated over the complete piecewise-linear torque interpolant, including possible peaks between knots. It is explanatory output, not a second tunable input.

| Car | Stock engine | Stock kW | Paths | Approx. stage 3 kW |
| --- | --- | ---: | --- | --- |
| Finch | 1.4 L naturally aspirated four | 65.2 | NA response / mild prototype turbo conversion | 86.4 / matched cooler 86.4 |
| Courier | 2.0 L naturally aspirated four | 84.7 | NA / turbo conversion / PD supercharger conversion | 113.0 / 162.5 / 154.5 |
| Lynx | 1.6 L factory-turbo four | 128.8 | Factory turbo development | 188.5 |
| Switch | 2.0 L factory-turbo four | 153.9 | Factory turbo development | 226.2 |
| Sable | 3.0 L naturally aspirated six | 200.1 | NA / turbo conversion / PD supercharger conversion | 258.7 / 367.6 / 336.9 |
| Quarry | 2.5 L factory-turbo four | 230.5 | Factory turbo development | 337.6 |
| Vela | 4.0 L high-revving naturally aspirated V8 | 298.5 | NA / turbo conversion | 369.2 / 510.9 |
| Apex | 4.0 L factory twin-turbo V8 | 400.1 | Factory twin-turbo development | 590.2 |

NA means naturally aspirated; PD means positive displacement. No centrifugal supercharger model is included in V1. Each powertrain has an `authored_peak_power_limit_kw` used to reject overpowered content during validation; it is not a runtime power clamp. The Finch's authored limit is 90 kW, preserving a substantial progression gap to the stock flagship. Later sports-car builds can become viable alternatives to buying the flagship.

Factory-turbo cars retain their existing aspiration and improve their complete package. An aftermarket conversion is supported only where explicitly listed. Do not attach a universal turbo item to every car or introduce arbitrary engine swaps.

## What stages buy

Real-world tuning stages are not universal engineering standards. In this game they are named, compatible bundles with disclosed hardware and a verified torque curve.

- **Stage 1:** a modest calibration/breathing package. Factory-turbo cars improve their existing setup. On NA cars, all available paths share the same stage 1 curve; choosing a conversion path reserves its stage 2 direction but does not secretly install boost at stage 1.
- **Stage 2 NA:** matched breathing, mild cams and fueling calibration. **Stage 2 conversion:** a low-boost turbo or PD supercharger kit, charge cooling, fueling and calibration. **Stage 2 factory turbo:** additional cooling, fueling and boost control.
- **Stage 3 NA:** head-flow, cam and valvetrain package. **Stage 3 boosted:** higher-output compressor hardware, forged internals, sufficient charge cooling and matched fueling/calibration. These parts are included in the package price; they are not extra undisclosed purchases.

Supporting transmission stages are separate disclosed prerequisites. NA engine stage 3 requires transmission stage 1. Boosted stage 2 requires transmission stage 1; boosted stage 3 requires transmission stage 2. Earlier engine stages remain required too. Transmission packages represent suitable clutch/gearbox support plus shorter torque-interruption time. `11_USED_CARS_MAINTENANCE_AND_BREAKDOWNS.md` adds deterministic gearbox condition, warnings and service; a healthy supported build does not receive arbitrary failure rolls.

### Purchase contract

Every category advances sequentially: stock → stage 1 → stage 2 → stage 3. Engine stages are separately paid steps. Reputation thresholds are 0, 150 and 500. A high-reputation player cannot buy stage 3 on a stock car, bypass stage 2, or unlock a package merely by previewing it.

| Engine path family | Stage 1 reference payment | Stage 2 reference payment | Stage 3 reference payment |
| --- | ---: | ---: | ---: |
| Naturally aspirated | 550 | 800 | 1,600 |
| Factory turbo | 550 | 1,500 | 3,000 |
| Aftermarket turbo or PD conversion | 550 | 2,500 | 4,500 |
| Apex factory twin turbo | 550 | 1,750 | 3,500 |

Scale and round **each payment** using the shared car-price multiplier in `upgrades.json`. Do not subtract prior engine investment from the next step price; the prior payment bought the prior package. Engine resale investment is the sum of actual committed engine payments, counted once. For example, the Courier conversion payments round to 450 + 2,000 + 3,550 = 6,000 credits, before separately purchased transmission support. Its NA path costs 450 + 650 + 1,250 = 2,350.

At stage 1 the player chooses an engine path. Lock that choice for this physical vehicle in V1. Before confirmation, show future stages, total expected engine investment, supporting purchases and the locked-choice rule. Another stock vehicle can choose another path. Selling and reacquiring the same vehicle preserves its identity, paid build and path lock; the purchase price includes the existing hardware, and previous-ownership investment is not credited again. There is no in-place path switching, downgrade, refundable part inventory or unpriced swapping.

One transaction validates current path/stage, the immediately preceding paid stage, transmission support, reputation and funds; then commits the new stage, configuration selection, investment and wallet together. Reject stale requests and duplicate clicks. Loading a save never reapplies stage bonuses to an already modified curve.

## Torque sampling and power

Each configuration explicitly stores its idle, redline, soft-limiter, automatic shift and launch-target RPM, plus `steady_wot_torque_curve_nm`. WOT means wide-open throttle. Curve samples are `[rpm, net_crank_torque_nm]`, ordered by RPM. Interpolate torque linearly between adjacent samples. Bound sampling to the supported range and use the limiter to cut drive torque at redline; do not extrapolate increasing torque indefinitely.

```text
power_kw(rpm) = torque_nm(rpm) * rpm / 9549.2966
```

To find peak power, evaluate every knot and every valid interior stationary point. For a segment `T(r) = a*r + b`, also evaluate `r = -b/(2*a)` when it lies inside that segment and `a < 0`. Taking only the largest knot power can miss the actual maximum. The garage and PI calculation use the derived steady-state peak. Launch, lag, traction, gearing and shift interruptions determine actual acceleration.

No engine upgrade applies an additional flat horsepower multiplier. No vehicle controller reads an obsolete `cars.power_kw`. All effects originate in the selected complete configuration.

## Turbo response without double-counting boost

Turbo and twin-turbo configurations additionally store an off-boost curve, `boost_threshold_rpm`, `full_boost_rpm`, and rise/fall response times. The off-boost and steady curves use the same RPM knots. Their values are identical at and below an exact threshold knot. Above it, the steady curve contains the authored boost-delivery shape already.

The threshold is an RPM/load operating-range concept; lag is a delay after a demand change. They must remain separate. Garrett discusses response and sizing tradeoffs in its [turbo lag guide](https://www.garrettmotion.com/it/knowledge-center-category/turbo-replacement/turbo-lag-causes-effects-and-solutions/). Our simplified implementation is a game model, not a compressor-map simulation.

Maintain per-vehicle turbo availability `b` between zero and one. Its target is zero below the threshold, during a shift torque cut, or without positive engaged drive demand. Above the threshold under load, use normalized throttle demand as the target. A simple grounded/engaged-drive load proxy is sufficient initially; free revving in neutral must not build useful launch boost. Use rise or fall time according to whether availability is increasing or decreasing:

```text
alpha = 1 - exp(-physics_dt / tau)
b = lerp(b, target_availability, alpha)
net_drive_torque = throttle * (offboost_torque + b * (steady_torque - offboost_torque))
```

The authored `spool_rise_seconds` and `spool_fall_seconds` are exponential time constants: approximately 63% response after one tau and 95% after three. They are not promised times to reach exactly 100%. `full_boost_rpm` describes the curve's delivery region for content review/UI. Never multiply by another RPM boost ramp, a separate turbo torque bonus or pressure-to-power multiplier. Those would count the same boost twice.

Larger stage 3 turbo packages generally trade slower response and a higher useful RPM range for more high-RPM output. Their lower-RPM torque need not improve at every point. Reset turbo availability on car spawn, replacement and recovery; it is transient vehicle state, not saved career wealth.

## Positive-displacement supercharger response

PD configurations use their net WOT curve directly, with ordinary throttle smoothing and no exhaust-spool delay. Their earlier torque delivery differs from a turbo conversion even when peak outputs are similar. Eaton's [TVS overview](https://www.eaton.com/us/en-us/products/engine-solutions/superchargers/tvs-overview.html) discusses low-speed airflow-oriented designs; Vortech's [PD/centrifugal comparison](https://vortechsuperchargers.com/pages/centrifugal-vs-roots-type-superchargers) illustrates why blower types and drive losses must not be treated identically.

Our SC curves already express torque remaining at the crank after compressor drive losses. Do not subtract those losses again. Add no turbo availability multiplier. In the Courier's stage 2 comparison, the SC has stronger early torque while the turbo reaches a slightly higher peak; the choice changes corner exits rather than just a shop label.

## RPM, gears and wheel force

Each powertrain defines actual forward ratios, reverse ratio, final drive, effective driven-wheel radius and drivetrain efficiency. Base shift latency remains the single stock `cars.json:shift_seconds` value, modified by the installed transmission stage. Gear ratios are unchanged by transmission stages in V1.

```text
engaged_rpm = abs(driven_wheel_angular_speed) * gear_ratio * final_drive * 60 / (2*pi)
wheel_torque = net_drive_torque * gear_ratio * final_drive * drivetrain_efficiency
total_drive_force = wheel_torque / wheel_radius
```

Distribute the total over driven grounded wheels once, then apply the existing tire-grip limit. Do not apply a full engine's torque to each wheel. Reverse uses its positive ratio magnitude with an explicit negative drive direction, while engine RPM remains nonnegative. Under ordinary engaged driving, RPM follows wheel speed; at launch, a bounded automatic-clutch assist lets the engine approach `launch_target_rpm` while coupling smoothly. It must not teleport the car or grant an extra launch impulse.

Automatic shifting uses the selected configuration's targets, checks that a downshift will not exceed redline, and interrupts delivered drive torque for the real shift duration. Apply engine braking and drag separately. Engine sound follows the resulting RPM/load; raising audio pitch alone does not implement an upgrade. No fixed top-speed clamp or independent acceleration boost replaces this force path.

## Validation and implementation gate

Validate unique IDs, references, finite values, ascending RPM, complete idle/redline coverage, compatible off-boost/steady grids, `0 < threshold < full_boost < redline`, positive response constants, and off-boost torque no greater than steady torque. Confirm equality throughout the range at/below the threshold. Recompute peak power against each car's authored limit. Conversion paths must share their stage 1 NA curve exactly, while their IDs remain unique.

Test sequential purchase rejection, supporting-stage rejection, exact per-step prices, path locking, rollback on save failure, and resale investment. Physics checks must show wheel force changing with the curve, RPM and ratios; lower gear versus higher gear; real shift torque interruption; a turbo load step; and immediate SC response without a second loss deduction. Compare NA/turbo/SC Courier builds on the same corner-exit and straight tests. The player and every rival use these same equations and transient-state rules.

Deliver the slice with the Finch, Courier and Lynx powertrain behavior before expanding content. The later-authorized maintenance specification includes component wear/damage, a small heat-load model, paid repairs and breakdowns. Apply its bounded condition effects after the healthy installed configuration; preserve the source curves and label PI/dyno potential as healthy output. Engine/transmission upgrades require a serviceable core and do not perform a free overhaul. Actual paid tire/brake/suspension replacements renew only their own listed component.

Full fluid/combustion thermodynamics, fuel economy, knock/detonation, compressor surge and workshop waiting timers remain outside V1.

Prototype amendment D046 adds a fourteenth path, mild Finch turbo, within its 90 kW authored limit. Runtime currently implements common Stage1 and turbo Stage2/basic or street cooler; authored later stages remain design data. Separate Apex testing data in sports_test.json is outside the eight-car career roster and its pricing/progression.
