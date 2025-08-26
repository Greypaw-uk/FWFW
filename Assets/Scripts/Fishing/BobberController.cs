using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BobberController : NetworkBehaviour, IBobberController
{
    public float arcHeight = 0.5f;
    //private GameObject bobberObject;

    public void Launch(Vector3 start, Vector3 end, float duration, IPlayerFishing owner)
    {
        //bobberObject = gameObject;
        StartCoroutine(ArcMove(start, end, duration));
    }

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

    public void Despawn()
    {
        var netObj = GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsSpawned && IsServer)
            netObj.Despawn(true);
        else
            Destroy(gameObject);
    }
}
