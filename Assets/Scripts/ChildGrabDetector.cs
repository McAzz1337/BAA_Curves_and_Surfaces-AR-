using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class ChildGrabDetector : MonoBehaviour
{

    //private GrabInteractable interactable;

    private HandGrabInteractable handInteractable;

    [SerializeField]
    private GrabDetector parentDetector;

    public GrabDetector ParentDetector
    {
        get { return parentDetector; }
        set { parentDetector = value; }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //interactable = GetComponentInChildren<GrabInteractable>();
        //interactable.WhenStateChanged += onStateChanged;
        handInteractable = GetComponent<HandGrabInteractable>();
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
        Debug.Log("Child control point grabbed");
        parentDetector.onGrab();
    }

    private void onRelease()
    {
        Debug.Log("Child control point released");
        parentDetector.onRelease();
    }

}
