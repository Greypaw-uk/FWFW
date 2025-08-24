using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class PlayerSaveData : NetworkBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public Vector3 position;
        public List<Items.Item> inventoryItems;
    }

    private Inventory inventory;

    public float saveInterval = 30f;
    private string savePath;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; // Only the owner loads their own data

        inventory = GetComponent<Inventory>();
        savePath = Path.Combine(Application.persistentDataPath, $"playerSave.json");

        LoadPlayerData();
        StartCoroutine(AutoSaveRoutine());
    }


    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(saveInterval);
            SavePlayerData();
        }
    }


    public void SavePlayerData()
    {
        if (inventory == null)
        {
            Debug.LogWarning("[PlayerSaveData] No Inventory found to save.");
            return;
        }

        SaveData data = new SaveData
        {
            position = transform.position,
            inventoryItems = inventory.inventoryItems
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Saved player data to {savePath}");
    }


    public void LoadPlayerData()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log($"[PlayerSaveData] No save file found for player {OwnerClientId} at {savePath}");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        // Apply saved inventory
        if (TryGetComponent(out Inventory inventory))
            inventory.inventoryItems = saveData.inventoryItems;
        else
            Debug.LogWarning("[PlayerSaveData] Inventory component not found.");

        // Apply saved position
        transform.position = saveData.position;
        RequestServerToSetPositionServerRpc(saveData.position);
    }
    

    /// <summary>
    /// Update server with client's transform after it has been loaded from file
    /// </summary>
    /// <param name="position"></param>
     [ServerRpc]
    private void RequestServerToSetPositionServerRpc(Vector3 position)
    {
        transform.position = position;
    }
}
