from verify import *
import copy
start=time.time()
def interpolate(points,step=.020):
 out=[np.array(points[0],float)]
 for p in points[1:]:
  p=np.array(p,float);prev=out[-1].copy();n=max(1,int(np.ceil(np.linalg.norm(p-prev)/step)))
  for i in range(1,n+1):out.append(prev+(p-prev)*i/n)
 return out

def move_points(cell,x,y,top,side):
 pickup=np.array([.450,0,.650]);ap=top+.100
 transit=max(.950,ap,max([.150]+[float(e.getTranslation()[2])+.169 for _,e in cell.placed])+.340+.030)
 nearx=-.150-cell.a/2
 entry=np.array([nearx,y,transit]);ang=math.atan2(y,nearx)
 if side>0 and ang<0:ang+=2*math.pi
 if side<0 and ang>0:ang-=2*math.pi
 # Alternative long arcs can trigger joint limits; retained as explicit candidates.
 points=[pickup,[.450,0,transit],[.400,0,transit]]
 for t in np.linspace(0,ang,max(2,int(abs(ang)/.10)+1))[1:]:points.append([.400*math.cos(t),.400*math.sin(t),transit])
 points.extend([entry,[nearx,y,ap],[x,y,ap],[x,y,top]])
 return interpolate(points),transit

def plan(cell,positions,startq):
 # Beam DP over IK branches; same branch continuity across Cartesian waypoints.
 prev=[(startq,0.,[startq])]
 for index,p in enumerate(positions[1:],1):
  candidates,_=cell.solutions(tuple(p));nxt=[]
  for q in candidates:
   possible=sorted([(cost+float(np.linalg.norm(q-old)),old,path) for old,cost,path in prev if np.max(np.abs(q-old))<.35],key=lambda t:t[0])
   for cost,old,path in possible:
    ok=True
    subdivisions=max(1,int(np.ceil(np.max(np.abs(q-old))/.008)))
    for t in range(1,subdivisions):
     qt=old+(q-old)*t/subdivisions
     if not cell.valid(qt,True)[0]:ok=False;break
    if ok:nxt.append((q,cost,path+[q]));break
  if not nxt:return None,{'waypoint':index,'point':p.tolist(),'valid_ik':len(candidates),'previous_states':len(prev)}
  prev=sorted(nxt,key=lambda t:t[1])[:16]
 return sorted(prev,key=lambda t:t[1])[0][2],None

report=[]
for am,bm,layers in [(360,280,3)]:
 initial=Cell(am/1000,bm/1000,H=.95);starts,_=initial.solutions((.450,0,.650));attempts=[];success=None
 for si,startq in enumerate(starts):
  cell=Cell(am/1000,bm/1000,H=.95);targets=[(-x,y) for x in [.150+1.5*cell.a+.010,.150+cell.a/2] for y in [-cell.b-.010,cell.b+.010,0]];records=[];good=True;paths=[]
  for layer in range(1,layers+1):
   for x,y in targets:
    top=.150+.340*layer;idx=len(records)+1;found=None;fail=[]
    for side in [1,-1]:
     positions,transit=move_points(cell,x,y,top,side);path,why=plan(cell,positions,startq)
     if path is None:fail.append({'side':side,'reason':why});continue
     cell.add(x,y,top,idx);revbad=None
     dense=[path[0]]
     for a,b in zip(path[:-1],path[1:]):
      n=max(1,int(np.ceil(np.max(np.abs(b-a))/.008)))
      dense.extend([a+(b-a)*t/n for t in range(1,n+1)])
     for qi in reversed(dense):
      ok,reason=cell.valid(qi,False)
      if not ok:revbad=reason;break
     if revbad:
      cell.placed.pop();fail.append({'side':side,'reverse':revbad});continue
     found={'id':idx,'layer':layer,'target':[x,y,top],'side':side,'transit_tcp_height_m':transit,'cartesian_waypoints':len(positions),'joint_samples_oneway':len(dense)};paths.append(np.array(dense));break
    if found is None:
     good=False;attempts.append({'start_branch':si,'failed_box':idx,'layer':layer,'failures':fail});print(am,bm,'BRANCH',si,'FAIL',idx,fail,flush=True);break
    records.append(found);print(am,bm,'branch',si,'box',idx,'OK',found['joint_samples_oneway'],flush=True)
   if not good:break
  if good:
   success={'start_branch':si,'records':records,'collision_calls':cell.calls,'paths':paths};break
 out={'box_mm':[am,bm,340],'layers':layers,'all_completed':success is not None,'attempts':attempts}
 if success:
  arrays=success.pop('paths');out.update(success)
  np.savez_compressed(Path(__file__).with_name(f'trajectory_950_{am}_{bm}.npz'),**{f'box_{i+1}':p for i,p in enumerate(arrays)})
 report.append(out)
 Path(__file__).with_name('path_results_950.json').write_text(json.dumps({'elapsed_s':time.time()-start,'results':report},ensure_ascii=False,indent=2))
print('DONE',round(time.time()-start,1),flush=True)
