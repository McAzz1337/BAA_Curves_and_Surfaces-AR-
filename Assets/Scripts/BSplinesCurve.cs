using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BSplinesCurve : MonoBehaviour
{
    public ControlPoints controlPoints;
    public int numSamples = 20;
    public float radius = 0.05f;
    public int degree = 3;
    private float[] knots;
    private Mesh mesh;

    void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        Mesh mesh = mf.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            this.mesh = mesh;
            mesh.name = "BSplinesCurveMesh";
            mf.sharedMesh = mesh;
        }

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null && mr.sharedMaterial == null)
        {
            mr.sharedMaterial = new Material(Shader.Find("Standard")) { color = Color.red };
        }
    }

    void Start()
    {
        generateKnots();
    }

    void Update()
    {
        if (controlPoints == null) return;
        List<Vector3> positions = controlPoints.getTransforms()
            .Select(t => transform.InverseTransformPoint(t.position)).ToList();
        if (positions.Count < degree + 1) return;
        float tMax = (int)generateKnots();
        List<Vector3> points = new List<Vector3>();
        float tMin = 0;
        for (int i = 0; i < numSamples; i++)
        {
            float t = tMin + (float)i / numSamples * (tMax - tMin);
            points.Add(evaluate(t, positions));
        }
        generateMesh(points);
    }

    int generateKnots()
    {
        if (controlPoints == null) return 0;
        int n = controlPoints.getTransforms().Length - 1;
        if (n < degree) return 0;
        int numKnots = n + degree + 1;
        knots = new float[numKnots + 1];
        // Clamped knot vector
        int firstEnd = degree;
        int lastStart = n + 1;
        int numMiddle = lastStart - firstEnd - 1;

        // try
        int indx = 0;
        int val = 0;
        for (int i = 0; i <= degree; i++)
        {
            knots[indx++] = val;
        }
        for (int i = 0; i < n - degree - 1; i++)
        {
            knots[indx++] = ++val;
        }
        ++val;
        for (int i = 0; i <= degree; i++)
        {
            knots[indx++] = val;
        }

        return val;
    }

    float basis(int i, int p, float t)
    {
        if (p == 0)
        {
            if (t >= knots[i] && t <= knots[i + 1]) return 1;
            return 0;
        }
        float left = 0;
        if (knots[i + p] != knots[i])
            left = (t - knots[i]) / (knots[i + p] - knots[i]);
        float right = 0;
        if (knots[i + p + 1] != knots[i + 1])
            right = (knots[i + p + 1] - t) / (knots[i + p + 1] - knots[i + 1]);
        return left * basis(i, p - 1, t) + right * basis(i + 1, p - 1, t);
    }

    public Vector3 evaluate(float t, List<Vector3> positions)
    {
        if (positions.Count < degree + 1 || knots == null) return Vector3.zero;
        Vector3 result = Vector3.zero;
        for (int i = 0; i < positions.Count; i++)
        {
            float b = basis(i, degree, t);
            result += b * positions[i];
        }
        return result;
    }

    private void generateMesh(List<Vector3> points)
    {
        int circleSegments = 20;

        if (points == null || points.Count < 2)
        {
            mesh.Clear();
            return;
        }

        List<Vector3> vertices = new List<Vector3>(points.Count * circleSegments);
        List<int> indices = new List<int>((points.Count - 1) * circleSegments * 6);

        Vector3 prevTangent = Vector3.zero;
        Vector3 prevNormal = Vector3.zero;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i];
            Vector3 tangent;
            if (i == 0)
                tangent = (points[i + 1] - p).normalized;
            else if (i == points.Count - 1)
                tangent = (p - points[i - 1]).normalized;
            else
                tangent = (points[i + 1] - points[i - 1]).normalized;

            Vector3 normal;
            if (i == 0)
            {
                Vector3 worldUp = Vector3.up;
                if (Mathf.Abs(Vector3.Dot(tangent, worldUp)) > 0.99f)
                    worldUp = Vector3.forward;

                normal = Vector3.Cross(tangent, worldUp).normalized;
            }
            else
            {
                if (prevTangent == Vector3.zero)
                {
                    normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    if (normal == Vector3.zero)
                        normal = Vector3.Cross(tangent, Vector3.forward).normalized;
                }
                else
                {
                    Quaternion rot = Quaternion.FromToRotation(prevTangent, tangent);
                    normal = rot * prevNormal;
                    normal = Vector3.ProjectOnPlane(normal, tangent).normalized;
                }
            }

            Vector3 axisA = Vector3.Cross(tangent, normal).normalized;
            Vector3 axisB = Vector3.Cross(tangent, axisA).normalized;

            for (int s = 0; s < circleSegments; s++)
            {
                float theta = Mathf.PI * 2f * s / circleSegments;
                Vector3 offset = (Mathf.Cos(theta) * axisA + Mathf.Sin(theta) * axisB) * radius * 0.5f;
                vertices.Add(p + offset);
            }

            prevTangent = tangent;
            prevNormal = normal;
        }

        for (int ring = 0; ring < points.Count - 1; ring++)
        {
            int ringStart = ring * circleSegments;
            int nextRingStart = (ring + 1) * circleSegments;

            for (int j = 0; j < circleSegments; j++)
            {
                int current = ringStart + j;
                int next = ringStart + ((j + 1) % circleSegments);
                int currentNext = nextRingStart + j;
                int nextNext = nextRingStart + ((j + 1) % circleSegments);

                indices.Add(current);
                indices.Add(currentNext);
                indices.Add(next);

                indices.Add(next);
                indices.Add(currentNext);
                indices.Add(nextNext);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
