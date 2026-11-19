import fs from 'node:fs';
const d=JSON.parse(fs.readFileSync('design-data/expanded_workshop.json','utf8')),ids=new Set();
for(const o of d.offers){if(ids.has(o.id)||o.price<=0||!Number.isInteger(o.price))throw Error('Invalid offer '+o.id);ids.add(o.id);}
for(const o of d.offers)for(const p of o.prerequisites)if(!ids.has(p))throw Error('Unknown prerequisite '+p);
for(const car of d.cars){for(const c of car.curves){if(c.torque.length!==car.stock.rpm.length||c.cooling1.length!==c.torque.length||c.torque.some(n=>!Number.isFinite(n)||n<=0))throw Error('Invalid curve');if(c.choice!=='roots')for(let i=0;i<c.torque.length;i++)if(c.offBoostTorque[i]>c.torque[i]||(car.stock.rpm[i]<=c.boostThreshold&&(c.torque[i]!==c.offBoostTorque[i]||c.cooling1[i]!==c.offBoostTorque[i])))throw Error('Boost onset/off-boost mismatch '+car.carId+'/'+c.choice+'/'+c.stage);}}
const dest=process.argv[2]||'Assets/HarborValeGreybox/Workshop/Resources';fs.mkdirSync(dest,{recursive:true});fs.writeFileSync(dest+'/ExpandedWorkshop.json',JSON.stringify(d,null,2)+'\n');console.log('Validated/imported '+d.offers.length+' staged workshop offers.');
