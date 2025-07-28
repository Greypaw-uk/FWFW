using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.IO.Compression;

public class AutoPatcher : MonoBehaviour
{
    [SerializeField] private string versionInfoUrl = "https://github.com/Greypaw-uk/FWFW/releases/download/release/version.txt";
    [SerializeField] private string latestReleaseURL = "https://github.com/Greypaw-uk/FWFW/releases/download/release/release.zip";

    private string versionFilePath => Path.Combine(Application.persistentDataPath, "version.txt");

    void Start()
    {
        StartCoroutine(CheckForUpdate());
    }


    /// <summary>
    /// Try and get the current version number from github release
    /// Compare version number against locally saved version number
    /// Start update if there is a mismatch
    /// </summary>
    IEnumerator CheckForUpdate()
    {
        string localVersion = ReadLocalVersion();

        using UnityWebRequest www = UnityWebRequest.Get(versionInfoUrl);
        yield return www.SendWebRequest();

        Debug.Log("Response code: " + www.responseCode);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch version info: " + www.error);
            yield break;
        }

        string remoteVersion = www.downloadHandler.text.Trim();
        Debug.Log($"Local version: {localVersion} | Remote version: {remoteVersion}");

        if (remoteVersion != localVersion)
        {
            Debug.Log("Update available. Downloading...");
            StartCoroutine(DownloadAndInstallUpdate(remoteVersion));
        }
        else
        {
            Debug.Log("Game is up to date.");
        }
    }


    /// <summary>
    /// Get updated .zip from github and extract files
    /// </summary>
    /// <param name="newVersion"></param>
    IEnumerator DownloadAndInstallUpdate(string newVersion)
    {
        string tempZipPath = Path.Combine(Application.persistentDataPath, "update.zip");

        using UnityWebRequest download = UnityWebRequest.Get(latestReleaseURL);
        download.downloadHandler = new DownloadHandlerFile(tempZipPath);
        yield return download.SendWebRequest();

        if (download.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Download failed: " + download.error);
            yield break;
        }

        Debug.Log("Download complete. Extracting...");

        string extractPath = Application.dataPath; // Adjust if needed

        try
        {
            ZipFile.ExtractToDirectory(tempZipPath, extractPath, true);
            Debug.Log("Update installed successfully.");

            File.WriteAllText(versionFilePath, newVersion);

            Application.Quit(); // Restart manually after update
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Extraction failed: " + ex.Message);
        }
        finally
        {
            if (File.Exists(tempZipPath))
                File.Delete(tempZipPath);
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