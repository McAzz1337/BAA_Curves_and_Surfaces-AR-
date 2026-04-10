using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuStatus : MonoBehaviour
{


    private int nodes;


    [SerializeField]
    private ApplicationController appController;

    public Button generateButton;
    public Slider nodesSlider;
    public TextMeshProUGUI nodesDisplay;
    public TMP_Dropdown typeDropdown;
    private EType selectedType;

    [SerializeField]
    private MenuCanvas menuCanvas;


    void Start()
    {
        generateButton.onClick.AddListener(OnGenerateButtonClicked);
        nodesSlider.onValueChanged.AddListener(OnSliderValueChanged);
        typeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        nodes = (int)nodesSlider.value;
        updateNodesText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGenerateButtonClicked()
    {
        GameObject obj = appController.GetComponent<Generator>().
            generate(selectedType,
                 nodes,
                 appController.Cam);

        appController.OBJ = obj;

        menuCanvas.hide();
    }

    private void OnSliderValueChanged(float value)
    {
        nodes = (int)value;
        updateNodesText();
    }

    private void updateNodesText()
    {
        nodesDisplay.text = "Nodes: " + nodes.ToString();

    }

    private void OnDropdownValueChanged(int index)
    {
        selectedType = (EType)index;
        string selectedText = typeDropdown.options[index].text;
        Debug.Log("Selected type: " + selectedText + " (Index: " + index + ")");
    }
}
