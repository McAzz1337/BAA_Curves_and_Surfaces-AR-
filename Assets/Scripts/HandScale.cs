using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Oculus.Interaction.HandGrab;

public class HandScale : MonoBehaviour
{

    [SerializeField]
    private ApplicationController appController;

    [DebugMember]
    public Hand leftHand;
    [DebugMember]
    public Hand rightHand;
    [DebugMember]
    public ControlsStatus controlsStatus;

    [DebugMember]
    public float startDistance;

    private bool posed = false;

    private bool posedLastFrame = false;

    private List<Vector3> initialPositions;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void onPose()
    {
        posed = true;
    }

    public void onUnpose()
    {
        posed = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (appController.RotationHand.IsConnected && appController.TranslationHand.IsConnected)
        {

            bool transition = false;

            if (posed && !posedLastFrame)
            {
                transition = true;
                controlsStatus.ScalingActive = !controlsStatus.ScalingActive;
            }


            if (transition)
            {
                if (controlsStatus.ScalingActive)
                {

                    startDistance = Vector3.Distance(appController.RotationHand.transform.position,
                         appController.TranslationHand.transform.position);
                    getInitalPositions();
                    toggleControlPointsEnabled(false);
                }
                else
                {
                    toggleControlPointsEnabled(true);
                }
            }

            if (controlsStatus.ScalingActive)
            {
                updateObject();
            }

            posedLastFrame = posed;
        }
    }

    private void getInitalPositions()
    {
        ControlPoints controlPoints = appController.OBJ.GetComponentInChildren<ControlPoints>();
        Transform[] transforms = controlPoints.getTransforms();
        initialPositions = transforms.Select(t => t.localPosition).ToList();
    }

    private void updateObject()
    {
        float currentDistance = Vector3.Distance(leftHand.transform.position, rightHand.transform.position);
        float scale = Math.Max(0.1f, currentDistance / startDistance);

        ControlPoints controlPoints = appController.OBJ.GetComponentInChildren<ControlPoints>();
        Transform[] transforms = controlPoints.getTransforms();
        Rigidbody[] rigidbodies = transforms
            .Select(t => t.gameObject.GetComponent<Rigidbody>())
            .ToArray();

        for (int i = 0; i < initialPositions.Count; i++)
        {
            Vector3 position = scale * initialPositions[i];
            rigidbodies[i].MovePosition(position);
        }
    }

    private void toggleControlPointsEnabled(bool enabled)
    {
        ControlPoints controlPoints = appController.OBJ.GetComponentInChildren<ControlPoints>();
        HandGrabInteractable[] interactables = controlPoints.getTransforms()
            .Select(t => t.gameObject.GetComponentInChildren<HandGrabInteractable>()).ToArray();

        foreach (HandGrabInteractable i in interactables)
        {
            i.enabled = enabled;
        }
    }

}
