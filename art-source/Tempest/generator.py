"""Aster Tempest R - original sports car generator (Blender 5.x, background).
Run:  blender -b --python generator.py [-- --norender]
Blender axes while modelling: +Y = car forward, +Z up, +X = car right. Units: metres.
Outputs next to this file: Tempest_R.blend, Tempest_R.fbx, render_*.png, verification.json
"""
import bpy, bmesh, math, os, sys, json
import numpy as np
from mathutils import Vector, Matrix, Euler, Quaternion

OUT = os.path.dirname(os.path.abspath(__file__))
NORENDER = "--norender" in sys.argv
bpy.ops.wm.read_factory_settings(use_empty=True)
V = Vector

# ------------------------------------------------------------------ materials
def mat(name, color, metallic=0.0, rough=0.5, emit=None, estr=0.0, coat=0.0, alpha=1.0):
    m = bpy.data.materials.new(name)
    m.use_nodes = True
    b = m.node_tree.nodes["Principled BSDF"]
    b.inputs["Base Color"].default_value = (*color, 1)
    b.inputs["Metallic"].default_value = metallic
    b.inputs["Roughness"].default_value = rough
    if coat:
        b.inputs["Coat Weight"].default_value = coat
        b.inputs["Coat Roughness"].default_value = 0.03
    if emit:
        b.inputs["Emission Color"].default_value = (*emit, 1)
        b.inputs["Emission Strength"].default_value = estr
    b.inputs["Alpha"].default_value = alpha
    m.diffuse_color = (*color, 1)
    m.metallic = metallic
    m.roughness = rough
    return m

M_PAINT = mat("TempestBlue_Paint", (0.008, 0.035, 0.21), 0.8, 0.22, coat=1.0)
M_CARBON = mat("Charcoal_Aero", (0.035, 0.037, 0.042), 0.35, 0.42)
M_GLASS = mat("Glass_Tint", (0.004, 0.006, 0.01), 0.0, 0.03)
M_ALLOY = mat("Alloy_Gunmetal", (0.30, 0.31, 0.33), 1.0, 0.22)
M_TIRE = mat("Tire_Rubber", (0.025, 0.025, 0.027), 0.0, 0.88)
M_ROTOR = mat("Brake_Rotor", (0.42, 0.42, 0.44), 0.95, 0.40)
M_CALIPER = mat("Brake_Caliper", (0.85, 0.30, 0.02), 0.2, 0.35)
M_LENS = mat("Headlight_Lens", (0.04, 0.05, 0.07), 0.0, 0.02, emit=(0.5, 0.7, 1.0), estr=0.25)
M_DRL = mat("DRL_White", (1, 1, 1), 0.0, 0.2, emit=(0.9, 0.95, 1.0), estr=6.0)
M_TAIL = mat("Taillight_Red", (0.5, 0.0, 0.0), 0.0, 0.15, emit=(1.0, 0.02, 0.01), estr=5.0)
M_BLACK = mat("Trim_Black", (0.005, 0.005, 0.006), 0.2, 0.55)
M_SEAM = mat("Panel_Seam", (0.002, 0.002, 0.003), 0.0, 0.7)
M_STEEL = mat("Exhaust_Steel", (0.55, 0.56, 0.58), 1.0, 0.3)

# ------------------------------------------------------------------ helpers
def sgn(a): return np.sign(a)

def new_obj(name, bm, mats, sharp_deg=40):
    bm.normal_update()
    lim = math.radians(sharp_deg)
    for e in bm.edges:
        if len(e.link_faces) == 2:
            e.smooth = e.calc_face_angle(0) < lim
    for f in bm.faces: f.smooth = True
    me = bpy.data.meshes.new(name)
    bm.to_mesh(me); bm.free()
    for m in mats: me.materials.append(m)
    o = bpy.data.objects.new(name, me)
    bpy.context.scene.collection.objects.link(o)
    return o

def dedupe(vs):
    out = []
    for v in vs:
        if not out or out[-1] is not v: out.append(v)
    if len(out) > 1 and out[0] is out[-1]: out.pop()
    return out

def loft(bm, rings, mi=0, cap0=False, cap1=False, closed=True):
    vs = []
    for ring in rings:
        pts = [V(p) for p in ring]
        if max((p - pts[0]).length for p in pts) < 1e-6:
            v = bm.verts.new(pts[0]); vs.append([v] * len(pts))
        else:
            vs.append([bm.verts.new(p) for p in pts])
    n = len(rings[0]); fs = []
    for i in range(len(rings) - 1):
        for j in range(n if closed else n - 1):
            j2 = (j + 1) % n
            q = dedupe([vs[i][j], vs[i][j2], vs[i + 1][j2], vs[i + 1][j]])
            if len(q) >= 3:
                try:
                    f = bm.faces.new(q); f.material_index = mi; fs.append(f)
                except ValueError: pass
    for flag, ring in ((cap0, vs[0]), (cap1, vs[-1])):
        if flag:
            q = dedupe(ring)
            if len(q) >= 3:
                try:
                    f = bm.faces.new(q); f.material_index = mi; fs.append(f)
                except ValueError: pass
    return fs

def solid_fix(bm):
    bmesh.ops.remove_doubles(bm, verts=bm.verts[:], dist=1e-5)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces[:])

def orient(bm, point, away=True):
    for f in bm.faces:
        d = (f.calc_center_median() - point)
        if (f.normal.dot(d) < 0) == away: f.normal_flip()

def ellipsoid(c, r, rot=(0, 0, 0), e=2.0, nlon=24, nlat=14, mi=0, bm=None):
    own = bm is None
    if own: bm = bmesh.new()
    rings = []
    for k in range(nlat + 1):
        phi = -math.pi / 2 + math.pi * k / nlat
        cp, sp = math.cos(phi), math.sin(phi)
        rr = sgn(cp) * abs(cp) ** (2 / e) if e != 2 else cp
        zz = sgn(sp) * abs(sp) ** (2 / e) if e != 2 else sp
        ring = []
        for i in range(nlon):
            th = 2 * math.pi * i / nlon
            ct, st = math.cos(th), math.sin(th)
            x = (sgn(ct) * abs(ct) ** (2 / e) if e != 2 else ct) * rr
            y = (sgn(st) * abs(st) ** (2 / e) if e != 2 else st) * rr
            ring.append((x * r[0], y * r[1], zz * r[2]))
        rings.append(ring)
    R = Euler(rot).to_matrix()
    rings = [[(R @ V(p)) + V(c) for p in ring] for ring in rings]
    loft(bm, rings, mi)
    if own:
        solid_fix(bm); return bm

def tube(bm, pts, rx, ry, n=10, mi=0, caps=True, ref=V((1, 0, 0))):
    """lofted tube along polyline pts; rx/ry are floats or functions of t; cross-section
    axes: ref (projected) and tangent x ref."""
    pts = [V(p) for p in pts]; rings = []
    for i, p in enumerate(pts):
        t = i / (len(pts) - 1)
        T = (pts[min(i + 1, len(pts) - 1)] - pts[max(i - 1, 0)]).normalized()
        A = (ref - T * ref.dot(T)).normalized()
        B = T.cross(A).normalized()
        a = rx(t) if callable(rx) else rx
        b = ry(t) if callable(ry) else ry
        rings.append([p + A * a * math.cos(2 * math.pi * k / n) + B * b * math.sin(2 * math.pi * k / n) for k in range(n)])
    loft(bm, rings, mi, caps, caps)

def revolve(bm, prof, x0=0.0, segs=64, mi=0, closed_prof=True):
    """revolve (r, x) profile around X axis (centre at x0)."""
    rings = []
    for k in range(segs):
        a = 2 * math.pi * k / segs
        rings.append(None)
    # build as loft along the profile: each 'ring' is the circle at that profile point
    circ = []
    for (r, x) in prof:
        circ.append([(x0 + x, r * math.cos(2 * math.pi * k / segs), r * math.sin(2 * math.pi * k / segs)) for k in range(segs)])
    loft(bm, circ, mi, closed=True)
    if closed_prof:
        loft(bm, [circ[-1], circ[0]], mi)

def smooth_interp(keys, ys, sig=0.12):
    k = np.array(keys, float)
    v = np.interp(ys, k[:, 0], k[:, 1])
    dy = ys[1] - ys[0]
    h = max(1, int(sig * 3 / dy))
    ker = np.exp(-0.5 * (np.arange(-h, h + 1) * dy / sig) ** 2); ker /= ker.sum()
    vp = np.concatenate([np.full(h, v[0]), v, np.full(h, v[-1])])
    return np.convolve(vp, ker, mode="valid")

# ------------------------------------------------------------------ body profile
L = 4.58; HL = L / 2
NS = 120
YS = np.linspace(-HL + 0.02, HL, NS)
def prof(keys, sig=0.10):
    return smooth_interp(keys, np.linspace(-HL - 0.4, HL + 0.4, 700), sig), np.linspace(-HL - 0.4, HL + 0.4, 700)

def mkfun(keys, sig=0.10):
    v, yy = prof(keys, sig)
    return lambda y: float(np.interp(y, yy, v))

def ext(keys):  # extend end keys beyond body for smoothing
    return [(-HL - 0.4, keys[0][1])] + keys + [(HL + 0.4, keys[-1][1])]

W_f = mkfun(ext([(-2.29, .74), (-2.0, .90), (-1.7, .965), (-1.38, .975), (-1.0, .965), (-0.5, .92), (0.0, .905), (0.5, .93),
                 (1.0, .965), (1.38, .975), (1.75, .94), (2.05, .84), (2.29, .66)]), .09)
ZT_f = mkfun(ext([(-2.29, .74), (-2.05, .88), (-1.7, .95), (-1.3, .975), (-0.8, .92), (-0.3, .86), (0.4, .825), (0.9, .80), (1.38, .835),
                  (1.6, .79), (1.9, .64), (2.29, .46)]), .10)
ZB_f = mkfun(ext([(-2.29, .32), (-2.0, .21), (-1.6, .125), (1.2, .125), (1.9, .14), (2.29, .24)]), .08)
NU, NL = 3.4, 4.5
TAPER0 = 2.05

def body_dims(y):
    W, Zt, Zb = W_f(y), ZT_f(y), ZB_f(y)
    f = 1.0
    if y < -2.0:
        s = (-2.0 - y) / 0.29
        W *= 1 - 0.24 * s ** 1.5
    if y > TAPER0:
        s = (y - TAPER0) / (HL - TAPER0)
        f = math.sqrt(max(0.0, 1 - s * s))
    return W * f, Zt, Zb, f

def ring_pts(y, N=64, nu=NU, nl=NL, inset=0.0):
    W, Zt, Zb, f = body_dims(y)
    Zt -= inset; Zb += inset; W = max(W - inset, 0)
    zc = (Zt + Zb) / 2; H = (Zt - Zb) / 2 * f
    pts = []
    for i in range(N):
        th = 2 * math.pi * i / N - math.pi / 2
        c, s = math.cos(th), math.sin(th)
        n = nu if s > 0 else nl
        xx = W * sgn(c) * abs(c) ** (2 / n)
        zz = zc + H * sgn(s) * abs(s) ** (2 / n) - (crown(y, xx) if s > 0 and inset == 0 else 0)
        pts.append((xx, y, zz))
    return pts

def sstep(a, b, y):
    t = min(max((y - a) / (b - a), 0.0), 1.0); return t * t * (3 - 2 * t)

def crown(y, x):
    return 0.05 * sstep(0.6, 1.3, y) * (1 - sstep(1.85, 2.2, y)) * math.exp(-(x / 0.30) ** 2)

def top_z(y, x):
    W, Zt, Zb, f = body_dims(y)
    zc = (Zt + Zb) / 2; H = (Zt - Zb) / 2 * f
    r = min(abs(x) / max(W, 1e-6), 1.0)
    return zc + H * (1 - r ** NU) ** (1 / NU) - crown(y, x)

def side_x(y, z):
    W, Zt, Zb, f = body_dims(y)
    zc = (Zt + Zb) / 2; H = (Zt - Zb) / 2 * f
    n = NU if z > zc else NL
    r = min(abs(z - zc) / H, 1.0)
    return W * (1 - r ** n) ** (1 / n)

def surf_frame(y, x):
    """position + outward normal on the upper surface."""
    z = top_z(y, x); d = 0.01
    dzdx = (top_z(y, x + d) - top_z(y, x - d)) / (2 * d)
    dzdy = (top_z(y + d, x) - top_z(y - d, x)) / (2 * d)
    n = V((-dzdx, -dzdy, 1)).normalized()
    return V((x, y, z)), n

def rot_to_normal(n, yaw_ref=V((0, 1, 0))):
    z = n.normalized()
    y = (yaw_ref - z * yaw_ref.dot(z)).normalized()
    x = y.cross(z)
    return Matrix((x, y, z)).transposed().to_euler()

objs = []

# ------------------------------------------------------------------ main body shell
bm = bmesh.new()
rings = [ring_pts(y) for y in YS]
loft(bm, rings, 0, cap0=False, cap1=True)
rr_ = rings[0]; N_ = len(rr_)
a_ = 0
while a_ + 1 < N_ // 2 and rr_[a_ + 1][2] <= 0.50 and rr_[N_ - a_ - 1][2] <= 0.50: a_ += 1
vlow = [bm.verts.new(rr_[i % N_]) for i in range(-a_, a_ + 1)]
vup = [bm.verts.new(rr_[i % N_]) for i in range(a_, N_ - a_ + 1)]
try:
    f1 = bm.faces.new(vlow); f1.material_index = 1
    f2 = bm.faces.new(vup); f2.material_index = 0
except ValueError as ex: print("cap fail", ex)
bmesh.ops.remove_doubles(bm, verts=bm.verts[:], dist=1e-5)
bmesh.ops.recalc_face_normals(bm, faces=bm.faces[:])
body_main = new_obj("Body_Shell", bm, [M_PAINT, M_CARBON])
# charcoal rear tail cap, mark faces after creation below

# inner shell (used to make recessed intake fills)
bm = bmesh.new()
loft(bm, [ring_pts(y, 40, inset=0.045) for y in YS[::2]], 0, True, True)
solid_fix(bm)
inner = new_obj("InnerShell", bm, [M_BLACK])

# ------------------------------------------------------------------ cabin / greenhouse
CY = [(1.20, .74), (1.05, .79), (0.85, .90), (0.55, 1.03), (0.25, 1.115), (-0.05, 1.148), (-0.40, 1.143), (-0.75, 1.09),
      (-1.10, 1.01), (-1.45, .90), (-1.62, .82)]
CW = [(1.20, .66), (0.85, .72), (0.40, .735), (-0.2, .735), (-0.8, .70), (-1.3, .60), (-1.62, .50)]
cy_ = np.linspace(-1.62, 1.20, 64)
cyv, cyy = smooth_interp([(a, b) for a, b in sorted(CY)], cy_, 0.07), cy_
cwv = smooth_interp(sorted(CW), cy_, 0.12)
bm = bmesh.new()
cab_rings = []
for y, zt, w in zip(cy_, cyv, cwv):
    zb = 0.58
    zc = (zt + zb) / 2; H = (zt - zb) / 2
    ring = []
    for i in range(40):
        th = 2 * math.pi * i / 40 - math.pi / 2
        c, s = math.cos(th), math.sin(th)
        n = 3.2 if s > 0 else 3.0
        ring.append((w * sgn(c) * abs(c) ** (2 / n), y, zc + H * sgn(s) * abs(s) ** (2 / n)))
    cab_rings.append(ring)
loft(bm, cab_rings, 0, True, True)
solid_fix(bm)
for f in bm.faces:
    c = f.calc_center_median(); n = f.normal
    paint = (n.z > 0.45 and -0.85 < c.y < 0.22 and abs(c.x) < 0.60) or (c.y < -0.75 and abs(n.x) > 0.25) or c.z < 0.88
    f.material_index = 0 if paint else 1
cabin = new_obj("Cabin", bm, [M_PAINT, M_GLASS], 55)

# ------------------------------------------------------------------ boolean cutters (intakes, vents) + dark fills
cut_bm = bmesh.new(); fill_bm = bmesh.new()
def cutter(c, r, rot=(0, 0, 0), e=2.0, fill=True):
    ellipsoid(c, r, rot, e, 20, 12, bm=cut_bm)
    if fill: ellipsoid(c, (r[0] * .97, r[1] * .97, r[2] * .97), rot, e, 20, 12, bm=fill_bm)

for sx in (-1, 1):
    # side intake scoop ahead of rear wheel
    y, z = -0.72, 0.52
    cutter((sx * (side_x(y, z) + 0.01), y, z), (0.16, 0.20, 0.15), (0, 0, 0.15 * sx))
    # small front-fender vent behind front wheel
    for k in range(3):
        y, z = 0.98 - 0.0 + k * 0.09 * 0, 0.42 + 0.075 * k
        cutter((sx * (side_x(1.00, z) + 0.01), 1.00, z), (0.05, 0.13, 0.026), (0, 0, 0), 2.0)
    # front corner intakes
    cutter((sx * 0.60, 2.03, 0.33), (0.19, 0.17, 0.08), (0, 0, -0.35 * sx))
    # hood vents (heat extractors)
    for k in range(2):
        yy = 1.20 + 0.0; xx = sx * (0.30 + 0.12 * k)
        p, n = surf_frame(1.30 + 0.02 * k, xx)
        cutter(p + n * 0.005, (0.03, 0.17, 0.035), tuple(rot_to_normal(n)), 2.0)
    # engine cover louvres
    for k in range(5):
        yy = -1.28 - 0.1 * k
        p, n = surf_frame(yy, sx * 0.34)
        cutter(p + n * 0.003, (0.22, 0.022, 0.03), tuple(rot_to_normal(n)), 2.0)
# central lower nose intake
cutter((0, 2.215, 0.28), (0.42, 0.13, 0.085), (0, 0, 0), 2.4)
# hood/tail details
cb = cut_bm; fb = fill_bm
solid_fix(cb)
cut_obj = new_obj("cutter", cb, [M_BLACK]); fill_obj = new_obj("filler", fb, [M_BLACK])

# wheel arches
WH_X, WH_Y, WH_Z = 0.84, 1.38, 0.345
AR = 0.375
arch_bm = bmesh.new()
for sx in (-1, 1):
    for sy in (-1, 1):
        bmesh.ops.create_cone(arch_bm, cap_ends=True, cap_tris=False, segments=72, radius1=AR, radius2=AR, depth=0.50,
                              matrix=Matrix.Translation(V((sx * (WH_X + 0.03), sy * WH_Y, WH_Z))) @ Matrix.Rotation(math.pi / 2, 4, 'Y'))
arch_obj = new_obj("arch_cut", arch_bm, [M_BLACK])

def bool_apply(target, other, op='DIFFERENCE'):
    bpy.context.view_layer.objects.active = target
    for o in bpy.context.selected_objects: o.select_set(False)
    target.select_set(True)
    m = target.modifiers.new("b", 'BOOLEAN'); m.operation = op; m.object = other; m.solver = 'EXACT'
    bpy.ops.object.modifier_apply(modifier=m.name)

# fills = inner shell âˆ© fillers
bool_apply(fill_obj, inner, 'INTERSECT')
bool_apply(body_main, cut_obj)
bool_apply(body_main, arch_obj)
bpy.data.objects.remove(cut_obj); bpy.data.objects.remove(arch_obj); bpy.data.objects.remove(inner)
for f in fill_obj.data.polygons: f.material_index = 0
# soften arch edges
bpy.context.view_layer.objects.active = body_main
bm = bmesh.new(); bm.from_mesh(body_main.data)
# charcoal tail panel: faces on the rear cap plane
for f in bm.faces:
    if f.calc_center_median().y < -HL + 0.01 and abs(f.normal.y) > 0.98:
        f.material_index = 1
bm.to_mesh(body_main.data); bm.free()
bev = body_main.modifiers.new("bev", 'BEVEL'); bev.width = 0.007; bev.segments = 1; bev.limit_method = 'ANGLE'; bev.angle_limit = math.radians(40)
bpy.ops.object.modifier_apply(modifier="bev")
fill_obj.data.materials.clear(); fill_obj.data.materials.append(M_BLACK)
objs += [body_main, cabin, fill_obj]

# ------------------------------------------------------------------ wheel arch liners
def arch_liner(sx, sy):
    bm = bmesh.new()
    rings = []
    for i in range(6):
        arc = []
        for j in range(30):
            a = math.radians(-12 + 204 * j / 29)
            yy_ = sy * WH_Y + math.cos(a) * 0.366; zz_ = WH_Z + math.sin(a) * 0.366
            xo = 0.62
            for xt in np.linspace(0.62, 0.92, 16):
                if top_z(yy_, xt) - 0.025 > zz_ and side_x(yy_, zz_) - 0.03 > xt: xo = xt
                else: break
            x = sx * (0.60 + (xo - 0.60) * i / 5)
            arc.append((x, yy_, zz_))
        rings.append(arc)
    loft(bm, rings, 0, closed=False)
    orient(bm, V((sx * 0.8, sy * WH_Y, WH_Z)), away=False)
    return new_obj(f"liner_{sx}_{sy}", bm, [M_BLACK], 80)
for sx in (-1, 1):
    for sy in (-1, 1): objs.append(arch_liner(sx, sy))

# ------------------------------------------------------------------ aero: splitter, diffuser, side skirts
def slab(poly, z0, z1, mat_i=0):
    bm = bmesh.new()
    vs = [bm.verts.new((x, y, z0)) for x, y in poly]
    f = bm.faces.new(vs)
    r = bmesh.ops.extrude_face_region(bm, geom=[f])
    vv = [e for e in r["geom"] if isinstance(e, bmesh.types.BMVert)]
    for v in vv: v.co.z = z1
    solid_fix(bm)
    return bm

# front splitter
poly = []
for i in range(41):
    x = -0.86 + 1.72 * i / 40
    y = 2.265 - 0.20 * (abs(x) / 0.86) ** 2.6
    poly.append((x, y))
for i in range(41):
    x = 0.78 - 1.56 * i / 40
    poly.append((x, 1.78 - 0.0 + 0.1 * (abs(x) / 0.78) ** 2))
poly = [p for p in poly]
bm = slab(poly, 0.085, 0.118)
bmesh.ops.bevel(bm, geom=[e for e in bm.edges], offset=0.006, segments=1, affect='EDGES')
objs.append(new_obj("Splitter", bm, [M_CARBON], 30))

# rear diffuser with fins
bm = bmesh.new()
for k in range(-3, 4):
    x = k * 0.115
    prof_ = [(-1.7, 0.115), (-2.29, 0.30), (-2.29, 0.095), (-2.05, 0.095), (-1.7, 0.095)]
    vs0 = [bm.verts.new((x - 0.006, y, z)) for y, z in prof_]
    f = bm.faces.new(vs0)
    r = bmesh.ops.extrude_face_region(bm, geom=[f])
    for v in [e for e in r["geom"] if isinstance(e, bmesh.types.BMVert)]: v.co.x += 0.012
solid_fix(bm)
objs.append(new_obj("Diffuser", bm, [M_CARBON], 30))
# diffuser floor
bm = slab([(-0.55, -1.55), (0.55, -1.55), (0.62, -2.285), (-0.62, -2.285)], 0.093, 0.108)
objs.append(new_obj("DiffFloor", bm, [M_CARBON], 30))

# ------------------------------------------------------------------ rear wing
bm = bmesh.new()
def foil(c, t=0.15, n=22, camber=0.05):
    xs = [(1 - math.cos(math.pi * i / n)) / 2 for i in range(n + 1)]
    up, lo = [], []
    for x in xs:
        yt = 5 * t * (0.2969 * math.sqrt(x) - 0.1260 * x - 0.3516 * x * x + 0.2843 * x ** 3 - 0.1036 * x ** 4)
        m = -camber * 4 * x * (1 - x)
        up.append((x, m + yt)); lo.append((x, m - yt))
    return up + lo[::-1][1:-1]
sec = foil(1)
WING_Y, WING_Z = -2.06, 1.085
wr = []
for i in range(41):
    u = i / 40; x = -0.76 + 1.52 * u
    ch = 0.31 - 0.04 * abs(2 * u - 1) ** 2
    sweep = 0.05 * abs(2 * u - 1) ** 2
    ang = math.radians(-9)
    ring = []
    for (xx, zz) in sec:
        py = (0.5 - xx) * ch; pz = zz * ch
        yy = py * math.cos(ang) - pz * math.sin(ang); zz2 = py * math.sin(ang) + pz * math.cos(ang)
        ring.append((x, WING_Y + yy + sweep, WING_Z + zz2 + 0.015 * abs(2 * u - 1) ** 2))
    wr.append(ring)
loft(bm, wr, 0, True, True); solid_fix(bm)
objs.append(new_obj("Wing", bm, [M_CARBON], 60))
# end plates
for sx in (-1, 1):
    bm = bmesh.new()
    pl = [(-2.215 + 0.10, 1.15), (-2.215 + 0.04, 1.02), (-1.90, 1.00), (-1.86, 1.10), (-1.92, 1.19), (-2.18, 1.20)]
    pl = [(WING_Y + 0.17, WING_Z + 0.04), (WING_Y + 0.14, WING_Z - 0.085), (WING_Y - 0.12, WING_Z - 0.07), (WING_Y - 0.19, WING_Z + 0.02), (WING_Y - 0.16, WING_Z + 0.075), (WING_Y + 0.05, WING_Z + 0.08)]
    vs0 = [bm.verts.new((sx * 0.765, y, z)) for y, z in pl]
    f = bm.faces.new(vs0)
    r = bmesh.ops.extrude_face_region(bm, geom=[f])
    for v in [e for e in r["geom"] if isinstance(e, bmesh.types.BMVert)]: v.co.x += sx * 0.012
    solid_fix(bm)
    bmesh.ops.bevel(bm, geom=bm.edges[:], offset=0.004, segments=1, affect='EDGES')
    objs.append(new_obj("WingPlate", bm, [M_CARBON], 40))
# swan-neck uprights
for sx in (-1, 1):
    bm = bmesh.new()
    path = []
    for i in range(14):
        t = i / 13
        path.append((sx * (0.38 + 0.0 * t), -1.80 - 0.26 * t ** 1.6 + 0.0, top_z(-1.80, 0.38) - 0.03 + (WING_Z - 0.03 - top_z(-1.80, 0.38)) * (1 - (1 - t) ** 1.8)))
    tube(bm, path, lambda t: 0.014, lambda t: 0.055 - 0.015 * t, 10, 0, True, ref=V((1, 0, 0)))
    solid_fix(bm)
    objs.append(new_obj("WingStrut", bm, [M_CARBON], 60))

# ------------------------------------------------------------------ lights
for sx in (-1, 1):
    # headlight: bezel + lens + DRL
    def lpath(off, lat=0.0):
        out = []
        for t in np.linspace(0, 1, 18):
            x = 0.30 + 0.40 * t; y = 2.03 - 0.25 * t ** 1.2 + lat
            p, n = surf_frame(y, x)
            out.append(V((sx * p.x, p.y, p.z)) + V((sx * n.x, n.y, n.z)) * off)
        return out
    _, nc = surf_frame(1.9, 0.5); nc = V((sx * nc.x, nc.y, nc.z))
    bm = bmesh.new()
    tube(bm, lpath(0.000), lambda t: 0.016, lambda t: 0.020 + 0.040 * math.sin(math.pi * min(max(t * 0.9 + 0.05, 0), 1)) ** 0.7, 12, 0, True, ref=nc)
    solid_fix(bm); objs.append(new_obj("HeadBezel", bm, [M_BLACK], 60))
    bm = bmesh.new()
    tube(bm, lpath(0.004), lambda t: 0.014, lambda t: 0.014 + 0.030 * math.sin(math.pi * min(max(t * 0.9 + 0.05, 0), 1)) ** 0.7, 12, 0, True, ref=nc)
    solid_fix(bm); objs.append(new_obj("HeadLens", bm, [M_LENS], 60))
    bm = bmesh.new()
    tube(bm, lpath(0.013, -0.018), 0.004, 0.0045, 6, 0, True, ref=nc)
    solid_fix(bm); objs.append(new_obj("DRL", bm, [M_DRL], 60))

# tail lights: bar + corner units + exhaust
bm = bmesh.new(); ellipsoid((0, -2.272, 0.64), (0.44, 0.018, 0.026), (0, 0, 0), 2.6, 32, 10, bm=bm); solid_fix(bm)
objs.append(new_obj("TailBar", bm, [M_TAIL], 60))
for sx in (-1, 1):
    bm = bmesh.new(); ellipsoid((sx * 0.43, -2.272, 0.60), (0.09, 0.018, 0.045), (0, 0, 0), 2.6, 28, 12, bm=bm); solid_fix(bm)
    objs.append(new_obj("TailLamp", bm, [M_TAIL], 60))
    bm = bmesh.new()
    prof_ = [(0.052, -0.05), (0.060, -0.045), (0.060, 0.0), (0.050, 0.0), (0.050, -0.045)]
    revolve(bm, [(r, y) for r, y in prof_], 0, 40, 0, True)
    # revolve around X; we need an axis along Y -> rotate
    solid_fix(bm)
    bmesh.ops.rotate(bm, verts=bm.verts, cent=(0, 0, 0), matrix=Matrix.Rotation(math.pi / 2, 3, 'Z'))
    bmesh.ops.translate(bm, verts=bm.verts, vec=(sx * 0.23, -2.235, 0.40))
    solid_fix(bm)
    objs.append(new_obj("ExhaustTip", bm, [M_STEEL], 50))
    # dark exhaust throat
    bm = bmesh.new(); ellipsoid((sx * 0.23, -2.262, 0.40), (0.048, 0.006, 0.048), (0, 0, 0), 2.0, 24, 6, bm=bm); solid_fix(bm)
    objs.append(new_obj("ExhaustHole", bm, [M_BLACK], 50))

bm = bmesh.new()
for sx in (-1, 1):
    for k in range(4):
        z_ = 0.42 + 0.055 * k
        ellipsoid((sx * (side_x(-0.72, z_) - 0.012), -0.72, z_), (0.012, 0.13, 0.008), (0, 0, 0.15 * sx), 2.4, 16, 6, bm=bm)
solid_fix(bm); objs.append(new_obj("IntakeSlats", bm, [M_CARBON], 60))
# front grille bars in nose intake & corner intakes (charcoal slats)
bm = bmesh.new()
for k in range(5):
    ellipsoid((0, 2.19, 0.225 + 0.035 * k), (0.34 - 0.01 * abs(k - 2), 0.05, 0.007), (0, 0, 0), 2.4, 24, 6, bm=bm)
for sx in (-1, 1):
    for k in range(3):
        ellipsoid((sx * 0.60, 2.06, 0.285 + 0.04 * k), (0.15, 0.05, 0.006), (0, 0, -0.35 * sx), 2.4, 20, 6, bm=bm)
solid_fix(bm); objs.append(new_obj("Grille", bm, [M_CARBON], 60))

# ------------------------------------------------------------------ mirrors
for sx in (-1, 1):
    bm = bmesh.new()
    mp = V((sx * 0.885, 0.62, 0.905))
    ellipsoid(mp, (0.062, 0.115, 0.048), (0, 0, 0.12 * sx), 2.3, 28, 14, bm=bm)
    solid_fix(bm); objs.append(new_obj("MirrorHousing", bm, [M_PAINT], 60))
    bm = bmesh.new()
    ellipsoid(mp + V((0, -0.0, 0)) + V((0, -0.105, 0)), (0.050, 0.010, 0.040), (0, 0, 0.12 * sx), 2.3, 20, 8, bm=bm)
    solid_fix(bm); objs.append(new_obj("MirrorGlass", bm, [M_GLASS], 60))
    bm = bmesh.new()
    tube(bm, [V((sx * 0.64, 0.74, 0.83)), V((sx * 0.78, 0.68, 0.88)), V((sx * 0.86, 0.64, 0.90))], 0.012, 0.024, 8, 0, True, ref=V((0, 0, 1)))
    solid_fix(bm); objs.append(new_obj("MirrorStalk", bm, [M_CARBON], 60))

# ------------------------------------------------------------------ panel seams
bm = bmesh.new()
def seam(pts_fn, ts, r=0.0032):
    pts = [pts_fn(t) for t in ts]
    tube(bm, pts, r, r, 6, 0, True, ref=V((0, 0, 1)))
def side_poly(sx, poly, off=0.001):
    out = []
    for (y, z) in poly: out.append(V((sx * (side_x(y, z) + off), y, z)))
    return out
def dense(pts, n=6):
    out = []
    for a, b in zip(pts[:-1], pts[1:]):
        for k in range(n): out.append(a.lerp(b, k / n))
    out.append(pts[-1]); return out
for sx in (-1, 1):
    door = [(0.62, 0.30), (0.70, 0.60), (0.74, 0.78), (0.70, 0.80)]
    door_outline = [(0.64, 0.33), (0.70, 0.60), (0.72, 0.80), (-0.15, 0.82), (-0.30, 0.80), (-0.34, 0.60), (-0.38, 0.33), (0.64, 0.33)]
    pts = []
    for a, b in zip(door_outline[:-1], door_outline[1:]):
        for k in range(8):
            y = a[0] + (b[0] - a[0]) * k / 8; z = a[1] + (b[1] - a[1]) * k / 8
            pts.append(V((sx * (side_x(y, z) + 0.001), y, z)))
    pts.append(pts[0])
    tube(bm, pts, 0.0032, 0.0032, 6, 0, True, ref=V((0, 0, 1)))
    # hood outline (fender gap)
    hp = []
    for (x, y) in [(0.52, 1.00), (0.74, 1.30), (0.77, 1.60), (0.70, 1.90), (0.52, 2.12)]:
        pass
    hl = [(0.50, 0.98), (0.66, 1.18), (0.75, 1.45), (0.74, 1.72), (0.68, 1.98), (0.52, 2.16)]
    hpts = []
    for a, b in zip(hl[:-1], hl[1:]):
        for k in range(8):
            x = a[0] + (b[0] - a[0]) * k / 8; y = a[1] + (b[1] - a[1]) * k / 8
            hpts.append(V((sx * x, y, top_z(y, x) + 0.001)))
    hpts.append(V((sx * hl[-1][0], hl[-1][1], top_z(hl[-1][1], hl[-1][0]))))
    tube(bm, hpts, 0.0032, 0.0032, 6, 0, True, ref=V((0, 0, 1)))
    # rear deck / engine cover gap
    rl = [(0.45, -1.10), (0.62, -1.35), (0.66, -1.70), (0.56, -2.02)]
    rpts = []
    for a, b in zip(rl[:-1], rl[1:]):
        for k in range(8):
            x = a[0] + (b[0] - a[0]) * k / 8; y = a[1] + (b[1] - a[1]) * k / 8
            rpts.append(V((sx * x, y, top_z(y, x) + 0.001)))
    tube(bm, rpts, 0.0032, 0.0032, 6, 0, True, ref=V((0, 0, 1)))
# centre hood crease line
cp = [V((0, y, top_z(y, 0) + 0.001)) for y in np.linspace(1.0, 2.12, 24)]
tube(bm, cp, 0.0028, 0.0028, 6, 0, True, ref=V((1, 0, 0)))
# front hood lip across
fl = [V((x, 2.16, top_z(2.16, x) + 0.001)) for x in np.linspace(-0.5, 0.5, 20)]
tube(bm, fl, 0.0028, 0.0028, 6, 0, True, ref=V((0, 0, 1)))
solid_fix(bm)
objs.append(new_obj("Seams", bm, [M_SEAM], 60))

# door handles
for sx in (-1, 1):
    bm = bmesh.new()
    y, z = -0.12, 0.72
    ellipsoid((sx * (side_x(y, z) + 0.0), y, z), (0.012, 0.075, 0.012), (0, 0, 0), 2.5, 16, 8, bm=bm)
    solid_fix(bm); objs.append(new_obj("DoorHandle", bm, [M_CARBON], 60))

# ------------------------------------------------------------------ join static body
bpy.ops.object.select_all(action='DESELECT')
for o in objs: o.select_set(True)
bpy.context.view_layer.objects.active = body_main
bpy.ops.object.join()
body = bpy.context.active_object
body.name = "Body"; body.data.name = "Body"
# remove duplicate material slots' unused
bpy.ops.object.material_slot_remove_unused()

# ------------------------------------------------------------------ wheels
def build_wheel(side, front):
    """left wheels: side=-1 (outward -X). geometry centred at origin, axis X."""
    h = (0.27 if front else 0.285) / 2
    s = side
    # tire
    bm = bmesh.new()
    P = [(0.252, -.78), (0.262, -.96), (0.295, -1.0), (0.328, -.93), (0.342, -.64), (0.345, -.30), (0.345, .30), (0.342, .64),
         (0.328, .93), (0.295, 1.0), (0.262, .96), (0.252, .78), (0.240, .5), (0.240, -.5)]
    prof_ = [(r, s * x * h) for r, x in P]
    revolve(bm, prof_, 0, 80, 0, True); solid_fix(bm)
    tire = new_obj("tire", bm, [M_TIRE], 60)
    # rim barrel
    bm = bmesh.new()
    P = [(0.252, .88), (0.264, .82), (0.256, .68), (0.222, .46), (0.220, -.50), (0.238, -.50), (0.240, .40)]
    revolve(bm, [(r, s * x * h) for r, x in P], 0, 60, 0, True); solid_fix(bm)
    rim = new_obj("rim", bm, [M_ALLOY], 50)
    # spokes
    bm = bmesh.new()
    for k in range(5):
        a0 = 2 * math.pi * k / 5
        rings = []
        for i in range(14):
            t = i / 13
            r = 0.050 + t * (0.205 - 0.0)
            ang = a0 + 0.07 * t
            xo = s * h * (0.62 - 0.12 * t)
            wd = 0.020 + 0.020 * t ** 1.2 + 0.010 * (1 - t)   # tangential half-width
            th = 0.020 + 0.006 * (1 - t)
            c = V((xo, r * math.cos(ang), r * math.sin(ang)))
            tang = V((0, -math.sin(ang), math.cos(ang)))
            ring = [c + V((1, 0, 0)) * th * math.cos(2 * math.pi * j / 8) * 1.0 + tang * wd * math.sin(2 * math.pi * j / 8) for j in range(8)]
            rings.append(ring)
        loft(bm, rings, 0, True, True)
    solid_fix(bm)
    spokes = new_obj("spokes", bm, [M_ALLOY], 60)
    # hub cap
    bm = bmesh.new()
    P = [(0.0, .70), (0.030, .69), (0.062, .62), (0.070, .45), (0.070, .0), (0.0, .0)]
    revolve(bm, [(r, s * x * h) for r, x in P], 0, 40, 0, True); solid_fix(bm)
    hub = new_obj("hub", bm, [M_ALLOY], 50)
    # dark dish behind spokes
    bm = bmesh.new()
    P = [(0.07, .42), (0.20, .40), (0.225, .36), (0.225, .2), (0.07, .2)]
    revolve(bm, [(r, s * x * h) for r, x in P], 0, 48, 0, True); solid_fix(bm)
    dish = new_obj("dish", bm, [M_BLACK], 50)
    # rotor
    bm = bmesh.new()
    P = [(0.085, -.05), (0.118, -.05), (0.120, -.22), (0.210, -.22), (0.214, -.05), (0.214, .02), (0.210, .06), (0.120, .06), (0.085, .06)]
    # simple rotor disc
    P = [(0.07, .10), (0.215, .10), (0.215, -.18), (0.07, -.18)]
    revolve(bm, [(r, s * x * h) for r, x in P], 0, 64, 0, True); solid_fix(bm)
    # drilled look: tiny dimples skipped; add vent ring
    rotor = new_obj("rotor", bm, [M_ROTOR], 50)
    bpy.ops.object.select_all(action='DESELECT')
    for o in (tire, rim, spokes, hub, dish, rotor): o.select_set(True)
    bpy.context.view_layer.objects.active = tire
    bpy.ops.object.join()
    w = bpy.context.active_object
    return w

def build_caliper(side, front):
    h = (0.27 if front else 0.295) / 2
    bm = bmesh.new(); rings = []
    a0, a1 = (math.radians(10), math.radians(60)) if front else (math.radians(120), math.radians(170))
    for i in range(14):
        a = a0 + (a1 - a0) * i / 13
        rr = []
        pr = [(0.150, -.20), (0.175, -.24), (0.225, -.24), (0.240, -.18), (0.240, .16), (0.225, .22), (0.175, .22), (0.150, .16)]
        for (r, x) in pr:
            rr.append((side * x * h * 0.9 + 0.0, r * math.cos(a), r * math.sin(a)))
        rings.append(rr)
    loft(bm, rings, 0, True, True); solid_fix(bm)
    return new_obj("caliper", bm, [M_CALIPER], 60)

root = None  # no extra root empty: Body and the four pivots are top-level (Unity makes the prefab root)
names = {(-1, 1): "Front left wheel pivot", (1, 1): "Front right wheel pivot", (-1, -1): "Rear left wheel pivot", (1, -1): "Rear right wheel pivot"}
tags = {(-1, 1): "FL", (1, 1): "FR", (-1, -1): "RL", (1, -1): "RR"}
PIV_Z = 0.465
for (sx, sy), nm in names.items():
    piv = bpy.data.objects.new(nm, None); piv.empty_display_type = 'ARROWS'; piv.empty_display_size = 0.15
    bpy.context.scene.collection.objects.link(piv)
    piv.location = (sx * 0.84, sy * 1.38, PIV_Z)
    # wheel mesh: its origin = wheel centre at ground+tire radius => local z = 0.345 - 0.465
    w = build_wheel(sx, sy > 0)
    w.name = f"Wheel_{tags[(sx, sy)]}"; w.data.name = w.name
    w.parent = piv; w.location = (0, 0, WH_Z - PIV_Z)
    c = build_caliper(sx, sy > 0)
    c.name = f"Caliper_{tags[(sx, sy)]}"; c.data.name = c.name
    c.parent = piv; c.location = (0, 0, WH_Z - PIV_Z)

# fix wheel material assignment (joined slot order) -> map by face material name
for o in bpy.data.objects:
    if o.name.startswith("Wheel_"):
        pass

bpy.context.view_layer.update()

# ------------------------------------------------------------------ stats / save / export
def tri_count(o):
    me = o.data; me.calc_loop_triangles(); return len(me.loop_triangles)
meshes = [o for o in bpy.data.objects if o.type == 'MESH']
stats = {o.name: {"verts": len(o.data.vertices), "tris": tri_count(o)} for o in meshes}
total_tris = sum(v["tris"] for v in stats.values())
# bounds of the whole car in world space
mn = V((1e9,) * 3); mx = V((-1e9,) * 3)
for o in meshes:
    for v in o.data.vertices:
        p = o.matrix_world @ v.co
        for i in range(3): mn[i] = min(mn[i], p[i]); mx[i] = max(mx[i], p[i])
bounds = {"x": [round(mn.x, 3), round(mx.x, 3)], "y_forward": [round(mn.y, 3), round(mx.y, 3)], "z_up": [round(mn.z, 3), round(mx.z, 3)]}
dims = {"width": round(mx.x - mn.x, 3), "length": round(mx.y - mn.y, 3), "height": round(mx.z - mn.z, 3)}
print("TRIS", total_tris, "DIMS", dims, "BOUNDS", bounds)

bpy.ops.wm.save_as_mainfile(filepath=os.path.join(OUT, "Tempest_R.blend"))
bpy.ops.object.select_all(action='DESELECT')
for o in bpy.data.objects:
    if o.type in ('MESH', 'EMPTY'): o.select_set(True)
FBX_FWD, FBX_UP = '-Z', 'Y'
bpy.ops.export_scene.fbx(filepath=os.path.join(OUT, "Tempest_R.fbx"), use_selection=True, global_scale=1.0,
                         apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL', axis_forward=FBX_FWD, axis_up=FBX_UP,
                         bake_space_transform=True, object_types={'MESH', 'EMPTY'}, mesh_smooth_type='EDGE',
                         use_mesh_modifiers=True, add_leaf_bones=False, path_mode='COPY', embed_textures=False)
info = {"asset": "Aster Tempest R", "blender": bpy.app.version_string, "dimensions_m": dims, "bounds_blender_m": bounds,
        "target": {"length": 4.58, "width_nominal": 1.96, "height": 1.16, "wheelbase": 2.76, "track": 1.68, "tire_radius": 0.345},
        "mesh_objects": len(meshes), "total_triangles": total_tris, "per_mesh": stats,
        "wheel_pivots": {n: [round(c, 3) for c in bpy.data.objects[n].location] for n in names.values()},
        "wheel_pivot_note": "Blender coords (X right, Y forward, Z up) = Unity (X, Z, Y) after FBX; pivot at spec height 0.465, wheel mesh child offset -0.12 so tire touches ground.",
        "fbx_export_axes": {"forward": FBX_FWD, "up": FBX_UP, "bake_space_transform": True, "scale": 1.0},
        "unity_import_tested": False}
json.dump(info, open(os.path.join(OUT, "verification.json"), "w"), indent=2)

# ------------------------------------------------------------------ render
if not NORENDER:
    sc = bpy.context.scene
    # studio
    w = bpy.data.worlds.new("W"); sc.world = w; w.use_nodes = True
    bg = w.node_tree.nodes["Background"]; bg.inputs[0].default_value = (0.02, 0.022, 0.03, 1); bg.inputs[1].default_value = 1.0
    bpy.ops.mesh.primitive_plane_add(size=60, location=(0, 0, 0))
    fl = bpy.context.active_object
    fm = mat("Floor", (0.035, 0.037, 0.042), 0.0, 0.22); fl.data.materials.append(fm)
    def area(name, loc, size, energy, color=(1, 1, 1), shape='RECTANGLE', size_y=None):
        ld = bpy.data.lights.new(name, 'AREA'); ld.energy = energy; ld.size = size; ld.color = color; ld.shape = shape
        if size_y: ld.size_y = size_y
        o = bpy.data.objects.new(name, ld); sc.collection.objects.link(o); o.location = loc
        d = (V((0, 0, 0.5)) - V(loc)); o.rotation_euler = d.to_track_quat('-Z', 'Y').to_euler(); return o
    area("top", (0, 0, 6), 5, 700, shape='SQUARE')
    area("keyL", (-6, 3, 3), 3, 1100, (1, 0.97, 0.92), 'RECTANGLE', 1.2)
    area("rimR", (6, -4, 2.5), 2, 1300, (0.75, 0.85, 1.0), 'RECTANGLE', 1.0)
    area("front", (1.5, 7, 1.5), 3, 500, (1, 1, 1), 'RECTANGLE', 1.0)
    sc.render.engine = 'CYCLES'; sc.cycles.samples = 96; sc.cycles.use_denoising = True; sc.cycles.device = 'CPU'
    sc.view_settings.exposure = -0.4
    sc.render.resolution_x = 1600; sc.render.resolution_y = 900
    sc.view_settings.view_transform = 'AgX' if 'AgX' in [i.identifier for i in sc.view_settings.bl_rna.properties['view_transform'].enum_items] else 'Standard'
    cam_d = bpy.data.cameras.new("Cam"); cam = bpy.data.objects.new("Cam", cam_d); sc.collection.objects.link(cam); sc.camera = cam
    shots = {"render_front34.png": ((-4.4, 5.0, 1.15), 48), "render_rear34.png": ((4.3, -5.4, 1.35), 48), "render_side.png": ((8.2, 0.0, 0.55), 62)}
    for fn, (loc, lens) in shots.items():
        cam.location = loc; cam_d.lens = lens
        tgt = V((0, 0, 0.5)); cam.rotation_euler = (tgt - V(loc)).to_track_quat('-Z', 'Y').to_euler()
        sc.render.filepath = os.path.join(OUT, fn)
        bpy.ops.render.render(write_still=True)
    print("RENDERED")
