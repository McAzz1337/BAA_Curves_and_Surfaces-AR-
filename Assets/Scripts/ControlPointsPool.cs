using System.Collections.Generic;
using UnityEngine;

public class ControlPointsPool : MonoBehaviour
{
    private List<GameObject> pool = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject g = transform.GetChild(i).gameObject;
            g.SetActive(false);
            pool.Add(g);
        }
    }

    public List<GameObject> requestControlPoints(int n)
    {
        if (pool.Count == 0)
        {
            Debug.Log("pool count is 0");
            return null;
        }
        else
        {
            List<GameObject> controlPoints = new List<GameObject>();
            for (int i = 0; i < n; i++)
            {
                GameObject controlPoint = pool[0];
                pool.RemoveAt(0);
                controlPoints.Add(controlPoint);
            }

            Debug.Log("Returning " + controlPoints.Count + " control points");
            return controlPoints;
        }
    }

    public void insertControlPoint(GameObject controlPoint)
    {
        controlPoint.transform.position = Vector3.zero;
        controlPoint.SetActive(false);
        controlPoint.transform.SetParent(transform);
        pool.Add(controlPoint);
    }

}
