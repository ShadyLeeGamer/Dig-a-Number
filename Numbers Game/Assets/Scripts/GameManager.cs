using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    const int MIN_NUMBER = 1, MAX_NUMBER = 10;
    public static int TotalNumbers => MAX_NUMBER - MIN_NUMBER + 1;
    int[] allNumbers;

    List<int> guessNumbersLeft;

    [SerializeField] public int numOptions;
    [SerializeField] float optionNumberOffsetX, optionNumberOffsetZ;

    NumberDigger numberDigger;
    OptionSelector optionSelector;

    [SerializeField] GuessNumber guessNumberPrefab;
    [SerializeField] OptionNumber optionNumberPrefab;

    [SerializeField] Transform numbersPose;

    GuessNumber guessNumber;
    OptionNumber[] optionNumbers;

    public int stars;

    [SerializeField] ParticleSystem numberGuessedVFX;

    [SerializeField] GameCanvas gameCanvas;

    [SerializeField] SoundsAsset soundsAsset;
    AudioManager audioManager;

    [SerializeField] float transitionToPlaySpaceDelay;

    public System.Action OnNumberIdentified;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        allNumbers = new int[TotalNumbers];
        int i = 0;
        for (int num = MIN_NUMBER; num <= MAX_NUMBER; num++)
        {
            allNumbers[i++] = num;
        }

        SetupGuessNumber();
        SetupOptionNumbers();

        optionSelector = OptionSelector.Instance;

        audioManager = AudioManager.Instance;

        numberDigger = NumberDigger.Instance;
        numberDigger.enabled = true;

        gameCanvas.gameObject.SetActive(true);
    }

    void SetupGuessNumber()
    {
        guessNumbersLeft = new List<int>(allNumbers);

        guessNumber = Instantiate(guessNumberPrefab, numbersPose);
        guessNumber.AddComponent<ARAnchor>();
    }

    void SetupOptionNumbers()
    {
        optionNumbers = new OptionNumber[numOptions];
        var centreOffset =
            Vector3.left * optionNumberOffsetX * ((float)(numOptions - 1f) / 2f);
        for (int i = 0; i < numOptions; i++)
        {
            optionNumbers[i] = Instantiate(optionNumberPrefab, numbersPose);
            optionNumbers[i].AddComponent<ARAnchor>();

            Vector3 offset = new();
            offset.x = optionNumberOffsetX * i;

            float t = (float)i / (numOptions - 1);
            offset.z = optionNumberOffsetZ * -Mathf.Sin(t * Mathf.PI);

            optionNumbers[i].transform.position = Vector3.zero + offset + centreOffset;
        }
    }

    public async void NextNumberAtPose(Pose centerPose)
    {
        numberDigger.enabled = false;

        numbersPose.SetPositionAndRotation(centerPose.position, centerPose.rotation);
        audioManager.PlaySFX(soundsAsset.digSFX);

        await EnterNewGuessNumber();
        audioManager.PlaySFX(soundsAsset.numberCountSFX[guessNumber.Number - 1]);

        await EnterNewOptionNumbers();
        audioManager.PlayTrack(soundsAsset.optionsTrack);

        await gameCanvas.textPrompt.Set(TextPrompt.Prompts.SelectOption);
        optionSelector.enabled = true;
    }

    async Task EnterNewGuessNumber()
    {
        gameCanvas.SetNumbersLeftTxt(guessNumbersLeft.Count);

        int guessNumberIndex = Random.Range(0, guessNumbersLeft.Count);
        await guessNumber.EnterTransition(guessNumbersLeft[guessNumberIndex]);
        guessNumbersLeft.RemoveAt(guessNumberIndex);
    }

    async Task EnterNewOptionNumbers()
    {
        int correctOptionIndex = Random.Range(0, numOptions);

        List<int> optionNumbersLeft = new(allNumbers);
        optionNumbersLeft.Remove(guessNumber.Number);

        for (int i = 0; i < numOptions; i++)
        {
            int optionNum = i == correctOptionIndex
                ? guessNumber.Number
                : optionNumbersLeft[Random.Range(0, optionNumbersLeft.Count)];

            await optionNumbers[i].EnterTransition(optionNum);
            optionNumbersLeft.Remove(optionNum);
        }
    }

    public async void SelectOption(OptionNumber optionNumber)
    {
        if (optionNumber.Number == guessNumber.Number)
        {
            optionSelector.enabled = false;

            foreach (OptionNumber option in optionNumbers)
            {
                if (!option.Cancelled)
                {
                    stars++;
                    option.starVFX.Play();
                }
            }
            gameCanvas.SetStarsTxt(stars);

            numberGuessedVFX.transform.position = guessNumber.transform.position;
            numberGuessedVFX.Play();
            OnNumberIdentified?.Invoke();

            if (guessNumbersLeft.Count > 0)
            {
                audioManager.PlaySFX(soundsAsset.correctOptionSFX);

                // Transition prompts
                await gameCanvas.textPrompt.Set(TextPrompt.Prompts.CorrectOption);
                await Task.Delay((int)(transitionToPlaySpaceDelay * 1000)); // Convert to ms
                await gameCanvas.textPrompt.Set(TextPrompt.Prompts.TapOnPlane);

                numberDigger.enabled = true;
            }
            else
            {
                float performance = 1 / (numOptions * TotalNumbers / (float)stars) * 100;
                gameCanvas.ShowResults((int)performance);
            }
        }
        else
        {
            optionNumber.Cancelled = true;
            await gameCanvas.textPrompt.Set(TextPrompt.Prompts.IncorrectOption);
        }
    }
}