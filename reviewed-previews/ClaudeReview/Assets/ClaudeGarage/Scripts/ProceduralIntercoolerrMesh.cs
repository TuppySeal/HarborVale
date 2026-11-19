using UnityEngine;

namespace ClaudeGaragePreview
{
    public class ProceduralIntercoolerrMesh : MonoBehaviour
    {
        [SerializeField] private float width = 0.6f;
        [SerializeField] private float height = 0.4f;
        [SerializeField] private float depth = 0.1f;
        [SerializeField] private Material coreMaterial;
        [SerializeField] private Material tankMaterial;

        private void Start()
        {
            if(transform.childCount==0)GenerateIntercoolerrVisuals();
        }

        public void GenerateIntercoolerrVisuals()
        {
            // Create main core
            CreateCoreMatrix();

            // Create inlet/outlet tanks
            CreateTanks();

            // Create mounting brackets
            CreateBrackets();

            // Create fin structure
            CreateFins();
        }

        private void CreateCoreMatrix()
        {
            GameObject core = new GameObject("CoreMatrix");
            core.transform.SetParent(transform, false);
            core.transform.localPosition = Vector3.zero;

            // Main core block
            GameObject coreBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            coreBlock.transform.SetParent(core.transform, false);
            coreBlock.transform.localScale = new Vector3(width, height, depth);
            coreBlock.name = "CoreBlock";
            DestroyImmediate(coreBlock.GetComponent<Collider>());

            Material coreMat = new Material(Shader.Find("Standard"));
            coreMat.color = new Color(0.8f, 0.8f, 0.9f);
            coreBlock.GetComponent<Renderer>().material = coreMat;
        }

        private void CreateTanks()
        {
            // Inlet tank (left)
            GameObject inletTank = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            inletTank.transform.SetParent(transform, false);
            inletTank.transform.localPosition = new Vector3(-width * 0.5f - 0.1f, 0, 0);
            inletTank.transform.localRotation = Quaternion.Euler(0, 0, 90);
            inletTank.transform.localScale = new Vector3(height * 0.5f, 0.15f, height * 0.5f);
            inletTank.name = "InletTank";
            DestroyImmediate(inletTank.GetComponent<Collider>());

            Material tankMat = new Material(Shader.Find("Standard"));
            tankMat.color = new Color(0.6f, 0.6f, 0.65f);
            inletTank.GetComponent<Renderer>().material = tankMat;

            // Outlet tank (right)
            GameObject outletTank = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            outletTank.transform.SetParent(transform, false);
            outletTank.transform.localPosition = new Vector3(width * 0.5f + 0.1f, 0, 0);
            outletTank.transform.localRotation = Quaternion.Euler(0, 0, 90);
            outletTank.transform.localScale = new Vector3(height * 0.5f, 0.15f, height * 0.5f);
            outletTank.name = "OutletTank";
            DestroyImmediate(outletTank.GetComponent<Collider>());
            outletTank.GetComponent<Renderer>().material = tankMat;

            // Top inlet port
            GameObject topPort = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            topPort.transform.SetParent(transform, false);
            topPort.transform.localPosition = new Vector3(0, height * 0.5f + 0.08f, 0);
            topPort.transform.localRotation = Quaternion.Euler(90, 0, 0);
            topPort.transform.localScale = new Vector3(0.08f, 0.1f, 0.08f);
            topPort.name = "InletPort";
            DestroyImmediate(topPort.GetComponent<Collider>());

            Material portMat = new Material(Shader.Find("Standard"));
            portMat.color = new Color(0.3f, 0.3f, 0.35f);
            topPort.GetComponent<Renderer>().material = portMat;

            // Bottom outlet port
            GameObject bottomPort = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bottomPort.transform.SetParent(transform, false);
            bottomPort.transform.localPosition = new Vector3(0, -height * 0.5f - 0.08f, 0);
            bottomPort.transform.localRotation = Quaternion.Euler(90, 0, 0);
            bottomPort.transform.localScale = new Vector3(0.08f, 0.1f, 0.08f);
            bottomPort.name = "OutletPort";
            DestroyImmediate(bottomPort.GetComponent<Collider>());
            bottomPort.GetComponent<Renderer>().material = portMat;
        }

        private void CreateBrackets()
        {
            // Corner brackets
            Vector3[] bracketPositions = new Vector3[]
            {
                new Vector3(-width * 0.5f, -height * 0.5f, 0),
                new Vector3(width * 0.5f, -height * 0.5f, 0),
                new Vector3(-width * 0.5f, height * 0.5f, 0),
                new Vector3(width * 0.5f, height * 0.5f, 0)
            };

            for (int i = 0; i < bracketPositions.Length; i++)
            {
                GameObject bracket = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bracket.transform.SetParent(transform, false);
                bracket.transform.localPosition = bracketPositions[i] + Vector3.back * (depth * 0.5f + 0.05f);
                bracket.transform.localScale = new Vector3(0.08f, 0.08f, 0.1f);
                bracket.name = $"Bracket_{i}";
                DestroyImmediate(bracket.GetComponent<Collider>());

                Material bracketMat = new Material(Shader.Find("Standard"));
                bracketMat.color = new Color(0.3f, 0.3f, 0.3f);
                bracket.GetComponent<Renderer>().material = bracketMat;
            }
        }

        private void CreateFins()
        {
            // Aluminum fins for cooling
            int finCount = 6;
            for (int i = 0; i < finCount; i++)
            {
                float yPos = -height * 0.5f + (height / (finCount - 1)) * i;
                GameObject fin = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fin.transform.SetParent(transform, false);
                fin.transform.localPosition = new Vector3(0, yPos, 0);
                fin.transform.localScale = new Vector3(width * 0.9f, 0.01f, depth * 0.8f);
                fin.name = $"Fin_{i}";
                DestroyImmediate(fin.GetComponent<Collider>());

                Material finMat = new Material(Shader.Find("Standard"));
                finMat.color = new Color(0.75f, 0.75f, 0.8f);
                fin.GetComponent<Renderer>().material = finMat;
            }
        }
    }
}
