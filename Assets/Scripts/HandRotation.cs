using UnityEngine;

using Oculus.Interaction.Input;
using Meta.XR.ImmersiveDebugger;


public class HandRotation : MonoBehaviour
{

    [DebugMember]
    public GameObject obj;
    [DebugMember]
    public Hand hand;

    [DebugMember]
    public HandRotation otherHand;
    public HandTranslation leftHandTranslation;
    public HandTranslation rightHandTranslation;

    [DebugMember]
    public bool active = false;
    [DebugMember]
    public bool locked = false;
    [DebugMember]
    public bool pinching = false;
    [DebugMember]
    public bool _wasPinchingLastFrame = false;

    [DebugMember]
    public Transform newObj;
    [DebugMember]
    public Transform handTransform;
    [DebugMember]
    public Quaternion objStartRotation;
    [DebugMember]
    public Vector3 pinchStartDirection;
    [DebugMember]
    public Transform newHandTransform;

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
        if (locked)
        {
            _wasPinchingLastFrame = false;
            return;
        }

        pinching = detectHalfItalianPinch();
        if (pinching)
        {
            updateHand();
        }
        else
        {
            setActive(false);
        }
        _wasPinchingLastFrame = pinching;
    }

    void updateHand()
    {
        Vector3 pinchDirection = getWristNormal();
        if (!_wasPinchingLastFrame)
        {
            pinchStartDirection = pinchDirection;
            objStartRotation = obj.transform.rotation;
            setActive(true);
        }
        else
        {
            Quaternion rotation =
                Quaternion.FromToRotation(pinchStartDirection, pinchDirection);

            obj.transform.rotation = objStartRotation * rotation;
        }

    }

    private void setActive(bool value)
    {
        active = value;
        leftHandTranslation.locked = value;
        rightHandTranslation.locked = value;
        otherHand.locked = value;
    }

    private Vector3 getPinchDirection()
    {
        if (!hand.GetJointPose(HandJointId.HandWristRoot, out Pose wristPose) ||
            !hand.GetJointPose(HandJointId.HandIndex0, out Pose indexPose))
        {
            return hand.transform.forward;
        }

        Vector3 wristPos = wristPose.position;
        Vector3 pinchDirection = indexPose.position - wristPos;

        return pinchDirection;
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

    private bool detectHalfItalianPinch()
    {
        thumbPinching = hand.GetFingerIsPinching(HandFinger.Thumb);
        indexPinching = hand.GetFingerIsPinching(HandFinger.Index);
        middlePinching = hand.GetFingerIsPinching(HandFinger.Middle);

        return thumbPinching && indexPinching && middlePinching;
    }
}
