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
            float firtDistance = distance * 0.5f;
            for (int i = 0; i < nodes / 2; i++)
            {
                Vector3 positionA = Vector3.zero;
                Vector3 positionB = Vector3.zero;
                if (i == 0)
                {
                    positionA += right * firtDistance;
                    positionB -= right * firtDistance;
                }
                else
                {
                    positionA += right * distance;
                    positionB -= right * distance;
                }

                GameObject cpA = Instantiate(controlPointPrefab, positionA, Quaternion.identity);
                GameObject cpB = Instantiate(controlPointPrefab, positionB, Quaternion.identity);
                cpA.transform.SetParent(controlPointsParent.transform);
                cpB.transform.SetParent(controlPointsParent.transform);
            }
        }
        else
        {
            GameObject cp = Instantiate(controlPointPrefab, Vector3.zero, Quaternion.identity);
            cp.transform.SetParent(controlPointsParent.transform);
            for (int i = 1; i <= (nodes - 1) / 2; i++)
            {
                Vector3 positionA = Vector3.zero;
                Vector3 positionB = Vector3.zero;

                positionA += right * distance;
                positionB -= right * distance;

                GameObject cpA = Instantiate(controlPointPrefab, positionA, Quaternion.identity);
                GameObject cpB = Instantiate(controlPointPrefab, positionB, Quaternion.identity);
                cpA.transform.SetParent(controlPointsParent.transform);
                cpB.transform.SetParent(controlPointsParent.transform);
            }
        }

        controlPointsParent.gatherControlPoints();
        return bezierCurve;
    }

}
