using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;
using UnityEngine.UIElements;
using System;

public class HandPinchScaleing : MonoBehaviour
{

    [DebugMember]
    public GameObject obj;
    [DebugMember]
    public Hand leftHand;
    [DebugMember]
    public Hand rightHand;
    [DebugMember]
    public ControlsStatus controlsStatus;

    [DebugMember]
    public float startDistance;
    [DebugMember]
    public Scale startScale;

    [DebugMember]
    public bool leftHandPinching;
    [DebugMember]
    public bool rightHandPinching;

    [DebugMember]
    public bool leftWasPinchingLastFrame;
    [DebugMember]
    public bool rightWasPinchingLastFrame;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!wristsPointingAtEachOther())
        {
            return;
        }

        bool transition = false;

        bool leftPinch = detectPinch(leftHand);
        if (leftPinch && !leftWasPinchingLastFrame)
        {
            transition = true;
            controlsStatus.ScalingActive = !controlsStatus.ScalingActive;
        }

        bool rightPinch = detectPinch(rightHand);
        if (rightPinch && !rightWasPinchingLastFrame)
        {
            transition = true;
            controlsStatus.ScalingActive = !controlsStatus.ScalingActive;
        }

        if (transition && controlsStatus.ScalingActive)
        {
            Debug.Log("Start scaling");
            startDistance = Vector3.Distance(leftHand.transform.position, rightHand.transform.position);
            startScale = obj.transform.localScale;
        }

        if (controlsStatus.ScalingActive)
        {
            updateObject();
        }

        leftWasPinchingLastFrame = leftPinch;
        rightWasPinchingLastFrame = rightPinch;
    }

    private void updateObject()
    {
        float currentDistance = Vector3.Distance(leftHand.transform.position, rightHand.transform.position);
        float scaleFactor = Math.Max(0.01f, currentDistance / startDistance);
        Vector3 newScale = startScale.value * scaleFactor;


        obj.transform.localScale.Set(newScale.x, newScale.y, newScale.z);
    }

    private Vector3 getWristNormal(Hand hand)
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

    private bool wristsPointingAtEachOther()
    {
        Vector3 leftWritNormal = getWristNormal(leftHand);
        Vector3 rightWritNormal = getWristNormal(rightHand);
        float dot = Vector3.Dot(leftWritNormal, rightWritNormal);
        return dot < -0.8f;
    }

    private bool detectPinch(Hand hand)
    {
        bool thumbPinching = hand.GetFingerIsPinching(HandFinger.Thumb);
        bool middlePinching = hand.GetFingerIsPinching(HandFinger.Middle);

        return thumbPinching && middlePinching;
    }
}
