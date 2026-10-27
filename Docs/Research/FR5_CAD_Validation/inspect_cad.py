from OCP.STEPControl import STEPControl_Reader
from OCP.BRepBndLib import BRepBndLib
from OCP.Bnd import Bnd_Box
from OCP.TopExp import TopExp_Explorer
from OCP.TopAbs import TopAbs_SOLID
from OCP.BRepGProp import BRepGProp
from OCP.GProp import GProp_GProps
from pathlib import Path
import json
out=[]
for p in Path('Docs/CAD').rglob('*.stp'):
 r=STEPControl_Reader();r.ReadFile(str(p));r.TransferRoots();s=r.OneShape();e=TopExp_Explorer(s,TopAbs_SOLID);sol=[]
 while e.More():
  b=Bnd_Box();BRepBndLib.AddOptimal_s(e.Current(),b);g=GProp_GProps();BRepGProp.VolumeProperties_s(e.Current(),g)
  sol.append(dict(id=len(sol),bbox=list(b.Get()),volume_mm3=g.Mass(),center=list(g.CentreOfMass().Coord())));e.Next()
 out.append(dict(file=str(p),solids=sol));print(str(p),len(sol),flush=True)
Path('Docs/Research/FR5_CAD_Validation/cad_solids.json').write_text(json.dumps(out,indent=2))
