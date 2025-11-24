using Unity.Netcode;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class FishingRodController : NetworkBehaviour, IFishingRodController
{
    [Header("Assign the networked Rod prefab here (must have NetworkObject)")]
    [SerializeField] private GameObject rodPrefab;

    [Header("Flick animation")]
    [SerializeField] private float flickAngle = 60f;
    [SerializeField] private float flickDuration = 1f;

    private Transform followTarget;              // player transform (this NetworkObject)
    private GameObject activeRod;                // spawned rod instance
    private NetworkObject activeRodNetObj;       // cached for despawn checks
    private Quaternion originalLocalRotation;    // stored after spawn for animation

    public void Initialize(Transform target)
    {
        followTarget = target;
    }

    /// Server-side: spawn the rod and parent under the player NetworkObject so it follows automatically.
    public void ShowRod(ulong ownerClientId)
    {
        if (!IsServer) return;                        // only the server spawns/despawns networked objects
        if (activeRod != null) return;                // already spawned
        if (rodPrefab == null)
        {
            Debug.LogError("FishingRodController: rodPrefab is not assigned on the server.");
            return;
        }

        if (followTarget == null)
        {
            // followTarget should be our own transform (player) — Initialize() must be called by owner before this
            followTarget = transform;
        }

        // Instantiate at player's position
        activeRod = Instantiate(rodPrefab, followTarget.position, Quaternion.identity);
        activeRodNetObj = activeRod.GetComponent<NetworkObject>();

        if (activeRodNetObj == null)
        {
            Debug.LogError("FishingRodController: rodPrefab is missing NetworkObject.");
            Destroy(activeRod);
            activeRod = null;
            return;
        }

        // Spawn with ownership so the owning client can animate/see it consistently
        activeRodNetObj.SpawnWithOwnership(ownerClientId);

        // Network parent under this player so the rod follows naturally (no per-frame sync needed)
        var myNetObj = GetComponent<NetworkObject>();
        if (myNetObj != null && myNetObj.IsSpawned)
        {
            // Parent on the server propagates to clients
            activeRodNetObj.TrySetParent(myNetObj, false); // worldPositionStays = false => snap to player's transform
            activeRod.transform.localPosition = Vector3.zero;
            activeRod.transform.localRotation = Quaternion.identity;
        }

        // Store original rotation for flick animation
        originalLocalRotation = activeRod.transform.localRotation;
    }

    /// Server-side: despawn the rod safely.
    public void HideRod()
    {
        if (!IsServer) return;

        if (activeRodNetObj != null && activeRodNetObj.IsSpawned)
        {
            activeRodNetObj.Despawn(true);
        }
        else if (activeRod != null)
        {
            // fallback if not networked/spawned for some reason
            Destroy(activeRod);
        }

        activeRod = null;
        activeRodNetObj = null;
    }

    /// Flick the currently spawned rod (runs on whoever calls it; visuals are client-side)
    public void Flick()
    {
        if (activeRod == null) return;
        StopAllCoroutines();
        StartCoroutine(FlickRoutine());
    }

    private IEnumerator FlickRoutine()
    {
        // Animate the spawned rod's local rotation relative to the player
        var start = originalLocalRotation;
        var target = originalLocalRotation * Quaternion.Euler(0f, 0f, flickAngle);
        float elapsed = 0f;

        while (elapsed < flickDuration)
        {
            float t = elapsed / flickDuration;
            // sin curve for a quick whip
            if (activeRod != null)
                activeRod.transform.localRotation = Quaternion.Slerp(start, target, Mathf.Sin(t * Mathf.PI));
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (activeRod != null)
            activeRod.transform.localRotation = originalLocalRotation;
    }
}
