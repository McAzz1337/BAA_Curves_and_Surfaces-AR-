using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BezierSurface : MonoBehaviour
{
    public Transform[,] controlPoints;

    public ControlPoints controlPointsProvider;
    public int resolution = 1;
    public bool showControlPointGizmos = true;
    public bool showSurfaceGizmos = true;
    public float gizmoControlPointSize = 0.1f;
    public float gizmoSurfaceResolution = 10;



    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;


    void Awake()
    {
        InitializeMesh();
    }

    void InitializeMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        mesh = mf.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "BezierSurface Mesh";
            mf.sharedMesh = mesh;
        }

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null && mr.sharedMaterial == null)
        {
            mr.sharedMaterial = new Material(Shader.Find("Standard")) { color = Color.red };
        }
    }



    void Update()
    {
        UpdateSurface();
    }

    [ContextMenu("UpdateSurface")]
    public void UpdateSurface()
    {
        if (mesh == null)
            InitializeMesh();

        // Debug.Log("ControlPoints: " + controlPointsProvider == null ? "Null" : "Not Null");

        setupControlPoints();
        if (controlPoints[0, 0] != null)
        {
            if (vertices == null || vertices.Length != (resolution + 1) * (resolution + 1))
            {
                GenerateMesh();
            }
            updateVertices();
        }
    }



    void setupControlPoints()
    {
        if (controlPointsProvider != null)
        {
            Transform[] points = controlPointsProvider.getTransforms();

            int gridSize = Mathf.RoundToInt(Mathf.Sqrt(points.Length));

            if (gridSize * gridSize == points.Length && gridSize > 0)
            {
                controlPoints = new Transform[gridSize, gridSize];

                int index = 0;
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        controlPoints[i, j] = points[index++];
                    }
                }
                return;
            }
        }
    }

    void GenerateMesh()
    {
        int vertCount = (resolution + 1) * (resolution + 1);
        vertices = new Vector3[vertCount];
        uvs = new Vector2[vertCount];

        triangles = new int[resolution * resolution * 6];

        int t = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = y * (resolution + 1) + x;

                triangles[t++] = i;
                triangles[t++] = i + 1;
                triangles[t++] = i + resolution + 2;

                triangles[t++] = i;
                triangles[t++] = i + resolution + 2;
                triangles[t++] = i + resolution + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void updateVertices()
    {
        int index = 0;
        for (int y = 0; y <= resolution; y++)
        {
            float v = (resolution > 0) ? (y / (float)resolution) : 0f;
            for (int x = 0; x <= resolution; x++)
            {
                float u = (resolution > 0) ? (x / (float)resolution) : 0f;
                vertices[index] = EvaluateSurface(u, v);
                uvs[index] = new Vector2(u, v);
                index++;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    Vector3 EvaluateSurface(float u, float v)
    {
        Vector3 point = Vector3.zero;

        int maxI = -1;
        int maxJ = -1;
        for (int i = 0; i < controlPoints.GetLength(0); i++)
        {
            for (int j = 0; j < controlPoints.GetLength(1); j++)
            {
                if (controlPoints[i, j] != null)
                {
                    maxI = Mathf.Max(maxI, i);
                    maxJ = Mathf.Max(maxJ, j);
                }
            }
        }

        if (maxI < 0 || maxJ < 0)
        {
            return point;
        }

        int degreeU = Mathf.Max(1, maxI);
        int degreeV = Mathf.Max(1, maxJ);

        for (int i = 0; i <= degreeU; i++)
        {
            for (int j = 0; j <= degreeV; j++)
            {
                if (controlPoints[i, j] == null) continue;

                float bu = Bernstein(i, degreeU, u);
                float bv = Bernstein(j, degreeV, v);

                Vector3 cpLocal = transform.InverseTransformPoint(controlPoints[i, j].position);
                point += bu * bv * cpLocal;
            }
        }

        return point;
    }

    float Bernstein(int i, int n, float t)
    {
        return Binomial(n, i) * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - i);
    }



    int Binomial(int n, int k)
    {
        int result = 1;

        for (int i = 1; i <= k; i++)
        {
            result *= (n - (k - i));
            result /= i;
        }

        return result;
    }


}