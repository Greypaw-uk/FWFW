using UnityEngine;
using UnityEngine.AI;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;
    public float currentZoomLevel;
    private float zoomSpeed = 3f;

    public float minZoom = 1f;
    public float maxZoom = 10f;


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        currentZoomLevel = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        currentZoomLevel -= scrollData * zoomSpeed;
        currentZoomLevel = Mathf.Clamp(currentZoomLevel, minZoom, maxZoom);

        if (currentZoomLevel > maxZoom)
            currentZoomLevel = maxZoom;

        cam.orthographicSize = currentZoomLevel;
    }
}
