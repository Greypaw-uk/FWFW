using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
    private CinemachineCamera cineCam;

    public float zoomSpeed = 5f;
    public float minZoom = 1f;
    public float maxZoom = 7f;

    private void Start()
    {
        cineCam = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        if (cineCam == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            float newZoom = Mathf.Clamp(
                cineCam.Lens.OrthographicSize - scroll * zoomSpeed,
                minZoom,
                maxZoom
            );

            cineCam.Lens.OrthographicSize = newZoom;
        }
    }
}
