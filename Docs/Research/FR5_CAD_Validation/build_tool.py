"""Build one SPB1 four-cup tool from the provided AP242 solids; mm inputs."""
from pathlib import Path
import json,hashlib
import numpy as np
HERE=Path(__file__).resolve().parent
info=json.loads((HERE/'cad_solids.json').read_text())
head=np.load(HERE/'head_solids_mm.npz');kit=np.load(HERE/'set_solids_mm.npz')
# Product 00269 contains two separate alternatives and raised text. The bellows
# alternative is centered at XY=(0,0), top connection Z=0, cup plane Z=-87.
ids=[s['id'] for s in info[0]['solids'] if s['bbox'][4]<100 and s['bbox'][2]<-1]
# Kit: keep pump, FR5 adapter, QCM and connector/cable. Remove original
# one-cup VEE head (8), stem (9), cup (1). Cable 12 extends to +Y232.4:
# it is supplied straight, without an installed routing; exclude it from the
# rigid tool mesh and record that routing still needs validation. End fitting
# 10 follows that free cable and is excluded too.
keep=[0,2,3,4,5,6,7,11,13]
vs=[];fs=[];parts=[]
for source,indices in [(kit,keep),(head,ids)]:
 for i in indices:
  v=source[f'v{i}'].copy();f=source[f'f{i}'].copy()
  if source is kit:v=v[:,[0,2,1]]*np.array([1,1,-1])
  else:v=v*np.array([1,-1,-1])+np.array([0,0,95.8])
  fs.append(f+sum(len(a) for a in vs));vs.append(v/1000)
  parts.append({'source':'set' if source is kit else 'head','solid':i})
v=np.vstack(vs);f=np.vstack(fs);np.savez_compressed(HERE/'assembled_tool_m.npz',vertices=v,faces=f)
summary={'tool_flange_to_uncompressed_contact_m':.1828,'interface_kit_y_mm':-95.8,'head_contact_z_mm':-87.,'robot_mount_plane_kit_y_mm':0.,'head_selected_solids':ids,'kit_selected_solids':keep,'omitted_original_one_cup_parts':[1,8,9],'unrouted_flexible_cable_excluded':[10,12],'vertices':len(v),'triangles':len(f),'bbox_m':[v.min(0).tolist(),v.max(0).tolist()],'cup_centers_tool_xy_mm':[[x,y] for x in [-62.2253967,62.2253967] for y in [-62.2253967,62.2253967]],'notes':['Robot mount plane is the large planar annulus Y=0 of kit solid11; +Y5 is locating protrusion, not mounting plane.','Tool length uses uncompressed CAD. Cup compression and actual routing not modeled.'],'sources':[{'file':a['file'],'sha256':hashlib.sha256(Path(a['file']).read_bytes()).hexdigest()} for a in info]}
(HERE/'tool_assembly.json').write_text(json.dumps(summary,indent=2));print(json.dumps(summary,indent=2))
