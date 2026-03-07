using System;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveTube : MonoBehaviour
{
    public ControlPoints controlPoints;
    public GameObject cylinderPrefab; // small cylinder
    public int numSamples = 20;
    public float radius = 0.05f;

    private List<GameObject> segments = new List<GameObject>();


    void Update()
    {
        Debug.Log("Updating Bezier Curve Tube");
        foreach (var seg in segments)
            Destroy(seg);
        segments.Clear();

        List<Vector3> cPoints = new List<Vector3>();
        for (int i = 0; i < controlPoints.controlPoints.Count; i++)
        {
            cPoints.Add(controlPoints.controlPoints[i].position);
        }

        List<Vector3> points = Bezier.curve(cPoints, numSamples);

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[i + 1];
            Vector3 dir = end - start;
            float length = dir.magnitude;

            GameObject seg = Instantiate(cylinderPrefab);
            seg.transform.position = start + dir / 2;
            seg.transform.up = dir.normalized;
            seg.transform.localScale = new Vector3(radius, length / 2, radius);
            seg.transform.SetParent(transform);
            segments.Add(seg);
        }
    }
}