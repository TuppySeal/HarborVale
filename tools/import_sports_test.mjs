import fs from 'node:fs';
import path from 'node:path';
const source='design-data/sports_test.json';
const data=JSON.parse(fs.readFileSync(source,'utf8'));
for(const key of ['stock','stageOne','turboBasic','turboStreet']){
 const d=data[key];
 if(d.carId!=='car_apex_test'||d.rpm.length!==d.torque.length||d.torque.some(n=>!Number.isFinite(n)||n<=0))throw Error('Invalid sports curve '+key);
 if(d.offBoostTorque&&d.offBoostTorque.length!==d.rpm.length)throw Error('Invalid off-boost curve');
}
if(!(data.gearboxShiftMultiplier>0&&data.gearboxShiftMultiplier<1))throw Error('Invalid shift multiplier');
const destination=process.argv[2]||'Assets/HarborValeGreybox/SportsTest/Resources';
fs.mkdirSync(destination,{recursive:true});fs.writeFileSync(path.join(destination,'ApexTestCatalog.json'),JSON.stringify(data,null,2)+'\n');
console.log('Validated/imported standalone sports test catalog. No career prices or car ownership added.');
