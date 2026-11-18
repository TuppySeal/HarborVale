import fs from 'node:fs';
const m=JSON.parse(fs.readFileSync('design-data/harbor_vale_expanded_map.json','utf8'));
const assert=(test,message)=>{if(!test)throw Error(message);};
const ids=[...m.roads,...m.sites,...m.junctions].map(o=>o.id);assert(new Set(ids).size===ids.length,'Duplicate IDs');
const prior=JSON.parse(fs.readFileSync('design-data/harbor_vale_map.json','utf8'));
for(const s of prior.sites.filter(s=>s.kind!=='block'))assert(m.sites.some(n=>n.id===s.id),'Lost location '+s.id);
let maxGrade=0,minRadius=Infinity,maxStep=0;const crossings=[];
for(const r of m.roads){
 assert(r.points.length>20,'Unsampled road '+r.id);assert(r.width===7.2||r.width===14,'Invalid width');
 for(let i=0;i<r.points.length;i++){
  const b=r.points[i];assert([b.x,b.y,b.z].every(Number.isFinite),'Nonfinite point');if(i===0)continue;
  const a=r.points[i-1],ab=Math.hypot(b.x-a.x,b.z-a.z);maxStep=Math.max(maxStep,ab);maxGrade=Math.max(maxGrade,Math.abs(b.y-a.y)/ab);
  if(i<r.points.length-1){const c=r.points[i+1],bc=Math.hypot(c.x-b.x,c.z-b.z),ac=Math.hypot(c.x-a.x,c.z-a.z),cross=Math.abs((b.x-a.x)*(c.z-a.z)-(b.z-a.z)*(c.x-a.x));if(cross>1e-5)minRadius=Math.min(minRadius,ab*bc*ac/(2*cross));}
 }
}
assert(maxGrade<=.12,'Grade exceeds 12%');assert(maxStep<7,'Coarse road sampling');assert(minRadius>12,'Tight curve radius');
// All named-road crossings must be represented by shared junctions.
function cross(a,b,c,d){
 const rx=b.x-a.x,rz=b.z-a.z,sx=d.x-c.x,sz=d.z-c.z,den=rx*sz-rz*sx;if(Math.abs(den)<1e-7)return null;
 const t=((c.x-a.x)*sz-(c.z-a.z)*sx)/den,u=((c.x-a.x)*rz-(c.z-a.z)*rx)/den;
 return t>=0&&t<=1&&u>=0&&u<=1?{x:a.x+t*rx,z:a.z+t*rz}:null;
}
for(let ri=0;ri<m.roads.length;ri++)for(let rj=ri+1;rj<m.roads.length;rj++){
 const a=m.roads[ri],b=m.roads[rj];
 for(let i=1;i<a.points.length;i++)for(let j=1;j<b.points.length;j++){
  const p=cross(a.points[i-1],a.points[i],b.points[j-1],b.points[j]);if(!p)continue;
  if(!m.junctions.some(n=>Math.hypot(n.x-p.x,n.z-p.z)<22)&&!crossings.some(n=>Math.hypot(n.x-p.x,n.z-p.z)<5))crossings.push({roads:[a.id,b.id],...p});
 }
}
assert(crossings.length===0,'Unrepresented crossings: '+JSON.stringify(crossings));
const graph=new Map();for(const r of m.roads)for(let i=1;i<r.knots.length;i++){const a=r.knots[i-1],b=r.knots[i];if(!graph.has(a))graph.set(a,new Set());if(!graph.has(b))graph.set(b,new Set());graph.get(a).add(b);graph.get(b).add(a);}
const seen=new Set(),queue=[graph.keys().next().value];while(queue.length){const n=queue.pop();if(seen.has(n))continue;seen.add(n);queue.push(...graph.get(n));}assert(seen.size===graph.size,'Disconnected roads');
for(const s of m.sites.filter(s=>!['event','lookout','meet'].includes(s.kind))){
 const a=s.yaw*Math.PI/180;
 for(const r of m.roads)assert(!r.points.some(p=>{const dx=p.x-s.x,dz=p.z-s.z;return Math.abs(Math.cos(a)*dx-Math.sin(a)*dz)<s.w/2+r.width/2+1&&Math.abs(Math.sin(a)*dx+Math.cos(a)*dz)<s.d/2+r.width/2+1;}),'Building/plot intrudes on road: '+s.id+'/'+r.id);
}
const report={pass:true,units:'meters',roads:m.roads.length,junctions:m.junctions.length,sites:m.sites.length,buildings:m.stats.buildingCount,roadKm:m.stats.roadKm,maxGradePercent:maxGrade*100,minCurveRadiusM:minRadius,maxSampleStepM:maxStep,graphConnected:true,unrepresentedCrossings:crossings,stableGameplayLocationIdsPreserved:true,starter:m.car};
fs.mkdirSync('artifacts/qa/M11-T02',{recursive:true});fs.writeFileSync('artifacts/qa/M11-T02/source-report.json',JSON.stringify(report,null,2)+'\n');console.log(JSON.stringify(report));
