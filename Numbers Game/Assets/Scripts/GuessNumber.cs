using System.Threading.Tasks;
using UnityEngine;

public class GuessNumber : Number3D
{
    [SerializeField] Vector3 endPosOffset;

    [SerializeField] ParticleSystem glowVFX;

    protected override void Awake()
    {
        startVector = Vector3.zero;
        endVector = endPosOffset;
    }

    void OnEnable()
    {
        meshFilter.gameObject.SetActive(true);

        glowVFX.Play();
    }

    void OnDisable()
    {
        glowVFX.Stop();
    }

    protected override async Task ExitTransition()
    {
        await base.ExitTransition();

        meshFilter.gameObject.SetActive(false);
    }

    protected override async Task AnimateTransition(Vector3 start, Vector3 end, float time)
    {
        float percent = 0f;
        while (percent < 1f)
        {
            percent += 1f / time * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, end, percent);

            await Task.Yield();
        }
    }
}