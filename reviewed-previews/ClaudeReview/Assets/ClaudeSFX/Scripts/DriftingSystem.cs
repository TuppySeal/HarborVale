using UnityEngine;
namespace ClaudeSFX {
public class DriftingSystem:MonoBehaviour {
 public ParticleSystem[] smoke;public float Intensity{get;private set;}
 public void Apply(EffectTelemetry state){Intensity=state.grounded&&state.speed>3?Mathf.InverseLerp(.12f,.8f,Mathf.Abs(state.slip)):0;foreach(var p in smoke){var emission=p.emission;emission.rateOverTime=Intensity*30;if(Intensity>0&&!p.isPlaying)p.Play();else if(Intensity==0&&p.isPlaying)p.Stop(true,ParticleSystemStopBehavior.StopEmitting);}}
 public static ParticleSystem Make(Transform parent,Vector3 local,string name){var root=new GameObject(name);root.transform.SetParent(parent,false);root.transform.localPosition=local;var p=root.AddComponent<ParticleSystem>();p.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var m=p.main;m.playOnAwake=false;m.loop=true;m.duration=1;m.startLifetime=.75f;m.startSize=.22f;m.startSpeed=.4f;m.startColor=new Color(.65f,.68f,.72f,.25f);m.simulationSpace=ParticleSystemSimulationSpace.World;m.gravityModifier=-.05f;var e=p.emission;e.rateOverTime=0;var shape=p.shape;shape.shapeType=ParticleSystemShapeType.Sphere;shape.radius=.12f;var renderer=p.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=new Material(Shader.Find("Particles/Standard Unlit")){color=new Color(.7f,.7f,.7f,.3f)};return p;}
}
}
