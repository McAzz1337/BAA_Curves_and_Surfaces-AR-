using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    public static Vector3 deCasteljau(List<Vector3> controlPoints, float t)
    {
        List<Vector3> newControlPoints = new List<Vector3>(controlPoints);
        while (controlPoints.Count > 1)
        {
            newControlPoints.Clear();
            for (int i = 0; i < controlPoints.Count - 1; i++)
            {
                Vector3 p0 = controlPoints[i];
                Vector3 p1 = controlPoints[i + 1];
                Vector3 newPoint = (1 - t) * p0 + t * p1;
                newControlPoints.Add(newPoint);
            }
            controlPoints = new List<Vector3>(newControlPoints);
        }

        return controlPoints[0];
    }




    public static List<Vector3> curve(List<Vector3> controlPoints, int samples)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / (samples - 1);
            points.Add(deCasteljau(controlPoints, t));
        }
        return points;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
