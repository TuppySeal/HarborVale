import fs from 'node:fs';
const all=JSON.parse(fs.readFileSync('design-data/powertrains.json','utf8'));
const car=JSON.parse(fs.readFileSync('design-data/cars.json','utf8')).cars.find(c=>c.id==='car_finch');
const engine=all.powertrains.find(p=>p.id===car.powertrain_id),s=engine.stock,g=engine.gearbox;
const folder=process.argv[2]||'Assets/HarborValeGreybox/Driving/Resources';
fs.mkdirSync(folder,{recursive:true});
const d={source:'design-data/powertrains.json:pt_finch.stock; cars.json:car_finch; visual tire radius 0.295 m',carId:car.id,massKg:car.mass_kg,wheelRadius:.295,efficiency:g.drivetrain_efficiency,finalDrive:g.final_drive,reverseRatio:g.reverse_ratio,idleRpm:s.idle_rpm,redlineRpm:s.redline_rpm,softLimiterRpm:s.soft_limiter_rpm,upshiftRpm:s.upshift_rpm,downshiftRpm:s.downshift_rpm,launchRpm:s.launch_target_rpm,shiftSeconds:car.shift_seconds,brakeDeceleration:car.brake_deceleration,ratios:g.forward_ratios,rpm:s.steady_wot_torque_curve_nm.map(k=>k[0]),torque:s.steady_wot_torque_curve_nm.map(k=>k[1])};
if(d.rpm.length!==d.torque.length||d.rpm.some((r,i)=>i&&r<=d.rpm[i-1]))throw Error('Invalid stock torque curve');
fs.writeFileSync(folder+'/FinchStockPowertrain.json',JSON.stringify(d,null,2)+'\n');
const map=JSON.parse(fs.readFileSync('design-data/harbor_vale_expanded_map.json','utf8')),anchors=[];
for(const road of map.roads)for(let i=2;i<road.points.length-2;i+=12){const p=road.points[i],a=road.points[i-1],b=road.points[i+1];anchors.push({x:p.x,y:p.y+.08,z:p.z,yaw:Math.atan2(b.x-a.x,b.z-a.z)*180/Math.PI});}
fs.writeFileSync(folder+'/FinchRoadRecovery.json',JSON.stringify({anchors},null,2)+'\n');
console.log(`Imported ${car.name} stock curve (${d.rpm.length} knots), ${d.ratios.length} gears, ${anchors.length} road recovery anchors. Radius matches authored model; no upgrade multipliers.`);
