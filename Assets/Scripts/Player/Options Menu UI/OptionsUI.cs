using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class OptionsUI : NetworkBehaviour
{
    [SerializeField] public GameObject optionsPanel;
    public Button quitButton;
    private IPlayerFishing fishing;
    private IInventoryUI inventory;


    public override void OnNetworkSpawn()
    {
        optionsPanel.SetActive(false);

        if (!IsOwner)
            enabled = false;
    }


    void Awake()
    {
        quitButton.onClick.AddListener(QuitButtonClicked);

        fishing = GetComponent<IPlayerFishing>();
        inventory = GetComponent<InventoryUI>();
    }


    void Update()
    {
        // Open the options menu if Escape key is pressed
        if (!fishing.isFishing &&
            !inventory.isActive && 
            Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = optionsPanel.activeSelf;
            optionsPanel.SetActive(!isActive);
        }
    }


    /// <summary>
    /// Save player's game state prior to closing application
    /// </summary>
    private void QuitButtonClicked()
    {
        PlayerSaveData saveData = GetComponent<PlayerSaveData>();
        if (saveData != null)
            saveData.SavePlayerData();
        else
            Debug.LogWarning("PlayerSaveData component not found.");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
