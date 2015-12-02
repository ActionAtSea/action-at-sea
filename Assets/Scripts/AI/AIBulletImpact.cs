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
            
            // Ensure the owner is not colliding with their own bullet
            if(bullet.Owner != Utilities.GetPlayerID(gameObject))
            {
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
