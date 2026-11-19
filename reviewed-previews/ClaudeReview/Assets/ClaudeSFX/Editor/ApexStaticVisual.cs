using UnityEngine;
namespace ClaudeSFX.Editor {public static class ApexStaticVisual {
public static void MakeVisual(GameObject car){
  foreach(var r in car.GetComponentsInChildren<Renderer>()){bool wheel=false;for(Transform t=r.transform;t!=car.transform&&t!=null;t=t.parent)if(t.name.Contains("wheel pivot"))wheel=true;if(!wheel)r.enabled=false;}
  var body=new Material(Shader.Find("Standard")){color=new Color(.06f,.34f,.75f)};body.SetFloat("_Metallic",.55f);body.SetFloat("_Glossiness",.65f);var dark=new Material(Shader.Find("Standard")){color=new Color(.025f,.035f,.055f)};var lamp=new Material(Shader.Find("Standard")){color=new Color(.85f,.94f,1)};var red=new Material(Shader.Find("Standard")){color=new Color(.8f,.02f,.025f)};
  var root=new GameObject("Original Aster Apex sports coupe | 4.3 x 1.85 m");root.transform.SetParent(car.transform,false);
  Box(root.transform,"Low sports body",new Vector3(0,.61f,0),new Vector3(1.85f,.37f,4.3f),body);Box(root.transform,"Hood",new Vector3(0,.82f,1.16f),new Vector3(1.72f,.12f,1.50f),body);Box(root.transform,"Rear deck",new Vector3(0,.86f,-1.42f),new Vector3(1.72f,.15f,1.12f),body);
  var cabin=Box(root.transform,"Raked dark canopy",new Vector3(0,1.02f,-.22f),new Vector3(1.42f,.42f,1.6f),dark);cabin.transform.localRotation=Quaternion.Euler(5,0,0);Box(root.transform,"Roof",new Vector3(0,1.25f,-.30f),new Vector3(1.40f,.06f,.86f),body);
  for(int i=-1;i<=1;i+=2){Box(root.transform,"Wing upright",new Vector3(i*.58f,1.05f,-1.73f),new Vector3(.07f,.40f,.10f),dark);Box(root.transform,"Headlamp",new Vector3(i*.64f,.77f,2.16f),new Vector3(.43f,.08f,.035f),lamp);Box(root.transform,"Rear lamp",new Vector3(i*.64f,.74f,-2.16f),new Vector3(.43f,.07f,.035f),red);}
  Box(root.transform,"Rear aero wing",new Vector3(0,1.25f,-1.73f),new Vector3(1.95f,.09f,.36f),dark);Box(root.transform,"Front splitter",new Vector3(0,.40f,1.97f),new Vector3(1.94f,.06f,.48f),dark);Box(root.transform,"Front intake",new Vector3(0,.59f,2.16f),new Vector3(1.2f,.18f,.03f),dark);
 }
 static GameObject Box(Transform parent,string name,Vector3 position,Vector3 scale,Material material){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=position;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=material;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
 }}
