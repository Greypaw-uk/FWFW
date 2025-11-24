using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class OptionsUI : NetworkBehaviour, IGameUIPanel
{
    [SerializeField] public GameObject optionsPanel;
    public Button quitButton;


    public bool IsOpen => optionsPanel.activeSelf;


    public override void OnNetworkSpawn()
    {
        optionsPanel.SetActive(false);

        if (!IsOwner)
            enabled = false;
    }


    void Awake()
    {
        quitButton.onClick.AddListener(QuitButtonClicked);
    }


    public void Open()
    {
        optionsPanel.SetActive(true);
        GlobalUIManager.Instance.RegisterOpenPanel(this);
    }


    public void Close()
    {
        optionsPanel.SetActive(false);
        GlobalUIManager.Instance.RegisterClosedPanel(this);
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
