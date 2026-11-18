# Unity map prototype environment

User-selected engine: Unity. Verified installed executable: `D:/Program Files/Unity/Hub/Editor/6000.0.18f1/Editor/Unity.exe`. Version metadata: 6000.0.18f1, revision 3699cf869f9b. Project: `game/`. Scene: `Assets/Scenes/HarborVale_Greybox.unity`.

This supersedes the original proposed Godot environment for the map task. No engine download/upgrade was performed. The project uses the built-in render pipeline and local built-in modules only.

Actual editor invocation reached licensing and exited 1: **No valid Unity Editor license found. Please activate your license.** The initial sandboxed attempt also stalled at local licensing IPC; an authorized unsandboxed invocation confirmed the missing license. It did not import or render the scene.

The C# files were independently compiled with this editor's bundled Roslyn compiler against its Unity/netstandard reference assemblies; see `artifacts/qa/M11-T01/csharp-check.txt`. This verifies source/API compilation, not Unity scene import or Play Mode.

Next: activate the editor in Unity Hub, open this project and its scene, check Scene/Game labels, press Play and verify inspection controls. Existing source exports are preserved; do not blindly invoke a generator over the hand-editable scene.

The PNG/SVG overview is a coordinate diagram from the shared layout data, not a Unity screenshot. No driving/physics/gameplay performance result is claimed.

## License resolution and editor verification

After user sign-in, Unity successfully resolved entitlements and imported the project. HarborValeVerification.Run generated Assets/Scenes/HarborVale_CityMap.unity, saved/reopened it, validated materials and camera overview methods, and rendered four PNGs. Final editor run exited0. The earlier license failure is resolved. Actual evidence: artifacts/qa/M11-T01/editor_verification.txt and unity-map-final.log. Physical input remains a user-review check.

## Canonical destination and revision 2 verification

Canonical project is now C:/Users/Justice/Documents/Unity/HarborVale using installed C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Unity.exe. Earlier engine paths above are historical. M11-T02 successfully compiled, generated, saved/reopened and rendered with Unity 6000.6.0f1 and built-in pipeline. Road surface collision and model bounds checks passed. Exact next-step source/evidence paths: project/EXPANSION_HANDOFF.md.


## Driving prototype
Unity 6000.6.0f1 built-in vehicles/audio modules (1.0.0) added. Isolated verification project .work/UnityDrivingValidation avoids editing open canonical scenes. Native physics, actual Play Mode and Windows standalone development export succeeded. Exact reports and known editor-only issue are documented in project/DRIVING_HANDOFF.md and KNOWN_ISSUES.md. No paid assets/plugins were added.

