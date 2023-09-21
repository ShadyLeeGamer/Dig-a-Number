using System.Threading.Tasks;
using UnityEngine;

public abstract class Number3D : MonoBehaviour
{
    [SerializeField] protected MeshFilter meshFilter;
    [SerializeField] Mesh[] mesh;

    [SerializeField] Transform looker;

    [SerializeField] protected float animEnterTime, animExitTime;

    protected Vector3 startVector, endVector;

    public int Number { get; private set; }

    Camera cam;

    protected abstract void Awake(); // Setup vars for anim

    protected virtual void Start()
    {
        cam = OptionSelector.Instance.Camera;

        GameManager.Instance.OnNumberIdentified += Exit;
    }

    public virtual async Task EnterTransition(int number)
    {
        enabled = true;

        Number = number;
        meshFilter.mesh = mesh[Number - 1];

        await AnimateTransition(startVector, endVector, animEnterTime);
    }

    private void Update()
    {
        Vector3 dirToCam =
            (cam.transform.position - looker.transform.position).normalized;
        looker.rotation = Quaternion.LookRotation(dirToCam);
    }

    async void Exit() => await ExitTransition();

    protected virtual async Task ExitTransition()
    {
        enabled = false;

        await AnimateTransition(endVector, startVector, animExitTime);
    }

    protected virtual async Task AnimateTransition(
        Vector3 startPos, Vector3 endPos, float time) { }
}