using UnityEngine;

public class ControlPoints : MonoBehaviour
{
    private Transform[] transforms;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        transforms = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            transforms[i] = transform.GetChild(i);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform[] getTransforms()
    {
        return transforms;
    }
}
