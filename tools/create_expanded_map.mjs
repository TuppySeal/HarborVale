import fs from 'node:fs';
// Authored in meters. Junction knots are shared by the roads that meet there.
const nodes={
 A:[-1300,-880],B:[-720,-910],C:[-230,-820],D:[440,-850],E:[1120,-650],
 F:[-1390,-420],G:[-820,-430],H:[-270,-330],I:[420,-350],J:[1230,-130],
 K:[-1280,100],L:[-760,170],M:[-190,140],N:[470,200],O:[1160,400],
 P:[-1430,570],Q:[-1020,950],R:[-500,1170],S:[140,1300],T:[740,1050],U:[1210,750],
 V:[-700,650],W:[-120,750],X:[490,700],Y:[-320,470],Z:[850,30],AA:[-310,150],
};
const smooth=t=>{t=Math.max(0,Math.min(1,t));return t*t*(3-2*t);};
const height=z=>84*smooth((z-340)/1000);
const roads=[];
function road(id,name,laneCount,ids,sidewalk=true){
 const knots=ids.map(k=>nodes[k]);const points=[];
 for(let j=0;j<knots.length-1;j++){
  const a=knots[j],b=knots[j+1],prev=knots[Math.max(0,j-1)],next=knots[Math.min(knots.length-1,j+2)];
  const length=Math.hypot(b[0]-a[0],b[1]-a[1]); const steps=Math.ceil(length/3);
  const ta=[(b[0]-prev[0])*.5,(b[1]-prev[1])*.5],tb=[(next[0]-a[0])*.5,(next[1]-a[1])*.5];
  if(j===0){ta[0]=b[0]-a[0];ta[1]=b[1]-a[1];}
  if(j===knots.length-2){tb[0]=b[0]-a[0];tb[1]=b[1]-a[1];}
  for(const tangent of [ta,tb]){const n=Math.hypot(...tangent);if(n>length){tangent[0]*=length/n;tangent[1]*=length/n;}}
  for(let k=0;k<steps;k++){
   const t=k/steps,t2=t*t,t3=t2*t;
   const x=(2*t3-3*t2+1)*a[0]+(t3-2*t2+t)*ta[0]+(-2*t3+3*t2)*b[0]+(t3-t2)*tb[0];
   const z=(2*t3-3*t2+1)*a[1]+(t3-2*t2+t)*ta[1]+(-2*t3+3*t2)*b[1]+(t3-t2)*tb[1];
   points.push({x,y:height(z),z});
  }
 }
 const last=knots.at(-1);points.push({x:last[0],y:height(last[1]),z:last[1]});
 roads.push({id,name,width:laneCount===4?14:7.2,lanes:laneCount,sidewalk,points,knots:ids});
}
road('RD01','Quay Road',2,['A','B','C','D','E']);
road('RD02','Freight Avenue',2,['F','G','H','I','J']);
road('RD03','Container Street',2,['B','G','L']);
road('RD04','Workshop Street',2,['C','H','M']);
road('RD05','Harbor Boulevard',4,['K','L','AA','M','N','O']);
road('RD06','Eastbank Drive',2,['E','J','O'],false);
road('RD07','Central Avenue',2,['D','I','N']);
road('RD08','Plaza Boulevard',2,['M','Y','W','S']);
road('RD09','Market Crescent',2,['I','Z','O']);
road('RD10','East Service Road',2,['N','X','T']);
road('RD11','Ridgeway Approach',2,['O','U','T'],false);
road('RD12','Coastal Bypass',2,['A','F','K','P','Q','R'],false);
road('RD13','Summit Scenic Route',2,['R','S','T'],false);
road('RD14','Plateau Road',2,['Q','V','W','X','U'],false);
road('RD15','Lookout Lane',2,['L','V','R'],false);
road('RD16','Old Town Link',2,['H','AA','Y','N']);
road('RD17','West Harbor Extension',2,['F','L']);
road('RD18','Ridge Farm Road',2,['R','W'],false);
// Shorter neighborhood blocks between the long arterials, with gently bowed streets.
function link(id,name,from,to,x,bow){
 function attach(roadId,key){
  const r=roads.find(r=>r.id===roadId);let index=0;
  for(let i=1;i<r.points.length;i++)if(Math.abs(r.points[i].x-x)<Math.abs(r.points[index].x-x))index=i;
  const p=r.points[index];nodes[key]=[p.x,p.z];
  let before=0;for(let n=0;n<r.knots.length;n++){const node=nodes[r.knots[n]];let ni=0;for(let j=1;j<r.points.length;j++)if(Math.hypot(r.points[j].x-node[0],r.points[j].z-node[1])<Math.hypot(r.points[ni].x-node[0],r.points[ni].z-node[1]))ni=j;if(ni<index)before=n;}
  r.knots.splice(before+1,0,key);return p;
 }
 const ka=id+'A',kb=id+'B',a=attach(from,ka),b=attach(to,kb),steps=Math.ceil(Math.hypot(b.x-a.x,b.z-a.z)/3),points=[];
 for(let i=0;i<=steps;i++){const t=i/steps,z=a.z+(b.z-a.z)*t;points.push({x:a.x+(b.x-a.x)*t+bow*Math.sin(Math.PI*t)**2,y:height(z),z});}
 roads.push({id,name,width:7.2,lanes:2,sidewalk:true,points,knots:[ka,kb]});
}
link('RD19','Drydock Crescent','RD01','RD02',-1100,32);
link('RD20','Foundry Lane','RD01','RD02',-520,-30);
link('RD21','Station Street','RD01','RD02',40,35);
link('RD22','Showroom Crescent','RD01','RD02',760,-30);
link('RD23','Canal Street','RD02','RD05',-530,28);
link('RD24','Civic Lane','RD02','RD05',70,-32);
const junctions=Object.entries(nodes).map(([id,[x,z]])=>({id:'J_'+id,x,y:height(z),z,radius:roads.filter(r=>r.knots.includes(id)).some(r=>r.lanes===4)?10:6.4}));
// Site placement follows a road frontage; plot depth is measured away from pavement.
const old=JSON.parse(fs.readFileSync('design-data/harbor_vale_map.json','utf8'));
const sites=[];
function frontage(source,roadId,fraction,side,w,d,h){
 const r=roads.find(r=>r.id===roadId),i=Math.max(1,Math.min(r.points.length-2,Math.round((r.points.length-1)*fraction)));
 const p=r.points[i],prev=r.points[i-1],next=r.points[i+1],dx=next.x-prev.x,dz=next.z-prev.z,len=Math.hypot(dx,dz);
 const offset=r.width/2+5+d/2+8,nx=dz/len,nz=-dx/len;
 const x=p.x+nx*offset*side,z=p.z+nz*offset*side;
 const yaw=Math.atan2(dx,dz)*180/Math.PI+90+(side<0?180:0);
 const a=yaw*Math.PI/180;let floorY=height(z);
 for(const lx of [-(w+6)/2,(w+6)/2])for(const lz of [-(d+12)/2,(d+12)/2])floorY=Math.max(floorY,height(z-Math.sin(a)*lx+Math.cos(a)*lz));
 const site={...source,x,y:floorY,z,w,d,h,yaw,roadId,side,accessX:p.x+nx*(r.width/2)*side,accessZ:p.z+nz*(r.width/2)*side};sites.push(site);return site;
}
const specs=[
 ['D01','RD02',.23,-1,18,12,5],['D02','RD02',.34,-1,22,14,5],['D03','RD04',.28,1,16,11,4.2],
 ['D04','RD03',.64,1,28,19,5],['D05','RD01',.17,-1,50,35,3],['D06','RD02',.40,1,23,18,4.5],
 ['D07','RD01',.35,-1,36,23,8],['D08','RD03',.40,-1,19,14,5],['D09','RD04',.48,-1,20,14,6.4],
 ['M01','RD02',.65,-1,23,16,6.4],['M02','RD01',.78,-1,32,22,8],['M03','RD06',.27,1,24,16,6.4],
 ['M04','RD06',.50,1,36,25,12.8],['M05','RD07',.65,1,24,20,4.5],['M06','RD16',.23,-1,22,16,3.2],
 ['M07','RD09',.42,-1,42,28,1],['M08','RD10',.12,1,24,16,6.4],['M09','RD05',.59,-1,45,30,1],
 ['R01','RD14',.60,-1,24,16,6.4],['R02','RD11',.54,-1,24,18,4.5],['R03','RD18',.22,1,20,14,7],
 ['R04','RD13',.34,1,40,28,1],['R05','RD13',.14,-1,8,8,35],
 ['V01','RD12',.12,-1,16,10,.3],['V02','RD12',.27,-1,16,10,.3],['V03','RD05',.62,1,16,10,.3],
 ['V04','RD06',.69,1,16,10,.3],['V05','RD12',.82,-1,16,10,.3],['V06','RD13',.57,1,16,10,.3],
];
for(const [id,r,f,s,w,d,h] of specs)frontage(old.sites.find(v=>v.id===id),r,f,s,w,d,h);
const events=old.sites.filter(s=>s.kind==='event');
events.forEach((e,i)=>frontage(e,roads[i%roads.length].id,.32+(i%3)*.16,1,4,4,.2));
function distanceRoad(x,z){let min=Infinity;for(const r of roads)for(const p of r.points)min=Math.min(min,Math.hypot(x-p.x,z-p.z)-r.width/2);return min;}
// Keep footprints and parking forecourts clear of every curved carriageway.
function clear(candidate){
 const radius=Math.hypot(candidate.w,candidate.d)/2+5;
 const a=candidate.yaw*Math.PI/180;
 for(const lx of [-candidate.w/2,0,candidate.w/2])for(const lz of [-candidate.d/2,0,candidate.d/2]){
  const x=candidate.x+Math.cos(a)*lx+Math.sin(a)*lz,z=candidate.z-Math.sin(a)*lx+Math.cos(a)*lz;
  if(distanceRoad(x,z)<5)return false;
 }
 return !sites.some(s=>s!==candidate&&Math.hypot(s.x-candidate.x,s.z-candidate.z)<radius+Math.hypot(s.w,s.d)/2+8);
}
let count=0;
for(const r of roads){
 for(let i=23;i<r.points.length-23;i+=26){
  for(const side of [-1,1]){
   const p=r.points[i],district=p.z>400?'Ridgeway':p.x< -200?'Dockside':'Midtown';
   const w=district==='Dockside'?24+(count%3)*8:district==='Midtown'?14+(count%4)*4:10+(count%3)*3;
   const d=district==='Dockside'?20:district==='Midtown'?18:11;
   const h=district==='Midtown'?9.6+(count%6)*3.2:district==='Dockside'?6.4+(count%3)*3.2:6.4;
   const candidate=frontage({id:'BLD_'+String(count+1).padStart(3,'0'),name:(district==='Dockside'?'Warehouse':district==='Midtown'?'Urban building':'Hillside home')+' '+(count+1),district,kind:'block'},r.id,i/(r.points.length-1),side,w,d,h);
   if(!clear(candidate)){sites.pop();continue;}count++;
  }
 }
}
let maxGrade=0,total=0;
for(const r of roads)for(let i=1;i<r.points.length;i++){const a=r.points[i-1],b=r.points[i],run=Math.hypot(b.x-a.x,b.z-a.z);maxGrade=Math.max(maxGrade,Math.abs(b.y-a.y)/run);total+=run;}
if(maxGrade>.12)throw Error('Road grade too steep');
const garage=sites.find(s=>s.id==='D01');
const car={id:'car_finch',name:'Aster Finch',length:4.05,width:1.70,height:1.46,wheelbase:2.45,wheelRadius:.295,massKg:1050,x:garage.x,z:garage.z,yaw:garage.yaw};
// Car sits on the flat forecourt, in front of the garage facade.
const a=garage.yaw*Math.PI/180;car.x=garage.x-Math.sin(a)*(garage.d/2+4);car.z=garage.z-Math.cos(a)*(garage.d/2+4);car.y=garage.y;
car.yaw=garage.yaw+180;
const output={units:'meters',version:2,north:'positive Z',extent:{width:3400,depth:3000},roads,junctions,sites,car,stats:{roadKm:total/1000,maxGrade,buildingCount:count}};
fs.writeFileSync('design-data/harbor_vale_expanded_map.json',JSON.stringify(output,null,2)+'\n');
fs.writeFileSync('project/MAP_LOCATION_INDEX.md','# Harbor Vale expanded map index\n\nCanonical layout: design-data/harbor_vale_expanded_map.json. One Unity unit = one meter.\n\n| ID | Location | District | X m | Y m | Z m |\n|---|---|---|---:|---:|---:|\n'+sites.filter(s=>s.kind!=='block').map(s=>`| ${s.id} | ${s.name} | ${s.district} | ${s.x.toFixed(1)} | ${s.y.toFixed(1)} | ${s.z.toFixed(1)} |`).join('\n')+'\n');
console.log(JSON.stringify({roads:roads.length,sites:sites.length,car,...output.stats}));
