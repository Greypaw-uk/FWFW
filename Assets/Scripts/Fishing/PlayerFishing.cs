using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerFishing : NetworkBehaviour, IPlayerFishing
{
    [SerializeField] private LayerMask Water;
    [SerializeField] private IFishingMinigame fishingMinigame;

    public PlayerMovement playerMovement;
    public GameObject bobberPrefab;

    private IFishingRodController fishingRodController;
    private IBobberController activeBobber;
    private IInventory inventory;

    public bool isTouchingFishingSpot = false;
    private bool rodIsVisible = false;
    public bool isFishing = false;
    public float castDistance = 1.5f;
    public float castDuration = 0.8f;

    bool IPlayerFishing.isFishing => isFishing;

    public override void OnNetworkSpawn()
    {
        fishingRodController = GetComponent<FishingRodController>();
        (fishingRodController as FishingRodController)?.Initialize(transform);

        inventory = GetComponent<Inventory>(); // Cast to interface

        if (!IsOwner) return;

        fishingMinigame = FindObjectOfType<FishingMinigame>();
        if (fishingMinigame is IFishingMinigame minigameInterface)
            fishingMinigame = minigameInterface;
    }


    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            // Do not allow fishing if inventory is full
            if (inventory.GetCurrentItemsCount() >= inventory.GetMaxItemsCount()) return;

            // Do not allow fishing if already fishing or not touching a fishing spot
            if (isFishing || !isTouchingFishingSpot) return;

            StartCoroutine(CastOff());
        }

        if (isFishing && Input.GetKeyDown(KeyCode.Escape))
        {
            EndFishing();
        }
    }

    #region Enter/Exit Fishing Spot

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("FishingSpot") && !rodIsVisible)
        {
            isTouchingFishingSpot = true;
            rodIsVisible = true;

            // ensure controller has our transform before spawning
            (fishingRodController as FishingRodController)?.Initialize(transform);

            ShowRodServerRpc();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("FishingSpot") && rodIsVisible)
        {
            isTouchingFishingSpot = false;
            rodIsVisible = false;

            HideRodServerRpc();

            isFishing = false;
            RequestHideBobberServerRpc();
        }
    }

    #endregion

    #region Casting & Bobber

    private IEnumerator CastOff()
    {
        isFishing = true;

        RodFlickServerRpc();
        yield return new WaitForSeconds(1f);

        RequestCastServerRpc(Vector2.right);

        yield return new WaitForSeconds(castDuration);

        StartFishingMinigame();
    }

    [ServerRpc(RequireOwnership = true)]
    private void RequestCastServerRpc(Vector2 direction)
    {
        CastBobber(direction);
    }

    private void CastBobber(Vector2 direction)
    {
        if (bobberPrefab == null || activeBobber != null) return;

        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)direction * castDistance;

        var bobberObj = Instantiate(bobberPrefab, start, Quaternion.identity);
        var netObj = bobberObj.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("PlayerFishing: bobberPrefab missing NetworkObject.");
            Destroy(bobberObj);
            return;
        }
        netObj.Spawn();

        activeBobber = bobberObj.GetComponent<IBobberController>();
        activeBobber?.Launch(start, end, castDuration, this);
    }


    [ServerRpc(RequireOwnership = true)]
    private void RequestHideBobberServerRpc()
    {
        HideBobber();
    }

    private void HideBobber()
    {
        activeBobber?.Despawn();
        activeBobber = null;
    }

    #endregion

    #region Rod RPC wrappers

    [ServerRpc(RequireOwnership = true)]
    private void ShowRodServerRpc()
    {
        fishingRodController?.ShowRod(OwnerClientId);
    }

    [ServerRpc(RequireOwnership = true)]
    private void HideRodServerRpc()
    {
        if  (fishingRodController != null &&
            (fishingRodController as MonoBehaviour).TryGetComponent<NetworkObject>(out var netObj))
        {
            if (netObj != null && netObj.IsSpawned)
            {
                fishingRodController.HideRod();
            }
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void RodFlickServerRpc()
    {
        fishingRodController?.Flick();
    }

    #endregion

    #region Minigame & Rewards

    private void StartFishingMinigame()
    {
        if (fishingMinigame != null )
        {
            fishingMinigame.OnCatchSuccess += HandleCatchSuccess;
            fishingMinigame.OnCatchFailed += HandleCatchFailed;
            fishingMinigame.StartMinigame();
        }
    }

    public void HandleCatchSuccess()
    {
        var fish = FishGenerator.GenerateRandomFish();
        AddFishServerRpc(fish.Name, fish.Weight, fish.Price);
        EndFishing();
    }

    public void HandleCatchFailed()
    {
        EndFishing();
    }

    private void EndFishing()
    {
        RodFlickServerRpc();
        RequestHideBobberServerRpc();

        isFishing = false;

        if (fishingMinigame != null)
        {
            fishingMinigame.OnCatchSuccess -= HandleCatchSuccess;
            fishingMinigame.OnCatchFailed -= HandleCatchFailed;
            fishingMinigame.CancelMinigame();
        }
    }

    [ServerRpc]
    private void AddFishServerRpc(string fishName, float weight, float price)
    {
        AddFishClientRpc(fishName, weight, price);
    }

    [ClientRpc]
    private void AddFishClientRpc(string fishName, float weight, float price)
    {
        Sprite fishIcon = GetComponent<Inventory>().GetSpriteForItem(fishName);
        Items.Item fish = new(fishName, weight, price, fishIcon);
        GetComponent<Inventory>().AddItem(fish);
    }

    #endregion
}
