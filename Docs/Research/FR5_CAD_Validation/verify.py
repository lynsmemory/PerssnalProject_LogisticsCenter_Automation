import sys,math,struct,json,time
from pathlib import Path
import numpy as np
sys.path.insert(0,'/tmp/logistics-fr5-validation-libs')
import fcl
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT))
import calculate_fr5_layout as k
TOOL=.1828
CAD=np.load(Path(__file__).with_name("assembled_tool_m.npz"))
TOOL_GEOM=fcl.BVHModel();TOOL_GEOM.beginModel(len(CAD["faces"]),len(CAD["vertices"]));TOOL_GEOM.addSubModel(CAD["vertices"],CAD["faces"]);TOOL_GEOM.endModel()
NAMES=['base_link','shoulder_link','upperarm_link','forearm_link','wrist1_link','wrist2_link','wrist3_link']
MESH=ROOT/'CAD_References/FR5/fairino_description/meshes/fairino5_v6'
request=fcl.CollisionRequest(num_max_contacts=1,enable_contact=False)
def tf(R=np.eye(3),p=np.zeros(3)):
 T=np.eye(4);T[:3,:3]=R;T[:3,3]=p;return T
def obj(geom,T=np.eye(4)):
 return fcl.CollisionObject(geom,fcl.Transform(T[:3,:3],T[:3,3]))
def box(dims,center):return obj(fcl.Box(*dims),tf(p=np.array(center)))
meshes=[]
for name in NAMES:
 b=(MESH/(name+'.STL')).read_bytes();n=struct.unpack_from('<I',b,80)[0]
 verts=np.frombuffer(b,dtype=np.dtype([('normal','<f4',3),('verts','<f4',(3,3)),('attr','<u2')]),offset=84,count=n)['verts'].reshape(-1,3).astype(float)
 geom=fcl.BVHModel();geom.beginModel(n,len(verts));geom.addSubModel(verts,np.arange(len(verts)).reshape(-1,3));geom.endModel();meshes.append(geom)

def transforms(q,H=.9):
 T=tf(p=np.array([0,0,H]));Ts=[T.copy()]
 for xyz,alpha,theta in zip([(0,0,0),(0,0,k.D1),(-k.A,0,0),(-k.B,0,0),(0,0,k.D4),(0,0,k.D5)],[0,math.pi/2,0,0,math.pi/2,-math.pi/2],q):
  T=T@tf(p=np.array(xyz))@tf(k.rx(alpha)@k.rz(theta));Ts.append(T.copy())
 return Ts

def ik_all(x,y,z,yaw=0,H=.9):
 r2=x*x+y*y
 if r2<k.D4*k.D4:return []
 sols=[]
 for w in [math.sqrt(r2-k.D4*k.D4),-math.sqrt(r2-k.D4*k.D4)]:
  for q1 in k.equivalents(math.atan2(y,x)-math.atan2(-k.D4,w),0):
   for sign in [1,-1]:
    u=w-sign*k.D5;v=z-H-k.D1+k.D6+TOOL;c=(u*u+v*v-k.A*k.A-k.B*k.B)/(2*k.A*k.B)
    if abs(c)>1+1e-9:continue
    for elbow in [math.acos(np.clip(c,-1,1)),-math.acos(np.clip(c,-1,1))]:
     shoulder=math.atan2(-v,-u)-math.atan2(k.B*math.sin(elbow),k.A+k.B*math.cos(elbow))
     for q2 in k.equivalents(shoulder,1):
      for q3 in k.equivalents(elbow,2):
       for q4 in k.equivalents(sign*math.pi/2-q2-q3,3):
        q=np.array([q1,q2,q3,q4,sign*math.pi/2,0.]);p,R=k.fk(q,base=H,tool=TOOL);phi=math.atan2(R[1,0],R[0,0])
        for desired in [yaw,yaw+math.pi]:
         for q6 in k.equivalents(phi-desired,5):
          qq=q.copy();qq[5]=q6
          if not any(np.max(np.abs(qq-s))<1e-6 for s in sols):sols.append(qq)
 return sols

def collide(a,b):return bool(fcl.collide(a,b,request,fcl.CollisionResult()))
class Cell:
 def __init__(self,a,b,H=.9):
  self.a=a;self.b=b;self.H=H;self.static=[('floor',box((4,4,.05),(0,0,-.026))),('pedestal',box((.2,.2,H-.002),(0,0,(H-.002)/2))),('pallet',box((1.1,1.1,.148),(-.700,0,.074)))]
  self.placed=[];self.calls=0
 def valid(self,q,loaded=True):
  self.calls+=1;Ts=transforms(q,self.H);ro=[obj(m,T) for m,T in zip(meshes,Ts)]
  for i in range(7):
   for j in range(i+2,7):
    if collide(ro[i],ro[j]):return False,('self',NAMES[i],NAMES[j])
  end=Ts[-1];R=end[:3,:3];tcp=end[:3,3]+R[:,2]*(.100+TOOL)
  tool=[("schmalz-cad",obj(TOOL_GEOM,end@tf(p=np.array([0,0,.100]))))]
  for idx,r in enumerate(ro):
   for name,e in self.static+self.placed:
    if idx==0 and name=='pedestal':continue
    if collide(r,e):return False,('environment',NAMES[idx],name)
  for tname,to in tool:
   for name,e in self.static+self.placed:
    if collide(to,e):return False,('tool',tname,name)
   for i in range(6):
    if collide(to,ro[i]):return False,('tool-self',tname,NAMES[i])
  if loaded:
   # Upright box, 1mm geometric contact tolerance per face (not clearance).
   Rb=R
   carry=obj(fcl.Box(self.a-.002,self.b-.002,.338),tf(Rb,tcp+R[:,2]*.17))
   for name,e in self.static+self.placed:
    if collide(carry,e):return False,('payload',name)
   for i,r in enumerate(ro):
    if collide(carry,r):return False,('payload-self',NAMES[i])
  return True,None
 def solutions(self,p,loaded=True):
  good=[];reasons={}
  for q in ik_all(*p,H=self.H):
   ok,why=self.valid(q,loaded)
   if ok:good.append(q)
   else:reasons[str(why)]=reasons.get(str(why),0)+1
  return good,reasons
 def add(self,x,y,top,index):
  self.placed.append((f'box{index}',box((self.a-.002,self.b-.002,.338),(x,y,top-.17))))

if __name__=='__main__':
 start=time.time();report=[]
 for am,bm,layers in [(360,280,3)]:
  cell=Cell(am/1000,bm/1000);a=am/1000;b=bm/1000
  targets=[(-x,y) for x in [.150+1.5*a+.010,.150+a/2] for y in [-b-.010,b+.010,0]]
  records=[];idx=0
  for layer in range(1,layers+1):
   for x,y in targets:
    top=.15+layer*.34;good,why=cell.solutions((x,y,top));ap,apwhy=cell.solutions((x,y,top+.1))
    idx+=1;records.append({'id':idx,'layer':layer,'target':[x,y,top],'valid_contact_solutions':len(good),'contact_rejections':why,'valid_approach_solutions':len(ap),'approach_rejections':apwhy})
    # Continue hypothetical filled pattern even after a failure to expose later collisions.
    cell.add(x,y,top,idx)
    print(am,bm,'target',idx,'contact',len(good),'approach',len(ap),flush=True)
  report.append({'box_mm':[am,bm,340],'layers':layers,'targets':records,'collision_calls':cell.calls})
 Path(__file__).with_name('static_results.json').write_text(json.dumps({'elapsed_s':time.time()-start,'results':report},ensure_ascii=False,indent=2))
 print('DONE',round(time.time()-start,1),flush=True)
