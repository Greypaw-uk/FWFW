using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour, IGameUIPanel
{
    public GameObject shopPanel;
    public Transform gridParent;
    public GameObject slotPrefab;
    public TMP_Text currencyText;

    private IInventory inventory;
    private ICurrency currency;
    private IShop shopLogic;
    private readonly List<GameObject> slots = new();
    private bool isPrepared = false;

    public bool IsOpen => shopPanel.activeSelf;

    public void Init(IInventory inv, ICurrency cur, IShop shop)
    {
        inventory = inv;
        currency = cur;
        shopLogic = shop;

        if (!isPrepared)
        {
            currency.OnMoneyChanged += UpdateCurrency;
            isPrepared = true;
        }

        Open();
    }

    public void Open()
    {
        shopPanel.SetActive(true);
        GlobalUIManager.Instance.RegisterOpenPanel(this);

        RefreshUI();
    }

    public void Close()
    {
        shopPanel.SetActive(false);
        GlobalUIManager.Instance.RegisterClosedPanel(this);
    }


    public void RefreshUI()
    {
        foreach (var slot in slots)
            Destroy(slot);
        slots.Clear();

        foreach (var item in inventory.GetItems())
        {
            var slot = Instantiate(slotPrefab, gridParent);
            var handler = slot.AddComponent<ShopItemClickHandler>();
            handler.Init(item, shopLogic, this);
            slots.Add(slot);
        }

        UpdateCurrency();
    }

    private void UpdateCurrency()
    {
        if (currencyText != null)
            currencyText.text = currency.GetMoney.ToString();
    }
}
