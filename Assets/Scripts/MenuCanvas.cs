using UnityEngine;
using Oculus.Interaction.Input;
using Meta.XR.ImmersiveDebugger;

public class MenuCanvas : MonoBehaviour
{

    public ApplicationController appController;

    [DebugMember]
    public bool active = false;

    private bool rotationWasPinchingLastFrame;
    private bool translationWasPinchingLastFrame;

    private bool menuPressedLastFrame;

    public CanvasGroup canvasGroup;

    public Camera cam;
    public float offsetX = 0.1f;
    public float offsetZ = 0.3f;

    void Start()
    {
        hide();
    }

    void Update()
    {
        if (appController.RotationActivationHand && appController.TranslationActivationHand.IsConnected)
        {

            bool rotationHandPinching = detectPinch(appController.TranslationActivationHand);
            bool translationHandPinching = detectPinch(appController.RotationActivationHand);

            if ((rotationHandPinching && !rotationWasPinchingLastFrame) ||
             (translationHandPinching && !translationWasPinchingLastFrame))
            {
                active = !active;
                toggleCanvas(active);
                if (active)
                {
                    positioncanvas();
                }
            }

            rotationWasPinchingLastFrame = rotationHandPinching;
            translationWasPinchingLastFrame = translationHandPinching;
        }
        else if (appController.RotationController.IsConnected && appController.TranslationController.IsConnected)
        {
            Controller controller;
            if (appController.RotationActivationHand.Handedness == Handedness.Left)
            {
                controller = appController.RotationController;
            }
            else
            {
                controller = appController.TranslationController;
            }

            bool menuPressed = controller.ControllerInput.SecondaryButton;

            if (menuPressed && !menuPressedLastFrame)

            {
                active = !active;
                toggleCanvas(active);
                if (active)
                {
                    positioncanvas();
                }
            }

            menuPressedLastFrame = menuPressed;
        }
    }

    public void hide()
    {
        active = false;
        toggleCanvas(active);
    }

    private void toggleCanvas(bool show)
    {
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
    }

    private void positioncanvas()
    {
        transform.position = cam.transform.position + new Vector3(0.0f, 0.0f, offsetZ);

        Quaternion rot = Quaternion.LookRotation(
            transform.position - cam.transform.position);

        transform.rotation = rot;
        transform.position += new Vector3(offsetX, 0.0f, 0.0f);
    }

    private bool detectPinch(Hand hand)
    {
        bool thumbPinching = hand.GetFingerIsPinching(HandFinger.Thumb);
        bool ringPinching = hand.GetFingerIsPinching(HandFinger.Ring);

        return thumbPinching && ringPinching;
    }
}
