using UnityEngine;

public class MenuCanvas : MonoBehaviour
{


    public Transform cam;
    public Vector3 offset = new Vector3(-0.3f, 0.2f, 0.6f);

    void LateUpdate()
    {
        transform.position = cam.position + cam.TransformDirection(offset);
        transform.rotation = Quaternion.LookRotation(
            transform.position - cam.position
        );
    }
}
