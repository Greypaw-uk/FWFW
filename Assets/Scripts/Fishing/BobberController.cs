using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BobberController : NetworkBehaviour
{
    public float arcHeight = 0.5f;

    private Vector3 target;
    private float duration;

    public void Launch(Vector3 start, Vector3 end, float duration)
    {
        this.duration = duration;
        StartCoroutine(ArcMove(start, end, duration));
    }

    /// <summary>
    /// Move the bobber away from the player in an arc
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator ArcMove(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float ease = Mathf.SmoothStep(0, 1, t);
            Vector3 flat = Vector3.Lerp(start, end, ease);
            flat.y += Mathf.Sin(Mathf.PI * ease) * arcHeight;
            transform.position = flat;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }
}
