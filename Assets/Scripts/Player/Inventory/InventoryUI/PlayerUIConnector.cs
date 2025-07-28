using UnityEngine;
using Unity.Netcode;

public class PlayerUIConnector : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            InventoryUI ui = FindObjectOfType<InventoryUI>();
            if (ui != null)
            {
                Inventory inventory = GetComponent<Inventory>();
                ui.SetInventory(inventory);
            }
            else
            {
                Debug.LogWarning("InventoryUI not found in scene.");
            }
        }
    }
}
