using UnityEngine;
namespace ClaudeGaragePreview {
public class GarageSceneSetup:MonoBehaviour {
 public static GameObject Box(string name,Transform parent,Vector3 pos,Vector3 size,Color color){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=new Material(Shader.Find("Standard")){name=name+" material",color=color};return g;}
 public static void BuildGarageScene(GameObject finchPrefab){
  if(finchPrefab==null)throw new System.InvalidOperationException("Static Finch prefab is required");
  var root=new GameObject("Garage | one meter per unit");var steel=new Color(.24f,.29f,.32f);var blue=new Color(.08f,.28f,.40f);
  Box("10 x 12 m workshop floor",root.transform,new Vector3(0,-.1f,0),new Vector3(10,.2f,12),new Color(.27f,.29f,.3f));Box("Back wall",root.transform,new Vector3(0,1.7f,-5.9f),new Vector3(10,3.4f,.2f),steel);foreach(float side in new[]{-1f,1f})Box("Side wall",root.transform,new Vector3(side*5,1.7f,-1),new Vector3(.2f,3.4f,10),steel);
  Box("Workbench top",root.transform,new Vector3(-3.4f,.9f,-4.9f),new Vector3(1.8f,.09f,.9f),new Color(.42f,.3f,.18f));foreach(float x in new[]{-4.1f,-2.7f})foreach(float z in new[]{-5.2f,-4.6f})Box("Workbench leg",root.transform,new Vector3(x,.43f,z),new Vector3(.07f,.86f,.07f),steel);
  Box("Rolling tool chest",root.transform,new Vector3(-3.6f,.43f,-2.9f),new Vector3(.75f,.74f,.48f),blue);for(int i=0;i<5;i++)Box("Drawer pull",root.transform,new Vector3(-3.6f,.2f+i*.12f,-2.65f),new Vector3(.54f,.025f,.035f),Color.gray);
  foreach(float x in new[]{3.1f,4.5f})Box("Shelf upright",root.transform,new Vector3(x,1,-5.1f),new Vector3(.06f,2,.45f),steel);for(int i=0;i<4;i++)Box("Parts shelf",root.transform,new Vector3(3.8f,.35f+i*.5f,-5.1f),new Vector3(1.5f,.06f,.45f),blue);
  var car=Instantiate(finchPrefab,root.transform);car.name="Aster Finch | static original visual";car.transform.localPosition=new Vector3(-1.35f,0,.65f);
  Box("One parts counter",root.transform,new Vector3(2.35f,.48f,.6f),new Vector3(1.8f,.96f,1.5f),blue);
  var light=new GameObject("Workshop key light").AddComponent<Light>();light.transform.SetParent(root.transform);light.type=LightType.Directional;light.intensity=1.35f;light.transform.rotation=Quaternion.Euler(45,-35,0);RenderSettings.ambientLight=new Color(.57f,.63f,.72f);
  var cam=new GameObject("Main Camera");cam.tag="MainCamera";var camera=cam.AddComponent<Camera>();camera.nearClipPlane=.05f;camera.backgroundColor=new Color(.11f,.14f,.18f);camera.clearFlags=CameraClearFlags.SolidColor;cam.AddComponent<AudioListener>();var orbit=cam.AddComponent<CameraOrbitController>();var target=new GameObject("Camera focus");target.transform.position=new Vector3(0,.85f,.4f);orbit.target=target.transform;orbit.ResetView();
  var manager=root.AddComponent<GaragePreviewManager>();manager.cameraOrbit=orbit;manager.displays=new GameObject[4];
  for(int i=0;i<4;i++){var display=new GameObject(GaragePreviewManager.Parts[i].id+" | visual display");display.transform.SetParent(root.transform,false);display.transform.localPosition=new Vector3(2.35f,1.2f,.6f);manager.displays[i]=display;
   if(i==2){display.AddComponent<ProceduralTurboMesh>().GenerateTurboVisuals();Box("Included basic intercooler",display.transform,new Vector3(.5f,0,0),new Vector3(.42f,.23f,.075f),new Color(.68f,.73f,.77f));}
   else if(i==3)display.AddComponent<ProceduralIntercoolerrMesh>().GenerateIntercoolerrVisuals();
   else {Box(i==0?"ECU / breathing module":"Gearbox case",display.transform,Vector3.zero,new Vector3(.36f,.24f,.42f),Color.gray);for(int k=0;k<3;k++)Box("Assembly detail",display.transform,new Vector3(-.13f+k*.13f,.15f,0),new Vector3(.08f,.08f,.3f),new Color(.55f,.62f,.65f));}
  }
  root.AddComponent<UpgradeUIManager>().manager=manager;manager.Select(0);
 }
}
}
