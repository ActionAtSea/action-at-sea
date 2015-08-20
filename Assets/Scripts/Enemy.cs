////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Enemy.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable, IKillable
{
    public float health = 100.0f;
    private float healthDeathValue = 0.0f;
    
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
