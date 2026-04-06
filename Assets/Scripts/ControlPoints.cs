using System.Collections.Generic;
using UnityEngine;

public class ControlPoints : MonoBehaviour
{
    private Transform[] transforms;
    [SerializeField]
    private ControlPointsPool controlPointPool;
    public ControlPointsPool ControlPointPool
    {
        set { controlPointPool = value; }
    }

    public bool log = false;
    void Awake()
    {

    }
    void Start()
    {
        gatherControlPoints();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform[] getTransforms()
    {
        if (log)
        {
            Debug.Log("Getting points: " + transforms.Length);
            log = false;
        }
        return transforms;
    }

    public void addControlPoint(Transform transform)
    {
        Debug.Log("ControlPoint added: " + transforms.Length);
    }

    public void gatherControlPoints()
    {
        int count = transform.childCount;
        Debug.Log("Gather: " + count);
        transforms = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            transforms[i] = transform.GetChild(i);
        }
    }


    void OnDestroy()
    {
        if (controlPointPool != null)
        {
            foreach (var cp in transforms)
            {
                controlPointPool.insertControlPoint(cp.gameObject);
            }
        }
    }


}
