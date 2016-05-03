using UnityEngine;
using System.Collections;

public class WaterScript : MonoBehaviour {

    [Header("Parameters")]
    public float leftSide;
    public float waterWidth;
    public float top;
    public float waterBottom;
    [Space]
    public Material mat;
    public GameObject waterMesh;
    [Space]
    public float springConstant = 0.02f;
    public float damping = 0.04f;
    public float spread = 0.05f;
    public float zPos = 1;
    public float nodeMass = 1;

    //storage arrays
    private float[] xPositions;
    private float[] yPositions;
    private float[] velocities;
    private float[] accelerations;
    LineRenderer waterTop;
    GameObject[] meshObjects;
    Mesh[] meshes;
    GameObject[] colliders;

    float baseHeight;
    float left;
    float bottom;

    void Start()
    {
        SpawnWater(leftSide, waterWidth, top, waterBottom);
    }

    public void SpawnWater(float Left, float Width, float Top, float Bottom)
    {
        int edgeCount = Mathf.RoundToInt(Width) * 10; //10 edges per meter, vary for performance
        int nodeCount = edgeCount + 1;

        waterTop = gameObject.AddComponent<LineRenderer>();
        waterTop.material = mat;
        waterTop.material.renderQueue = 1000;
        waterTop.SetVertexCount(nodeCount);
        waterTop.SetWidth(0.3f, 0.3f); //width of line

        xPositions = new float[nodeCount];
        yPositions = new float[nodeCount];
        velocities = new float[nodeCount];
        accelerations = new float[nodeCount];

        meshObjects = new GameObject[edgeCount];
        meshes = new Mesh[edgeCount];
        colliders = new GameObject[edgeCount];

        baseHeight = Top;
        bottom = Bottom;
        left = Left;

        for (int i = 0; i < nodeCount; i++)
        {
            yPositions[i] = Top;
            xPositions[i] = Left + Width * i / edgeCount;
            accelerations[i] = 0;
            velocities[i] = 0;
            waterTop.SetPosition(i, new Vector3(xPositions[i], yPositions[i], zPos));
        }

        for (int i = 0; i < edgeCount; i++)
        {
            meshes[i] = new Mesh();
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(xPositions[i], yPositions[i], zPos);
            vertices[1] = new Vector3(xPositions[i + 1], yPositions[i + 1], zPos);
            vertices[2] = new Vector3(xPositions[i], bottom, zPos);
            vertices[3] = new Vector3(xPositions[i + 1], bottom, zPos);

            Vector2[] UVs = new Vector2[4];
            UVs[0] = new Vector2(0, 1);
            UVs[1] = new Vector2(1, 1);
            UVs[2] = new Vector2(0, 0);
            UVs[3] = new Vector2(1, 0);

            int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };

            meshes[i].vertices = vertices;
            meshes[i].uv = UVs;
            meshes[i].triangles = tris;

            meshObjects[i] = Instantiate(waterMesh, Vector3.zero, Quaternion.identity) as GameObject;
            meshObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshObjects[i].transform.parent = transform;
            meshObjects[i].layer = 0; //set meshes to parallax layer for shading

            colliders[i] = new GameObject();
            colliders[i].name = "Trigger";
            colliders[i].AddComponent<BoxCollider2D>();
            colliders[i].transform.parent = transform;
            colliders[i].transform.position = new Vector3(Left + Width * (i + 0.5f) / edgeCount, Top - 0.5f, 0);
            colliders[i].transform.localScale = new Vector3(Width / edgeCount, 1, 1);
            colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
            colliders[i].AddComponent<WaterDetector>();
        }
    }

    public void UpdateMeshes()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(xPositions[i], yPositions[i], zPos);
            vertices[1] = new Vector3(xPositions[i + 1], yPositions[i + 1], zPos);
            vertices[2] = new Vector3(xPositions[i], bottom, zPos);
            vertices[3] = new Vector3(xPositions[i + 1], bottom, zPos);

            meshes[i].vertices = vertices;
        }
    }
    public void Splash(float xPos, float velocity)
    {
        if (xPos >= xPositions[0] && xPos <= xPositions[xPositions.Length - 1])
        {
            xPos -= xPositions[0];

            int index = Mathf.RoundToInt((xPositions.Length - 1) * (xPos / (xPositions[xPositions.Length - 1] - xPositions[0])));
            velocities[index] = velocity;
        }
    }
    //fixedUpdate for physics calculations
    void FixedUpdate()
    {
        for (int i = 0; i < xPositions.Length; i++)
        {
            float force = springConstant * (yPositions[i] - baseHeight) + velocities[i] * damping;
            accelerations[i] = -force / nodeMass;
            yPositions[i] += velocities[i];
            velocities[i] += accelerations[i];
            waterTop.SetPosition(i, new Vector3(xPositions[i], yPositions[i], zPos));
        }

        float[] leftDeltas = new float[xPositions.Length];
        float[] rightDeltas = new float[xPositions.Length];

        for (int j = 0; j < 15; j++) //15 times for smooth transitions, can be varied
        {
            for (int i = 0; i < xPositions.Length; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (yPositions[i] - yPositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < xPositions.Length - 1)
                {
                    rightDeltas[i] = spread * (yPositions[i] - yPositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }
        }

        for (int i = 0; i < xPositions.Length; i++)
        {
            if (i > 0)
            {
                yPositions[i - 1] += leftDeltas[i];
            }
            if (i < xPositions.Length - 1)
            {
                xPositions[i + 1] += rightDeltas[i];
            }
        }

        UpdateMeshes();
    }
}
