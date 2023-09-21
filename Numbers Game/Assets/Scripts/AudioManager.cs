using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource trackSource;
    [SerializeField] AudioSource sfxSource;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnNumberIdentified += StopTrack;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void PlayTrack(AudioClip clip)
    {
        trackSource.clip = clip;
        trackSource.Play();
    }

    public void StopTrack()
    {
        trackSource.Stop();
    }
}