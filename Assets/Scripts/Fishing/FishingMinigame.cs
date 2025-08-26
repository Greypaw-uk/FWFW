using UnityEngine;

public class FishingMinigame : MonoBehaviour, IFishingMinigame
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

    public void StartMinigame()
    {
        fishingProgress = 0f;
        minigameActive = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        minigamePanel.SetActive(minigameActive);

        if (!minigameActive) return;

        // --- Hook follows mouse ---
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minigamePanel.GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out localMousePos
        );

        Vector2 hookPos = hook.anchoredPosition;
        RectTransform panelRect = minigamePanel.GetComponent<RectTransform>();
        hookPos.y = Mathf.Clamp(localMousePos.y,
            -(panelRect.rect.height - hook.rect.height) / 2,
            (panelRect.rect.height - hook.rect.height) / 2);
        hook.anchoredPosition = hookPos;

        // --- Target box bounce ---
        float yOffset = Mathf.Sin(Time.time * boxMoveSpeed) * boxMoveRange;
        var boxPos = targetBox.anchoredPosition;
        boxPos.y = yOffset;
        targetBox.anchoredPosition = boxPos;

        // --- Progress check ---
        if (RectTransformUtility.RectangleContainsScreenPoint(targetBox, hook.position))
            fishingProgress += Time.deltaTime;
        else
            fishingProgress -= Time.deltaTime * 2;

        fishingProgress = Mathf.Clamp(fishingProgress, 0f, requiredFishingProgress);

        if (fishingProgress >= requiredFishingProgress)
        {
            minigameActive = false;
            OnCatchSuccess?.Invoke();
            gameObject.SetActive(false);
        }
    }

    public void CancelMinigame()
    {
        minigameActive = false;
        OnCatchFailed?.Invoke();
        gameObject.SetActive(false);
    }
}
