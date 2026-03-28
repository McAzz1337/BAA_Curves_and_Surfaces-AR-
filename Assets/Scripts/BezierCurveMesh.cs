using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierCurveMesh : MonoBehaviour
{
    public ControlPoints controlPoints;
    public GameObject cylinderPrefab;
    public int numSamples = 20;
    public float radius = 0.05f;

    private Mesh mesh;

    void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        mesh = mf.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "BezierCurveMesh";
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

        /*
                List<Vector3> positions = controlPoints.getTransforms()
                    .Select(t => transform.InverseTransformPoint(t.transform.position)).ToList();
          */
        List<Vector3> positions = controlPoints.getTransforms()
                    .Select(t => transform.position).ToList();

        List<Vector3> points = Bezier.curve(positions, numSamples);
        generateMesh(points);
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

    private List<Vector3> generateCircle(Vector3 position, Vector3? last, Vector3? next, float radius = 0.5f, int segments = 24)
    {
        if (!last.HasValue)
        {
            last = position + (position - next);
        }
        else if (!next.HasValue)
        {
            next = position + (position - last);
        }

        Vector3 toLast = (last.Value - position).normalized;
        Vector3 toNext = (next.Value - position).normalized;

        Vector3 tangent = ((toLast + toNext) * 0.5f).normalized;
        if (tangent == Vector3.zero)
            tangent = toLast;

        Vector3 normal = Vector3.Cross(toLast, toNext).normalized;
        if (normal == Vector3.zero)
        {
            normal = Vector3.Cross(tangent, Vector3.up).magnitude > 0.001f
                ? Vector3.Cross(tangent, Vector3.up).normalized
                : Vector3.Cross(tangent, Vector3.right).normalized;
        }

        Vector3 axisA = Vector3.Cross(normal, tangent).normalized;
        Vector3 axisB = Vector3.Cross(normal, axisA).normalized;

        var circlePoints = new List<Vector3>(segments);
        for (int i = 0; i < segments; i++)
        {
            float theta = Mathf.PI * 2f * i / segments;
            Vector3 offset = (Mathf.Cos(theta) * axisA + Mathf.Sin(theta) * axisB) * radius;
            circlePoints.Add(position + offset);
        }

        return circlePoints;
    }


}