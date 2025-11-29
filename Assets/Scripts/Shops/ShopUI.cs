using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour, IGameUIPanel
{
    [Header("UI Elements")]
    [SerializeField] GameObject shopPanel;
    [SerializeField] Transform gridParent;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Tooltip tooltip;
    [SerializeField] TMP_Text currencyText;

    private IInventory playerInventoryRef;
    private ICurrency playerCurrencyRef;
    private IShop currentShop;              
    private readonly List<GameObject> slots = new();

    public bool IsOpen => shopPanel.activeSelf;


    /// <summary>
    /// Exists only to satisfy IGameUIPanel interface.
    /// </summary>
    public void Open()
    {
        var cur = GetComponent<ICurrency>();
        Debug.Log($"[ShopUI] Currency object: {cur.GetHashCode()}");


        shopPanel.SetActive(true);
        GlobalUIManager.Instance.RegisterOpenPanel(this);

        RefreshUI();
    }


    /// <summary>
    /// Initializes the shop UI with the given shop instance and player references.
    /// </summary>
    /// <param name="shop"></param>
    /// <param name="inv"></param>
    /// <param name="currency"></param>
    public void Init(IShop shop, IInventory inv, ICurrency currency)
    {
        currentShop = shop;
        playerInventoryRef = inv;
        playerCurrencyRef = currency;
    }


    /// <summary>
    /// Closes the shop UI.
    /// </summary>
    public void Close()
    {
        shopPanel.SetActive(false);
        GlobalUIManager.Instance.RegisterClosedPanel(this);
    }


    /// <summary>
    /// Refreshes the shop UI to reflect current inventory and currency.
    /// </summary>
    public void RefreshUI()
    {
        // clear old slots
        foreach (var s in slots) Destroy(s);
        slots.Clear();

        // Use the player's inventory reference (IInventory) to build slots
        var items = playerInventoryRef.GetItems();
        int maxSlots = playerInventoryRef.GetMaxItemsCount();

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);
            slot.SetActive(true);

            Transform iconTransform = slot.transform.Find("ItemIcon");
            if (iconTransform == null) { slots.Add(slot); continue; }

            GameObject iconObject = iconTransform.gameObject;
            Image iconImage = iconObject.GetComponent<Image>();

            if (i < items.Count)
            {
                Items.Item item = items[i];
                Sprite itemSprite = playerInventoryRef.GetSpriteForItem(item.Name);
                iconImage.sprite = itemSprite;
                iconObject.SetActive(itemSprite != null);

                // add hover tooltip (your existing code)
                var hoverHandler = iconObject.AddComponent<SlotHoverHandler>();
                hoverHandler.Init(tooltip, item.Name, item.Weight.ToString(), item.Price.ToString());

                // add the click handler and pass the CURRENT shop instance
                var clickHandler = iconObject.AddComponent<ShopItemClickHandler>();
                clickHandler.Init(item, currentShop, this);
            }
            else
            {
                iconObject.SetActive(false);
            }

            slots.Add(slot);
        }

        UpdateCurrency();
    }


    /// <summary>
    /// Updates the displayed currency amount.
    /// </summary>
    private void UpdateCurrency()
    {
        Debug.Log("ShopUI: Updating currency");

        if (playerCurrencyRef == null)
        {
            Debug.LogWarning("ShopUI: playerCurrencyRef is null!");
            return;
        }
        else if (currencyText == null)
        {
            Debug.LogWarning("ShopUI: currencyText is null!");
            return;
        }
        else
            currencyText.text = playerCurrencyRef.GetMoney.ToString();
    }
}
