using UnityEngine;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager Instance { get; private set; }

    private IGameUIPanel activePanel;

    public bool IsAnyPanelOpen => activePanel != null && activePanel.IsOpen;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        // ESC closes the active panel
        if (activePanel != null && activePanel.IsOpen)
        {
            activePanel.Close();
        }
        else
        {
            var optionsPanel = FindFirstObjectByType<OptionsUI>();
            if (optionsPanel != null && optionsPanel.IsOwner)
                optionsPanel.Open();
        }
    }


    /// <summary>
    /// Called by UI panels when they open.
    /// Ensures only one UI panel is open at a time.
    /// </summary>
    public void RegisterOpenPanel(IGameUIPanel panel)
    {
        if (activePanel != null && activePanel.IsOpen && activePanel != panel)
        {
            activePanel.Close();
        }

        activePanel = panel;
    }


    /// <summary>
    /// Called by UI panels when they close.
    /// </summary>
    public void RegisterClosedPanel(IGameUIPanel panel)
    {
        if (activePanel == panel)
            activePanel = null;
    }


    /// <summary>
    /// Returns the currently active panel, or null if none are open.
    /// </summary>
    /// <returns></returns>
    public IGameUIPanel GetActivePanel()
    {
        return activePanel;
    }
}
