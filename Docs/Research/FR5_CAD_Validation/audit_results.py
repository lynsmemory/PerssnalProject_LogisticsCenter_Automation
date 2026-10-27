from pathlib import Path
import json,sys,math
import numpy as np
HERE=Path(__file__).resolve().parent
sys.path.insert(0,str(HERE));import verify as v
out=[]
for name,H,trajectory in [('original_900',.9,None),('original_950',.95,'trajectory_950_360_280.npz'),('local_900',.9,'trajectory_local_360_280.npz')]:
 result=HERE/('path_results.json' if name=='original_900' else 'path_results_950.json' if name=='original_950' else 'path_results_local.json')
 if not result.exists():continue
 d=json.loads(result.read_text())['results'][0];a={'name':name,'base_m':H,'completed':d['all_completed']}
 if a['completed']:
  paths=np.load(HERE/trajectory);qq=np.concatenate([paths[f'box_{i+1}'] for i in range(18)])
  a['joint_samples_roundtrip']=int(len(qq)*2)
  a['joint_limit_margin_deg']=float(np.rad2deg(np.minimum(qq-v.k.LIMITS[:,0],v.k.LIMITS[:,1]-qq).min()))
  a['max_joint_sample_step_rad']=float(max(np.max(np.abs(np.diff(paths[key],axis=0))) for key in paths.files))
  a['collision_calls']=d['collision_calls'];a['max_contact_error_m']=0.;a['max_tool_axis_tilt_deg']=0.
  for i,record in enumerate(d['records']):
   p,R=v.k.fk(paths[f'box_{i+1}'][-1],base=H,tool=v.TOOL)
   a['max_contact_error_m']=max(a['max_contact_error_m'],float(np.linalg.norm(p-record['target'])))
   # Linear joint interpolation retains the analytic downward orientation.
   for q in paths[f'box_{i+1}'][::20]:
    R=v.transforms(q,H)[-1][:3,:3];a['max_tool_axis_tilt_deg']=max(a['max_tool_axis_tilt_deg'],math.degrees(math.acos(np.clip(-R[2,2],-1,1))))
 a['far_outer_approach_upper_bound_margin_mm']=(v.k.envelope(math.hypot(.7,.29),base=H,tool=v.TOOL)[1]-1.270)*1000
 out.append(a)
(HERE/'validation_summary.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2))
