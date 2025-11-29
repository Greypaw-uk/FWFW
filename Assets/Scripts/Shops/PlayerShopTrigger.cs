using UnityEngine;

public class PlayerShopTrigger : MonoBehaviour
{
    [SerializeField] private Shop shop; 

    private bool playerInTrigger = false;
    private GameObject currentPlayer;


    /// <summary>
    /// Detects when the player enters the shop trigger area.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var netObj = collision.GetComponent<Unity.Netcode.NetworkObject>();
        if (netObj == null || !netObj.IsOwner) return;

        playerInTrigger = true;
        currentPlayer = collision.gameObject;
    }


    /// <summary>
    /// Detects when the player exits the shop trigger area.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == currentPlayer)
        {
            playerInTrigger = false;
            currentPlayer = null;
        }
    }


    /// <summary>
    /// Handles player input to open the shop UI when in the trigger area.
    /// </summary>
    private void Update()
    {
        if (!playerInTrigger || currentPlayer == null) return;

        var inventory = currentPlayer.GetComponent<IInventory>();
        var currency = currentPlayer.GetComponent<ICurrency>();
        var shopUI = currentPlayer.GetComponent<ShopUI>();

        if (Input.GetKeyDown(KeyCode.F))
        {
            shop.Init(inventory, currency);
            shopUI.Init(shop, inventory, currency);
            shopUI.Open();
        }
    }
}
