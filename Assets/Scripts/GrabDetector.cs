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

        Renderer r = gameObject.GetComponent<Renderer>();
        ColorProvider colorProvider = FindFirstObjectByType<ColorProvider>();
        r.material.color = colorProvider.orange.color;
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

    public void onGrab()
    {
        Debug.Log("Grabbed controlHande");
        appController.OBJ = gameObject;
        appController.IsGrabbed = true;
    }

    public void onRelease()
    {
        Debug.Log("Released controlHande");
        appController.IsGrabbed = false;
    }


}
