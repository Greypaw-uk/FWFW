using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public GameObject tooltipObject;
    public TextMeshProUGUI tooltipText;

    // Tool tip location
    public RectTransform canvasRectTransform;
    public Vector2 padding = new Vector2(10f, 10f); // Space between mouse and tooltip

    private RectTransform tooltipRectTransform;

    void Awake()
    {
        tooltipRectTransform = tooltipObject.GetComponent<RectTransform>();

        if (canvasRectTransform == null)
        {
            // Try to find parent canvas automatically
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
    }

    public void Show(string content)
    {
        tooltipText.text = content;
        tooltipObject.SetActive(true);
        UpdatePosition();
    }

    public void Hide()
    {
        tooltipObject.SetActive(false);
    }

    void Update()
    {
        if (tooltipObject.activeSelf)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 anchoredPos;

        // Start with tooltip to the right of the mouse
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePos, null, out anchoredPos);
        anchoredPos += new Vector2(padding.x, -padding.y);

        // Get tooltip size
        Vector2 tooltipSize = tooltipRectTransform.sizeDelta;
        Vector2 canvasSize = canvasRectTransform.sizeDelta;

        // Convert mouse position to canvas local point
        Vector2 canvasMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePos, null, out canvasMousePos);

        // Check if tooltip would go off the right edge
        if (canvasMousePos.x + tooltipSize.x + padding.x > canvasSize.x / 2f)
        {
            // Shift to the left
            anchoredPos.x -= tooltipSize.x + 2 * padding.x;
        }

        tooltipRectTransform.anchoredPosition = anchoredPos;
    }
}
