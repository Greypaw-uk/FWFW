using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerCameraInitializer : NetworkBehaviour
{
    private bool cameraAssigned = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // If scene already loaded, assign camera after a short delay
        if (SceneManager.GetActiveScene().isLoaded)
        {
            Invoke(nameof(AssignCamera), 0.1f);
        }

        // Subscribe to future scene loads just in case
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsOwner) return;

        // Assign after delay to ensure everything is in place
        Invoke(nameof(AssignCamera), 0.1f);
    }

    private void AssignCamera()
    {
        if (cameraAssigned) return;

        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = transform;
            vcam.LookAt = transform;
            cameraAssigned = true;
            Debug.Log($"🎥 Camera assigned to player {OwnerClientId}");
        }
        else
        {
            Debug.LogWarning("❌ Camera not found when assigning.");
        }
    }
}
