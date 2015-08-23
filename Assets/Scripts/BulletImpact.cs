////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - BulletImpact.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BulletImpact : MonoBehaviour 
{
    private Health m_parentHealth = null; // Health bar for the parent of this script

    /**
    * Initialises the script
    */
    void Start () 
    {
        m_parentHealth = GetComponentInParent<Health>();
    }

    /**
    * Collision detection between the bullet and player
    */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            var bullet = other.gameObject.GetComponent<Bullet>();

            Debug.Log("hit:" + NetworkedPlayer.GetPlayerID(gameObject));

            // Ensure the owner is not colliding with their own bullet
            if(bullet.Owner != NetworkedPlayer.GetPlayerID(gameObject))
            {
                m_parentHealth.InflictDamage(bullet.Damage);
                other.gameObject.GetComponent<Bullet>().DestroyOnImpact();

                if(bullet.Owner == "Player")
                {
                    var player = PlayerManager.GetControllablePlayer();
                    if(player)
                    {
                        player.GetComponent<PlayerScore>().AddScore(1.0f);
                    }
                }
            }
        }
    }
}
