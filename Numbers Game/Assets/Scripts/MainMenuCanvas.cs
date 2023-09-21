using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    GameManager gameManager;

    public static MainMenuCanvas Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void PlayBtn()
    {
        gameManager.enabled = true;

        gameObject.SetActive(false);
    }
}