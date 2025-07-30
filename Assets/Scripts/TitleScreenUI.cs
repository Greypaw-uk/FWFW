using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using TMPro;

public class TitleScreenUI : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public GameObject buttonsPanel;
    public Button hostButton;
    public Button joinButton;

    [Header("Join Prompt UI")]
    public GameObject joinPromptPanel;
    public TMP_InputField joinCodeInputField;
    public Button acceptButton;
    public Button cancelButton;

    public int maxConnections = 4;

    void Awake()
    {
        joinCodeInputField.text = "";
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        joinPromptPanel.SetActive(false);

        hostButton.onClick.AddListener(async () => await OnHostClicked());
        joinButton.onClick.AddListener(OnJoinClickedPrompt);

        // Attempt to join if player presses Enter in the input field
        joinCodeInputField.onEndEdit.AddListener(async input =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                await OnJoinAccepted();
            }
        });

        // Attempt to join if the player clicks the Accept Button
        acceptButton.onClick.AddListener(async () => await OnJoinAccepted());

        // Return to the title screen if player clicks Cancel Button
        cancelButton.onClick.AddListener(OnJoinCancelled);
    }

    void Update()
    {
        joinCodeInputField.text = joinCodeInputField.text.ToUpper();
    }

    public async Task OnHostClicked()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        NetworkSessionInfo.JoinCode = joinCode;
        Debug.Log($"Join code: {joinCode}");

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        // Start the host and spawn host's player
        bool success = NetworkManager.Singleton.StartHost();
        if (success)
            NetworkManager.Singleton.SceneManager.LoadScene("MainGameScene", LoadSceneMode.Single);
        else
            Debug.LogError("Failed to start host.");
    }


    void OnJoinClickedPrompt()
    {
        joinPromptPanel.SetActive(true);
        joinCodeInputField.Select();

        buttonsPanel.SetActive(false);
    }

    public async Task OnJoinAccepted()
    {
        string joinCode = joinCodeInputField.text.Trim();

        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogWarning("Join code is empty.");
            return;
        }

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            joinPromptPanel.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Invalid join code: {e.Message}");
            // Optionally show an error message to the user
        }
    }

    void OnJoinCancelled()
    {
        joinPromptPanel.SetActive(false);
        joinCodeInputField.text = "";

        buttonsPanel.SetActive(true);
    }
}
