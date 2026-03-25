using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;

public class HandPinchTranslation : MonoBehaviour
{

    [DebugMember]
    public GameObject obj;
    [DebugMember]
    public Hand pinchHand;
    [DebugMember]
    public Hand controlHand;

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
        isHandConnected = pinchHand.IsConnected;
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
        if (controlHand.GetJointPose(HandJointId.HandWristRoot, out Pose pose))
        {
            return pose.position;
        }

        return controlHand.transform.position;
    }

    private bool detectPinch()
    {
        thumbPinching = pinchHand.GetFingerIsPinching(HandFinger.Thumb);
        middlePinching = pinchHand.GetFingerIsPinching(HandFinger.Middle);

        return thumbPinching && middlePinching;
    }
}
