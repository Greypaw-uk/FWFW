using System.Collections;
using UnityEngine;

public class BobberWaterCollider : MonoBehaviour
{
    public PlayerFishing playerFishing;

    /// <summary>
    /// Check collision between bobber and water tilemap. 
    /// If collides, call PlayerFishing.cs method OnBobberLandedInWater()
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            playerFishing?.NotifyBobberInWater();
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
