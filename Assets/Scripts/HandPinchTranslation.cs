using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;

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

    private float multiplier = 1.0f;

    public float Multiplier
    {
        get => multiplier;
        set
        {
            multiplier = value;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (appController.TranslationActivationHand.IsConnected)
        {

            pinching = detectPinch();
            bool transition = false;

            if (pinching)
            {
                if (!wasPinchingLastFrame)
                {
                    transition = true;
                    controlsStatus.TranslationActive = !controlsStatus.TranslationActive;
                    Debug.Log("Transition");
                }
            }

            if (transition && controlsStatus.TranslationActive)
            {
                Debug.Log("Set start position");
                handStartPosition = getHandRootPosition();
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
        Vector3 delta = getHandRootPosition() - handStartPosition;
        appController.OBJ.transform.position = objectStartPosition + multiplier * delta;
    }

    private void updateObjectByController()
    {
        Vector3 delta = getControllerPosition() - controllerStartPostion;
        appController.OBJ.transform.position = objectStartPosition + multiplier * delta;
    }

    public void resetStartPosition()
    {
        if (appController.TranslationActivationHand.IsConnected)
        {
            handStartPosition = getHandRootPosition();
            objectStartPosition = appController.OBJ.transform.position;
        }
        else if (appController.TranslationController.IsConnected)
        {
            controllerStartPostion = getControllerPosition();
            objectStartPosition = appController.OBJ.transform.position;
        }

    }

    private Vector3 getHandRootPosition()
    {
        if (appController.RotationActivationHand.GetJointPose(HandJointId.HandWristRoot, out Pose pose))
        {
            return pose.position;
        }

        return appController.RotationActivationHand.transform.position;
    }

    private Vector3 getControllerPosition()
    {
        if (appController.TranslationController.TryGetPose(out Pose pose))
        {
            return pose.position;
        }

        return Vector3.zero;
    }

    private bool detectPinch()
    {
        thumbPinching = appController.TranslationActivationHand.GetFingerIsPinching(HandFinger.Thumb);
        middlePinching = appController.TranslationActivationHand.GetFingerIsPinching(HandFinger.Middle);

        bool indexPinching = appController.TranslationActivationHand.GetFingerIsPinching(HandFinger.Index);
        bool ringPinching = appController.TranslationActivationHand.GetFingerIsPinching(HandFinger.Ring);
        bool pinkyPinching = appController.TranslationActivationHand.GetFingerIsPinching(HandFinger.Pinky);

        return (thumbPinching && middlePinching) &&
            !(indexPinching || ringPinching || pinkyPinching);
    }
}
