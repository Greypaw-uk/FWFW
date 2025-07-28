using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class FishingRodController : NetworkBehaviour
{
    public float flickAngle = 60f;
    public float flickDuration = 1f;

    // Used to return rod prefab to original rotation after flick
    private Quaternion originalRotation;

    // Allows rod to follow its owner
    private Transform targetToFollow;

    public void Initialize(Transform followTarget)
    {
        targetToFollow = followTarget;
    }

    private void Awake()
    {
        originalRotation = transform.localRotation;
    }

        private void Update()
    {
        if (targetToFollow != null)
        {
            transform.position = targetToFollow.position;
        }
    }

    public void Flick()
    {
        StopAllCoroutines();
        StartCoroutine(FlickRoutine());
    }

    /// <summary>
    /// Rotate the rod back X degrees
    /// Wait for duration
    /// Return rod to originalRotation (default position)
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlickRoutine()
    {
        Quaternion flickRotation = originalRotation * Quaternion.Euler(0, 0, flickAngle);
        float elapsed = 0f;

        while (elapsed < flickDuration)
        {
            float t = elapsed / flickDuration;
            transform.localRotation = Quaternion.Slerp(originalRotation, flickRotation, Mathf.Sin(t * Mathf.PI));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = originalRotation;
    }
}
