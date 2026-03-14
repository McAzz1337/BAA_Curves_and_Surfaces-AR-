using System;
using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Meta.XR.ImmersiveDebugger;

public class HandTranlation : MonoBehaviour
{
    [DebugMember]
    public GameObject obj;
    [DebugMember]

    public Hand hand;
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

    private bool _wasPalmOpenLastFrame = false;


    // Update is called once per frame
    void Update()
    {
        isHandConnected = hand.IsConnected;

        thumbPinching = hand.GetFingerIsPinching(HandFinger.Thumb);
        indexPinching = hand.GetFingerIsPinching(HandFinger.Index);
        middlePinching = hand.GetFingerIsPinching(HandFinger.Middle);
        ringPinching = hand.GetFingerIsPinching(HandFinger.Ring);
        pinkyPinching = hand.GetFingerIsPinching(HandFinger.Pinky);

        isPalmOpen = detectOpenPalm();

        // Compute palm facing direction relative to the camera.
        palmNormal = GetPalmNormal();
        Vector3 cameraForward = cam.transform.forward;
        palmDotToCamera = Vector3.Dot(palmNormal, -cameraForward);
        isPalmFacingCamera = palmDotToCamera > 0.8f;

        // These logs are only for verifying the state in the Unity console.
        Debug.Log("Hand connected: " + isHandConnected);
        Debug.Log("Palm open: " + isPalmOpen + " (facing camera: " + isPalmFacingCamera + ")");

        if (isPalmOpen && isPalmFacingCamera)
        {
            // When the palm opens, start tracking the initial positions.
            if (!_wasPalmOpenLastFrame)
            {
                handOpenStartPosition = GetHandRootPosition();
                objectStartPosition = obj.transform.position;
                active = true;
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
            // Stop translating when the palm closes or is turned away.
            active = false;
        }

        _wasPalmOpenLastFrame = isPalmOpen;
    }

    private Vector3 GetHandRootPosition()
    {
        if (hand.GetJointPose(HandJointId.HandWristRoot, out Pose pose))
        {
            return pose.position;
        }

        // Fallback to the component transform if joint tracking is unavailable.
        return hand.transform.position;
    }

    private Vector3 GetPalmNormal()
    {
        // Use 3 key hand joints to define the palm plane: wrist root, index base, pinky base.
        // The normal is the cross product between the vectors from wrist->index and wrist->pinky.
        // This should point roughly out of the palm (or the back of the hand); we will use the dot
        // product with the camera forward vector to determine which side is facing the viewer.

        if (!hand.GetJointPose(HandJointId.HandWristRoot, out Pose wristPose) ||
            !hand.GetJointPose(HandJointId.HandIndex1, out Pose indexPose) ||
            !hand.GetJointPose(HandJointId.HandPinky0, out Pose pinkyPose))
        {
            // Fallback: if joint data is missing, use the wrist rotation forward vector.
            return hand.transform.forward;
        }

        Vector3 wristPos = wristPose.position;
        Vector3 toIndex = indexPose.position - wristPos;
        Vector3 toPinky = pinkyPose.position - wristPos;

        Vector3 normal = Vector3.Cross(toIndex, toPinky).normalized;
        if (normal == Vector3.zero)
        {
            return hand.transform.forward;
        }

        return normal;
    }

    private bool detectOpenPalm()
    {
        return hand.IsConnected &&
            !hand.GetFingerIsPinching(HandFinger.Thumb) &&
            !hand.GetFingerIsPinching(HandFinger.Index) &&
            !hand.GetFingerIsPinching(HandFinger.Middle) &&
            !hand.GetFingerIsPinching(HandFinger.Ring) &&
            !hand.GetFingerIsPinching(HandFinger.Pinky);
    }
}
