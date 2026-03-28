using UnityEngine;
using Oculus.Interaction.Input;


public class ApplicationController : MonoBehaviour
{

    [SerializeField]
    private GameObject obj;
    public GameObject OBJ
    {
        get { return obj; }
        set => replaceObj(value);
    }
    [SerializeField]
    private Camera cam;
    public Camera Cam
    {
        get { return cam; }
    }

    [SerializeField]
    private Hand translationActivationHand;
    public Hand TranslationActivationHand
    {
        get { return translationActivationHand; }
    }

    [SerializeField]
    private Hand rotationActivationHand;
    public Hand RotationActivationHand
    {
        get { return rotationActivationHand; }
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void replaceObj(GameObject obj)
    {
        if (this.obj != null)
        {
            Destroy(this.obj);
        }

        this.obj = obj;
    }

    public void swapHandFunction()
    {
        Hand temp = translationActivationHand;
        translationActivationHand = rotationActivationHand;
        rotationActivationHand = temp;
    }
}
