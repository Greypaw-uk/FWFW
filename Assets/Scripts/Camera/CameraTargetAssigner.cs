using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetAssigner : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        var vcam = FindFirstObjectByType<CinemachineCamera>();
        if (vcam == null) return;

        vcam.Follow = transform;
        // vcam.LookAt = transform;   // Only if Rotation Control is Hard Look At
    }
}
