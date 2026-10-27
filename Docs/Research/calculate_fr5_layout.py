"""FR5 downward-tool geometric feasibility study; SI units, no collision model.

Source: FAIR-INNOVATION/frcobot_ros2 fairino5_v6.urdf (2026-10-27).
The wrist3 link origin is NOT the physical tool flange. Add the 100 mm
axial flange offset from the FAIRINO catalogue drawing (printed pp.19/20).
This is an optimistic kinematic envelope, not validated robot motion planning.
"""
import json
import math
from pathlib import Path
import numpy as np
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib.patches import Rectangle, Circle

OUT = Path(__file__).resolve().parent
A, B, D1, D4, D5, D6 = .425, .39501, .152, .1021, .102, .100
TOOL, BASE, APPROACH, GAP = .125, 1.100, .100, .200
LIMITS = np.array([[-3.0543, 3.0543], [-4.6251, 1.4835],
                   [-2.7925268, 2.7925268], [-4.6251, 1.4835],
                   [-3.0543, 3.0543], [-3.0543, 3.0543]])
# Manufacturer elbow +/-160 deg overrides the slightly wider URDF +/-162 deg.

def rz(q):
    c, s = math.cos(q), math.sin(q)
    return np.array([[c,-s,0],[s,c,0],[0,0,1.]])

def rx(q):
    c, s = math.cos(q), math.sin(q)
    return np.array([[1.,0,0],[0,c,-s],[0,s,c]])

def fk(q, base=BASE, tool=TOOL):
    origins = [(0,0,0), (0,0,D1), (-A,0,0), (-B,0,0), (0,0,D4), (0,0,D5)]
    roll = [0,math.pi/2,0,0,math.pi/2,-math.pi/2]
    p, R = np.array([0.,0.,base]), np.eye(3)
    for xyz, alpha, theta in zip(origins, roll, q):
        p = p + R @ np.array(xyz)
        R = R @ rx(alpha) @ rz(theta)
    return p + R[:,2]*(D6+tool), R

def equivalents(q, i):
    return [q+2*math.pi*k for k in range(-2,3)
            if LIMITS[i,0] <= q+2*math.pi*k <= LIMITS[i,1]]

def ik_down(x,y,z,base=BASE,tool=TOOL):
    r2=x*x+y*y
    if r2 < D4*D4: return None
    for w in [math.sqrt(r2-D4*D4), -math.sqrt(r2-D4*D4)]:
        q1s=equivalents(math.atan2(y,x)-math.atan2(-D4,w),0)
        for sign in [1,-1]:
            u=w-sign*D5
            v=z-base-D1+D6+tool
            c=(u*u+v*v-A*A-B*B)/(2*A*B)
            if abs(c)>1+1e-10: continue
            for elbow in [math.acos(np.clip(c,-1,1)), -math.acos(np.clip(c,-1,1))]:
                shoulder=math.atan2(-v,-u)-math.atan2(B*math.sin(elbow),A+B*math.cos(elbow))
                for q2 in equivalents(shoulder,1):
                    for q3 in equivalents(elbow,2):
                        q4s=equivalents(sign*math.pi/2-q2-q3,3)
                        if q1s and q4s:
                            return np.array([q1s[0],q2,q3,q4s[0],sign*math.pi/2,0.])
    return None

def envelope(r,base=BASE,tool=TOOL,arm_reserve=0.):
    """Necessary outer envelope, allowing free yaw and all downward branches."""
    if r<D4: return None
    u=math.sqrt(r*r-D4*D4)-D5
    effective=A+B-arm_reserve
    if abs(u)>effective: return None
    v=math.sqrt(effective*effective-u*u)
    center=base+D1-D6-tool
    return center-v, center+v

def rotated_points(x,y):
    # x,y are pallet-local coordinates about its center.
    for theta in [0, math.pi/2, math.pi, 3*math.pi/2]:
        c,s=math.cos(theta),math.sin(theta)
        yield GAP+.55+c*x-s*y, s*x+c*y

def verify():
    rng=np.random.default_rng(55)
    checked=0
    for _ in range(2000):
        x,y,z=rng.uniform(.15,.9),rng.uniform(-.6,.6),rng.uniform(.3,1.9)
        q=ik_down(x,y,z)
        if q is not None:
            p,R=fk(q)
            assert np.linalg.norm(p-[x,y,z])<1e-9
            assert np.linalg.norm(R[:,2]-[0,0,-1])<1e-9
            assert np.all(q>=LIMITS[:,0]) and np.all(q<=LIMITS[:,1])
            checked+=1
    assert checked>100
    # Analytic maximum: fully extended arms upward, downward flange axis.
    q=np.deg2rad([0,-90,0,-180,90,0])
    p,R=fk(q)
    assert abs(p[2]-(BASE+D1+A+B-D6-TOOL))<1e-9
    assert np.linalg.norm(R[:,2]-[0,0,-1])<1e-9
    return checked

def main():
    checked=verify()
    radii=[.30,.40,.50,.60,.70,.75,.80,.90]
    table=[]
    for r in radii:
        lo,hi=envelope(r)
        table.append(dict(radius_m=r,contact_min_m=lo,contact_max_m=hi,
                          stack_max_with_100mm_approach_m=hi-APPROACH))
    cases=[]
    for box in [.25,.30,.50]:
        x,y=GAP+1.1-box/2,.55-box/2
        cases.append(dict(box_width_m=box,far_center_xy_m=[x,y],
                          radius_m=math.hypot(x,y),envelope=envelope(math.hypot(x,y))))
    # Four-way pallet rotation on a flush turntable. Vertical axis range
    # is explored, not assumed to be a certified machine design.
    rotated_failures=[]
    checked_targets=0
    for x in np.linspace(-.425,.425,9):
        for y in np.linspace(-.425,.425,9):
            px,py=min(rotated_points(x,y),key=lambda p:math.hypot(*p))
            r=math.hypot(px,py)
            for top in np.linspace(.35,2.15,19):
                lo,hi=envelope(r,base=0.,arm_reserve=.020)
                min_base=top+APPROACH-hi
                max_base=top-lo
                h=max(.750,min_base)
                ok=h<=min(1.900,max_base)+1e-9
                if ok:
                    for z in np.linspace(top,top+APPROACH,6):
                        if ik_down(px,py,z,base=h) is None: ok=False
                if not ok: rotated_failures.append([float(x),float(y),float(top),float(h)])
                checked_targets+=1
    suction=dict(box_kg=3.,tool_mass_budget_kg=.8,cup_count=4,
                 effective_cup_diameter_m=.05,vacuum_Pa=30000,
                 area_efficiency=.5,safety_factor=2.5,acceleration_m_s2=1.,mu=.3)
    suction['estimated_available_N']=4*math.pi*.05**2/4*30000*.5
    suction['required_vertical_N']=3*(9.81+1)*2.5
    suction['required_horizontal_N']=3*(9.81+1/.3)*2.5
    rotary_swept_radius=1.1/math.sqrt(2)
    rotary_clearance=dict(square_pallet_swept_radius_m=rotary_swept_radius,
        pedestal_half_width_m=.15,assumed_clearance_m=.05,
        minimum_slide_away_m=rotary_swept_radius+.15+.05-(GAP+.55),
        proposed_slide_away_m=.25,
        pedestal_clearance_after_slide_m=GAP+.55+.25-rotary_swept_radius-.15,
        note='Bare pallet planar envelope only; retract robot first. Does not validate loaded-stack swept collisions.')
    result=dict(parameters=dict(base_m=BASE,tool_flange_to_contact_m=TOOL,
        wrist_origin_to_flange_m=D6,pallet_m=[1.1,1.1,.15],
        base_axis_to_near_edge_m=GAP,approach_m=APPROACH),
        analytic_IK_FK_verified_solutions=checked,
        absolute_contact_height_upper_bound_m=BASE+D1+A+B-D6-TOOL,
        radial_outer_bound_m=math.hypot(A+B+D5,D4),
        radius_table=table,far_box_cases=cases,
        rotary_lift_grid=dict(base_range_m=[.75,1.9],positions=81,heights=19,
            target_pairs=checked_targets,failures=rotated_failures,
            note='Only point reach and sampled vertical segments; no collision or rotation path checks'),
        rotary_planar_clearance=rotary_clearance,
        suction_assumptions_and_calculation=suction)
    (OUT/'fr5_layout_results.json').write_text(json.dumps(result,indent=2)+'\n')
    fig,axs=plt.subplots(1,2,figsize=(13,5.8),layout='constrained')
    ax=axs[0]
    xx,yy=np.meshgrid(np.linspace(GAP,GAP+1.1,111),np.linspace(-.55,.55,111))
    zz=np.full(xx.shape,np.nan)
    for i in range(xx.shape[0]):
        for j in range(xx.shape[1]):
            e=envelope(math.hypot(xx[i,j],yy[i,j]))
            if e: zz[i,j]=e[1]-APPROACH
    ax.set_facecolor('#e2e8f0')
    im=ax.pcolormesh(xx,yy,zz,cmap='viridis',vmin=.8,vmax=1.8,shading='auto')
    ax.add_patch(Rectangle((GAP,-.55),1.1,1.1,fill=False,lw=2))
    ax.add_patch(Rectangle((-.15,-.15),.3,.3,facecolor='#334155'))
    ax.scatter([0],[0],color='#ef4444',s=50,zorder=5)
    ax.annotate('Robot axis',(0,0),xytext=(-.10,-.32),arrowprops=dict(arrowstyle='->'))
    ax.annotate('200 mm axis-to-edge',(.10,0),xytext=(.25,-.73),arrowprops=dict(arrowstyle='->'))
    ax.text(.83,.62,'1,100 x 1,100 mm pallet',ha='center')
    ax.text(1.14,0,'Outside\nradial reach',ha='center',va='center',color='#475569',fontsize=10)
    ax.set(xlim=(-.25,1.4),ylim=(-.82,.78),aspect='equal',xlabel='Forward distance from robot axis (m)',ylabel='Lateral distance (m)',title='Fixed base: upper stack-height envelope')
    fig.colorbar(im,ax=ax,label='Stack top incl. 100 mm approach (m)',shrink=.75)
    ax=axs[1]
    rr=np.linspace(D4+.001,.927,500)
    low,high=[],[]
    for r in rr:
        e=envelope(r)
        low.append(e[0] if e else np.nan)
        high.append(e[1] if e else np.nan)
    ax.fill_between(rr,low,high,color='#bae6fd',label='Optimistic downward TCP envelope')
    ax.plot(rr,np.array(high)-APPROACH,color='#0284c7',label='Stack top with 100 mm approach')
    ax.axhline(2.,color='#ef4444',ls='--',label='2.00 m floor-based stack top')
    ax.axhline(2.15,color='#b91c1c',ls=':',label='2.00 m cargo + 0.15 m pallet')
    ax.axhline(.15,color='#64748b',ls=':')
    ax.scatter([.75],[envelope(.75)[1]-.1],color='#0284c7')
    ax.annotate('Pallet center: r = 0.75 m',(.75,envelope(.75)[1]-.1),xytext=(.3,1.12),arrowprops=dict(arrowstyle='->'))
    ax.set(xlim=(.1,.95),ylim=(0,2.35),xlabel='Horizontal distance from robot axis (m)',ylabel='TCP height above floor (m)',title='Base 1.10 m / flange-to-cup 125 mm')
    ax.grid(alpha=.2)
    ax.legend(loc='lower left',fontsize=8)
    fig.suptitle('FR5 geometric screening - no collision, load or trajectory validation',fontsize=13)
    fig.savefig(OUT/'fr5_layout_envelope.png',dpi=180)
    plt.close(fig)
    print(json.dumps(result,indent=2))

if __name__=='__main__': main()
