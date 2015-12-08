using UnityEngine;
using System.Collections;

public class PlayerBulletImpact : BulletImpact 
{
	// Use this for initialization
	void Start () 
    {
        m_parentHealth = GetComponentInParent<PlayerHealth>();
	}

    /// <summary>
    /// Collision detection between the bullet and player
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            var bullet = other.gameObject.GetComponent<Bullet>();
            Debug.Log("Collision Detected 1");
            // Ensure the owner is not colliding with their own bullet
            Debug.Log("Controlled player ID: " + Utilities.GetPlayerID(gameObject));
            if(bullet.Owner != Utilities.GetPlayerID(gameObject))
            {
                Debug.Log("Collision Detected 2");
                m_parentHealth.InflictDamage(bullet.Damage);
                other.gameObject.GetComponent<Bullet>().DestroyOnImpact();
                
                var player = PlayerManager.GetControllablePlayer();
                if(player != null && bullet.Owner == Utilities.GetPlayerID(player))
                {
                    player.GetComponent<PlayerScore>().AddScore(1.0f);
                }
            }
        }
    }
}
