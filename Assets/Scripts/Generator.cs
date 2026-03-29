using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Generator : MonoBehaviour
{

    [SerializeField]
    private GameObject berzierCurve;

    [SerializeField]
    private GameObject berzierSurface;
    [SerializeField]
    private GameObject bSplinesCurve;

    [SerializeField]
    private GameObject controlPointPrefab;


    public GameObject generate(EType type, int nodes, Camera cam)
    {
        switch (type)
        {
            case EType.BEZIER_CURVVE:
                return generateCurvePoints(nodes, cam);
            case EType.BEZIER_SURFACE:
                return generateBezierSurfacePoints(nodes, cam);
            case EType.BSPLINES_CURVE:
                return null;
            default:
                Debug.Log("Invalid type: " + type);
                return null;
        }
    }


    private List<Vector3> calculateCurveControlPoints(int nodes, Camera cam)
    {
        List<Vector3> positions = new List<Vector3>();
        float distance = 0.25f;
        Vector3 right = cam.transform.right;
        Vector3 inFront = cam.transform.position + cam.transform.forward * 0.5f;
        Vector3 farLeft = inFront - right * ((nodes - 1) / 2.0f * distance);

        for (int i = 0; i < nodes; i++)
        {
            Vector3 position = farLeft + right * (i * distance);
            positions.Add(position);
        }

        return positions;
    }

    private List<Vector3> calculateSurfaceControlPoints(int nodes, Camera cam)
    {
        List<Vector3> positions = new List<Vector3>();
        float distance = 0.25f;
        Vector3 inFront = cam.transform.position + cam.transform.forward * 0.5f;
        Vector3 farBottom = inFront - Vector3.up * ((nodes - 1) / 2.0f * distance);

        for (int i = 0; i < nodes; i++)
        {
            List<Vector3> row = calculateCurveControlPoints(nodes, cam);
            float y = farBottom.y + i * distance;
            for (int j = 0; j < row.Count; j++)
            {
                Vector3 v = row[j];
                v.y = y;
                row[j] = v;
            }

            positions.AddRange(row);
        }

        return positions;
    }

    private GameObject generateCurvePoints(int nodes, Camera cam)
    {
        Vector3 inFront = cam.transform.position + cam.transform.forward * 0.5f;

        GameObject bezierCurve = Instantiate(berzierCurve, inFront, Quaternion.identity);
        if (bezierCurve == null)
        {
            Debug.LogError("Failed to instantiate bezier curve.");
            return null;
        }

        ControlPoints controlPointsParent = bezierCurve.GetComponentInChildren<ControlPoints>();
        if (controlPointsParent == null)
        {
            Debug.LogError("ControlPoints component not found on bezier curve.");
            return null;
        }

        List<Vector3> controlPoints = calculateCurveControlPoints(nodes, cam);
        foreach (Vector3 position in controlPoints)
        {
            GameObject cp = Instantiate(controlPointPrefab, position, Quaternion.identity);
            cp.transform.SetParent(controlPointsParent.transform);
        }

        controlPointsParent.gatherControlPoints();
        return bezierCurve;
    }

    private GameObject generateBezierSurfacePoints(int nodes, Camera cam)
    {
        Vector3 inFront = cam.transform.position + cam.transform.forward * 0.5f;

        GameObject bezierSurface = Instantiate(berzierSurface, inFront, Quaternion.identity);
        if (bezierSurface == null)
        {
            Debug.LogError("Failed to instantiate bezier surface.");
            return null;
        }

        ControlPoints controlPointsParent = bezierSurface.GetComponentInChildren<ControlPoints>();
        if (controlPointsParent == null)
        {
            Debug.LogError("ControlPoints component not found on bezier surface.");
            return null;
        }

        List<Vector3> controlPoints = calculateSurfaceControlPoints(nodes, cam);
        foreach (Vector3 position in controlPoints)
        {
            GameObject cp = Instantiate(controlPointPrefab, position, Quaternion.identity);
            cp.transform.SetParent(controlPointsParent.transform);
        }

        controlPointsParent.gatherControlPoints();
        return bezierSurface;
    }

}
