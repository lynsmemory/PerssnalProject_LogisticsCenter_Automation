import sys,json,math
from pathlib import Path
sys.path.insert(0,'/tmp/logistics-cad-libs')
import numpy as np
from OCP.STEPControl import STEPControl_Reader
from OCP.BRepMesh import BRepMesh_IncrementalMesh
from OCP.BRep import BRep_Tool
from OCP.TopoDS import TopoDS
from OCP.TopExp import TopExp_Explorer
from OCP.TopAbs import TopAbs_SOLID,TopAbs_FACE,TopAbs_REVERSED
from OCP.TopLoc import TopLoc_Location
from OCP.BRepAdaptor import BRepAdaptor_Surface
from OCP.GeomAbs import GeomAbs_Plane
from OCP.BRepGProp import BRepGProp
from OCP.GProp import GProp_GProps
OUT=Path('Docs/Research/FR5_CAD_Validation'); data=json.load(open(OUT/'cad_solids.json'))
for d,key in zip(data,['head','set','cup']):
 r=STEPControl_Reader();r.ReadFile(d['file']);r.TransferRoots();s=r.OneShape();ex=TopExp_Explorer(s,TopAbs_SOLID);arr={};i=0
 while ex.More():
  solid=ex.Current();BRepMesh_IncrementalMesh(solid,.15,False,.25,False).Perform();fe=TopExp_Explorer(solid,TopAbs_FACE);vs=[];fs=[]
  while fe.More():
   face=TopoDS.Face_s(fe.Current());loc=TopLoc_Location();tr=BRep_Tool.Triangulation_s(face,loc)
   if tr is not None:
    shift=len(vs);vs.extend([tr.Node(j).Transformed(loc.Transformation()).Coord() for j in range(1,tr.NbNodes()+1)])
    for j in range(1,tr.NbTriangles()+1):
     ids=list(tr.Triangle(j).Get())
     if face.Orientation()==TopAbs_REVERSED:ids[1],ids[2]=ids[2],ids[1]
     fs.append([shift+k-1 for k in ids])
   if key=='set' and i in [7,8,11]:
    a=BRepAdaptor_Surface(face)
    if a.GetType()==GeomAbs_Plane:
     g=GProp_GProps();BRepGProp.SurfaceProperties_s(face,g)
     if g.Mass()>80:print('plane',i,'area',round(g.Mass(),2),'center',g.CentreOfMass().Coord(),'normal',a.Plane().Axis().Direction().Coord())
   fe.Next()
  arr[f'v{i}']=np.array(vs);arr[f'f{i}']=np.array(fs);i+=1;ex.Next()
 np.savez_compressed(OUT/f'{key}_solids_mm.npz',**arr);print(key,i,flush=True)
