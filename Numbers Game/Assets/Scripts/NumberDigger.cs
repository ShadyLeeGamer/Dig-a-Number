using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class NumberDigger : MonoBehaviour
{
    Camera cam;

    [SerializeField] Transform indicator;

    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARRaycastManager raycastManager;

    [SerializeField] GameObject spade;
    [SerializeField] ParticleSystem digVFX;

    bool onPlane;

    Pose dugPose;

    GameManager gameManager;

    public static NumberDigger Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    bool GetPlaneHit(out ARRaycastHit hit)
    {
        List<ARRaycastHit> hits = new();

        Vector2 screenCenter = cam.ViewportToScreenPoint(new(0.5f, 0.5f));
        if (raycastManager.Raycast(screenCenter, hits,
            TrackableType.PlaneWithinPolygon))
        {
            ARPlane plane = planeManager.GetPlane(hits[0].trackableId);
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                hit = hits[0];
                return true;
            }
        }

        hit = default;
        return false;
    }


    private void Update()
    {
        if (onPlane = GetPlaneHit(out ARRaycastHit hit))
        {
            dugPose = hit.pose;
            Vector3 dirToCam = (cam.transform.position - dugPose.position).normalized;
            indicator.SetPositionAndRotation(
                dugPose.position, Quaternion.LookRotation(dirToCam));
        }

        indicator.gameObject.SetActive(onPlane);

        HandleInput();
    }

    void HandleInput()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            Dig();
        }
    }

    void Dig()
    {
        if (!onPlane)
            return;

        gameManager.NextNumberAtPose(dugPose);

        digVFX.transform.position = dugPose.position;
        digVFX.Play();
    }

    private void OnEnable()
    {
        SetAllPlanesActive(true);

        spade.SetActive(true);
    }

    private void OnDisable()
    {
        SetAllPlanesActive(false);

        indicator.gameObject.SetActive(false);

        spade.SetActive(false);
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }

        planeManager.enabled = raycastManager.enabled = value;
    }
}