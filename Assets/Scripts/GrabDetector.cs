using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class GrabDetector : MonoBehaviour
{
    private GrabInteractable interactable;
    private HandGrabInteractable handInteractable;

    [SerializeField]
    private ApplicationController appController;

    void Start()
    {
        appController = FindFirstObjectByType<ApplicationController>();
        interactable = GetComponentInChildren<GrabInteractable>();
        interactable.WhenStateChanged += onStateChanged;
        handInteractable = GetComponentInChildren<HandGrabInteractable>();
        handInteractable.WhenStateChanged += onStateChanged;

    }

    private void onStateChanged(InteractableStateChangeArgs args)
    {
        Debug.Log("InteracatableState: " + args.NewState);
        if (args.NewState == InteractableState.Select)
        {
            onGrab();
        }

        if (args.NewState == InteractableState.Normal)
        {
            onRelease();
        }
    }

    private void onGrab()
    {
        Debug.Log("Grabbed controlHande");
        appController.OBJ = gameObject;
    }

    private void onRelease()
    {
        Debug.Log("Released controlHande");
    }


}
