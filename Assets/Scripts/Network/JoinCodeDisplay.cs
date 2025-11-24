using UnityEngine;
using TMPro;

public class JoinCodeDisplay : MonoBehaviour
{
    public TextMeshProUGUI joinCodeText;
    //public Button copyButton;

    void Start()
    {
        string code = NetworkSessionInfo.JoinCode;

        if (!string.IsNullOrEmpty(code))
        {
            joinCodeText.text = $"Join Code: {code}";
            //copyButton.onClick.AddListener(() => GUIUtility.systemCopyBuffer = code);
        }
        else
        {
            joinCodeText.text = "";
        }
    }
}
