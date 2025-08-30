using System.Collections.Generic;

public interface IInventoryUI
{
    void SetInventory(IInventory inventory);
    void RefreshInventory();
    void ToggleInventory();
    bool isActive { get; }
}