////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - BulletImpact.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BulletImpact : MonoBehaviour 
{
    
    private Health parentHealth;

    void Start () 
    {
        parentHealth = GetComponentInParent<Health>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            var bullet = other.gameObject.GetComponent<Bullet>();

            // Don't kill yourself!
            if(bullet.Owner == transform.parent.GetComponent<NetworkedPlayer>().PlayerID)
            {
                return;
            }

            parentHealth.InflictDamage(bullet.Damage);
            other.gameObject.GetComponent<Bullet>().DestroyOnImpact();

            if(bullet.Owner == "Player")
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if(player)
                {
                    player.GetComponent<PlayerScore>().AddScore(1.0f);
                }
            }
        }
    }
}
