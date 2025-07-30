using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.IO.Compression;
using TMPro;

public class AutoPatcher : MonoBehaviour
{
    // File locations for version and .zip
    [SerializeField] private string versionInfoUrl = "https://github.com/Greypaw-uk/FWFW/releases/download/release/version.txt";
    [SerializeField] private string latestReleaseURL = "https://github.com/Greypaw-uk/FWFW/releases/download/release/release.zip";

    // Panel holding host/join buttons
    [SerializeField] public GameObject buttonsPanel;
    
    // Update status UI elements
    [SerializeField] public GameObject updateProcessPanel;
    [SerializeField] TMP_Text tmpUpdateProcess;

    // Version number
    [SerializeField] TMP_Text tmpVersion;
    private string localVersion;
    private string versionFilePath => Path.Combine(Application.persistentDataPath, "version.txt");

    void Start()
    {
        updateProcessPanel.SetActive(true);
        buttonsPanel.SetActive(false);

        localVersion = ReadLocalVersion();
        tmpVersion.text = $"Alpha: {localVersion}";

        StartCoroutine(CheckForUpdate());

        updateProcessPanel.SetActive(false);
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
            tmpUpdateProcess.text = "Failed to get version number";
            yield break;
        }

        string remoteVersion = www.downloadHandler.text.Trim();
        tmpUpdateProcess.text = $"Local version: {localVersion} | Remote version: {remoteVersion}";

        if (remoteVersion != localVersion)
            StartCoroutine(DownloadAndInstallUpdate(remoteVersion));
        else
        {
            tmpUpdateProcess.text = "Game is up to date.";

            buttonsPanel.SetActive(true);
            updateProcessPanel.SetActive(false);
        }
    }


    /// <summary>
    /// Get updated .zip from github and extract files
    /// </summary>
    /// <param name="newVersion"></param>
    IEnumerator DownloadAndInstallUpdate(string newVersion)
    {
        Debug.Log("Update available. Downloading...");

        // Do not remove - may attempt to overwrite files when launching from Unity instead of from a build.
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            tmpUpdateProcess.text = "Auto-update skipped in Unity Editor.";
            yield break;
        }
#endif

        string tempZipPath = Path.Combine(Application.persistentDataPath, "update.zip");

        using UnityWebRequest download = UnityWebRequest.Get(latestReleaseURL);
        download.downloadHandler = new DownloadHandlerFile(tempZipPath);
        yield return download.SendWebRequest();

        if (download.result != UnityWebRequest.Result.Success)
        {
            tmpUpdateProcess.text = "Download failed: " + download.error;
            yield break;
        }

        tmpUpdateProcess.text = "Download complete. Extracting...";

        string extractPath = Application.dataPath; // Adjust if needed

        try
        {
            ZipFile.ExtractToDirectory(tempZipPath, extractPath, true);
            tmpUpdateProcess.text = "Update installed successfully.";

            File.WriteAllText(versionFilePath, newVersion);

            Application.Quit(); // Restart manually after update
        }
        catch (System.Exception ex)
        {
            tmpUpdateProcess.text = "Extraction failed: " + ex.Message;
        }
        finally
        {
            if (File.Exists(tempZipPath))
            {
                File.Delete(tempZipPath);
            }

            buttonsPanel.SetActive(true);
            updateProcessPanel.SetActive(false);
            tmpVersion.text = $"Alpha: {localVersion}";
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
            Debug.Log("No local version file found. Assuming 0.0.000");
            return "0.0.000";
        }

        try
        {
            return File.ReadAllText(versionFilePath).Trim();
        }
        catch
        {
            Debug.LogWarning("Failed to read version file. Defaulting to 0.0.000");
            return "0.0.000";
        }
    }
}