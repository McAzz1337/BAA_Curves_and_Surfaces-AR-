using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;
using Oculus.Interaction;
using System.Collections.Generic;

public class HandPinchRotation : MonoBehaviour
{

    [SerializeField]
    private ApplicationController appController;

    [DebugMember]
    public ControlsStatus controlsStatus;

    [DebugMember]
    public bool pinching = false;
    [DebugMember]
    public bool wasPinchingLastFrame = false;

    [DebugMember]
    public bool buttonPressedLastFrame = false;

    [DebugMember]
    public Quaternion wristStartRotation;
    [DebugMember]
    public Quaternion controllerStartRotation;
    [DebugMember]
    public Quaternion objStartRotation;
    [DebugMember]
    public Vector3 rotationAxis;

    [DebugMember]
    public bool isHandConnected;

    [DebugMember]
    public bool thumbPinching;
    [DebugMember]
    public bool indexPinching;
    [DebugMember]
    public bool middlePinching;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void onPose()
    {
        Debug.Log("Scissor pose detected left");
        pinching = true;
    }

    public void onUnposed()
    {
        Debug.Log("Scissor unpose detected left");
        pinching = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (appController.RotationActivationHand.IsConnected)
        {
            bool validOrientation =
            HandUtils.writstNormalDotCamForwardGreaterTan(appController.RotationActivationHand,
                 appController.Cam,
                  -0.3f);

            //pinching = validOrientation && HandUtils.detectPinch(appController.RotationActivationHand,
            //new List<HandFinger> { HandFinger.Thumb, HandFinger.Middle });


            bool transition = false;
            if (pinching && !wasPinchingLastFrame)
            {
                transition = true;
                controlsStatus.RotationActive = !controlsStatus.RotationActive;
            }

            if (transition && controlsStatus.RotationActive)
            {
                wristStartRotation = HandUtils.getWristRotation(appController.RotationActivationHand);
                rotationAxis = HandUtils.getFingerAxis(appController.RotationActivationHand, HandJointId.HandIndexTip);
                objStartRotation = appController.OBJ.transform.rotation;
            }

            if (controlsStatus.RotationActive)
            {
                updateObjectByHand();
            }

            wasPinchingLastFrame = pinching;
        }
        else if (appController.RotationController.IsConnected)
        {

            bool buttonPressed = appController.RotationController.ControllerInput.PrimaryButton;
            bool transition = false;
            if (buttonPressed && !buttonPressedLastFrame)
            {
                controllerStartRotation = getControllerRotation();
                objStartRotation = appController.OBJ.transform.rotation;
                rotationAxis = getControllerRotationAxis();
                transition = true;
            }

            if (transition)
            {
                controlsStatus.RotationActive = !controlsStatus.RotationActive;
            }


            if (controlsStatus.RotationActive)
            {
                updateObjectByController();
            }

            buttonPressedLastFrame = buttonPressed;
        }
    }

    private void updateObjectByHand()
    {
        Quaternion currentWrist = HandUtils.getWristRotation(appController.RotationActivationHand);
        Quaternion delta = currentWrist * Quaternion.Inverse(wristStartRotation);

        Quaternion twist = HandUtils.getTwist(delta, rotationAxis);
        float angle = 2f * Mathf.Acos(Mathf.Clamp(twist.w, -1f, 1f)) * Mathf.Rad2Deg;
        if (Vector3.Dot(new Vector3(twist.x, twist.y, twist.z), rotationAxis) < 0f)
        {
            angle = -angle;
        }

        appController.OBJ.transform.rotation = objStartRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }

    private void updateObjectByController()
    {
        Quaternion currentRot = getControllerRotation();
        Quaternion delta = currentRot * Quaternion.Inverse(controllerStartRotation);
        Quaternion twist = HandUtils.getTwist(delta, rotationAxis);

        float angle = 2f * Mathf.Acos(Mathf.Clamp(twist.w, -1f, 1f)) * Mathf.Rad2Deg;
        if (Vector3.Dot(new Vector3(twist.x, twist.y, twist.z), rotationAxis) < 0f)
            angle = -angle;

        appController.OBJ.transform.rotation = objStartRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }

    private Quaternion getControllerRotation()
    {
        if (appController.RotationController.TryGetPose(out Pose pose))
        {
            return pose.rotation;
        }

        return Quaternion.identity;
    }

    private Vector3 getControllerRotationAxis()
    {
        Quaternion rotation = getControllerRotation();
        return rotation * Vector3.up;
    }

    private Quaternion getTwist(Quaternion q, Vector3 axis)
    {
        Vector3 vec = new Vector3(q.x, q.y, q.z);
        Vector3 proj = Vector3.Project(vec, axis.normalized);
        Quaternion twist = new Quaternion(proj.x, proj.y, proj.z, q.w);
        return twist.normalized;
    }

}
