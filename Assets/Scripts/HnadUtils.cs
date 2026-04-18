
using UnityEngine;
using Oculus.Interaction.Input;
using System.Collections.Generic;
using System.Linq;

public static class HandUtils
{

    private static readonly List<HandFinger> fingerIds = new List<HandFinger>{
        HandFinger.Thumb,
        HandFinger.Index,
        HandFinger.Middle,
        HandFinger.Ring,
        HandFinger.Pinky
    };

    public static bool detectPinch(Hand hand, List<HandFinger> fingersToCheck)
    {
        bool notPinching = fingerIds
            .Where(f => !fingersToCheck.Contains(f))
            .All(f => !hand.GetFingerIsPinching(f));

        bool allPinching = fingersToCheck.All(f => hand.GetFingerIsPinching(f));

        return notPinching && allPinching;
    }

    public static Quaternion getWristRotation(Hand hand)
    {
        if (hand.GetJointPose(HandJointId.HandWristRoot, out Pose wristPose))
        {
            return wristPose.rotation;
        }

        return hand.transform.rotation;
    }

    public static Vector3 getFingerAxis(Hand hand, HandJointId id0, HandJointId id1)
    {
        if (hand.GetJointPose(id0, out Pose joint0))
        {
            if (hand.GetJointPose(id1, out Pose joint1))
            {
                Vector3 axis = (joint1.position - joint0.position).normalized;
                if (axis != Vector3.zero) return axis;
            }
        }

        return hand.transform.forward;
    }

    public static Quaternion getTwist(Quaternion q, Vector3 axis)
    {
        Vector3 vec = new Vector3(q.x, q.y, q.z);
        Vector3 proj = Vector3.Project(vec, axis.normalized);
        Quaternion twist = new Quaternion(proj.x, proj.y, proj.z, q.w);
        return twist.normalized;
    }

    public static Vector3 getWristNormal(Hand hand)
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

        Vector3 normal = hand.Handedness == Handedness.Left ?
            Vector3.Cross(toIndex, toPinky).normalized :
            Vector3.Cross(toPinky, toIndex).normalized;

        if (normal == Vector3.zero)
        {
            return hand.transform.forward;
        }

        return normal;
    }

    public static Vector3 getHandRootPosition(Hand hand)
    {
        if (hand.GetJointPose(HandJointId.HandWristRoot, out Pose pose))
        {
            return pose.position;
        }

        return hand.transform.position;
    }

    public static bool writstNormalDotCamForwardGreaterTan(Hand hand, Camera cam, float threshold)
    {
        Vector3 wristNormal = getWristNormal(hand);
        Vector3 toCam = cam.transform.position - getHandRootPosition(hand);

        float dot = Vector3.Dot(toCam.normalized, wristNormal.normalized);

        return dot > threshold;
    }

    public static bool detectTopGesture(Hand hand)
    {
        if (hand.GetJointPose(HandJointId.HandIndex0, out Pose indexKnuckle) &&
            hand.GetJointPose(HandJointId.HandIndexTip, out Pose indexTip) &&
            hand.GetJointPose(HandJointId.HandMiddle0, out Pose middleKnuckle) &&
            hand.GetJointPose(HandJointId.HandMiddleTip, out Pose middleTip) &&
            hand.GetJointPose(HandJointId.HandRing0, out Pose ringKnuckle) &&
            hand.GetJointPose(HandJointId.HandRingTip, out Pose ringTip) &&
            hand.GetJointPose(HandJointId.HandPinky0, out Pose pinkyKnuckle) &&
            hand.GetJointPose(HandJointId.HandPinkyTip, out Pose pinkyTip) &&
            hand.GetJointPose(HandJointId.HandWristRoot, out Pose wrist)
            )
        {
            Vector3 indexDir = direction(indexTip, indexKnuckle);
            Vector3 toIndex = direction(wrist, indexKnuckle);
            Vector3 middleDir = direction(middleTip, middleKnuckle);
            Vector3 toMiddle = direction(wrist, middleKnuckle);
            Vector3 ringDir = direction(ringTip, ringKnuckle);
            Vector3 toRing = direction(wrist, ringKnuckle);
            Vector3 pinkyDir = direction(pinkyTip, pinkyKnuckle);
            Vector3 toPinky = direction(wrist, pinkyKnuckle);

            List<float> dotProducts = new List<float>
            {
                Vector3.Dot(indexDir, toIndex),
                Vector3.Dot(middleDir, toMiddle),
                Vector3.Dot(ringDir, toRing),
                Vector3.Dot(pinkyDir, toPinky)
            };
            return dotProducts.All(p => p >= -0.2 && p <= 0.2);
        }
        else
        {
            return false;
        }
    }

    private static Vector3 direction(Pose a, Pose b)
    {
        return (a.position - b.position).normalized;
    }

}