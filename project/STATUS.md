# Current status — scaled Unity city and starter-car model verified

Canonical project: C:/Users/Justice/Documents/Unity/HarborVale, Unity 6000.6.0f1. Open Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity. Workspace game/ remains the earlier map revision.

M11-T01 and M11-T02 are complete for their bounded map/model scopes. The expanded city has 24 named roads, 39 junctions, about 29.5 km of road centerline, three districts, 550 ordinary building placeholders and 45 stable service/activity markers. One unit = one meter, 7.2 m two-lane carriageways, 14 m four-lane boulevard. Streets include gentle curves, coastal bends, hillside routes and shorter neighborhood blocks. Foundations, sidewalks and driveways establish scale.

Original Aster Finch starter hatchback prefab is at Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab, parked outside the player garage. Body dimensions 4.05 x 1.70 x 1.46 m; separate labeled model parts and wheel pivots. Press Play then 5 for car close-up or 6 for street view.

Verification: source checks passed road connectivity, every crossing/junction, preserved gameplay IDs, building/road footprint clearance, <=11.75% longitudinal road grade and >=37.8 m sampled bend radius. Unity saved/reopened/rendered the scene, checked materials and actual model bounds, and passed 288 road collider raycasts. Four actual Unity PNGs and check reports are in artifacts/qa/M11-T02/. Manual keyboard/mouse interaction, driving feel and performance have not been playtested.

This is an editable city greybox and static car visual. Driving, racing, career, traffic, police and shop behavior remain future work. Original 42 gameplay tasks remain unstarted. Next: user reviews this revision and directs the next change. Continuation: project/EXPANSION_HANDOFF.md. Existing private remote is https://github.com/TuppySeal/HarborVale; inspect Git for the current commit.
