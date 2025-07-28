using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    /// <summary>
    /// Used to ensure components are not destroyed between scenes
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
