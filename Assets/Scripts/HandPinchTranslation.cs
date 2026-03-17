using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;

public class HandPinchTranslation : MonoBehaviour
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
    public Vector3 handStartPosition;
    [DebugMember]
    public Vector3 objectStartPosition;

    [DebugMember]
    public bool isHandConnected;

    [DebugMember]
    public bool thumbPinching;
    [DebugMember]
    public bool middlePinching;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isHandConnected = hand.IsConnected;
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
            objectStartPosition = obj.transform.position;
        }

        if (controlsStatus.TranslationActive)
        {
            updateObject();
        }

        wasPinchingLastFrame = pinching;
    }

    private void updateObject()
    {
        Vector3 delta = getHandRootPosition() - handStartPosition;
        obj.transform.position = objectStartPosition + delta;
    }

    private Vector3 getHandRootPosition()
    {
        if (hand.GetJointPose(HandJointId.HandWristRoot, out Pose pose))
        {
            return pose.position;
        }

        return hand.transform.position;
    }

    private bool detectPinch()
    {
        thumbPinching = hand.GetFingerIsPinching(HandFinger.Thumb);
        middlePinching = hand.GetFingerIsPinching(HandFinger.Middle);

        return thumbPinching && middlePinching;
    }
}
