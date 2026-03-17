using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;
public class HandPinchRotation : MonoBehaviour
{

    [DebugMember]
    public GameObject obj;
    [DebugMember]
    public Hand hand;
    [DebugMember]
    public ControlsStatus controlsStatus;

    [DebugMember]
    public bool pinching = false;
    [DebugMember]
    public bool wasPinchingLastFrame = false;

    [DebugMember]
    public Quaternion wristStartRotation;
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
            objStartRotation = obj.transform.rotation;
        }

        if (controlsStatus.RotationActive)
        {
            updateObject();
        }

        wasPinchingLastFrame = pinching;
    }

    private void updateObject()
    {
        Quaternion currentWrist = getWristRotation();
        Quaternion delta = currentWrist * Quaternion.Inverse(wristStartRotation);

        Quaternion twist = getTwist(delta, rotationAxis);
        float angle = 2f * Mathf.Acos(Mathf.Clamp(twist.w, -1f, 1f)) * Mathf.Rad2Deg;
        if (Vector3.Dot(new Vector3(twist.x, twist.y, twist.z), rotationAxis) < 0f)
        {
            angle = -angle;
        }

        obj.transform.rotation = objStartRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }

    private Vector3 getWristNormal()
    {

        if (!hand.GetJointPose(HandJointId.HandWristRoot, out Pose wristPose) ||
            !hand.GetJointPose(HandJointId.HandIndex1, out Pose indexPose) ||
            !hand.GetJointPose(HandJointId.HandPinky0, out Pose pinkyPose))
        {
            return hand.transform.forward;
        }

        Vector3 wristPos = wristPose.position;
        Vector3 toIndex = indexPose.position - wristPos;
        Vector3 toPinky = pinkyPose.position - wristPos;

        Vector3 normal = Vector3.Cross(toIndex, toPinky).normalized;
        if (hand.Handedness == Handedness.Left)
        {
            normal = Vector3.Cross(toIndex, toPinky).normalized;
        }
        else
        {
            normal = Vector3.Cross(toPinky, toIndex).normalized;
        }

        if (normal == Vector3.zero)
        {
            return hand.transform.forward;
        }

        return normal;
    }

    private Quaternion getWristRotation()
    {
        if (hand.GetJointPose(HandJointId.HandWristRoot, out Pose wristPose))
        {
            return wristPose.rotation;
        }

        return hand.transform.rotation;
    }

    private Vector3 getFingerAxis()
    {
        if (hand.GetJointPose(HandJointId.HandIndex1, out Pose i1) &&
            hand.GetJointPose(HandJointId.HandIndexTip, out Pose iTip))
        {
            Vector3 axis = (iTip.position - i1.position).normalized;
            if (axis != Vector3.zero) return axis;
        }

        if (hand.GetJointPose(HandJointId.HandMiddle1, out Pose m1) &&
            hand.GetJointPose(HandJointId.HandMiddleTip, out Pose mTip))
        {
            Vector3 axis = (mTip.position - m1.position).normalized;
            if (axis != Vector3.zero) return axis;
        }

        return hand.transform.forward;
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
        thumbPinching = hand.GetFingerIsPinching(HandFinger.Thumb);
        middlePinching = hand.GetFingerIsPinching(HandFinger.Middle);

        return thumbPinching && middlePinching;
    }
}
