using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;

public class InventoryUI : NetworkBehaviour
{
    public int InventorySize = 10; 

    [SerializeField] public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject slotPrefab;

    private Inventory playerInventory;

    private List<GameObject> slots = new();

    public Tooltip tooltip;


    /// <summary>
    /// Assigns inventory to player if they are active
    /// </summary>
    public override void OnNetworkSpawn()
    {
        inventoryPanel.SetActive(false);

        if (!IsOwner)
            enabled = false;
    }


    public void SetInventory(Inventory inventory)
    {
        playerInventory = inventory;
    }


    void Update()
    {
        // Display player's inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
            ToggleTooltip();
        }
    }


    /// <summary>
    /// Display player's inventory if tag is pressed
    /// </summary>
    void ToggleInventory()
    {
        bool isActive = inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!isActive);

        if (!isActive)
            RefreshInventory();
    }


        /// <summary>
    /// Display player's inventory if tag is pressed
    /// </summary>
    void ToggleTooltip()
    {
        if (tooltip.tooltipObject.activeSelf)
            tooltip.Hide();
        else
        {
            tooltip.Hide(); // Just in case
            RefreshInventory(); // or whatever re-evaluates hovered item
        }
    }


    void RefreshInventory()
    {
        // Purge Inventory
        foreach (var obj in slots)
            Destroy(obj);

        slots.Clear();

        // Ensure Inventory is assigned to player
        if (playerInventory == null)
        {
            Debug.LogWarning("InventoryUI: No playerInventory assigned!");
            return;
        }

        // Read items in inventory
        List<Items.Item> itemsList = new();
        itemsList = playerInventory.GetItems();

        int totalSlots = 10;

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);
            slot.SetActive(true);

            // Get the ItemIcon GameObject and its Image component
            Transform iconTransform = slot.transform.Find("ItemIcon");
            if (iconTransform == null)
                continue;

            GameObject iconObject = iconTransform.gameObject;
            Image iconImage = iconObject.GetComponent<Image>();
            if (iconImage == null)
                continue;

            if (i < itemsList.Count)
            {
                string itemName = itemsList[i].Name;
                string itemWeight = itemsList[i].Weight.ToString();
                string itemPrice = itemsList[i].Price.ToString();

                Sprite itemSprite = playerInventory.GetSpriteForItem(itemName); 

                if (itemSprite != null)
                {
                    // Show item's icon
                    iconImage.sprite = itemSprite;
                    iconObject.SetActive(true); 
                }
                else
                {
                    // Hide if no sprite found
                    iconObject.SetActive(false); 
                }

                // Add tooltip handler to the iconObject (ItemIcon)
                var hoverHandler = iconObject.AddComponent<SlotHoverHandler>();
                // Set itemName to tooltip
                hoverHandler.Init(tooltip, itemName, itemWeight, itemPrice); 
            }
            else
            {
                iconObject.SetActive(false); // Empty slot
            }

            slots.Add(slot);
        }
    }
}
