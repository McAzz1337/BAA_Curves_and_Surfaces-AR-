using UnityEngine;

public class ControlPoints : MonoBehaviour
{
    private Transform[] transforms;

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
        return transforms;
    }

    public void gatherControlPoints()
    {
        int count = transform.childCount;
        transforms = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            transforms[i] = transform.GetChild(i);
        }
    }
}
