using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuStatus : MonoBehaviour
{

    private enum Type
    {
        BEZIER_CURVVE,
        BEZIER_SURFACE,
        BSPLINES_CURVE
    }

    private int nodes;


    public Button generateButton;
    public Slider nodesSlider;
    public TextMeshProUGUI nodesDisplay;
    public TMP_Dropdown typeDropdown;
    private Type selectedType;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generateButton.onClick.AddListener(OnGenerateButtonClicked);
        nodesSlider.onValueChanged.AddListener(OnSliderValueChanged);
        typeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGenerateButtonClicked()
    {
        Debug.Log("Generate button clicked!");
    }

    private void OnSliderValueChanged(float value)
    {
        int nodesValue = (int)value;
        nodes = nodesValue;
        nodesDisplay.text = "Nodes: " + nodesValue.ToString();
    }

    private void OnDropdownValueChanged(int index)
    {
        selectedType = (Type)index;
        string selectedText = typeDropdown.options[index].text;
        Debug.Log("Selected type: " + selectedText + " (Index: " + index + ")");
    }
}
