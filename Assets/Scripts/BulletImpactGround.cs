////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - BulletImpactGround.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class BulletImpactGround : MonoBehaviour 
{
    /// <summary>
    /// Collision detection between the bullet and ground
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            other.gameObject.GetComponent<Bullet>().DestroyOnSplash();
        }
    }
}
