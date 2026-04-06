using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;
using Oculus.Interaction;

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

    // Update is called once per frame
    void Update()
    {

        if (appController.RotationActivationHand.IsConnected)
        {
            pinching = detectPinch();
            bool transition = false;
            if (pinching && !wasPinchingLastFrame)
            {
                transition = true;
                controlsStatus.RotationActive = !controlsStatus.RotationActive;
            }

            if (transition && controlsStatus.RotationActive)
            {
                Debug.Log("Set start rotation");
                wristStartRotation = getWristRotation();
                rotationAxis = getFingerAxis();
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
        Quaternion currentWrist = getWristRotation();
        Quaternion delta = currentWrist * Quaternion.Inverse(wristStartRotation);

        Quaternion twist = getTwist(delta, rotationAxis);
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
        Quaternion twist = getTwist(delta, rotationAxis);

        float angle = 2f * Mathf.Acos(Mathf.Clamp(twist.w, -1f, 1f)) * Mathf.Rad2Deg;
        if (Vector3.Dot(new Vector3(twist.x, twist.y, twist.z), rotationAxis) < 0f)
            angle = -angle;

        appController.OBJ.transform.rotation = objStartRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }

    private Vector3 getWristNormal()
    {

        if (!appController.TranslationActivationHand.GetJointPose(HandJointId.HandWristRoot, out Pose wristPose) ||
            !appController.TranslationActivationHand.GetJointPose(HandJointId.HandIndex1, out Pose indexPose) ||
            !appController.TranslationActivationHand.GetJointPose(HandJointId.HandPinky0, out Pose pinkyPose))
        {
            return appController.TranslationActivationHand.transform.forward;
        }

        Vector3 wristPos = wristPose.position;
        Vector3 toIndex = indexPose.position - wristPos;
        Vector3 toPinky = pinkyPose.position - wristPos;

        Vector3 normal = Vector3.Cross(toIndex, toPinky).normalized;
        if (appController.TranslationActivationHand.Handedness == Handedness.Left)
        {
            normal = Vector3.Cross(toIndex, toPinky).normalized;
        }
        else
        {
            normal = Vector3.Cross(toPinky, toIndex).normalized;
        }

        if (normal == Vector3.zero)
        {
            return appController.TranslationActivationHand.transform.forward;
        }

        return normal;
    }

    private Quaternion getWristRotation()
    {
        if (appController.TranslationActivationHand.GetJointPose(HandJointId.HandWristRoot, out Pose wristPose))
        {
            return wristPose.rotation;
        }

        return appController.TranslationActivationHand.transform.rotation;
    }

    private Vector3 getFingerAxis()
    {
        if (appController.TranslationActivationHand.GetJointPose(HandJointId.HandWristRoot, out Pose wrist))
        {
            if (appController.TranslationActivationHand.GetJointPose(HandJointId.HandIndexTip, out Pose iTip))
            {
                Vector3 axis = (iTip.position - wrist.position).normalized;
                if (axis != Vector3.zero) return axis;
            }

            if (appController.TranslationActivationHand.GetJointPose(HandJointId.HandMiddleTip, out Pose mTip))
            {
                Vector3 axis = (mTip.position - wrist.position).normalized;
                if (axis != Vector3.zero) return axis;
            }

        }
        return appController.TranslationActivationHand.transform.forward;
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

    private bool detectPinch()
    {
        thumbPinching = appController.RotationActivationHand.GetFingerIsPinching(HandFinger.Thumb);
        middlePinching = appController.RotationActivationHand.GetFingerIsPinching(HandFinger.Middle);

        bool indexPinching = appController.RotationActivationHand.GetFingerIsPinching(HandFinger.Index);
        bool ringPinching = appController.RotationActivationHand.GetFingerIsPinching(HandFinger.Ring);
        bool pinkyPinching = appController.RotationActivationHand.GetFingerIsPinching(HandFinger.Pinky);

        return (thumbPinching && middlePinching) &&
            !(indexPinching || ringPinching || pinkyPinching);
    }
}
