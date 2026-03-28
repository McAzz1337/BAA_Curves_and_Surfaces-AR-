using UnityEngine;
using Oculus.Interaction.Input;
using Meta.XR.ImmersiveDebugger;

public class MenuCanvas : MonoBehaviour
{

    public Hand leftHand;
    public Hand rightHand;

    [DebugMember]
    public bool leftPinching;
    [DebugMember]
    public bool rightPinching;

    [DebugMember]
    public bool active = false;

    private bool leftWasPinchingLastFrame;
    private bool rightWasPinchingLastFrame;

    public CanvasGroup canvasGroup;

    public Camera cam;
    public float offsetX = 0.1f;
    public float offsetZ = 0.3f;

    void Aake()
    {
        hide();
    }

    void Update()
    {
        bool leftPinching = detectPinch(leftHand);
        bool rightPinching = detectPinch(rightHand);

        if ((leftPinching && !leftWasPinchingLastFrame) ||
         (rightPinching && !rightWasPinchingLastFrame))
        {
            active = !active;
            toggleCanvas(active);
            if (active)
            {
                positioncanvas();
            }
        }

        leftWasPinchingLastFrame = leftPinching;
        rightWasPinchingLastFrame = rightPinching;
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
