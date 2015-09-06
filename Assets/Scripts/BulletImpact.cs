////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - BulletImpact.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BulletImpact : MonoBehaviour 
{
    private Health m_parentHealth = null; // Health bar for the parent of this script

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start () 
    {
        m_parentHealth = GetComponentInParent<Health>();
    }

    /// <summary>
    /// Collision detection between the bullet and player
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            var bullet = other.gameObject.GetComponent<Bullet>();

            // Ensure the owner is not colliding with their own bullet
            if(bullet.Owner != NetworkedPlayer.GetPlayerID(gameObject))
            {
                m_parentHealth.InflictDamage(bullet.Damage);
                other.gameObject.GetComponent<Bullet>().DestroyOnImpact();

                var player = PlayerManager.GetControllablePlayer();
                if(player != null && bullet.Owner == NetworkedPlayer.GetPlayerID(player))
                {
                    player.GetComponent<PlayerScore>().AddScore(1.0f);
                }
            }
        }
    }
}
