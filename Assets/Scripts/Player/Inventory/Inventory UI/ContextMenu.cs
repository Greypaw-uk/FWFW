using UnityEngine;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button deleteButton;
    [SerializeField] private InventoryUI inventoryUI; // 🔥 assign in Inspector

    private Inventory currentInventory;
    private Items.Item currentItem;

    private void Awake()
    {
        Hide();
        deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    public void Show(Vector3 position, Inventory inventory, Items.Item item)
    {
        currentInventory = inventory;
        currentItem = item;

        panel.SetActive(true);
        panel.transform.position = position;
    }

    public void Hide()
    {
        panel.SetActive(false);
        currentInventory = null;
        currentItem = null;
    }

    private void OnDeleteClicked()
    {
        Debug.Log($"[ContextMenu] Deleting item: {currentItem?.Name}");

        if (currentInventory != null && currentItem != null)
        {
            currentInventory.RemoveItem(currentItem);
            inventoryUI?.RefreshInventory(); // ✅ refresh here
        }

        Hide();
    }

    public bool IsVisible => panel.activeSelf;

    public bool IsPointerOverMenu()
    {
        if (!IsVisible) return false;
        if (UnityEngine.EventSystems.EventSystem.current == null) return false;

        var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            if (r.gameObject == null) continue;
            if (r.gameObject.transform.IsChildOf(panel.transform))
                return true;
        }
        return false;
    }
}
