using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]

public class BulletImpact : MonoBehaviour {


    private Health parentHealth;
	// Use this for initialization
	void Start () 
    {
        parentHealth = GetComponentInParent<Health>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
		{
			var bullet = other.gameObject.GetComponent<Bullet>();

			if(GameInformation.IsPVP())
			{
				// Don't kill yourself!s
				if(bullet.owner == transform.parent.GetComponent<NetworkedPlayer>().PlayerID)
				{
					return;
				}
			}

			parentHealth.InflictDamage(bullet.Damage);
			other.gameObject.GetComponent<BulletDestoryScript>().DestroyOnImpact();

			if(GameInformation.IsPVP() && bullet.owner == "Player")
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
