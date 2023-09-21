using System.Threading.Tasks;
using UnityEngine;

public class OptionNumber : Number3D
{
    [SerializeField] GameObject progressBar;
    [SerializeField] Transform progressFillPivot;
    Vector3 progressFillScaleStart = new(1f, 1f, 0f),
        progressFillScaleEnd = new(1f, 1f, 1f);

    [SerializeField] ParticleSystem enterVFX;
    public ParticleSystem starVFX;

    public bool Cancelled { get; set; }

    protected override void Awake()
    {
        startVector = Vector3.zero;
        endVector = transform.localScale;

        SetProgressBarActive(false);
    }

    public override async Task EnterTransition(int number)
    {
        enterVFX.Play();

        SetProgressBarActive(true);

        Cancelled = false;

        await base.EnterTransition(number); // Enable, set model and value, animate enter
    }

    protected override async Task ExitTransition()
    {
        SetProgressBarActive(false);

        await base.ExitTransition(); // Disable, animate exit
    }

    protected override async Task AnimateTransition(Vector3 start, Vector3 end, float time)
    {
        float percent = 0f;
        while (percent < 1f)
        {
            percent += 1f / time * Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, end, percent);

            await Task.Yield();
        }
    }

    void SetProgressBarActive(bool value)
    {
        progressBar.SetActive(value);
        SetProgressFill(0f);
    }

    public void SetProgressFill(float percent)
    {
        progressFillPivot.localScale =
            Vector3.Lerp(progressFillScaleStart, progressFillScaleEnd, percent);
    }
}