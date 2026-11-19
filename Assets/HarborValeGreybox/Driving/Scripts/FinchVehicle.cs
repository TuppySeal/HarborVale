using UnityEngine;
namespace HarborValeGreybox.Driving {
public interface IFinchRecoveryHandler {bool TryRecover(FinchVehicle vehicle);}
[DisallowMultipleComponent]
public class FinchVehicle:MonoBehaviour {
 public TextAsset stockDefinition;
 [Header("Accessible stock handling")]
 public float lowSpeedSteer=32,highSpeedSteer=7.5f,steeringResponse=2.4f,antiRoll=3800,aeroCdA=.88f;
 public string PowertrainCarId="car_finch",displayName="ASTER FINCH",driveLabel="FWD";
 public float frontDriveShare=1,forwardGrip=1.15f,lateralGrip=1.10f,downforceCoefficient=.35f;
 public bool playerInput=true,externalInput; public bool controlsLocked; public IFinchRecoveryHandler RecoveryHandler{get;set;}
 public float commandedPedal,commandedSteer;public bool commandedHandbrake;
 public Rigidbody Body{get;private set;}public WheelCollider[] Wheels{get;private set;}
 public FinchPowertrain Engine{get;private set;}
 public float SpeedKph=>Body==null?0:Body.linearVelocity.magnitude*3.6f;
 public float ForwardSpeed=>Body==null?0:Vector3.Dot(Body.linearVelocity,transform.forward);
 public float Steering{get;private set;}public int GroundedWheels{get;private set;}public float Brake{get;private set;}public float Throttle{get;private set;}
 Transform[] visual=new Transform[4];float steer,handbrake;Vector3 spawn;Quaternion spawnRotation;
 public void Initialize(){
  if(Engine!=null)return;
  if(stockDefinition==null)stockDefinition=Resources.Load<TextAsset>("FinchStockPowertrain");
  if(stockDefinition==null)throw new System.InvalidOperationException("Finch stock definition missing");
  var d=JsonUtility.FromJson<FinchDefinition>(stockDefinition.text);Engine=new FinchPowertrain(d);
  Body=GetComponent<Rigidbody>();if(Body==null)Body=gameObject.AddComponent<Rigidbody>();Body.mass=d.massKg;Body.centerOfMass=new Vector3(0,.43f,.08f);Body.interpolation=RigidbodyInterpolation.Interpolate;Body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;Body.linearDamping=.01f;Body.angularDamping=1;Body.solverIterations=12;Body.solverVelocityIterations=8;
  // Body excludes tire clearance; visible glass/roof remain inside the collision shell.
  var shell=GetComponent<BoxCollider>();if(shell!=null){shell.center=new Vector3(0,.84f,0);shell.size=new Vector3(1.58f,1.18f,3.90f);}
  Wheels=new WheelCollider[4];string[] names={"Front left wheel pivot","Front right wheel pivot","Rear left wheel pivot","Rear right wheel pivot"};
  var physics=new GameObject("Vehicle physics | four suspension contacts");physics.transform.SetParent(transform,false);
  for(int i=0;i<4;i++){
   visual[i]=transform.Find(names[i]);var wheelObject=new GameObject(names[i].Replace("pivot","collider"));wheelObject.transform.SetParent(physics.transform,false);wheelObject.transform.localPosition=new Vector3(i%2==0?-.777f:.777f,.415f,i<2?1.225f:-1.225f);
   var w=wheelObject.AddComponent<WheelCollider>();Wheels[i]=w;w.radius=d.wheelRadius;w.mass=18;w.suspensionDistance=.24f;w.forceAppPointDistance=.18f;w.wheelDampingRate=.05f;
   w.suspensionSpring=new JointSpring{spring=24500,damper=3150,targetPosition=.5f};w.sprungMass=d.massKg/4;
   var f=w.forwardFriction;f.extremumSlip=.32f;f.extremumValue=1;f.asymptoteSlip=.85f;f.asymptoteValue=.75f;f.stiffness=1.15f;w.forwardFriction=f;
   f=w.sidewaysFriction;f.extremumSlip=.22f;f.extremumValue=1;f.asymptoteSlip=.65f;f.asymptoteValue=.78f;f.stiffness=1.10f;w.sidewaysFriction=f;
  }
  Wheels[0].ConfigureVehicleSubsteps(5,8,12);spawn=transform.position;spawnRotation=transform.rotation;
 }
 public int ResetSequence{get;private set;}
 public int InstalledPrototypeStage{get;private set;}
 public void InstallPowertrain(FinchDefinition definition,int stage=0){Initialize();if(definition==null||definition.carId!=PowertrainCarId)throw new System.ArgumentException("Incompatible powertrain");var mode=Engine.Mode;Engine=new FinchPowertrain(definition);Engine.SetMode(mode);InstalledPrototypeStage=stage;}
 public bool ToggleTransmissionMode(){Initialize();if(!playerInput||controlsLocked)return false;Engine.SetMode(Engine.Mode==TransmissionMode.Automatic?TransmissionMode.Manual:TransmissionMode.Automatic);return true;}
 public bool PaddleShift(int direction){Initialize();return playerInput&&!controlsLocked&&Engine.RequestShift(direction,ForwardSpeed);}
 void Awake(){Initialize();}
 void Update(){
  if(!externalInput){commandedPedal=playerInput&&!controlsLocked?FinchInput.Pedal:0;commandedSteer=playerInput&&!controlsLocked?FinchInput.Steer:0;commandedHandbrake=controlsLocked||!playerInput||!Application.isFocused||FinchInput.Held(KeyCode.Space);}
  if(playerInput&&!externalInput&&!controlsLocked){if(FinchInput.Shortcut(KeyCode.R))Recover();if(FinchInput.Shortcut(KeyCode.M))ToggleTransmissionMode();if(FinchInput.Shortcut(KeyCode.Z))PaddleShift(-1);if(FinchInput.Shortcut(KeyCode.X))PaddleShift(1);}
  if(transform.position.y< -40)Recover();
 }
 void FixedUpdate(){Tick(Time.fixedDeltaTime);}
 public void Tick(float dt){
  Initialize();float forward=ForwardSpeed;float pedal=Mathf.Clamp(commandedPedal,-1,1);
  bool opposite=pedal!=0&&Mathf.Sign(pedal)!=Mathf.Sign(forward)&&Mathf.Abs(forward)>1;
  float brakeDemand=opposite?Mathf.Abs(pedal):0;
  Brake=controlsLocked?1:Mathf.MoveTowards(Brake,brakeDemand,dt*10);
  float demand=controlsLocked||opposite?0:Mathf.Abs(pedal);Throttle=controlsLocked||opposite?0:Mathf.MoveTowards(Throttle,demand,dt*(demand>Throttle?5.5f:8));
  bool reverse=pedal<0||(pedal==0&&Engine.Gear<0);
  steer=Mathf.MoveTowards(steer,Mathf.Clamp(commandedSteer,-1,1),steeringResponse*dt);handbrake=Mathf.MoveTowards(handbrake,commandedHandbrake?1:0,dt*6);
  Steering=steer*Mathf.Lerp(lowSpeedSteer,highSpeedSteer,Mathf.Clamp01(SpeedKph/130));
  float torque=Engine.Step(forward,((Wheels[0].rpm+Wheels[1].rpm)*frontDriveShare+(Wheels[2].rpm+Wheels[3].rpm)*(1-frontDriveShare))*.5f,Throttle,reverse,dt,GroundedWheels>0,Brake);
  // Avoid accelerating into the opposite direction until braking reaches walking speed.
  if(opposite)torque=0;
  GroundedWheels=0;float totalBrake=Engine.definition.brakeDeceleration*Body.mass*Engine.definition.wheelRadius;
  for(int i=0;i<4;i++){
   var w=Wheels[i];if(w.isGrounded)GroundedWheels++;
   w.steerAngle=i<2?Steering:0;w.motorTorque=torque*(i<2?frontDriveShare:1-frontDriveShare)*.5f;
   float coastBrake=pedal==0?Engine.definition.massKg*9.81f*.012f*Engine.definition.wheelRadius/4:0;
   float hold=pedal==0&&Mathf.Abs(forward)<.35f?220:0;
   w.brakeTorque=Mathf.Max(totalBrake*Brake*(i<2?.325f:.175f),coastBrake,hold,i>=2?handbrake*1900:0);
   var friction=w.sidewaysFriction;friction.stiffness=i<2?lateralGrip:Mathf.Lerp(lateralGrip,.55f,handbrake);w.sidewaysFriction=friction;var longitudinal=w.forwardFriction;longitudinal.stiffness=forwardGrip;w.forwardFriction=longitudinal;
  }
  if(GroundedWheels>0){AntiRoll(0,1);AntiRoll(2,3);}
  Vector3 velocity=Body.linearVelocity;Body.AddForce(-velocity*velocity.magnitude*(.5f*1.225f*aeroCdA),ForceMode.Force);
  // Aerodynamic downforce is small; no artificial acceleration or yaw teleportation.
  if(GroundedWheels>=2)Body.AddForce(-transform.up*velocity.sqrMagnitude*downforceCoefficient,ForceMode.Force);
 }
 void AntiRoll(int a,int b){
  float travelA=1,travelB=1;bool ga=Wheels[a].GetGroundHit(out var ha),gb=Wheels[b].GetGroundHit(out var hb);
  if(ga)travelA=(-Wheels[a].transform.InverseTransformPoint(ha.point).y-Wheels[a].radius)/Wheels[a].suspensionDistance;
  if(gb)travelB=(-Wheels[b].transform.InverseTransformPoint(hb.point).y-Wheels[b].radius)/Wheels[b].suspensionDistance;
  float force=(travelA-travelB)*antiRoll;if(ga)Body.AddForceAtPosition(Wheels[a].transform.up*-force,Wheels[a].transform.position);if(gb)Body.AddForceAtPosition(Wheels[b].transform.up*force,Wheels[b].transform.position);
 }
 void LateUpdate(){if(Wheels==null)return;for(int i=0;i<4;i++)if(visual[i]!=null){Wheels[i].GetWorldPose(out var p,out var q);visual[i].SetPositionAndRotation(p,q);}}
 public void ResetAt(Vector3 position,Quaternion rotation){Initialize();ResetSequence++;GroundedWheels=0;Body.linearVelocity=Vector3.zero;Body.angularVelocity=Vector3.zero;Body.position=position;Body.rotation=rotation;transform.SetPositionAndRotation(position,rotation);Engine.Reset();Throttle=Brake=0;steer=0;commandedPedal=commandedSteer=0;foreach(var w in Wheels){w.motorTorque=0;w.brakeTorque=0;w.steerAngle=0;}Body.WakeUp();Physics.SyncTransforms();}
 public void Recover(){if(RecoveryHandler!=null&&RecoveryHandler.TryRecover(this))return;if((gameObject.scene.name=="HarborVale_CityMap"||gameObject.scene.name=="HarborVale_SportsTest")&&FinchDrivingBootstrap.TryRecovery(transform.position,transform.forward,out var p,out var q))ResetAt(p,q);else ResetAt(spawn+Vector3.up*.2f,spawnRotation);}
}
}
