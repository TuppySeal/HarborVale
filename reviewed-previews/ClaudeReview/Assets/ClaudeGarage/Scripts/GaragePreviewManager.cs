using System;
using UnityEngine;
namespace ClaudeGaragePreview {
public class GaragePreviewManager:MonoBehaviour {
 public GameObject[] displays;public CameraOrbitController cameraOrbit;public int SelectedIndex{get;private set;}public bool MenuOpen=true;public bool[] fitted=new bool[4];
 public static readonly UpgradePart[] Parts={new UpgradePart("engine_stage1","Stage 1 engine tune","ECU and breathing package. Visual preview; performance comes from the main game's torque curves."),new UpgradePart("gearbox_stage1","Gearbox support","Gearbox and clutch support. Visual preview only."),new UpgradePart("turbo_stage2","Turbo + basic intercooler","Turbo hardware, piping and a basic cooler are included. Calibration and purchases remain in the main game."),new UpgradePart("intercooler_stage1","Street intercooler","Larger street cooler replaces the basic cooler. Shown on a stand; no modeled engine-bay mounting is implied.")};
 public event Action<string> PartSelected,PreviewFitmentRequested;
 public string SelectedId=>Parts[SelectedIndex].id;
 void Start(){Select(0);}
 public void Select(int index){if(index<0||index>=Parts.Length)throw new ArgumentOutOfRangeException(nameof(index));SelectedIndex=index;for(int i=0;i<displays.Length;i++)displays[i].SetActive(i==index);PartSelected?.Invoke(SelectedId);}
 public void ToggleFitment(){fitted[SelectedIndex]=!fitted[SelectedIndex];PreviewFitmentRequested?.Invoke(SelectedId);}
 void Update(){if(Input.GetKeyDown(KeyCode.G))MenuOpen=!MenuOpen;}
}
}
