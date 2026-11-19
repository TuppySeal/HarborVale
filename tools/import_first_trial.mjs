import fs from 'node:fs';
const m=JSON.parse(fs.readFileSync('design-data/harbor_vale_expanded_map.json','utf8'));
const events=JSON.parse(fs.readFileSync('design-data/events.json','utf8'));
const economy=JSON.parse(fs.readFileSync('design-data/economy.json','utf8'));
const event=events.events.find(e=>e.id==='evt_dock_01');
function slice(id,from,to){const r=m.roads.find(r=>r.id===id);function index(node){const p=m.junctions.find(j=>j.id==='J_'+node);let n=0;for(let i=1;i<r.points.length;i++)if(Math.hypot(r.points[i].x-p.x,r.points[i].z-p.z)<Math.hypot(r.points[n].x-p.x,r.points[n].z-p.z))n=i;return n;}const a=index(from),b=index(to);const p=r.points.slice(Math.min(a,b),Math.max(a,b)+1);return a>b?p.reverse():p;}
const first=slice('RD02','G','H');let start=0,d=0;while(d<45){d+=Math.hypot(first[start+1].x-first[start].x,first[start+1].z-first[start].z);start++;}
const paths=[first.slice(start),slice('RD04','H','C'),slice('RD01','C','B'),slice('RD03','B','G'),first.slice(0,start+1)];
const route=[];for(const path of paths)for(const p of path)if(!route.length||Math.hypot(p.x-route.at(-1).x,p.z-route.at(-1).z)>.01)route.push({...p,y:p.y+.08});
const checkpoints=[];let since=0,total=0;
for(let i=0;i<route.length;i++){
 if(i){const length=Math.hypot(route[i].x-route[i-1].x,route[i].z-route[i-1].z);since+=length;total+=length;}
 if(i===0||i===route.length-1||since>=55){let a=route[Math.max(0,i-1)],b=route[Math.min(route.length-1,i+1)];if(i===route.length-1){a=route[0];b=route[1];}const length=Math.hypot(b.x-a.x,b.z-a.z);checkpoints.push({...route[i],forwardX:(b.x-a.x)/length,forwardZ:(b.z-a.z)/length,halfWidth:10});since=0;}
}
const def={id:event.id,title:'First Lap — Dockside',description:'Follow the marked loop. Brake for the junction turns.',roadIds:['RD02','RD04','RD01','RD03'],lengthM:total,countdownSeconds:3,recoveryPenaltySeconds:3,goldSeconds:115,silverSeconds:145,bronzeSeconds:185,baseReward:event.base_reward,firstClearBonus:event.first_clear_bonus,maxReputation:event.max_reputation,creditBps:economy.result_multipliers.slice(0,4).map(x=>Math.round(x.credit*10000)),repBps:economy.result_multipliers.slice(0,4).map(x=>Math.round(x.reputation*10000)),route,checkpoints};
if(checkpoints.length<10||total<1000)throw Error('Trial route incomplete');
for(let i=1;i<route.length;i++)if(Math.hypot(route[i].x-route[i-1].x,route[i].z-route[i-1].z)>6)throw Error('Disconnected route');
if(Math.hypot(route[0].x-route.at(-1).x,route[0].z-route.at(-1).z)>.01)throw Error('Route not closed');
const folder=process.argv[2]||'Assets/HarborValeGreybox/Racing/Resources';fs.mkdirSync(folder,{recursive:true});fs.writeFileSync(folder+'/FirstDocksideTrial.json',JSON.stringify(def,null,2)+'\n');
fs.mkdirSync('artifacts/qa/M12-T01',{recursive:true});const report={pass:true,routeLengthM:total,checkpoints:checkpoints.length,closed:true,roads:def.roadIds,baseReward:def.baseReward,firstClearBonus:def.firstClearBonus,maxReputation:def.maxReputation,medalSeconds:[def.goldSeconds,def.silverSeconds,def.bronzeSeconds],medalTimingStatus:'provisional; user-driving calibration pending'};fs.writeFileSync('artifacts/qa/M12-T01/route-report.json',JSON.stringify(report,null,2)+'\n');console.log(JSON.stringify(report));
