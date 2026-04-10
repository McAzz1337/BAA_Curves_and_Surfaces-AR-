using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;
using System.Collections.Generic;

public class HandPinchTranslation : MonoBehaviour
{
    [SerializeField]
    private ApplicationController appController;

    [DebugMember]
    public ControlsStatus controlsStatus;

    [DebugMember]
    public bool pinching = false;
    [DebugMember]
    public bool wasPinchingLastFrame = false;

    public bool buttonPressedLastFrame = false;

    [DebugMember]
    public Vector3 handStartPosition;

    [DebugMember]
    public Vector3 controllerStartPostion;
    [DebugMember]
    public Vector3 objectStartPosition;


    [DebugMember]
    public bool thumbPinching;
    [DebugMember]
    public bool middlePinching;

    private float sensitivity = 1.0f;

    public float Sensitivity
    {
        get => sensitivity;
        set
        {
            sensitivity = value;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void onPose()
    {
        Debug.Log("Scissor pose detected");
        pinching = true;
    }

    public void onUnposed()
    {
        Debug.Log("Scissor unpose detected");
        pinching = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (appController.TranslationActivationHand.IsConnected)
        {
            bool validOrientation = HandUtils.writstNormalDotCamForwardGreaterTan(appController.TranslationActivationHand,
                 appController.Cam,
                  -0.3f);

            //pinching = validOrientation && HandUtils.detectPinch(appController.TranslationActivationHand,
            //new List<HandFinger> { HandFinger.Thumb, HandFinger.Middle });

            //pinching = HandUtils.detectTopGesture(appController.TranslationActivationHand);

            bool transition = false;

            if (pinching && !wasPinchingLastFrame)
            {
                transition = true;
                controlsStatus.TranslationActive = !controlsStatus.TranslationActive;
            }

            if (transition && controlsStatus.TranslationActive)
            {
                handStartPosition = HandUtils.getHandRootPosition(appController.TranslationActivationHand);
                objectStartPosition = appController.OBJ.transform.position;
            }

            if (controlsStatus.TranslationActive)
            {
                updateObjectByHand();
            }

            wasPinchingLastFrame = pinching;
        }
        else if (appController.TranslationController.IsConnected)
        {
            bool buttonPressed = appController.TranslationController.ControllerInput.PrimaryButton;
            bool transition = false;
            if (buttonPressed && !buttonPressedLastFrame)
            {
                controllerStartPostion = getControllerPosition();
                objectStartPosition = appController.OBJ.transform.position;
                transition = true;
            }

            if (transition)
            {
                controlsStatus.TranslationActive = !controlsStatus.TranslationActive;
            }


            if (controlsStatus.TranslationActive)
            {
                updateObjectByController();
            }

            buttonPressedLastFrame = buttonPressed;
        }
    }

    private void updateObjectByHand()
    {
        Vector3 delta = HandUtils.getHandRootPosition(appController.TranslationActivationHand) - handStartPosition;
        appController.OBJ.transform.position = objectStartPosition + sensitivity * delta;
    }

    private void updateObjectByController()
    {
        Vector3 delta = getControllerPosition() - controllerStartPostion;
        appController.OBJ.transform.position = objectStartPosition + sensitivity * delta;
    }

    public void resetStartPosition()
    {
        if (appController.TranslationActivationHand.IsConnected)
        {
            handStartPosition = HandUtils.getHandRootPosition(appController.TranslationActivationHand);
            objectStartPosition = appController.OBJ.transform.position;
        }
        else if (appController.TranslationController.IsConnected)
        {
            controllerStartPostion = getControllerPosition();
            objectStartPosition = appController.OBJ.transform.position;
        }

    }


    private Vector3 getControllerPosition()
    {
        if (appController.TranslationController.TryGetPose(out Pose pose))
        {
            return pose.position;
        }

        return Vector3.zero;
    }

}
