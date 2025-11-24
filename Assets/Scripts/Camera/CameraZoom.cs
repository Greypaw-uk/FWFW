using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    public float currentZoomLevel;
    private float zoomSpeed = 3f;

    public float minZoom = 1f;
    public float maxZoom = 10f;

    void Start()
    {
        if (vcam == null)
            vcam = GetComponent<CinemachineVirtualCamera>();

        currentZoomLevel = vcam.m_Lens.OrthographicSize;
    }

    void Update()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        currentZoomLevel -= scrollData * zoomSpeed;
        currentZoomLevel = Mathf.Clamp(currentZoomLevel, minZoom, maxZoom);

        vcam.m_Lens.OrthographicSize = currentZoomLevel;
    }
}

