import sys,math,json
from pathlib import Path
import numpy as np
ROOT=Path('/home/lyn/Unity/LogisticsCenter_Automation/Docs/Research')
sys.path.insert(0,str(ROOT))
import calculate_fr5_layout as k
# Upright boxes, 480x350mm footprint, height340mm, horizontal-only rotations.
# Fixed pallet; 150mm axis-to-near-edge, 125mm straight axial tool,
# 100mm local approach and 20mm link-length screening reserve.
patterns={
 'six_2x3': [(-x,y,0) for x in [.390,.880] for y in [-.360,0,.360]],
 'five_split_3near_2far': [(-.390,y,0) for y in [-.360,0,.360]]+[(-.815,y,math.pi/2) for y in [-.245,.245]],
 'four_compact_columns': [(-x,y,math.pi/2) for x in [.325,.685] for y in [-.245,.245]],
 'four_pinwheel': [(-.750,-.245,0),(-.815,.180,math.pi/2),(-.390,.245,0),(-.325,-.180,math.pi/2)],
}
def pose(x,y,z,yaw,H):
 q=k.ik_down(x,y,z,base=H,tool=.125)
 if q is None:return None
 p,R=k.fk(q,base=H,tool=.125)
 phi=math.atan2(R[1,0],R[0,0])
 for ang in [yaw,yaw+math.pi,yaw-math.pi]:
  variants=k.equivalents(phi-ang,5)
  if variants:
   q[5]=variants[0];p,R=k.fk(q,base=H,tool=.125)
   err=abs(math.atan2(math.sin(math.atan2(R[1,0],R[0,0])-yaw),math.cos(math.atan2(R[1,0],R[0,0])-yaw)))
   assert min(err,abs(math.pi-err))<1e-8
   assert np.linalg.norm(p-[x,y,z])<1e-8
   return q
 return None
results={}
for name,pts in patterns.items():
 radii=[math.hypot(x,y) for x,y,_ in pts]
 if any(k.envelope(r,base=0,tool=.125,arm_reserve=.02) is None for r in radii):
  results[name]={'centers_m':pts,'radii_m':radii,'all_positions_reachable':False,'reason':'outside downward radial envelope even before selecting base height'};continue
 envs=[k.envelope(r,base=0,tool=.125,arm_reserve=.02) for r in radii]
 plans=[]
 for mm in range(600,1301):
  H=mm/1000;counts=[];margins=[]
  for (x,y,yaw),(lo,hi) in zip(pts,envs):
   if .490<H+lo-1e-10:counts.append(0);continue
   ct=0
   for tier in range(1,7):
    z=.15+.34*tier
    if z+.100>H+hi+1e-10:break
    # Check actual joint limits at bottom and approach endpoint while scanning.
    if pose(x,y,z,yaw,H) is None or pose(x,y,z+.1,yaw,H) is None:break
    ct=tier
   counts.append(ct)
   if ct:margins.extend([.490-(H+lo),H+hi-(.15+.34*ct+.1)])
  if min(counts or [0])>=1:
   plans.append({'base_mm':mm,'per_column_layers':counts,'total_boxes':sum(counts),'full_layers':min(counts),'max_layer':max(counts),'screen_margin_mm':1000*min(margins)})
 if not plans:results[name]={'all_positions_reachable':False,'reason':'no common fixed-base first-layer and approach solution'};continue
 best=max(plans,key=lambda x:(x['total_boxes'],x['screen_margin_mm']))
 uniform=max(plans,key=lambda x:(x['full_layers'],min([.490-(x['base_mm']/1000+lo) for lo,_ in envs]+[x['base_mm']/1000+hi-(.15+.34*x['full_layers']+.1) for _,hi in envs])))
 def validate(plan,uniform_only=False):
  H=plan['base_mm']/1000;counts=plan['per_column_layers'] if not uniform_only else [plan['full_layers']]*len(pts);n=0;missing=[];maxerr=0
  for (x,y,yaw),count in zip(pts,counts):
   for tier in range(1,count+1):
    for z in np.linspace(.15+.34*tier,.25+.34*tier,11):
     n+=1;q=pose(x,y,float(z),yaw,H)
     if q is None:missing.append([x,y,float(z)])
     else:
      p,R=k.fk(q,base=H,tool=.125);maxerr=max(maxerr,float(np.linalg.norm(p-[x,y,z])))
  return {'samples':n,'failed':missing,'max_fk_error_m':maxerr}
 results[name]={'centers_m':pts,'radii_m':radii,'all_positions_reachable':True,'best_mixed':best,'best_uniform':{'base_mm':uniform['base_mm'],'layers':uniform['full_layers'],'boxes':len(pts)*uniform['full_layers']},'mixed_verification':validate(best),'uniform_verification':validate(uniform,True),'at_base900':next((p for p in plans if p['base_mm']==900),None),'at_base950':next((p for p in plans if p['base_mm']==950),None),'at_base1000':next((p for p in plans if p['base_mm']==1000),None)}
output={'box_mm':[480,350,340],'pallet_mm':[1100,1100,150],'design_parameters':{'axis_to_near_edge_mm':150,'tool_mm':125,'approach_mm':100,'link_sum_screening_reserve_mm':20,'base_search_mm':[600,1300],'base_step_mm':1,'box_gap_mm':10},'results':results,'limits':['candidate-pattern comparison, not global optimization over all layouts','20mm link reserve is a geometric screening margin, not certified safety clearance','no self/environment/carrying-box collision check','no continuous joint-path, pickup-site or transport stability validation','box size remains a comparison scenario until user selects it']}
(ROOT/'fr5_480x350x340_robot_capacity.json').write_text(json.dumps(output,ensure_ascii=False,indent=2))
print(json.dumps(output,ensure_ascii=False,indent=2))
