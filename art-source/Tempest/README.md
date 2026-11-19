# Aster Tempest R (visual only)

Files: `Tempest_R.blend`, `Tempest_R.fbx`, `generator.py` (reproducible), `render_front34.png`, `render_rear34.png`, `render_side.png`, `verification.json`.
Rebuild: `"C:/Program Files/Blender Foundation/Blender 5.3/blender.exe" -b --python generator.py` (add `-- --norender` to skip renders).

Verified (Blender 5.3, FBX re-imported and measured):
- Size 1.965 w x 4.58 l x 1.165 h m (width = rear tire outer; mirrors inside); wheelbase 2.76, track 1.68, tire radius 0.345.
- 76,126 triangles; 9 mesh objects (Body + 4 Wheel_xx + 4 Caliper_xx). 13 flat Principled materials (base colour / metallic / roughness, no textures).
- Pivot empties (exact names): `Front left wheel pivot`, `Front right wheel pivot`, `Rear left wheel pivot`, `Rear right wheel pivot` at X ±0.84, Z ±1.38 (forward), height 0.465. Left = -X.
- Each pivot has a `Wheel_xx` (tire, rim, spokes, rotor; origin = wheel centre, rolls about local X) and a non-spinning `Caliper_xx` as children, offset -0.12 so the tire touches ground (centre height 0.345).
- Origin at ground, centred between axles; transforms applied (scale 1, no rotation except pivot-child offsets).
- FBX export: forward `-Z`, up `Y`, `bake_space_transform=True`, scale 1.0, no root empty. Re-import round-trip reproduces exact bounds and wheel positions.

Not done: Unity import was NOT tested. Normals/handedness in Unity unverified. No physics/colliders/scripts included.
