using System;
using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Meta.XR.ImmersiveDebugger;

public class HandTranslation : MonoBehaviour
{
    [DebugMember]
    public GameObject obj;
    [DebugMember]
    public Hand hand;
    [SerializeField]
    public HandTranslation otherHand;

    [DebugMember]
    public bool locked = false;

    [DebugMember]
    public bool active = false;

    [DebugMember]
    public Transform newObj;
    [DebugMember]
    public Transform handTransform;
    [DebugMember]
    public Transform newHandTransform;

    [DebugMember]
    public bool isHandConnected;

    [DebugMember]
    public bool isPalmOpen;

    [DebugMember]
    public bool thumbPinching;
    [DebugMember]
    public bool indexPinching;
    [DebugMember]
    public bool middlePinching;
    [DebugMember]
    public bool ringPinching;
    [DebugMember]
    public bool pinkyPinching;

    [DebugMember]
    public Vector3 palmNormal;
    [DebugMember]
    public Camera cam;
    [DebugMember]
    public float palmDotToCamera;
    [DebugMember]
    public bool isPalmFacingCamera;

    [DebugMember]
    public Vector3 handOpenStartPosition;
    [DebugMember]
    public Vector3 objectStartPosition;
    [DebugMember]
    private bool _wasPalmOpenLastFrame = false;
    [DebugMember]
    private bool _wasFacingCameraLastFrame = false;

    void Update()
    {
        if (locked)
        {
            _wasPalmOpenLastFrame = false;
            return;
        }

        isPalmOpen = detectOpenPalm();
        if (isPalmOpen)
        {
            updateHand();
        }
        _wasPalmOpenLastFrame = isPalmOpen;
    }


    // Update is called once per frame
    void updateHand()
    {
        isHandConnected = hand.IsConnected;

        palmNormal = GetPalmNormal();
        Vector3 toCamera = cam.transform.position - GetHandRootPosition();
        palmDotToCamera = Vector3.Dot(palmNormal, toCamera.normalized);
        isPalmFacingCamera = palmDotToCamera > 0.8f;

        if (isPalmFacingCamera)
        {
            if (!_wasPalmOpenLastFrame || !_wasFacingCameraLastFrame)
            {
                handOpenStartPosition = GetHandRootPosition();
                objectStartPosition = obj.transform.position;
                setActive(true);
            }

            if (active)
            {
                Vector3 currentHandPosition = GetHandRootPosition();
                Vector3 handDelta = currentHandPosition - handOpenStartPosition;
                obj.transform.position = objectStartPosition + handDelta;
            }
        }
        else
        {
            setActive(false);
        }

        _wasFacingCameraLastFrame = isPalmFacingCamera;
    }

    private void setActive(bool value)
    {
        active = value;
        otherHand.locked = value;
    }

    private Vector3 GetHandRootPosition()
    {
        if (hand.GetJointPose(HandJointId.HandWristRoot, out Pose pose))
        {
            return pose.position;
        }

        return hand.transform.position;
    }

    private Vector3 GetPalmNormal()
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

    private bool detectOpenPalm()
    {

        thumbPinching = hand.GetFingerIsPinching(HandFinger.Thumb);
        indexPinching = hand.GetFingerIsPinching(HandFinger.Index);
        middlePinching = hand.GetFingerIsPinching(HandFinger.Middle);
        ringPinching = hand.GetFingerIsPinching(HandFinger.Ring);
        pinkyPinching = hand.GetFingerIsPinching(HandFinger.Pinky);

        return !thumbPinching
            && !indexPinching
            && !middlePinching
            && !ringPinching
            && !pinkyPinching;
    }
}
