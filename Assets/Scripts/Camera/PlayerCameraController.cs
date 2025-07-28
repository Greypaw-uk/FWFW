using Unity.Netcode;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    public Camera playerCamera;

    void Start()
    {
        if (!IsOwner && playerCamera != null)
        {
            playerCamera.enabled = false;
            return;
        }

        if (IsOwner && playerCamera != null)
        {
            playerCamera.enabled = true;
        }
    }
}