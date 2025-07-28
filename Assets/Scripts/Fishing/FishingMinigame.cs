using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private RectTransform hook;
    [SerializeField] private RectTransform targetBox;
    [SerializeField] float requiredFishingProgress = 5f;
    [SerializeField] float boxMoveSpeed = 2f;
    [SerializeField] float boxMoveRange = 100f;

    private float fishingProgress = 0f;
    private bool minigameActive = false;

    public System.Action OnCatchSuccess;
    public System.Action OnCatchFailed;

    public void StartMinigame()
    {
        fishingProgress = 0f;
        minigameActive = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        minigamePanel.SetActive(minigameActive);

        if (!minigameActive)
            return;

        // Move hook with mouse position
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minigamePanel.GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out localMousePos
        );

        // Update only Y axis, keep hook's X unchanged
        Vector2 hookPos = hook.anchoredPosition;
        RectTransform panelRect = minigamePanel.GetComponent<RectTransform>();

        // Set hook position to mouse position. Clamp within confines of panel's height, minus the height of the hook so that it doesn't poke out the box
        hookPos.y = Mathf.Clamp(localMousePos.y, -(panelRect.rect.height - hook.rect.height) / 2, (panelRect.rect.height - hook.rect.height) / 2);
        hook.anchoredPosition = hookPos;

        // Move target box automatically (bouncing)
        float yOffset = Mathf.Sin(Time.time * boxMoveSpeed) * boxMoveRange;
        var boxPos = targetBox.anchoredPosition;
        boxPos.y = yOffset;
        targetBox.anchoredPosition = boxPos;

        // Check overlap
        if (RectTransformUtility.RectangleContainsScreenPoint(targetBox, hook.position))
        {
            // Increase fishing progress if there is overlap
            fishingProgress += Time.deltaTime;
        }
        else
        {
            // Reduce fishing progress if no overlap
            fishingProgress -= Time.deltaTime * 2;
        }

        fishingProgress = Mathf.Clamp(fishingProgress, 0f, requiredFishingProgress);

        Debug.Log($"{fishingProgress}/{requiredFishingProgress}");

        // Victory
        if (fishingProgress >= requiredFishingProgress)
        {
            Debug.LogWarning("Fishing successful");

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