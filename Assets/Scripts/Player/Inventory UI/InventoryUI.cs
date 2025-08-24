using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;

public class InventoryUI : NetworkBehaviour
{
    [SerializeField] int inventorySize = 10;

    [SerializeField] public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject slotPrefab;

    private Inventory playerInventory;

    private List<GameObject> slots = new();

    public Tooltip tooltip;
    public ContextMenu contextMenu;


    /// <summary>
    /// Assigns inventory to player if they are active
    /// </summary>
    public override void OnNetworkSpawn()
    {
        inventoryPanel.SetActive(false);

        if (!IsOwner)
            enabled = false;
    }


    void Update()
    {
        // Display player's inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
            ToggleTooltip();
        }

        // Control right-click context menu
        if (Input.GetMouseButtonDown(0) && contextMenu.IsVisible && !contextMenu.IsPointerOverMenu())
            contextMenu.Hide();
    }


    #region Inventory Management

    public void SetInventory(Inventory inventory)
    {
        playerInventory = inventory;
    }


    /// <summary>
    /// Display player's inventory if tab is pressed
    /// </summary>
    void ToggleInventory()
    {
        bool isActive = inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!isActive);

        if (!isActive)
            RefreshInventory();
    }


    /// <summary>
    /// Refresh inventory display based on itemsList
    /// Displays inventory slots defined by inventorySize
    /// For each item in itemsList, shows item's icon in inventory grid and adds tooltip handler
    /// </summary>
    public void RefreshInventory()
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

        for (int i = 0; i < inventorySize; i++)
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

                // Add right-click context menu
                var rightClickHandler = iconObject.AddComponent<SlotRightClickHandler>();
                rightClickHandler.Init(contextMenu, playerInventory, itemsList[i]);
            }
            else
            {
                iconObject.SetActive(false); // Empty slot
            }

            slots.Add(slot);
        }
    }

    #endregion


    #region Tooltips and Context Menus

    /// <summary>
    /// Display descriptive tooltip when hovering over item icon
    /// </summary>
    void ToggleTooltip()
    {
        if (tooltip.tooltipObject.activeSelf)
            tooltip.Hide();
        else
        {
            tooltip.Hide();
            RefreshInventory();
        }
    }


    void ToggleContextMenu()
    {
        // Future feature: Right-click context menu for item actions (use, drop, inspect)
    }
    
    #endregion
}
