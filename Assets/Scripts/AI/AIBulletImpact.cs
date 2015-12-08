using UnityEngine;
using System.Collections;

public class AIBulletImpact : BulletImpact {

	// Use this for initialization
	void Start () 
    {
        m_parentHealth = GetComponentInParent<AIHealth>();
	}
	
    /// <summary>
    /// Collision detection between the bullet and ai
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            var bullet = other.gameObject.GetComponent<Bullet>();
            
            //TODO: Ensure the owner is not colliding with their own bullet
            // also make sure that owned AIs can't be hurt by their owners
            // bullets.

            // Also store the owner of the last bullet that hits so the player to add score to is known.
            Debug.Log("AI player ID: " + Utilities.GetPlayerID(gameObject));
            if (bullet.Owner != Utilities.GetPlayerID(gameObject))
            {
                m_parentHealth.InflictDamage(bullet.Damage);
                other.gameObject.GetComponent<Bullet>().DestroyOnImpact();

                var player = PlayerManager.GetControllablePlayer();
                if (player != null && bullet.Owner == Utilities.GetPlayerID(player))
                {
                    player.GetComponent<PlayerScore>().AddScore(1.0f);
                }
            }
        }
    }
}
