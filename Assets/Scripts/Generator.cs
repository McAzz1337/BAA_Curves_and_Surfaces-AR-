using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    [SerializeField]
    private GameObject berzierStructPrefab;

    [SerializeField]
    private GameObject berzierSurfacbeStructPrefab;
    [SerializeField]
    private GameObject bSplinesCurveStructPrefab;

    [SerializeField]
    private GameObject bSplinesSurfaceStructPrefab;

    [SerializeField]
    private GameObject controlPointPrefab;

    private ApplicationController appController;


    void Start()
    {
        appController = GetComponent<ApplicationController>();
        Debug.Log("ApplicationController: " + appController == null ? "Null" : "Not Null");
    }

    public GameObject generate(EType type, int nodes, Camera cam)
    {
        Debug.Log("Generating " + type + " with " + nodes + " nodes.");
        switch (type)
        {
            case EType.BEZIER_CURVE:
                return generateBezierCurvePoints(nodes, cam);
            case EType.BEZIER_SURFACE:
                return generateBezierSurfacePoints(nodes, cam);
            case EType.BSPLINES_CURVE:
                return generateBSplinesCurvePoints(nodes, cam);
            case EType.BSPLINES_SURFACE:
                return generateBSplinesSurfacePoints(nodes, cam);
            default:
                Debug.Log("Invalid type: " + type);
                return null;
        }
    }


    private List<Vector3> calculateCurveControlPoints(int nodes, Vector3 offset, Vector3 right)
    {
        List<Vector3> positions = new List<Vector3>();
        float distance = 0.25f;
        Vector3 farLeft = offset - right * ((nodes - 1) / 2.0f * distance);

        for (int i = 0; i < nodes; i++)
        {
            Vector3 position = farLeft + right * (i * distance);
            positions.Add(position);
        }

        return positions;
    }

    private List<Vector3> calculateSurfaceControlPoints(int nodes, Vector3 offset, Vector3 right)
    {
        List<Vector3> positions = new List<Vector3>();
        float distance = 0.25f;
        Vector3 farBottom = offset - Vector3.up * ((nodes - 1) / 2.0f * distance);

        for (int i = 0; i < nodes; i++)
        {
            List<Vector3> row = calculateCurveControlPoints(nodes, offset, right);
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

    private GameObject generateBezierCurvePoints(int nodes, Camera cam)
    {
        Debug.Log("Generating Bezier Curve");
        Vector3 inFront = cam.transform.position + cam.transform.forward * 0.5f;

        GameObject bezierStruct = Instantiate(berzierStructPrefab, inFront, Quaternion.identity);
        if (bezierStruct == null)
        {
            Debug.Log("Failed to instantiate bezier curve.");
            return null;
        }

        GameObject bezierCurve = bezierStruct.GetComponentInChildren<BezierCurveMesh>().gameObject;
        if (bezierCurve == null)
        {
            Debug.Log("bezierCurve instantiation was null");
            return null;
        }

        ControlPoints controlPointsParent = bezierCurve.GetComponentInChildren<ControlPoints>();
        if (controlPointsParent == null)
        {
            Debug.Log("ControlPoints component not found on bezier curve.");
            return null;
        }

        Vector3 offset = bezierStruct.transform.position + new Vector3(0.0f, -0.25f, 0.0f);
        List<Vector3> controlPointPositions = calculateCurveControlPoints(nodes, offset, cam.transform.right);
        for (int i = 0; i < nodes; i++)
        {
            GameObject controlPoint = Instantiate(controlPointPrefab, controlPointPositions[i], Quaternion.identity);
            controlPoint.transform.SetParent(controlPointsParent.transform);
            ChildGrabDetector cgd = controlPoint.GetComponent<ChildGrabDetector>();
            GrabDetector gd = bezierStruct.GetComponent<GrabDetector>();
            cgd.ParentDetector = gd;
        }

        Debug.Log("Bezier curve successfully instantiated");
        controlPointsParent.gatherControlPoints();
        return bezierStruct;
    }

    private GameObject generateBezierSurfacePoints(int nodes, Camera cam)
    {
        Vector3 inFront = cam.transform.position + cam.transform.forward * 0.5f;

        GameObject bezierSurfaceStruct = Instantiate(berzierSurfacbeStructPrefab, inFront, Quaternion.identity);
        if (bezierSurfaceStruct == null)
        {
            Debug.LogError("Failed to instantiate bezier surface.");
            return null;
        }

        GameObject bezierSurface = bezierSurfaceStruct.GetComponentInChildren<BezierSurface>().gameObject;
        if (bezierSurface == null)
        {
            Debug.LogError("Failed to get Component BezierSurface");
            return null;
        }
        Vector3 center = inFront - (nodes + 1) / 2 * 0.25f * Vector3.up;
        bezierSurface.transform.position = center;

        ControlPoints controlPointsParent = bezierSurface.GetComponentInChildren<ControlPoints>();
        if (controlPointsParent == null)
        {
            Debug.LogError("ControlPoints component not found on bezier surface.");
            return null;
        }

        List<Vector3> controlPointPositions = calculateSurfaceControlPoints(nodes, center, cam.transform.right);
        for (int i = 0; i < nodes * nodes; i++)
        {
            GameObject controlPoint = Instantiate(controlPointPrefab, controlPointPositions[i], Quaternion.identity);
            controlPoint.transform.SetParent(controlPointsParent.transform);
            ChildGrabDetector cgd = controlPoint.GetComponent<ChildGrabDetector>();
            GrabDetector gd = bezierSurfaceStruct.GetComponent<GrabDetector>();
            cgd.ParentDetector = gd;
        }

        controlPointsParent.gatherControlPoints();
        return bezierSurfaceStruct;
    }


    private GameObject generateBSplinesCurvePoints(int nodes, Camera cam)
    {

        Vector3 inFront = cam.transform.position + cam.transform.forward * 0.5f;

        GameObject bSplinesStruct = Instantiate(bSplinesCurveStructPrefab, inFront, Quaternion.identity);

        GameObject bSplinesCurve = bSplinesStruct.GetComponentInChildren<BSplinesCurve>().gameObject;
        if (bSplinesCurve == null)
        {
            Debug.LogError("Failed to instantiate b splines curve.");
            return null;
        }

        ControlPoints controlPointsParent = bSplinesCurve.GetComponentInChildren<ControlPoints>();
        if (controlPointsParent == null)
        {
            Debug.LogError("ControlPoints component not found on bezier surface.");
            return null;
        }

        Vector3 offset = bSplinesStruct.transform.position + new Vector3(0.0f, -0.25f, 0.0f);
        List<Vector3> controlPointPositions = calculateCurveControlPoints(nodes, offset, cam.transform.right);
        for (int i = 0; i < nodes; i++)
        {
            GameObject controlPoint = Instantiate(controlPointPrefab, controlPointPositions[i], Quaternion.identity);
            controlPoint.transform.SetParent(controlPointsParent.transform);
            ChildGrabDetector cgd = controlPoint.GetComponent<ChildGrabDetector>();
            GrabDetector gd = bSplinesStruct.GetComponent<GrabDetector>();
            cgd.ParentDetector = gd;
        }

        controlPointsParent.gatherControlPoints();
        return bSplinesStruct;
    }

    private GameObject generateBSplinesSurfacePoints(int nodes, Camera cam)
    {
        Vector3 inFront = cam.transform.position + cam.transform.forward * 0.5f;

        GameObject bSplinesSurfaceStruct = Instantiate(bSplinesSurfaceStructPrefab, inFront, Quaternion.identity);
        if (bSplinesSurfaceStruct == null)
        {
            Debug.LogError("Failed to instantiate b splines surface.");
            return null;
        }

        GameObject bSplinesSurface = bSplinesSurfaceStruct.GetComponentInChildren<BSplinesSurface>()?.gameObject;
        if (bSplinesSurface == null)
        {
            Debug.LogError("Failed to get Component BSplinesSurface");
            return null;
        }

        Vector3 center = inFront - (nodes + 1) / 2 * 0.25f * Vector3.up;
        bSplinesSurface.transform.position = center;

        ControlPoints controlPointsParent = bSplinesSurface.GetComponentInChildren<ControlPoints>();
        if (controlPointsParent == null)
        {
            Debug.LogError("ControlPoints component not found on b splines surface.");
            return null;
        }

        List<Vector3> controlPointPositions = calculateSurfaceControlPoints(nodes, center, cam.transform.right);

        for (int i = 0; i < nodes * nodes; i++)
        {
            GameObject controlPoint = Instantiate(controlPointPrefab, controlPointPositions[i], Quaternion.identity);
            controlPoint.transform.SetParent(controlPointsParent.transform);
            ChildGrabDetector cgd = controlPoint.GetComponent<ChildGrabDetector>();
            GrabDetector gd = bSplinesSurfaceStruct.GetComponent<GrabDetector>();
            cgd.ParentDetector = gd;
        }

        controlPointsParent.gatherControlPoints();

        return bSplinesSurfaceStruct;
    }
}
