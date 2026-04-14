using UnityEngine;
using Meta.XR.ImmersiveDebugger;
using UnityEngine.UI;

public class ControlsStatus : MonoBehaviour
{
    [SerializeField]
    private ApplicationController appController;

    [SerializeField]
    public ColorProvider colorProvider;

    [SerializeField]
    private TranslationSpeedCanvas translationSpeedCanvas;

    [SerializeField]
    private Slider translationSpeedSlider;
    [SerializeField]
    private HandTranslation handTranslation;

    private bool translationActive;
    private bool rotationActive;
    private bool scalingActive;

    [DebugMember]
    public bool TranslationActive
    {
        get => translationActive;
        set
        {
            translationActive = value;
            translationSpeedCanvas.toggleCanvas(translationActive);
            handTranslation.Sensitivity = translationSpeedSlider.value;
            updateColor();
        }
    }

    [DebugMember]
    public bool RotationActive
    {
        get => rotationActive;
        set
        {
            rotationActive = value;
            updateColor();
        }
    }

    [DebugMember]
    public bool ScalingActive
    {
        get => scalingActive;
        set
        {
            scalingActive = value;
            updateColor();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        translationSpeedSlider.onValueChanged.AddListener(changeTranslationSpeed);
    }

    private void changeTranslationSpeed(float value)
    {
        handTranslation.Sensitivity = value;
        handTranslation.resetStartPosition();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void updateColor()
    {
        Color targetColor = Color.white;

        if (translationActive && rotationActive)
        {
            targetColor = colorProvider.purple.color;
        }
        else if (translationActive)
        {
            targetColor = colorProvider.red.color;
        }
        else if (rotationActive)
        {
            targetColor = colorProvider.green.color;
        }
        else if (scalingActive)
        {
            targetColor = colorProvider.yellow.color; ;
        }
        else
        {
            targetColor = colorProvider.blue.color;
        }

        ControlPoints controlPoints = appController.OBJ.GetComponentInChildren<ControlPoints>();
        foreach (var t in controlPoints.getTransforms())
        {
            Renderer r = t.GetComponent<Renderer>();
            if (r != null && r.material != null)
            {
                r.material.color = targetColor;
            }
        }
    }

}
