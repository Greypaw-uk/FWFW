using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

[RequireComponent(typeof(Currency))]
public class InventoryUI : NetworkBehaviour, IInventoryUI, IGameUIPanel
{
    [Header("UI Elements")]
    [SerializeField] public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject slotPrefab;
    public TMP_Text currencyText;

    [Header("Extras")]
    public Tooltip tooltip;
    public ContextMenu contextMenu;

    private IInventory playerInventory;
    private Currency currency;
    private readonly List<GameObject> slots = new();

    public bool IsOpen => inventoryPanel.activeSelf;
    bool IInventoryUI.isActive => IsOpen;

    public override void OnNetworkSpawn()
    {
        inventoryPanel.SetActive(false);

        currency = GetComponent<Currency>();
        currency.OnMoneyChanged += UpdateCurrencyUI;
        UpdateCurrencyUI();
    }

    void Update()
    {
        if (!IsOwner) return;

        // Tab key opens/closes inventory if no other panel is open ---
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (GlobalUIManager.Instance.IsAnyPanelOpen)
            {
                Close();
                return;
            }
            if (!GlobalUIManager.Instance.IsAnyPanelOpen)
                Open();
        }
    }

    #region Inventory Management

    public void SetInventory(IInventory inventory)
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= RefreshInventory;

        playerInventory = inventory;
        playerInventory.OnInventoryChanged += RefreshInventory;
    }

    public void RefreshInventory()
    {
        foreach (var obj in slots)
            Destroy(obj);
        slots.Clear();

        if (playerInventory == null) return;

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

                iconImage.sprite = itemSprite ?? null;
                iconObject.SetActive(itemSprite != null);

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

    #region Currency

    private void UpdateCurrencyUI() => currencyText.text = $"¢: {currency.GetMoney}";

    #endregion

    #region IGameUIPanel Implementation

    public void Open()
    {
        inventoryPanel.SetActive(true);
        RefreshInventory();
        GlobalUIManager.Instance.RegisterOpenPanel(this);
    }

    public void Close()
    {
        inventoryPanel.SetActive(false);
        GlobalUIManager.Instance.RegisterClosedPanel(this);
    }

    public void ToggleInventory()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
