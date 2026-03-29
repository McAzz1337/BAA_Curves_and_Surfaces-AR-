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
    private Camera cam;

    [SerializeField]
    private float offsetX = 0.3f;

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

    public void toggleCanvas(bool show)
    {
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
        translationSpeedSlider.value = 1.0f;
        updateSpeedDisplayText(translationSpeedSlider.value);
    }

    private void updateSpeedDisplayText(float value)
    {
        speedDisplay.text = "Translation Speed: X" + value.ToString("0.0");
    }

    void LateUpdate()
    {
        transform.position = cam.transform.position + new Vector3(0.0f, 0.0f, 0.5f);

        Quaternion rot = Quaternion.LookRotation(
            transform.position - cam.transform.position);

        transform.rotation = rot;
        transform.position += new Vector3(offsetX, offsetX, 0.0f);
    }
}
