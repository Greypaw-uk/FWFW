using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.IO.Compression;
using TMPro;
using UnityEngine.UI;

public class AutoPatcher : MonoBehaviour
{
    // File locations for version.txt and .zip for latest build
    [SerializeField] private string versionInfoUrl = "https://github.com/Greypaw-uk/FWFW/releases/download/release/version.txt";
    [SerializeField] private string latestReleaseURL = "https://github.com/Greypaw-uk/FWFW/releases/download/release/release.zip";

    [Header("UI")]
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] TMP_Text tmpVersion;

    [Header("Host/Join panel elements")]
    [SerializeField] public GameObject updateProcessPanel;
    [SerializeField] TMP_Text tmpUpdateProcess;


    private string localVersion;
    private string versionFilePath => Path.Combine(Application.persistentDataPath, "version.txt");


    void Start()
    {
        buttonsPanel.SetActive(false);

        localVersion = ReadLocalVersion();
        tmpVersion.text = localVersion;

        StartCoroutine(CheckForUpdate());
    }


    /// <summary>
    /// Try and get the current version number from github release
    /// Compare version number against locally saved version number
    /// Start update if there is a mismatch
    /// </summary>
    IEnumerator CheckForUpdate()
    {
        using UnityWebRequest www = UnityWebRequest.Get(versionInfoUrl);
        yield return www.SendWebRequest();

        Debug.Log("Response code: " + www.responseCode);

        if (www.result != UnityWebRequest.Result.Success)
        {
            SetUpdateProgressText("Failed to get latest version number: " + www.result.ToString());
            Debug.LogError("Could not retrieve version number from server");
            yield break;
        }

        string remoteVersion = www.downloadHandler.text.Trim();
        SetUpdateProgressText("Local version: {localVersion} | Remote version: {remoteVersion}");

        if (remoteVersion != localVersion)
            StartCoroutine(DownloadAndInstallUpdate(remoteVersion));
        else
        {
            SetUpdateProgressText("Game is up to date.");
            Debug.Log("Game is up to date");

            updateProcessPanel.SetActive(false);
            buttonsPanel.SetActive(true);
        }
    }


    /// <summary>
    /// Get updated .zip from github and extract files
    /// </summary>
    /// <param name="newVersion"></param>
    IEnumerator DownloadAndInstallUpdate(string newVersion)
    {
        bool updatedSuccessfully = false;

        SetUpdateProgressText("Update available. \n Downloading...");
        Debug.Log("Attempting to download update");

        // Ignore download attempt if within the Unity editor
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            SetUpdateProgressText("Auto-update skipped in Unity Editor.");
            Debug.Log("Unity editor detected, aborting download");
            yield break;
        }
#endif


        string tempZipPath = Path.Combine(Application.persistentDataPath, "update.zip");

        using UnityWebRequest download = UnityWebRequest.Get(latestReleaseURL);
        download.downloadHandler = new DownloadHandlerFile(tempZipPath);

        // Begin the request
        var operation = download.SendWebRequest();

        // Wait for download while updating UI with progress
        while (!operation.isDone)
        {
            float percent = download.downloadProgress * 100f;
            SetUpdateProgressText($"Downloading update... \n {percent:F0}%");
            yield return null;
        }

        // Check if download failed
        if (download.result != UnityWebRequest.Result.Success)
        {
            SetUpdateProgressText("Download failed. \n Check log for details");
            Debug.LogError($"Download failed: {download.error}");
            yield return new WaitForSeconds(5);
            yield break;
        }

        SetUpdateProgressText("Download complete. Extracting...");
        Debug.Log("Extracting update...");

        string extractPath = Application.dataPath;

        try
        {
            ZipFile.ExtractToDirectory(tempZipPath, extractPath, true);
            File.WriteAllText(versionFilePath, newVersion);
            updatedSuccessfully = true;
        }
        catch (System.Exception ex)
        {
            SetUpdateProgressText("Extraction failed. Check log for details");
            Debug.LogError($"Extraction failed: {ex.Message}");
        }
        finally
        {
            if (File.Exists(tempZipPath))
                File.Delete(tempZipPath);
        }

        if (updatedSuccessfully)
        {
            SetUpdateProgressText("Update successful. \n Game restarting.");
            Debug.Log("Update successful. Restarting...");

            yield return new WaitForSeconds(2);
            RestartGame();
        }
    }



    /// <summary>
    /// Read and return the local file's version number
    /// </summary>
    /// <returns> Local version number </returns>
    string ReadLocalVersion()
    {
        if (!File.Exists(versionFilePath))
        {
            Debug.Log("No local version file found. Defaulting to 0.0.000");
            return "0.0.000";
        }

        try
        {
            return File.ReadAllText(versionFilePath).Trim();
        }
        catch
        {
            Debug.Log("Failed to read version file. Defaulting to 0.0.000");
            return "0.0.000";
        }
    }


    /// <summary>
    /// Display patching progress on player's screen via tmpUpdateProcess 
    /// </summary>
    /// <param name="updateMessage"></param>
    void SetUpdateProgressText(string updateMessage)
    {
        tmpUpdateProcess.text = updateMessage;
    }


    /// <summary>
    /// Auto restart the game to apply updates
    /// </summary>
    public void RestartGame()
    {
        #if UNITY_EDITOR
            Debug.Log("Restart not supported in Editor.");
        #else
            string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            // Launch new instance
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true
            });

            // Force quit to avoid hang
            Application.Quit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        #endif
    }
}