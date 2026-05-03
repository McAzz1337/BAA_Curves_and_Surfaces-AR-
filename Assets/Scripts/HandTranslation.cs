using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using Oculus.Interaction.Input;
using System.Collections.Generic;

public class HandTranslation : MonoBehaviour
{
    [SerializeField]
    private ApplicationController appController;

    [DebugMember]
    public ControlsStatus controlsStatus;

    [DebugMember]
    public bool posed = false;
    [DebugMember]
    public bool posedLastFrame = false;


    [DebugMember]
    public Vector3 handStartPosition;

    [DebugMember]
    public Vector3 objectStartPosition;


    private float sensitivity = 1.0f;

    public float Sensitivity
    {
        get => sensitivity;
        set
        {
            sensitivity = value;
        }
    }

    void Start()
    {

    }

    public void onPose()
    {
        posed = true;
    }

    public void onUnposed()
    {
        posed = false;
    }

    void Update()
    {
        if (appController.TranslationHand.IsConnected && appController.OBJ != null)
        {

            bool transition = false;

            if (!appController.IsGrabbed && posed && !posedLastFrame)
            {
                transition = true;
                controlsStatus.TranslationActive = !controlsStatus.TranslationActive;
            }

            if (transition && controlsStatus.TranslationActive)
            {
                handStartPosition = HandUtils.getHandRootPosition(appController.TranslationHand);
                objectStartPosition = appController.OBJ.transform.position;
            }

            if (controlsStatus.TranslationActive)
            {
                updateObjectByHand();
            }

            posedLastFrame = posed;
        }
    }

    private void updateObjectByHand()
    {
        Vector3 delta = HandUtils.getHandRootPosition(appController.TranslationHand) - handStartPosition;
        appController.OBJ.transform.position = objectStartPosition + sensitivity * delta;
    }


    public void resetStartPosition()
    {
        if (appController.TranslationHand.IsConnected)
        {
            handStartPosition = HandUtils.getHandRootPosition(appController.TranslationHand);
            objectStartPosition = appController.OBJ.transform.position;
        }
    }
}
