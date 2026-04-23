using UnityEngine;
using Oculus.Interaction.Input;
using System.Collections;


public class ApplicationController : MonoBehaviour
{


    private GameObject obj;
    public GameObject OBJ
    {
        get { return obj; }
        set => selectObj(value);
    }
    [SerializeField]
    private Camera cam;
    public Camera Cam
    {
        get { return cam; }
    }

    [SerializeField]
    private Hand translationHand;
    public Hand TranslationHand
    {
        get { return translationHand; }
    }


    [SerializeField]
    private Hand rotationHand;

    public Hand RotationHand
    {
        get { return rotationHand; }
    }

    [SerializeField]
    private ColorProvider colorProvider;

    [SerializeField]
    private ControlsStatus controlsStatus;

    public ControlsStatus ControlsStatus
    {
        get { return controlsStatus; }
    }



    void Start()
    {
        StartCoroutine(generateAtStartup(1f));
    }

    IEnumerator generateAtStartup(float delay)
    {
        yield return new WaitForSeconds(delay);

        Generator generator = GetComponent<Generator>();
        selectObj(generator.generate(EType.BEZIER_CURVE, 5, cam));
    }

    private void selectObj(GameObject obj)
    {
        if (this.obj != null)
        {
            Renderer renderer = this.obj.GetComponent<Renderer>();
            renderer.material.color = colorProvider.orange.color;
            controlsStatus.resetColor(this.obj);
            controlsStatus.TranslationActive = false;
        }

        Renderer r = obj.GetComponent<Renderer>();
        r.material.color = colorProvider.white.color;

        this.obj = obj;
    }

    public void deleteObject()
    {
        if (obj != null)
        {
            controlsStatus.deactivateAllCointrols();
            Destroy(obj);
        }
    }

    public void swapHandFunction()
    {
        Hand temp = translationHand;
        translationHand = rotationHand;
        rotationHand = temp;
    }
}
