using UnityEngine;
using Oculus.Interaction.Input;
using Meta.XR.ImmersiveDebugger;
using Unity.VisualScripting;

public class MenuCanvas : MonoBehaviour
{

    public ApplicationController appController;

    [DebugMember]
    public bool active = false;
    private bool posed = false;
    private bool posedLastFrame = false;

    public CanvasGroup canvasGroup;

    [SerializeField]
    private DeleteCanvas deleteCanvas;

    public Camera cam;
    public float offsetX = 0.1f;
    public float offsetZ = 0.3f;

    void Start()
    {
        hide();
    }

    public void onPose()
    {
        posed = true;
    }

    public void onUnpose()
    {
        posed = false;
    }

    void Update()
    {
        if (appController.RotationHand.IsConnected && appController.TranslationHand.IsConnected)
        {

            if (posed && !posedLastFrame)
            {
                deleteCanvas.hide();
                active = !active;
                toggleCanvas(active);
                if (active)
                {
                    //positioncanvas();
                }
            }

            posedLastFrame = posed;
        }
    }

    public void hide()
    {
        active = false;
        toggleCanvas(active);
    }

    private void tuckAway()
    {
        Transform t = cam.transform;
        transform.position = t.position - t.forward * 100.0f - t.up * 100.0f;
    }

    private void toggleCanvas(bool show)
    {
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
        if (!show)
        {
            tuckAway();
        }
    }

    private void positioncanvas()
    {
        transform.position = cam.transform.position + new Vector3(0.0f, 0.0f, offsetZ);

        Quaternion rot = Quaternion.LookRotation(
            transform.position - cam.transform.position);

        transform.rotation = rot;
        transform.position += new Vector3(offsetX, 0.0f, 0.0f);
    }

    void LateUpdate()
    {
        if (!active) return;

        Camera cam = appController.Cam;

        Transform camTransform = cam.transform;


        transform.position =
            camTransform.position
            + camTransform.forward * offsetZ
            + camTransform.right * offsetX;

        transform.rotation = camTransform.rotation;
    }

}
