using UnityEngine;

public class OptionSelector : MonoBehaviour
{
    [SerializeField] float detectionRadius;

    [SerializeField] LayerMask optionNumberLayerMask;
    [SerializeField] float timeToSelect;
    float selectTimer;

    OptionNumber currentOption;
    bool selected;

    [SerializeField] ParticleSystem selectionVFX;

    public Camera Camera { get; set; }

    GameManager gameManager;

    [SerializeField] SoundsAsset soundsAsset;
    AudioManager audioManager;

    public static OptionSelector Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        audioManager = AudioManager.Instance;
    }

    private void Update()
    {
        if (Physics.SphereCast(
            transform.position, detectionRadius, Vector3.down, out RaycastHit hit,
            optionNumberLayerMask))
        {
            if (!selected)
            {
                OptionNumber optionNumber = hit.transform.GetComponent<OptionNumber>();
                // Enter
                if (optionNumber != currentOption)
                {
                    OnOptionExit();

                    currentOption = optionNumber;
                    OnOptionEnter();
                }
                // Stay
                else
                {
                    OnOptionStay();
                }
            }
        }
        // Exit
        else
        {
            OnOptionExit();
        }
    }

    void PlayVFXAtOptionNumPos(ParticleSystem FX)
    {
        FX.transform.position = currentOption.transform.position;
        FX.Play();
    }

    void OnOptionEnter()
    {
        PlayVFXAtOptionNumPos(selectionVFX);
        audioManager.PlaySFX(soundsAsset.selectionSFX);
    }

    void OnOptionStay()
    {
        selectTimer -= Time.deltaTime;

        float percent = 1f - selectTimer / timeToSelect;
        currentOption.SetProgressFill(percent);

        if (selectTimer <= 0)
        {
            selected = true;

            selectionVFX.Stop();

            gameManager.SelectOption(currentOption);
        }
    }

    void OnOptionExit()
    {
        selectTimer = timeToSelect;
        selected = false;

        if (currentOption != null)
        {
            currentOption.SetProgressFill(0f);
            currentOption = null;

            selectionVFX.Stop();
            audioManager.StopSFX();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}