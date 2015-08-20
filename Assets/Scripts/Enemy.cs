using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable, IKillable
{

    #region variables
    public float health = 100.0f;
    private float healthDeathValue = 0.0f;

    private Rigidbody2D body2D;
    private Collider2D triggeredCollider;
    #endregion

    // Use this for initialization
	void Start () 
    {
	    body2D = GetComponent<Rigidbody2D>();
        triggeredCollider = GetComponentInChildren<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            InflictDamage(10.0f);
            Debug.Log(health);
        }
    }

    public void InflictDamage(float damage)
    {
        health -= damage;
        Debug.Log(damage);
    }

    public void RepairDamage(float repairAmount)
    {
        health += repairAmount;
    }

    public void Death()
    {
       
    }

    void CheckAlive()
    {
        if (health <= healthDeathValue)
        {
            Death();
        }
    }
}
