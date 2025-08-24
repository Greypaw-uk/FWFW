using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerFishing : NetworkBehaviour
{
    [SerializeField] private LayerMask Water;
    [SerializeField] private FishingMinigame fishingMinigame;

    // Fishing
    //private FishingMinigame fishingMinigame;
    public float fishingDuration = 5f; 
    public bool isTouchingFishingSpot = false;
    public bool isFishing = false;

    // Bobber
    public PlayerMovement playerMovement;
    public GameObject bobberPrefab;
    private GameObject activeBobber;
    public bool BobberIsInWater;
    public float castDistance = 1.5f; 
    public float castDuration = 0.8f;

    // Fishing Rod
    public GameObject rodPrefab;
    private GameObject activeRod;
    public float CastingAngle = 60f;


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            fishingMinigame = FindObjectOfType<FishingMinigame>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (activeRod != null)
            activeRod.transform.position = transform.position;


        // Attempt to cast off if not already fishing and are at fishing spot
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isFishing || !isTouchingFishingSpot)
                return;

            StartCoroutine(CastOff());

            // If bobber is in water, begin fishing process
            if (BobberIsInWater && !isFishing)
            {
                //StartCoroutine(FishCoroutine());

                // Start the fishing minigame
                fishingMinigame.OnCatchSuccess = HandleCatchSuccess;
                fishingMinigame.OnCatchFailed = HandleCatchFailed;
                fishingMinigame.StartMinigame();
            }
        }
    }


    /// <summary>
    /// Enable isTouchingFishingSpot on collision
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("FishingSpot"))
        {
            isTouchingFishingSpot = true;
            RequestShowRodServerRpc();
        }
    }


    /// <summary>
    /// Disable isTouchingFishingSpot on collision exit
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("FishingSpot"))
        {
            isTouchingFishingSpot = false;
            HideRod();

            isFishing = false;
            RequestHideBobberServerRpc();
        }
    }


    /// <summary>
    /// Recieve notification from Bobber when it touches water
    /// </summary>
    public void NotifyBobberInWater()
    {
        BobberIsInWater = true;
    }


    /// <summary>
    /// Animate player's rod 
    /// Then create and animate bobber
    /// </summary>
    /// <returns></returns>
    IEnumerator CastOff()
    {
        RequestRodFlickServerRpc();

        yield return new WaitForSeconds(1f);

        Vector2 direction = playerMovement.LastMovementDirection.normalized;
        RequestCastServerRpc(direction);
    }


    [ServerRpc(RequireOwnership = false)]
    public void RequestCastServerRpc(Vector2 direction)
    {
        CastBobber(direction);
    }


    void HandleCatchSuccess()
    {
        var fish = FishGenerator.GenerateRandomFish();
        AddFishServerRpc(fish.Name, fish.Weight, fish.Price);

        EndFishing();
    }


    void HandleCatchFailed()
    {
        EndFishing();
    }


    void EndFishing()
    {
        RequestRodFlickServerRpc();
        RequestHideBobberServerRpc();

        isFishing = false;
    }


    /// <summary>
    /// Start the fishing mini game, add a random fish if successful, then show the rod flick once finished.
    /// </summary>
    /// <param name="success"></param>
    private void OnMinigameResult(bool caught)
    {
        if (caught)
        {
            var fish = FishGenerator.GenerateRandomFish();
            AddFishServerRpc(fish.Name, fish.Weight, fish.Price);
        }

        // Continue cleanup (flick rod etc.)
        RequestRodFlickServerRpc();
        RequestHideBobberServerRpc();
    }


    /// <summary>
    /// Notify server of change
    /// </summary>
    /// <param name="fishName"></param>
    [ServerRpc]
    void AddFishServerRpc(string fishName, float weight, float price)
    {
        AddFishClientRpc(fishName, weight, price);
    }

    /// <summary>
    /// Add fish to player's inventory
    /// </summary>
    /// <param name="fishName"></param>
    [ClientRpc]
    void AddFishClientRpc(string fishName, float weight, float price)
    {   
        Sprite fishIcon = GetComponent<Inventory>().GetSpriteForItem(fishName);
        Items.Item fish = new Items.Item(fishName, weight, price, fishIcon);

        GetComponent<Inventory>().AddItem(fish);
    }


    #region Fishing Rod

    [ServerRpc(RequireOwnership = false)]
    public void RequestShowRodServerRpc()
    {
        ShowRod();
    }

    private void ShowRod()
    {
        if (!IsServer) return;

        if (rodPrefab != null && activeRod == null)
        {
            // Instantiate the rod without parenting
            activeRod = Instantiate(rodPrefab, transform.position, Quaternion.identity);

            // Get and spawn the NetworkObject with ownership
            var rodNetObj = activeRod.GetComponent<NetworkObject>();
            rodNetObj.SpawnWithOwnership(OwnerClientId);

            // Assign this player as the follow target
            var rodController = activeRod.GetComponent<FishingRodController>();
            if (rodController != null)
            {
                rodController.Initialize(transform);
            }
        }
    }


    private void HideRod()
    {
        if (activeRod != null)
        {
            if (IsServer)
            {
                // Despawn rod on all clients
                activeRod.GetComponent<NetworkObject>().Despawn(true);
            }
            else
            {
                // Just destroy locally
                Destroy(activeRod);
            }

            activeRod = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRodFlickServerRpc()
    {
        if (activeRod != null)
        {
            activeRod.GetComponent<FishingRodController>()?.Flick();
        }
    }

    private void TriggerRodFlick()
    {
        if (activeRod != null)
        {
            activeRod.GetComponent<FishingRodController>()?.Flick();
        }
    }

    #endregion


    #region Bobber

    [ServerRpc(RequireOwnership = false)]
    private void RequestCastBobberServerRpc(Vector2 direction)
    {
        CastBobber(direction);
    }

    private void CastBobber(Vector2 direction)
    {
        if (bobberPrefab != null && activeBobber == null)
        {
            Vector3 start = transform.position;
            Vector3 end = start + (Vector3)direction * castDistance;

            activeBobber = Instantiate(bobberPrefab, start, Quaternion.identity);
            activeBobber.GetComponent<NetworkObject>().Spawn();
            activeBobber.GetComponent<BobberController>()?.Launch(start, end, castDuration);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHideBobberServerRpc()
    {
        HideBobber();
    }

    private void HideBobber()
    {
        if (activeBobber != null)
        {
            if (IsServer)
            {
                // Despawn the bobber on all clients
                activeBobber.GetComponent<NetworkObject>().Despawn(true);
            }
            else
            {
                // If you're not the server, just destroy the local instance
                Destroy(activeBobber);
            }

            activeBobber = null;
        }
    }

    #endregion
}