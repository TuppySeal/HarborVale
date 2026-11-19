using UnityEngine;

namespace ClaudeGaragePreview
{
    public class ProceduralTurboMesh : MonoBehaviour
    {
        [SerializeField] private float turbineSize = 0.3f;
        [SerializeField] private float compressorSize = 0.4f;
        [SerializeField] private float shaftLength = 0.22f;
        [SerializeField] private Material turbineMaterial;
        [SerializeField] private Material housingMaterial;

        private void Start()
        {
            if(transform.childCount==0)GenerateTurboVisuals();
        }

        public void GenerateTurboVisuals()
        {
            if(turbineMaterial==null)turbineMaterial=new Material(Shader.Find("Standard")){color=new Color(.42f,.47f,.50f)};
            if(housingMaterial==null)housingMaterial=new Material(Shader.Find("Standard")){color=new Color(.28f,.31f,.34f)};
            // Create turbine rotor (rear)
            CreateTurbineRotor();

            // Create compressor rotor (front)
            CreateCompressorRotor();

            // Create housing/shaft
            CreateHousing();

            // Create intake/outlet ports
            CreatePorts();
        }

        private void CreateTurbineRotor()
        {
            GameObject turbine = new GameObject("TurbineRotor");
            turbine.transform.SetParent(transform, false);
            turbine.transform.localPosition = new Vector3(0, 0, -shaftLength * 0.5f);

            // Main rotor disc
            GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.transform.SetParent(turbine.transform, false);
            disc.transform.localScale = new Vector3(turbineSize, 0.1f, turbineSize);
            disc.name = "TurbineDisc";
            DestroyImmediate(disc.GetComponent<Collider>());
            ApplyMaterial(disc, turbineMaterial);

            // Blades
            for (int i = 0; i < 3; i++)
            {
                float angle = i * 120f;
                GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
                blade.transform.SetParent(turbine.transform, false);
                blade.transform.localPosition = Vector3.zero;
                blade.transform.localRotation = Quaternion.Euler(0, angle, 0);
                blade.transform.localScale = new Vector3(0.08f, 0.15f, turbineSize * 0.6f);
                blade.name = $"Blade_{i}";
                DestroyImmediate(blade.GetComponent<Collider>());
                ApplyMaterial(blade, turbineMaterial);
            }
        }

        private void CreateCompressorRotor()
        {
            GameObject compressor = new GameObject("CompressorRotor");
            compressor.transform.SetParent(transform, false);
            compressor.transform.localPosition = new Vector3(0, 0, shaftLength * 0.5f);

            // Main rotor disc
            GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.transform.SetParent(compressor.transform, false);
            disc.transform.localScale = new Vector3(compressorSize, 0.1f, compressorSize);
            disc.name = "CompressorDisc";
            DestroyImmediate(disc.GetComponent<Collider>());
            ApplyMaterial(disc, turbineMaterial);

            // Impeller blades
            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f;
                GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
                blade.transform.SetParent(compressor.transform, false);
                blade.transform.localPosition = Vector3.zero;
                blade.transform.localRotation = Quaternion.Euler(0, angle, 0);
                blade.transform.localScale = new Vector3(0.1f, 0.12f, compressorSize * 0.7f);
                blade.name = $"Impeller_{i}";
                DestroyImmediate(blade.GetComponent<Collider>());
                ApplyMaterial(blade, turbineMaterial);
            }
        }

        private void CreateHousing()
        {
            GameObject housing = new GameObject("Housing");
            housing.transform.SetParent(transform, false);

            // Main cylindrical housing
            GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.transform.SetParent(housing.transform, false);
            shaft.transform.localRotation=Quaternion.Euler(90,0,0);
            shaft.transform.localScale = new Vector3(0.25f, shaftLength * .5f, 0.25f);
            shaft.name = "CentralShaft";
            DestroyImmediate(shaft.GetComponent<Collider>());
            ApplyMaterial(shaft, housingMaterial);

            // Oil cooling lines
            for (int i = 0; i < 2; i++)
            {
                GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                line.transform.SetParent(housing.transform, false);
                line.transform.localPosition = new Vector3((i == 0 ? -0.15f : 0.15f), 0, 0);
                line.transform.localRotation = Quaternion.Euler(90, 0, 0);
                line.transform.localScale = new Vector3(0.03f, shaftLength * 0.8f, 0.03f);
                line.name = $"CoolingLine_{i}";
                DestroyImmediate(line.GetComponent<Collider>());
                ApplyMaterial(line, new Color(0.3f, 0.3f, 0.3f));
            }
        }

        private void CreatePorts()
        {
            // Intake port (front)
            GameObject intake = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            intake.transform.SetParent(transform, false);
            intake.transform.localPosition = new Vector3(0, 0.25f, shaftLength * 0.5f + 0.15f);
            intake.transform.localRotation = Quaternion.Euler(90, 0, 0);
            intake.transform.localScale = new Vector3(0.08f, 0.15f, 0.08f);
            intake.name = "IntakePort";
            DestroyImmediate(intake.GetComponent<Collider>());
            ApplyMaterial(intake, housingMaterial);

            // Exhaust port (rear)
            GameObject exhaust = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            exhaust.transform.SetParent(transform, false);
            exhaust.transform.localPosition = new Vector3(0, -0.25f, -shaftLength * 0.5f - 0.15f);
            exhaust.transform.localRotation = Quaternion.Euler(90, 0, 0);
            exhaust.transform.localScale = new Vector3(0.1f, 0.18f, 0.1f);
            exhaust.name = "ExhaustPort";
            DestroyImmediate(exhaust.GetComponent<Collider>());
            ApplyMaterial(exhaust, new Color(0.4f, 0.1f, 0.1f));
        }

        private void ApplyMaterial(GameObject obj, Material material)
        {
            if (material != null)
            {
                obj.GetComponent<Renderer>().material = material;
            }
        }

        private void ApplyMaterial(GameObject obj, Color color)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            obj.GetComponent<Renderer>().material = mat;
        }
    }
}
