using UnityEngine;

public class FishingMinigame : MonoBehaviour, IFishingMinigame, IGameUIPanel
{
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private RectTransform hook;
    [SerializeField] private RectTransform targetBox;
    [SerializeField] float requiredFishingProgress = 5f;
    [SerializeField] float boxMoveSpeed = 2f;
    [SerializeField] float boxMoveRange = 100f;

    private float fishingProgress = 0f;
    private bool minigameActive = false;

    public event System.Action OnCatchSuccess;
    public event System.Action OnCatchFailed;

    public bool IsOpen => minigamePanel != null && minigamePanel.activeSelf;

    public void Open()
    {
        if (minigamePanel != null)
        {
            minigamePanel.SetActive(true);
            minigameActive = true;
        }
    }

    public void Close()
    {
        if (minigamePanel != null)
        {
            minigamePanel.SetActive(false);
            minigameActive = false;
        }
    }

    // -----------------------------
    // IFishingMinigame
    // -----------------------------
    public void StartMinigame()
    {
        fishingProgress = 0f;

        // Use GlobalUIManager to open this panel (it will call Open() on this)
        GlobalUIManager.Instance.RegisterOpenPanel(this);
        Open();
    }

    public void CancelMinigame()
    {
        EndMinigame(success: false);
    }

    private void EndMinigame(bool success)
    {
        minigameActive = false;

        if (success)
            OnCatchSuccess?.Invoke();
        else
            OnCatchFailed?.Invoke();

        // Notify GlobalUIManager that this panel closed
        GlobalUIManager.Instance.RegisterClosedPanel(this);

        Close();
    }

    private void Update()
    {
        if (!minigameActive) return;

        // --- Hook follows mouse ---
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minigamePanel.GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out Vector2 localMousePos))
        {
            Vector2 hookPos = hook.anchoredPosition;
            RectTransform panelRect = minigamePanel.GetComponent<RectTransform>();

            hookPos.y = Mathf.Clamp(localMousePos.y,
                -(panelRect.rect.height - hook.rect.height) / 2,
                (panelRect.rect.height - hook.rect.height) / 2);

            hook.anchoredPosition = hookPos;
        }

        // --- Target box bounce ---
        float yOffset = Mathf.Sin(Time.time * boxMoveSpeed) * boxMoveRange;
        Vector2 boxPos = targetBox.anchoredPosition;
        boxPos.y = yOffset;
        targetBox.anchoredPosition = boxPos;

        // --- Progress check ---
        if (RectTransformUtility.RectangleContainsScreenPoint(targetBox, hook.position))
            fishingProgress += Time.deltaTime;
        else
            fishingProgress -= Time.deltaTime * 2f;

        fishingProgress = Mathf.Clamp(fishingProgress, 0f, requiredFishingProgress);

        if (fishingProgress >= requiredFishingProgress)
        {
            EndMinigame(success: true);
        }
    }
}
