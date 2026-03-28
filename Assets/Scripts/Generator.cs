using System;
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
                return null;
            case EType.BSPLINES_CURVE:
                return null;
            default:
                Debug.Log("Invalid type: " + type);
                return null;
        }
    }


    private GameObject generateCurvePoints(int nodes, Camera cam)
    {
        bool evenNodesCount = nodes % 2 == 0;
        float distance = 0.25f;
        Vector3 inFront = cam.transform.position + cam.transform.forward * 0.5f;
        Vector3 right = cam.transform.right;
        Vector3 up = cam.transform.up;

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

        if (evenNodesCount)
        {
            Vector3 farLeft = inFront - right * ((nodes - 1) * distance * 0.5f);
            for (int i = 0; i < nodes; i++)
            {
                Vector3 position = farLeft + right * (i * distance);

                GameObject cp = Instantiate(controlPointPrefab, position, Quaternion.identity);
                cp.transform.SetParent(controlPointsParent.transform);
            }
        }
        else
        {
            Vector3 farLeft = inFront - right * (distance * (nodes / 2));
            for (int i = 0; i < nodes; i++)
            {
                Vector3 position = farLeft + right * (i * distance);

                GameObject cp = Instantiate(controlPointPrefab, position, Quaternion.identity);
                cp.transform.SetParent(controlPointsParent.transform);
            }
        }

        controlPointsParent.gatherControlPoints();
        return bezierCurve;
    }

}
