using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;
using Oculus.Interaction;
using System.Collections.Generic;

public class HandRotation : MonoBehaviour
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
        pinching = true;
    }

    public void onUnposed()
    {
        pinching = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (appController.RotationHand.IsConnected)
        {

            bool transition = false;
            if (pinching && !wasPinchingLastFrame)
            {
                transition = true;
                controlsStatus.RotationActive = !controlsStatus.RotationActive;
            }

            if (transition && controlsStatus.RotationActive)
            {
                wristStartRotation = HandUtils.getWristRotation(appController.RotationHand);
                rotationAxis = HandUtils.getFingerAxis(appController.RotationHand, HandJointId.HandIndexTip);
                objStartRotation = appController.OBJ.transform.rotation;
            }

            if (controlsStatus.RotationActive)
            {
                updateObjectByHand();
            }

            wasPinchingLastFrame = pinching;
        }
    }

    private void updateObjectByHand()
    {
        Quaternion currentWrist = HandUtils.getWristRotation(appController.RotationHand);
        Quaternion delta = currentWrist * Quaternion.Inverse(wristStartRotation);

        Quaternion twist = HandUtils.getTwist(delta, rotationAxis);
        float angle = 2f * Mathf.Acos(Mathf.Clamp(twist.w, -1f, 1f)) * Mathf.Rad2Deg;
        if (Vector3.Dot(new Vector3(twist.x, twist.y, twist.z), rotationAxis) < 0f)
        {
            angle = -angle;
        }

        appController.OBJ.transform.rotation = objStartRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }
}
