using TMPro;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI
        numbersLeftTxt,
        starsTxt;

    public TextPrompt textPrompt;

    [SerializeField] GameObject resultsUI;
    [SerializeField] TextMeshProUGUI resultsPerformanceTxt;

    MainMenuCanvas mainMenuCanvas;

    public static GameCanvas Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainMenuCanvas = MainMenuCanvas.Instance;
    }

    public void SetNumbersLeftTxt(int numsLeft)
    {
        numbersLeftTxt.text = $"{numsLeft}/{GameManager.TotalNumbers} numbers left";
    }

    public void SetStarsTxt(int stars)
    {
        starsTxt.text = stars.ToString();
    }

    public void ShowResults(int performance)
    {
        resultsUI.SetActive(true);
        resultsPerformanceTxt.text = performance + "%";
    }

    public void ResultsHomeBtn()
    {
        mainMenuCanvas.gameObject.SetActive(true);

        resultsUI.SetActive(false);
        gameObject.SetActive(false);
    }
}