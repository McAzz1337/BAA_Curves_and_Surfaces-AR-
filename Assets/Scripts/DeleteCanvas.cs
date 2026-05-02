using UnityEngine;
using UnityEngine.UI;

public class DeleteCanvas : MonoBehaviour
{

    [SerializeField]
    private Button deleteButton;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private MenuCanvas menuCanvas;

    [SerializeField]
    private TranslationSpeedCanvas translationSpeedCanvas;

    private bool posed = false;
    private bool posedLastFrame = false;

    private bool active = false;
    public float offsetX = 0.1f;
    public float offsetZ = 0.5f;


    [SerializeField]
    private ApplicationController appController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deleteButton.onClick.AddListener(onDeleteClicked);
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

    private void onDeleteClicked()
    {
        appController.deleteObject();
        hide();
    }

    public void hide()
    {
        active = false;
        toggleCanvas(active);
    }

    private void tuckAway()
    {
        Transform t = appController.Cam.transform;
        transform.position = t.position - t.forward * 100.0f - t.up * 100.0f;
    }


    void Update()
    {
        if (appController.RotationHand.IsConnected)
        {
            if (posed && !posedLastFrame)
            {
                menuCanvas.hide();
                translationSpeedCanvas.hide();
                appController.ControlsStatus.deactivateAllCointrols();
                active = !active;
                toggleCanvas(active);
                if (active)
                {
                    //positionCanvas();
                }

            }
            posedLastFrame = posed;
        }
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

    void LateUpdate()
    {
        if (!active) return;

        Camera cam = appController.Cam;

        Transform camTransform = cam.transform;


        transform.position =
            camTransform.position
            + camTransform.forward * offsetZ
            + camTransform.right * offsetX;

        Vector3 forward = camTransform.forward;
        Vector3 up = Vector3.up;

        transform.rotation = Quaternion.LookRotation(forward, up);
    }

}
