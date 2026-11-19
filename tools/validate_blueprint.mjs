// Checks the written design package. This does not test a game implementation.
// Run from any directory: node tools/validate_blueprint.mjs [--write-report]
import assert from 'node:assert/strict';
import { readFileSync, readdirSync, existsSync, writeFileSync } from 'node:fs';
import { resolve, dirname, extname, relative } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = fileURLToPath(new URL('../', import.meta.url));
const read = path => readFileSync(resolve(root, path), 'utf8');
const json = path => JSON.parse(read(path));
const groups = [];
function check(label, fn) {
  fn();
  groups.push(label);
}
function unique(items) { return new Set(items).size === items.length; }
const cars = json('design-data/cars.json').cars;
const upgrades = json('design-data/upgrades.json');
const events = json('design-data/events.json').events;
const economy = json('design-data/economy.json');
const scopes = json('design-data/release_scopes.json');
const powertrainData = json('design-data/powertrains.json');
const powertrains = powertrainData.powertrains;
const maintenance = json('design-data/maintenance.json');
const usedData = json('design-data/used_car_listings.json');
const usedListings = usedData.listings;
const ledger = json('project/TASKS.json');
const roadmap = read('docs/05_BUILD_ROADMAP.md');
const byCar = new Map(cars.map(x => [x.id, x]));
const byEvent = new Map(events.map(x => [x.id, x]));
const byPowertrain = new Map(powertrains.map(x => [x.id, x]));
const components = maintenance.components;
const componentIds = components.map(c => c.id);
const percent = (value, fraction) => Math.floor(value * Math.round(fraction * 100) / 100);

function torqueAt(curve, rpm) {
  if (rpm <= curve[0][0]) return curve[0][1];
  for (let i = 1; i < curve.length; i++) if (rpm <= curve[i][0]) {
    const [r0, t0] = curve[i - 1], [r1, t1] = curve[i];
    return t0 + (t1 - t0) * (rpm - r0) / (r1 - r0);
  }
  return curve.at(-1)[1];
}
function peakPower(configuration) {
  const curve = configuration.steady_wot_torque_curve_nm;
  const candidates = [configuration.idle_rpm, configuration.redline_rpm];
  for (const [rpm] of curve) if (rpm >= configuration.idle_rpm && rpm <= configuration.redline_rpm) candidates.push(rpm);
  for (let i = 1; i < curve.length; i++) {
    const [r0, t0] = curve[i - 1], [r1, t1] = curve[i];
    const a = (t1 - t0) / (r1 - r0), b = t0 - a * r0;
    if (a < 0) {
      const vertex = -b / (2 * a);
      if (vertex > Math.max(r0, configuration.idle_rpm) && vertex < Math.min(r1, configuration.redline_rpm)) candidates.push(vertex);
    }
  }
  const values = candidates.map(rpm => ({ rpm, power_kw: torqueAt(curve, rpm) * rpm / 9549.2966 }));
  return values.reduce((best, value) => value.power_kw > best.power_kw ? value : best);
}
function enginePath(car, pathId) {
  const pt = byPowertrain.get(car.powertrain_id);
  const path = pathId ? pt.paths.find(p => p.id === pathId) : pt.paths[0];
  assert(path, 'Unknown compatible engine path');
  return path;
}
const allConfigurations = powertrains.flatMap(pt => [pt.stock, ...pt.paths.flatMap(p => p.stages.map(s => s.configuration))]);

check('Reference JSON parses and content IDs/counts agree', () => {
  assert.equal(cars.length, 8);
  assert.equal(events.length, 16);
  assert.equal(upgrades.categories.length, 6);
  assert.equal(powertrains.length, 8);
  assert.equal(powertrains.reduce((sum, pt) => sum + pt.paths.length, 0), 14); // D046 adds compatible mild Finch turbo path.
  assert.equal(allConfigurations.length, 50); // Three authored Finch turbo stages; runtime ships through Stage 2.
  assert(unique(powertrains.map(pt => pt.id)));
  assert(unique(allConfigurations.map(c => c.id)));
  assert(unique(cars.map(x => x.id)));
  assert(unique(events.map(x => x.id)));
  assert(unique(upgrades.categories.map(x => x.id)));
  assert.equal(cars.filter(x => x.included_in_slice).length, 3);
  assert.equal(events.filter(x => x.included_in_slice).length, 4);
});

check('Starter choices are affordable and all stock physics values are positive', () => {
  for (const id of economy.initial_starter_ids) {
    assert(byCar.has(id));
    assert(byCar.get(id).purchase_price <= economy.starting_wallet);
    assert.equal(byCar.get(id).reputation_required, 0);
  }
  for (const car of cars) {
    for (const key of ['mass_kg', 'grip', 'brake_deceleration', 'chassis', 'target_top_speed_kph', 'shift_seconds']) {
      assert(Number.isFinite(car[key]) && car[key] > 0, `${car.id}.${key}`);
    }
    assert(Number.isSafeInteger(car.purchase_price) && car.purchase_price > 0);
    assert(['FWD', 'RWD', 'AWD'].includes(car.drivetrain));
    assert(!Object.hasOwn(car, 'power_kw'), 'Peak power must be derived from torque, not duplicated');
    assert.equal(byPowertrain.get(car.powertrain_id)?.car_id, car.id);
  }
});

check('Event modes, district counts, AI fields and rival/finale references agree', () => {
  assert.deepEqual(['dockside', 'midtown', 'ridgeway'].map(id => events.filter(e => e.district_id === id).length), [6, 5, 5]);
  assert.equal(events.filter(e => e.rival_id).length, 6);
  assert.equal(events.filter(e => e.is_finale).length, 1);
  assert.equal(events.find(e => e.is_finale).id, 'evt_ridge_05');
  for (const e of events) {
    assert(['sprint', 'circuit', 'time_trial'].includes(e.type));
    assert.equal(e.ai_opponents, e.type === 'time_trial' ? 0 : e.type === 'sprint' && e.rival_id ? 1 : 3);
    assert.deepEqual(e.prerequisite_ids, []);
    for (const key of ['reputation_required', 'base_reward', 'first_clear_bonus', 'max_reputation']) {
      assert(Number.isSafeInteger(e[key]) && e[key] >= 0, `${e.id}.${key}`);
    }
  }
});

check('Slice and V1 release manifests reference the right definitions', () => {
  assert.equal(scopes.default_build_scope, 'slice');
  const slice = scopes.scopes.find(s => s.id === 'slice');
  const v1 = scopes.scopes.find(s => s.id === 'v1');
  assert.deepEqual(slice.car_ids, cars.filter(c => c.included_in_slice).map(c => c.id));
  assert.deepEqual(slice.event_ids, events.filter(e => e.included_in_slice).map(e => e.id));
  assert.deepEqual(slice.upgrade_stages, [1, 2]);
  assert.deepEqual(v1.car_ids, cars.map(c => c.id));
  assert.deepEqual(v1.event_ids, events.map(e => e.id));
  assert.deepEqual(v1.upgrade_stages, [1, 2, 3]);
  for (const scope of scopes.scopes) {
    assert(unique(scope.car_ids) && unique(scope.event_ids) && unique(scope.rival_ids));
    for (const id of scope.car_ids) assert(byCar.has(id));
    for (const id of scope.event_ids) {
      const event = byEvent.get(id);
      assert(event && scope.district_ids.includes(event.district_id));
      if (event.rival_id) assert(scope.rival_ids.includes(event.rival_id));
    }
  }
});

function roundedReferencePrice(car, referencePrice) {
  const scale = upgrades.price_scaling;
  const multiplier = Math.max(scale.minimum_multiplier, Math.min(scale.maximum_multiplier,
    (car.purchase_price / scale.reference_car_price) ** scale.power));
  return Math.floor(referencePrice * multiplier / scale.round_to_nearest + 0.5) * scale.round_to_nearest;
}
function price(car, category, stage, pathId) {
  if (stage === 0) return 0;
  if (category.id === 'engine') return enginePath(car, pathId).stages.slice(0, stage)
    .reduce((sum, step) => sum + roundedReferencePrice(car, step.paid_reference_price), 0);
  return roundedReferencePrice(car, category.stages[stage - 1].reference_total_price);
}
function effective(car, stage, pathId) {
  const pt = byPowertrain.get(car.powertrain_id);
  const configuration = stage ? enginePath(car, pathId).stages[stage - 1].configuration : pt.stock;
  const values = { ...car, power_kw: peakPower(configuration).power_kw };
  if (stage) for (const category of upgrades.categories.filter(c => c.id !== 'engine')) {
    const effect = category.stages[stage - 1].effect;
    values[category.attribute] = category.operation === 'multiply'
      ? car[category.attribute] * effect : car[category.attribute] + effect;
  }
  return values;
}
function pi(car) {
  const f = economy.pi_formula;
  const values = {
    power_to_weight_kw_per_tonne: car.power_kw * 1000 / car.mass_kg,
    grip: car.grip, chassis: car.chassis,
    brake_deceleration: car.brake_deceleration, shift_seconds: car.shift_seconds
  };
  let raw = f.constant;
  for (const [key, value] of Object.entries(values)) raw += f[key].weight * (value - f[key].baseline);
  return Math.max(f.minimum, Math.min(f.maximum, Math.floor(raw + 0.5)));
}

check('Torque interpolation, power units, interior peak search and wheel-force examples agree', () => {
  assert.equal(torqueAt([[2000, 100], [4000, 200]], 3000), 150);
  assert(Math.abs(200 * 3000 / 9549.2966 - 62.831853) < 0.000001);
  const interior = peakPower({ idle_rpm: 1000, redline_rpm: 3000, steady_wot_torque_curve_nm: [[1000, 200], [3000, 100]] });
  assert.equal(interior.rpm, 2500);
  assert(interior.power_kw > 3000 * 100 / 9549.2966);
  assert.equal(200 * 2 * 4 * 0.9 / 0.3, 4800);
});

function validateCurve(curve, configuration) {
  assert(Array.isArray(curve) && curve.length >= 2, configuration.id);
  assert.equal(curve[0][0], configuration.idle_rpm);
  assert.equal(curve.at(-1)[0], configuration.redline_rpm);
  for (let i = 0; i < curve.length; i++) {
    assert(curve[i].length === 2 && curve[i].every(Number.isFinite));
    assert(curve[i][0] > 0 && curve[i][1] >= 0);
    if (i) assert(curve[i][0] > curve[i - 1][0], configuration.id + ' RPM order');
  }
}
check('All authored powertrains, compatible packages, supporting stages and gearbox values are valid', () => {
  const aspirations = ['naturally_aspirated', 'turbocharged', 'twin_turbocharged', 'positive_displacement_supercharged'];
  assert.equal(powertrainData.curve_interpolation, 'piecewise_linear_torque_over_rpm');
  assert.equal(powertrainData.engine_stage_purchase_rule, 'next_stage_only_individually_paid_step');
  for (const pt of powertrains) {
    assert.equal(byCar.get(pt.car_id)?.powertrain_id, pt.id);
    assert(unique(pt.paths.map(p => p.id)));
    const g = pt.gearbox;
    assert(g.forward_ratios.length >= 4 && g.forward_ratios.every(r => Number.isFinite(r) && r > 0));
    for (const key of ['reverse_ratio', 'final_drive', 'wheel_radius_m', 'drivetrain_efficiency']) assert(g[key] > 0 && Number.isFinite(g[key]));
    assert(g.drivetrain_efficiency <= 1);
    for (const path of pt.paths) {
      assert.deepEqual(path.stages.map(s => s.stage), [1, 2, 3]);
      for (const stage of path.stages) {
        assert.equal(stage.min_reputation, upgrades.stage_reputation_required[stage.stage]);
        assert(Number.isInteger(stage.required_transmission_stage) && stage.required_transmission_stage >= 0 && stage.required_transmission_stage <= 3);
        assert(Number.isSafeInteger(stage.paid_reference_price) && stage.paid_reference_price > 0);
        assert(Array.isArray(stage.included_hardware) && stage.included_hardware.length > 0);
      }
    }
    for (const c of [pt.stock, ...pt.paths.flatMap(p => p.stages.map(s => s.configuration))]) {
      assert(aspirations.includes(c.aspiration));
      assert(c.idle_rpm > 0 && c.redline_rpm > c.idle_rpm);
      assert(c.soft_limiter_rpm <= c.redline_rpm && c.soft_limiter_rpm > c.idle_rpm);
      assert(c.upshift_rpm <= c.soft_limiter_rpm && c.downshift_rpm < c.upshift_rpm && c.downshift_rpm > c.idle_rpm);
      assert(c.launch_target_rpm >= c.idle_rpm && c.launch_target_rpm < c.upshift_rpm);
      validateCurve(c.steady_wot_torque_curve_nm, c);
      assert(peakPower(c).power_kw <= pt.authored_peak_power_limit_kw, c.id + ' exceeds authored package limit');
    }
  }
});

check('Turbo curves respect threshold, bounded off-boost output and positive spool time constants', () => {
  for (const c of allConfigurations.filter(c => ['turbocharged', 'twin_turbocharged'].includes(c.aspiration))) {
    validateCurve(c.off_boost_torque_curve_nm, c);
    assert(c.boost_threshold_rpm > c.idle_rpm && c.full_boost_rpm > c.boost_threshold_rpm && c.full_boost_rpm < c.redline_rpm);
    assert(c.spool_rise_seconds > 0 && c.spool_fall_seconds > 0);
    const rpms = [...new Set([...c.steady_wot_torque_curve_nm.map(x => x[0]), ...c.off_boost_torque_curve_nm.map(x => x[0]), c.boost_threshold_rpm])];
    for (const rpm of rpms) {
      const steady = torqueAt(c.steady_wot_torque_curve_nm, rpm), off = torqueAt(c.off_boost_torque_curve_nm, rpm);
      assert(off <= steady + 1e-8, c.id + ' invalid off-boost torque');
      if (rpm <= c.boost_threshold_rpm) assert(Math.abs(steady - off) < 1e-8, c.id + ' boost below authored threshold');
    }
  }
});

check('Conversion choices retain the same paid naturally aspirated first-stage preparation', () => {
  for (const carId of ['car_finch', 'car_courier', 'car_sable', 'car_vela']) {
    const pt = byPowertrain.get(byCar.get(carId).powertrain_id);
    const first = pt.paths[0].stages[0];
    for (const path of pt.paths) {
      assert.equal(path.stages[0].configuration.aspiration, 'naturally_aspirated');
      assert.equal(path.stages[0].paid_reference_price, first.paid_reference_price);
      assert.deepEqual(path.stages[0].configuration.steady_wot_torque_curve_nm, first.configuration.steady_wot_torque_curve_nm);
    }
  }
  for (const carId of ['car_courier', 'car_sable']) {
    const pt = byPowertrain.get(byCar.get(carId).powertrain_id);
    assert.equal(pt.paths.length, 3);
    for (const path of pt.paths.filter(p => p.stages[1].configuration.aspiration !== 'naturally_aspirated')) {
      assert.equal(path.stages[1].required_transmission_stage, 1);
      assert.equal(path.stages[2].required_transmission_stage, 2);
    }
  }
});

const stepCostExamples = {};
check('Independent sequential-fee and required-transmission budget examples match', () => {
  const finch = byCar.get('car_finch'), courier = byCar.get('car_courier');
  const engine = upgrades.categories.find(c => c.id === 'engine');
  const tires = upgrades.categories.find(c => c.id === 'tires');
  const transmission = upgrades.categories.find(c => c.id === 'transmission');
  const steps = (car, category, pathId) => [1, 2, 3].map(s => price(car, category, s, pathId) - price(car, category, s - 1, pathId));
  assert.deepEqual(steps(finch, engine), [400, 600, 1200]);
  assert.deepEqual(steps(finch, tires), [350, 350, 650]);
  assert.deepEqual(steps(courier, engine, 'path_courier_turbo'), [450, 2000, 3550]);
  assert.equal(price(courier, transmission, 1), 300);
  assert.equal(price(courier, transmission, 2), 650);
  assert.equal(price(courier, engine, 3, 'path_courier_turbo') + price(courier, transmission, 2), 6650);
  stepCostExamples.finch_engine_steps = [400, 600, 1200];
  stepCostExamples.courier_turbo_engine_steps = [450, 2000, 3550];
  stepCostExamples.courier_turbo_stage_3_engine_and_required_transmission = 6650;
});

const allPathExamples = [];
check('All thirteen path performance and complete-build cost examples agree', () => {
  const expected = [
    ['car_finch', 'path_finch_na', 149, 270, 7850],
    ['car_courier', 'path_courier_na', 163, 294, 8200],
    ['car_courier', 'path_courier_turbo', 163, 393, 11850],
    ['car_courier', 'path_courier_supercharger', 163, 377, 11850],
    ['car_lynx', 'path_lynx_turbo', 285, 493, 15250],
    ['car_switch', 'path_switch_turbo', 296, 507, 19700],
    ['car_sable', 'path_sable_na', 386, 584, 20800],
    ['car_sable', 'path_sable_turbo', 386, 779, 30000],
    ['car_sable', 'path_sable_supercharger', 386, 724, 30000],
    ['car_quarry', 'path_quarry_turbo', 426, 704, 30600],
    ['car_vela', 'path_vela_na', 578, 819, 31200],
    ['car_vela', 'path_vela_turbo', 578, 999, 45000],
    ['car_apex', 'path_apex_twin_turbo', 728, 999, 39750]
  ];
  for (const [carId, pathId, stockPi, maxPi, totalCost] of expected) {
    const car = byCar.get(carId);
    assert.equal(pi(effective(car, 0)), stockPi);
    assert.equal(pi(effective(car, 3, pathId)), maxPi);
    const computedCost = upgrades.categories.reduce((sum, category) => sum + price(car, category, 3, pathId), 0);
    assert.equal(computedCost, totalCost);
    allPathExamples.push({ car_id: carId, path_id: pathId, stock_pi: stockPi, all_stage_3_pi: maxPi, total_upgrade_cost: totalCost });
  }
});

function minimumRepair(condition) {
  return components.reduce((sum, c) => sum + (condition[c.id] < maintenance.roadworthy_minimum ? c.minimum_roadworthy_repair_price : 0), 0);
}
function repairToMinimum(condition) {
  return Object.fromEntries(componentIds.map(id => [id, condition[id] < maintenance.roadworthy_minimum ? maintenance.minimum_service_restores_to : condition[id]]));
}
function conditionFactor(condition) {
  const f = maintenance.resale_factors;
  return f.condition_floor_bps + Math.floor(f.condition_span_bps * componentIds.reduce((sum, id) => sum + condition[id], 0) / (componentIds.length * maintenance.condition_scale));
}
function conditionResale(purchasePaid, investment, condition, incidentSeverity) {
  const basis = percent(purchasePaid, economy.resale.base_price_paid_fraction) + percent(investment, economy.resale.installed_investment_fraction);
  const history = maintenance.resale_factors.history_factor_by_worst_recorded_accident[incidentSeverity];
  return Math.floor(basis * conditionFactor(condition) * history / maintenance.condition_scale ** 2);
}
check('Used offers, persistent identities, inspection fields and repair histories are consistent', () => {
  assert.equal(usedListings.length, 10);
  assert.equal(usedListings.filter(l => l.included_in_slice).length, 4);
  assert.equal(new Set(usedListings.map(l => l.car_id)).size, 8);
  assert(unique(usedListings.map(l => l.id)) && unique(usedListings.map(l => l.vehicle_id)));
  assert.equal(usedData.mechanic_inspection_allowed_all_listings, true);
  const historyIds = [];
  for (const listing of usedListings) {
    assert(byCar.has(listing.car_id));
    assert(Number.isSafeInteger(listing.asking_price) && listing.asking_price > 0);
    assert(Number.isSafeInteger(listing.odometer_m) && listing.odometer_m >= 0);
    assert(listing.owner_count === null || Number.isSafeInteger(listing.owner_count) && listing.owner_count > 0);
    assert.equal(typeof listing.inspection_report_included, 'boolean');
    assert(!Object.hasOwn(listing, 'inspection_report_available'));
    assert.deepEqual(Object.keys(listing.condition).sort(), [...componentIds].sort());
    for (const id of componentIds) {
      assert(Number.isInteger(listing.condition[id]) && listing.condition[id] >= 0 && listing.condition[id] <= 10000);
      assert(listing.wear_fraction_by_component[id] >= 0 && listing.wear_fraction_by_component[id] < 1);
    }
    assert(listing.known_condition_components.every(id => componentIds.includes(id)));
    assert(Object.hasOwn(maintenance.resale_factors.history_factor_by_worst_recorded_accident, listing.worst_recorded_accident_severity));
    let previousOdometer = 0, previousSequence = 0;
    for (const record of listing.history_records) {
      assert(record.sequence > previousSequence && record.odometer_m >= previousOdometer && record.odometer_m <= listing.odometer_m);
      assert(Number.isSafeInteger(record.cost_credits) && record.cost_credits >= 0);
      previousSequence = record.sequence; previousOdometer = record.odometer_m;
      historyIds.push(record.record_id);
    }
    assert.equal(listing.installed_build.engine_stage, 0);
    assert.equal(listing.installed_build.engine_path_id, null);
  }
  assert(unique(historyIds));
  for (const id of usedData.guaranteed_initial_starter_listing_ids) {
    const l = usedListings.find(l => l.id === id);
    assert(l.eligible_as_initial_starter && l.inspection_report_included && l.included_in_slice);
    assert.equal(l.asking_price, byCar.get(l.car_id).purchase_price);
    assert(Object.values(l.condition).every(value => value === 10000));
    assert.equal(l.reputation_required, 0);
  }
  for (const scope of scopes.scopes) {
    const expected = scope.id === 'slice' ? usedListings.filter(l => l.included_in_slice) : usedListings;
    assert.deepEqual(scope.used_listing_ids, expected.map(l => l.id));
    assert.deepEqual(scope.recovery_activity_ids, ['act_workshop_recovery']);
    assert(expected.every(l => scope.car_ids.includes(l.car_id)));
  }
});

check('Maintenance thresholds, paid replacement mapping and noninvestment expenses agree', () => {
  assert.deepEqual(componentIds, ['engine', 'cooling', 'transmission', 'brakes', 'tires', 'suspension']);
  assert.equal(maintenance.condition_scale, 10000);
  assert(maintenance.hard_failure_at_or_below < maintenance.roadworthy_minimum);
  assert(maintenance.minimum_service_restores_to >= maintenance.roadworthy_minimum);
  assert.equal(maintenance.inspection_price, 100);
  assert.equal(maintenance.owned_roadworthy_quote_price, 0);
  for (const id of ['tires', 'brakes', 'suspension']) assert.deepEqual(maintenance.upgrade_renewal_by_category[id], [id]);
  for (const id of ['engine', 'transmission', 'weight']) assert.deepEqual(maintenance.upgrade_renewal_by_category[id], []);
  assert.equal(maintenance.upgrade_readiness.renewal_condition_on_successful_paid_stage, 10000);
  assert.equal(maintenance.upgrade_readiness.repeat_same_stage_cannot_renew, true);
  assert.equal(economy.maintenance.service_spending_counts_as_upgrade_investment, false);
  assert.equal(maintenance.resale_factors.service_or_inspection_spend_adds_resale_investment, false);
  assert.equal(maintenance.roadside.tow_price, 0);
  assert.equal(maintenance.roadside.restores_condition, false);
  assert.equal(maintenance.roadside.distance_wear_during_tow, false);
});

check('All 64 unsafe-component subsets have a finite repair and at most four recovery jobs', () => {
  const job = maintenance.workshop_recovery_activity;
  assert.equal(job.id, economy.emergency_income.id);
  assert.equal(job.reward_credits, 300);
  assert.equal(job.reward_credits, economy.emergency_income.payout_on_valid_finish);
  assert.equal(job.reputation_reward, 0);
  assert.equal(job.first_clear_bonus, 0);
  assert.equal(job.entry_fee, 0);
  assert.equal(job.loaner_wear, false);
  assert.equal(job.owned_vehicle_wear, false);
  assert.equal(job.upgrades_or_sale_allowed, false);
  assert.equal(economy.emergency_income.career_event, false);
  for (let mask = 0; mask < 64; mask++) {
    const condition = Object.fromEntries(componentIds.map((id, index) => [id, mask & 1 << index ? 1999 : 6000]));
    const quote = minimumRepair(condition);
    assert.equal(quote > 0, mask !== 0);
    assert(quote <= job.maximum_minimum_repair_quote);
    assert(Math.ceil(quote / job.reward_credits) <= job.maximum_valid_jobs_from_zero_cash);
    assert(Object.values(repairToMinimum(condition)).every(value => value >= maintenance.roadworthy_minimum));
  }
  assert.equal(minimumRepair(Object.fromEntries(componentIds.map(id => [id, 0]))), 1200);
  assert.equal(components.reduce((sum, c) => sum + c.full_restore_price, 0), 2400);
  assert.equal(job.maximum_valid_jobs_from_zero_cash, 4);
});

const usedCarExamples = {};
check('Used-car inspection/repair budgets, condition/history resale and buybacks match examples', () => {
  const worn = usedListings.find(l => l.id === 'listing_courier_002');
  assert.equal(minimumRepair(worn.condition), 300);
  assert.equal(conditionFactor(worn.condition), 6550);
  assert.equal(conditionFactor(repairToMinimum(worn.condition)), 7120);
  assert.equal(conditionResale(worn.asking_price, 0, worn.condition, 'minor'), 1524);
  assert.equal(conditionResale(worn.asking_price, 0, repairToMinimum(worn.condition), 'minor'), 1657);
  assert.equal(conditionResale(worn.asking_price, 0, Object.fromEntries(componentIds.map(id => [id, 10000])), 'minor'), 2327);
  assert.equal(1000 - maintenance.inspection_price - minimumRepair(worn.condition), 600);
  for (const l of usedListings) {
    const sale = conditionResale(l.asking_price, 0, l.condition, l.worst_recorded_accident_severity);
    const buyback = Math.max(l.asking_price, sale + economy.used_vehicle_buyback.margin);
    const resale = conditionResale(buyback, 0, l.condition, l.worst_recorded_accident_severity);
    assert(sale < l.asking_price && resale < buyback);
    assert(sale - buyback + resale < sale, 'Buyback must lose cash');
  }
  usedCarExamples.worn_courier = { purchase: 3500, minimum_repair: 300, resale_before: 1524, resale_after_minimum_repair: 1657, reserve_after_100_inspection_and_300_repair: 600 };
});

check('Fractional wear persists mathematically and useful heat thresholds are reachable', () => {
  const rate = components.find(c => c.id === 'engine').wear_points_per_km;
  let fraction = 125 / 1000 * rate;
  assert.equal(Math.floor(fraction), 0);
  assert.equal(fraction, 0.5);
  fraction += 125 / 1000 * rate;
  assert.equal(Math.floor(fraction), 1);
  assert.equal(maintenance.wear_rules.offline_wear, false);
  assert.equal(maintenance.wear_rules.paused_wear, false);
  const t = maintenance.thermal_model;
  const target = (load, cooling) => Math.max(0, Math.min(t.scale, t.running_target_base + t.running_load_gain * load + t.cooling_deficit_gain * Math.max(0, t.cooling_deficit_reference - cooling)));
  assert.equal(target(1, 6000), 6000);
  assert(target(1, 6000) < t.warning_at);
  assert.equal(target(1, 4000), 9600);
  assert(target(1, 4000) > t.protection_cut_at);
  assert.equal(target(1, 4500), 8700);
  assert(target(1, 4500) < t.protection_cut_at);
  assert(t.protection_releases_below < t.warning_at && t.warning_at < t.protection_cut_at);
  assert.equal(t.offline_cooling, false);
  assert.equal(t.paused_cooling, false);
  assert.equal(t.tow_or_reset_instant_cooling, false);
  assert.equal(t.unpaused_garage_and_roadside_ui_tick_engine_off_cooling, true);
  assert.equal(t.persist_protection_latch_until_release, true);
});

check('Preinstalled used stages price by installed entitlement without double-counting ownership spending', () => {
  const courier = byCar.get('car_courier');
  const tires = upgrades.categories.find(c => c.id === 'tires');
  assert.equal(price(courier, tires, 2) - price(courier, tires, 1), 400);
  assert.equal(price(courier, tires, 3) - price(courier, tires, 2), 650);
  assert.equal(maintenance.buyback.resets_current_ownership_upgrade_investment_to, 0);
  assert.equal(maintenance.buyback.previous_installed_stages_included_in_purchase, true);
  assert.equal(maintenance.buyback.history_investment_receipts_are_not_current_ownership_resale_investment, true);
  assert.equal(maintenance.buyback.rerolls_or_free_repairs_on_relisting, false);
  assert.equal(economy.used_vehicle_buyback.margin, 500);
  assert.equal(economy.upgrade_price_basis, 'cars_json_catalog_purchase_price_msrp_not_instance_listing_price_paid');
});

check('Mileage units and advisory check intervals do not add a second condition or price penalty', () => {
  const m = maintenance.mileage_tracking;
  assert.equal(m.canonical_unit, 'metres');
  assert.equal(m.metres_per_kilometre, 1000);
  assert.equal(m.metres_per_mile, 1609.344);
  assert.equal(1609.344 / m.metres_per_mile, 1);
  assert.equal(1609.344 / m.metres_per_kilometre, 1.609344);
  assert.equal(m.lifetime_odometer_never_resets, true);
  assert.equal(m.trip_reset_changes_lifetime_odometer_or_wear, false);
  assert.equal(m.component_replacement_resets_chassis_odometer, false);
  assert.equal(m.inspection_is_not_a_service, true);
  assert.equal(m.reminder_adds_wear_power_penalty_fee_or_entry_gate, false);
  assert.equal(m.raw_odometer_adds_extra_resale_factor, false);
  assert.deepEqual(Object.keys(m.check_interval_m_by_component).sort(), [...componentIds].sort());
  assert(Object.values(m.check_interval_m_by_component).every(value => Number.isSafeInteger(value) && value > 0));
});

function lastDocumentedService(records, component) {
  const eligible = records.filter(record =>
    (['maintenance', 'repair', 'component_replacement'].includes(record.type) && record.components?.includes(component))
    || (record.type === 'upgrade' && record.renewed_components?.includes(component)));
  return eligible.length ? eligible.at(-1).odometer_m : null;
}
check('Component service mileage uses scoped work and leaves missing service history unknown', () => {
  const worn = usedListings.find(l => l.id === 'listing_courier_002');
  assert.equal(lastDocumentedService(worn.history_records, 'engine'), null);
  assert.equal(lastDocumentedService(worn.history_records, 'cooling'), 91500000);
  assert.equal(worn.odometer_m - lastDocumentedService(worn.history_records, 'cooling'), 26500000);
  const inspected = [...worn.history_records, { type: 'inspection', odometer_m: worn.odometer_m, components: ['engine', 'cooling'] }];
  assert.equal(lastDocumentedService(inspected, 'engine'), null);
  assert.equal(lastDocumentedService(inspected, 'cooling'), 91500000);
  const brakeRepair = [...inspected, { type: 'repair', odometer_m: worn.odometer_m, components: ['brakes'] }];
  assert.equal(lastDocumentedService(brakeRepair, 'brakes'), worn.odometer_m);
  assert.equal(lastDocumentedService(brakeRepair, 'engine'), null);
  const tune = [...brakeRepair, { type: 'upgrade', odometer_m: worn.odometer_m, components: ['engine'], renewed_components: [] }];
  assert.equal(lastDocumentedService(tune, 'engine'), null);
  const finch = usedListings.find(l => l.id === 'listing_finch_001');
  for (const id of componentIds) assert.equal(lastDocumentedService(finch.history_records, id), finch.odometer_m);
});

check('All categories require sequential payments and stage totals remain well-defined', () => {
  assert.equal(upgrades.direct_stage_purchase_allowed, false);
  assert.equal(upgrades.purchase_sequence, 'exactly_next_stage_required_in_every_category');
  assert.equal(upgrades.engine_path_change_allowed, false);
  for (const category of upgrades.categories) {
    assert.deepEqual(category.stages.map(s => s.stage), [1, 2, 3]);
    if (category.id === 'engine') {
      assert.equal(category.operation, 'select_authored_engine_configuration');
      assert.deepEqual(category.stages.map(s => s.previous_stage_required), [0, 1, 2]);
    } else assert(['multiply', 'add'].includes(category.operation));
    for (const car of cars) {
      const totals = [0, 1, 2, 3].map(s => price(car, category, s));
      assert(totals.every(Number.isSafeInteger));
      assert(totals[1] > 0 && totals[2] > totals[1] && totals[3] > totals[2]);
      const sequential = (totals[1] - totals[0]) + (totals[2] - totals[1]) + (totals[3] - totals[2]);
      assert.equal(sequential, totals[3]);
    }
  }
});

const piExamples = {};
check('Performance examples and upgraded configurations agree with the formula', () => {
  for (const car of cars) for (const path of byPowertrain.get(car.powertrain_id).paths) for (let stage = 0; stage <= 3; stage++) {
    const config = effective(car, stage, path.id);
    assert(config.power_kw > 0 && config.mass_kg > 0 && config.grip > 0 && config.shift_seconds > 0);
    assert(pi(config) >= 100 && pi(config) <= 999);
  }
  for (const [id, stock, maximum] of [['car_finch', 149, 270], ['car_lynx', 285, 493], ['car_apex', 728, 999]]) {
    const car = byCar.get(id);
    assert.equal(pi(effective(car, 0)), stock);
    assert.equal(pi(effective(car, 3)), maximum);
    piExamples[id] = { stock, all_stage_3: maximum };
  }
});

function reachable(fraction) {
  let reputation = 0;
  const completed = new Set();
  let changed = true;
  while (changed) {
    changed = false;
    for (const event of events) if (!completed.has(event.id) && reputation >= event.reputation_required) {
      completed.add(event.id);
      reputation += percent(event.max_reputation, fraction);
      changed = true;
    }
  }
  return { event_count: completed.size, reputation };
}
const reachability = {};
check('Gold and silver results can numerically unlock every event', () => {
  reachability.gold = reachable(1);
  reachability.silver = reachable(0.8);
  assert.deepEqual(reachability.gold, { event_count: 16, reputation: 2260 });
  assert.deepEqual(reachability.silver, { event_count: 16, reputation: 1808 });
});
check('Opening bronze/worst-result skill gates are explicit and recoverable by improvement', () => {
  reachability.bronze = reachable(0.6);
  reachability.worst_valid_finish = reachable(0.4);
  assert.deepEqual(reachability.bronze, { event_count: 3, reputation: 108 });
  assert.deepEqual(reachability.worst_valid_finish, { event_count: 3, reputation: 72 });
  const improved = 108 + percent(byEvent.get('evt_dock_03').max_reputation, 0.8) - percent(byEvent.get('evt_dock_03').max_reputation, 0.6);
  assert.equal(improved, 122);
  assert(improved >= byEvent.get('evt_dock_04').reputation_required);
  assert.equal(economy.entry_fee, 0);
});

check('First-session arithmetic and maximum first-clear rewards agree', () => {
  const finch = byCar.get('car_finch');
  let wallet = economy.starting_wallet - finch.purchase_price;
  assert.equal(wallet, 4000);
  for (const id of ['tires', 'brakes', 'engine']) wallet -= price(finch, upgrades.categories.find(c => c.id === id), 1);
  assert.equal(wallet, 3050);
  for (const [id, expected] of [['evt_dock_01', 4150], ['evt_dock_02', 5550], ['evt_dock_03', 7150]]) {
    const event = byEvent.get(id);
    wallet += event.base_reward + event.first_clear_bonus;
    assert.equal(wallet, expected);
  }
  assert.equal(events.reduce((sum, e) => sum + e.base_reward + e.first_clear_bonus, 0), 65200);
});

check('Car and upgrade resale loops lose credits for every defined configuration', () => {
  for (const car of cars) for (const path of byPowertrain.get(car.powertrain_id).paths) for (let stage = 0; stage <= 3; stage++) {
    const invested = upgrades.categories.reduce((sum, category) => sum + price(car, category, stage, path.id), 0);
    const quote = percent(car.purchase_price, economy.resale.base_price_paid_fraction)
      + percent(invested, economy.resale.installed_investment_fraction);
    assert(quote < car.purchase_price + invested);
    assert(Number.isSafeInteger(quote));
  }
});

check('Task ledger matches all roadmap IDs and has a valid acyclic dependency graph', () => {
  const rows = [...roadmap.matchAll(/^\| (M\d{2}-T\d{2}) \| ([^|]+) \| ([^|]+) \| (not_started|in_progress|blocked|done) \|$/gm)];
  const ids = ledger.tasks.map(t => t.id);
  assert.equal(ids.length, rows.length);
  assert(unique(ids));
  assert.deepEqual(ids, rows.map(r => r[1]));
  const taskById = new Map(ledger.tasks.map(t => [t.id, t]));
  for (const task of ledger.tasks) {
    assert(ledger.status_values.includes(task.status));
    assert(existsSync(resolve(root, task.spec_ref)));
    assert(Array.isArray(task.evidence) && Array.isArray(task.remaining_steps) && Array.isArray(task.blockers));
    for (const dependency of task.depends_on) assert(taskById.has(dependency));
    const row = rows.find(r => r[1] === task.id);
    assert.deepEqual(task.depends_on, row[3].trim() === '-' ? [] : row[3].trim().split(',').map(s => s.trim()));
  }
  const visited = new Set(), active = new Set();
  function visit(id) {
    assert(!active.has(id), `Dependency cycle at ${id}`);
    if (visited.has(id)) return;
    active.add(id);
    for (const dependency of taskById.get(id).depends_on) visit(dependency);
    active.delete(id); visited.add(id);
  }
  for (const id of ids) visit(id);
  if (ledger.project_stage === 'design_complete_implementation_not_started') {
    assert(ledger.tasks.every(t => t.status === 'not_started'));
    assert.equal(ledger.current_task_id, null);
    assert.equal(ledger.next_task_id, 'M00-T01');
  }
});

function walk(directory) {
  return readdirSync(directory, { withFileTypes: true }).flatMap(entry => {
    if (['.git', '.godot', 'node_modules', 'builds', 'deliverables', 'artifacts'].includes(entry.name)) return [];
    const path = resolve(directory, entry.name);
    return entry.isDirectory() ? walk(path) : [path];
  });
}
check('Local Markdown links resolve and no stale START_HERE reference remains', () => {
  for (const file of walk(root).filter(p => extname(p) === '.md')) {
    const content = readFileSync(file, 'utf8');
    assert(!content.includes('START_HERE.md'), relative(root, file));
    for (const match of content.matchAll(/\[[^\]]*\]\(([^)]+)\)/g)) {
      let target = match[1].trim();
      if (/^(https?:|mailto:|#)/.test(target)) continue;
      target = target.replace(/^<|>$/g, '').split('#')[0];
      if (!target) continue;
      assert(existsSync(resolve(dirname(file), decodeURIComponent(target))), `${relative(root, file)} -> ${target}`);
    }
  }
});

const summary = {
  result: 'pass',
  checked_at: new Date().toISOString(),
  check_groups: groups.length,
  content: { cars: cars.length, upgrade_categories: upgrades.categories.length, engine_paths: powertrains.reduce((sum, pt) => sum + pt.paths.length, 0), engine_configurations: allConfigurations.length, events: events.length, used_offers: usedListings.length, condition_components: components.length, implementation_tasks: ledger.tasks.length },
  reachability, pi_examples: piExamples, step_cost_examples: stepCostExamples, used_car_examples: usedCarExamples,
  game_runtime_tested: false
};
console.log(JSON.stringify(summary, null, 2));
if (process.argv.includes('--write-report')) {
  const report = [
    '# Blueprint validation', '',
    `Checked: ${summary.checked_at}`, '',
    `Result: **PASS — ${groups.length} design-package check groups**.`, '',
    'Command: `node tools/validate_blueprint.mjs --write-report`', '',
    'This validates the written reference data, task structure and document links. It does not test a playable game, actual handling, saves, AI drivers, performance or an exported build.', '',
    ...groups.map(g => `- Pass: ${g}.`), '',
    '## Numerical progression scenarios', '',
    '| Fixed result band | Reachable events | Reputation |',
    '|---|---:|---:|',
    ...Object.entries(reachability).map(([band, r]) => `| ${band.replaceAll('_', ' ')} | ${r.event_count} | ${r.reputation} |`), '',
    'Bronze-only and worst-finish scenarios intentionally need an improved best result before unlocking the next event. Credits remain replayable. Numerical reachability does not prove that a player can beat the AI or that pacing is enjoyable.', '',
    '## Performance-score examples', '',
    '| Car | Stock PI | All stage 3 PI |', '|---|---:|---:|',
    ...Object.entries(piExamples).map(([id, v]) => `| ${id} | ${v.stock} | ${v.all_stage_3} |`), '',
    '## Sequential engine payment examples', '',
    'Finch engine steps: 400 + 600 + 1,200 = 2,200 credits. Courier turbo engine steps: 450 + 2,000 + 3,550 = 6,000 credits. Its required transmission stage 2 costs 650 in total, so that engine/transmission project costs 6,650 beyond buying the car.', '',
    '## Complete-build reference examples', '',
    '| Engine path | Stock PI | All stage 3 PI | All stage 3 upgrades, credits |',
    '|---|---:|---:|---:|',
    ...allPathExamples.map(v => `| ${v.path_id} | ${v.stock_pi} | ${v.all_stage_3_pi} | ${v.total_upgrade_cost} |`), '',
    '## Used cars and recovery', '',
    'Ten initial physical vehicles and four slice offers have unique IDs, bounded condition and chronological records. Guaranteed starters preserve their original price and healthy condition. A worn Courier has a 300-credit minimum quote, a resale quote of 1,524 before repair and 1,657 afterward. A 1,000-credit reserve leaves 600 after its 100-credit inspection and 300-credit fix.', '',
    'All 64 unsafe-component combinations have a valid minimum repair of at most 1,200 credits. Four completed 300-credit workshop jobs therefore fund the worst minimum repair from zero. Buyback/repair valuation, purchased-component renewal and preinstalled-stage price references were checked as data/calculation contracts.', '',
    'These checks cover authored torque/maintenance data and independent calculation examples. Actual wear, breakdowns, workshop jobs, torque response, transaction rejection and save recovery still require the game implementation and its tests.', '',
    'All financial and physics values remain proposed tuning baselines. Runtime implementation tasks are still tracked independently in `TASKS.json`.', ''
  ].join('\n');
  writeFileSync(resolve(root, 'project/BLUEPRINT_VALIDATION.md'), report);
}
