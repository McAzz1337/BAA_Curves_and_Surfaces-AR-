using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TranslationSpeedCanvas : MonoBehaviour
{

    [SerializeField]
    private HandPinchTranslation handTranslation;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private ApplicationController appController;

    [SerializeField]
    private Vector3 offset = new Vector3(-0.15f, 0.15f, 0.5f);

    private bool active = false;
    [SerializeField]
    private Slider translationSpeedSlider;
    [SerializeField]
    private TextMeshProUGUI speedDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        translationSpeedSlider.onValueChanged.AddListener(updateSpeedDisplayText);
        hide();
    }

    public void hide()
    {
        active = false;
        toggleCanvas(active);
    }

    private void tuckAway()
    {
        Transform t = appController.Cam.transform;
        transform.position = t.position - t.forward * 100.0f - t.up * 100.0f;
    }

    public void toggleCanvas(bool show)
    {
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
        translationSpeedSlider.value = 1.0f;
        updateSpeedDisplayText(translationSpeedSlider.value);
        active = show;
        if (!show)
        {
            tuckAway();
        }
    }

    private void updateSpeedDisplayText(float value)
    {
        speedDisplay.text = "Translation Speed: X" + value.ToString("0.0");
    }

    void LateUpdate()
    {
        if (active)
        {
            Camera cam = appController.Cam;
            transform.position = cam.transform.position + offset;

            Quaternion rot = Quaternion.LookRotation(
                transform.position - cam.transform.position);

            transform.rotation = rot;
        }
    }
}
