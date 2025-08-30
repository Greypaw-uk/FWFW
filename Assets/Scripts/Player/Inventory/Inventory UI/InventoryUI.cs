using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;

public class InventoryUI : NetworkBehaviour, IInventoryUI
{
    [SerializeField] public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject slotPrefab;

    private IInventory playerInventory;
    private readonly List<GameObject> slots = new();

    public Tooltip tooltip;
    public ContextMenu contextMenu;

    bool IInventoryUI.isActive => inventoryPanel.activeSelf;

    public override void OnNetworkSpawn()
    {
        inventoryPanel.SetActive(false);
    }


    void Update()
    {
        // Toggle Inventory panel on Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
            ToggleTooltip();
        }

        // Close Inventory panel on Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryPanel.activeSelf)
                ToggleInventory();
            if (tooltip.tooltipObject.activeSelf)
                tooltip.Hide();
            if (contextMenu.IsVisible)
                contextMenu.Hide();
        }

        // Hide context menu on left mouse click outside of it
        if (Input.GetMouseButtonDown(0) && contextMenu.IsVisible && !contextMenu.IsPointerOverMenu())
            contextMenu.Hide();
    }


    #region Inventory Management

    public void SetInventory(IInventory inventory)
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= RefreshInventory;

        playerInventory = inventory;
        playerInventory.OnInventoryChanged += RefreshInventory;
    }


    public void ToggleInventory()
    {
        bool isActive = inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!isActive);

        if (!isActive)
            RefreshInventory();
    }


    public void RefreshInventory()
    {
        foreach (var obj in slots)
            Destroy(obj);
        slots.Clear();

        if (playerInventory == null)
        {
            Debug.LogWarning("InventoryUI: No playerInventory assigned!");
            return;
        }

        List<Items.Item> itemsList = playerInventory.GetItems();

        for (int i = 0; i < playerInventory.GetMaxItemsCount(); i++)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);
            slot.SetActive(true);

            Transform iconTransform = slot.transform.Find("ItemIcon");
            if (iconTransform == null) continue;

            GameObject iconObject = iconTransform.gameObject;
            Image iconImage = iconObject.GetComponent<Image>();
            if (iconImage == null) continue;

            if (i < itemsList.Count)
            {
                Items.Item item = itemsList[i];

                Sprite itemSprite = playerInventory.GetSpriteForItem(item.Name);

                if (itemSprite != null)
                {
                    iconImage.sprite = itemSprite;
                    iconObject.SetActive(true);
                }
                else
                {
                    iconObject.SetActive(false);
                }

                var hoverHandler = iconObject.AddComponent<SlotHoverHandler>();
                hoverHandler.Init(tooltip, item.Name, item.Weight.ToString(), item.Price.ToString());

                var rightClickHandler = iconObject.AddComponent<SlotRightClickHandler>();
                rightClickHandler.Init(contextMenu, (Inventory)playerInventory, item);
            }
            else
            {
                iconObject.SetActive(false);
            }

            slots.Add(slot);
        }
    }

    #endregion


    #region Tooltips and Context Menus

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

    #endregion
}
