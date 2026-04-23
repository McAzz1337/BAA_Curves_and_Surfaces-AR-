using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BSplinesSurface : MonoBehaviour
{
    public Transform[,] controlPoints;
    public ControlPoints controlPointsProvider;

    public int resolution = 10;
    public int degree = 3;

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
            mesh.name = "BSplineSurface Mesh";
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

    public void UpdateSurface()
    {
        if (mesh == null)
            InitializeMesh();

        setupControlPoints();

        if (controlPoints == null || controlPoints[0, 0] == null)
            return;

        if (vertices == null || vertices.Length != (resolution + 1) * (resolution + 1))
        {
            generateMesh();
        }

        updateVertices();
    }

    void setupControlPoints()
    {
        if (controlPointsProvider == null) return;

        Transform[] points = controlPointsProvider.getTransforms();
        int gridSize = Mathf.RoundToInt(Mathf.Sqrt(points.Length));

        if (gridSize * gridSize != points.Length || gridSize == 0)
            return;

        controlPoints = new Transform[gridSize, gridSize];

        int index = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                controlPoints[i, j] = points[index++];
            }
        }
    }

    void generateMesh()
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
    }

    void updateVertices()
    {
        int index = 0;

        for (int y = 0; y <= resolution; y++)
        {
            float v = y / (float)resolution;

            for (int x = 0; x <= resolution; x++)
            {
                float u = x / (float)resolution;

                vertices[index] = evaluateSurface(u, v);
                uvs[index] = new Vector2(u, v);
                index++;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    Vector3 evaluateSurface(float uNorm, float vNorm)
    {
        int rows = controlPoints.GetLength(0);
        int cols = controlPoints.GetLength(1);

        int degreeU = Mathf.Min(degree, cols - 1);
        int degreeV = Mathf.Min(degree, rows - 1);

        List<Vector3> tempPoints = new List<Vector3>();

        for (int i = 0; i < rows; i++)
        {
            List<Vector3> row = getRow(i);

            if (row.Count < degreeU + 1)
            {
                tempPoints.Add(Vector3.zero);
                continue;
            }

            float[] knotsU = generateKnotsForCount(row.Count, degreeU);

            float tMin = knotsU[degreeU];
            float tMax = knotsU[knotsU.Length - degreeU - 1];

            float t = Mathf.Lerp(tMin, tMax, uNorm);
            t = Mathf.Min(t, tMax - 0.0001f);

            Vector3 point = BSplines.evaluate(t, row, degreeU, knotsU);
            tempPoints.Add(point);
        }

        if (tempPoints.Count < degreeV + 1)
            return Vector3.zero;

        float[] knotsV = generateKnotsForCount(tempPoints.Count, degreeV);

        float vMin = knotsV[degreeV];
        float vMax = knotsV[knotsV.Length - degreeV - 1];

        float tv = Mathf.Lerp(vMin, vMax, vNorm);
        tv = Mathf.Min(tv, vMax - 0.0001f);

        return BSplines.evaluate(tv, tempPoints, degreeV, knotsV);
    }

    List<Vector3> getRow(int row)
    {
        List<Vector3> list = new List<Vector3>();
        int cols = controlPoints.GetLength(1);

        for (int j = 0; j < cols; j++)
        {
            if (controlPoints[row, j] != null)
            {
                list.Add(transform.InverseTransformPoint(controlPoints[row, j].position));
            }
        }
        return list;
    }


    float[] generateKnotsForCount(int count, int degree)
    {
        int n = count - 1;
        int p = degree;

        int m = n + p + 1;
        float[] knots = new float[m + 1];

        for (int i = 0; i <= p; i++)
            knots[i] = 0;

        for (int i = p + 1; i <= n; i++)
            knots[i] = i - p;

        for (int i = n + 1; i <= m; i++)
            knots[i] = n - p + 1;

        return knots;
    }
}