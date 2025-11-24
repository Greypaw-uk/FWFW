using UnityEngine;

public class PlayerShopTrigger : MonoBehaviour
{
    [SerializeField] private Shop shop;

    private bool playerInTrigger = false;
    private GameObject currentPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var netObj = collision.GetComponent<Unity.Netcode.NetworkObject>();
        if (netObj == null || !netObj.IsOwner) return;

        playerInTrigger = true;
        currentPlayer = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == currentPlayer)
        {
            playerInTrigger = false;
            currentPlayer = null;
        }
    }

    private void Update()
    {
        if (!playerInTrigger || currentPlayer == null) return;

        var inventory = currentPlayer.GetComponent<Inventory>();
        var currency = currentPlayer.GetComponent<Currency>();
        var shopUI = currentPlayer.GetComponent<ShopUI>();

        if (!IsValid(inventory, currency, shopUI)) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            // Do not re-open if already open
            if (!shopUI.IsOpen)
                shopUI.Init(inventory, currency, shop);
        }
    }

    private bool IsValid(Inventory inventory, Currency currency, ShopUI shopUI)
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory missing!");
            return false;
        }
        if (currency == null)
        {
            Debug.LogError("Currency missing!");
            return false;
        }
        if (shopUI == null)
        {
            Debug.LogError("ShopUI missing!");
            return false;
        }

        return true;
    }
}
