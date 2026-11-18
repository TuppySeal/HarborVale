# Unity map handoff — verified

Current scene: game/Assets/Scenes/HarborVale_CityMap.unity. Earlier Greybox/EditorVerified/LabeledCity scenes are preserved intermediate sources; use CityMap.

License activation succeeded after the user signed in. Unity imported/compiled the project and generated, reopened and rendered the map with exit0. Materials/shaders and four camera framing targets passed checks. Final labels were resized after visual inspection. See artifacts/qa/M11-T01/editor_verification.txt and unity-map-final.log.

M11-T01 done. No other gameplay systems implemented. Remaining optional manual check: keyboard/mouse fly controls in Play Mode. Four actual Unity renders are in the evidence directory. Source-coordinate SVG is retained separately and is not an editor render.
