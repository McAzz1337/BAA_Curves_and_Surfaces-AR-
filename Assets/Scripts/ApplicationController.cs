using UnityEngine;
using Oculus.Interaction.Input;
using System.Collections;


public class ApplicationController : MonoBehaviour
{


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
    private Controller translationController;
    public Controller TranslationController
    {
        get { return translationController; }
    }

    [SerializeField]
    private Hand rotationActivationHand;

    public Hand RotationActivationHand
    {
        get { return rotationActivationHand; }
    }

    [SerializeField]
    private Controller rotationController;
    public Controller RotationController
    {
        get { return rotationController; }
    }


    void Start()
    {
        StartCoroutine(generateAtStartup(1f));
    }

    IEnumerator generateAtStartup(float delay)
    {
        yield return new WaitForSeconds(delay);

        Generator generator = GetComponent<Generator>();
        OBJ = generator.generate(EType.BEZIER_CURVE, 5, cam);
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
