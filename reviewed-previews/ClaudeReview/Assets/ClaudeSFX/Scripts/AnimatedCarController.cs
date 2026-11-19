using UnityEngine;
namespace ClaudeSFX {
// This component owns only effects children. It never writes the car root, wheel pivots or physics.
public class AnimatedCarController:MonoBehaviour {
 public Transform turboRotor;public DriftingSystem drift;public ParticleSystem exhaust;
 public void Apply(EffectTelemetry state){drift.Apply(state);if(turboRotor!=null&&state.turbo)turboRotor.Rotate(0,0,Time.deltaTime*state.boostBar*900,Space.Self);var emission=exhaust.emission;emission.rateOverTime=state.downpipe?2+state.load*9:0;if(state.downpipe&&!exhaust.isPlaying)exhaust.Play();else if(!state.downpipe&&exhaust.isPlaying)exhaust.Stop(true,ParticleSystemStopBehavior.StopEmitting);}
}
}
