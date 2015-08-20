////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Bullet.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour 
{
    public float initialVelocity = 15.0f;
    public string owner = "";
    private float damage = 10.0f;    //Percentage of health removed when damage is inflicted.
	Rigidbody2D body2D = null;
	
    void Start () 
    {
        body2D = GetComponent<Rigidbody2D>(); 
        body2D.velocity = transform.forward*initialVelocity; 
    }
    
    void Update() 
    {
        body2D.AddForce(transform.forward*initialVelocity);
    }

    public float Damage
    {
        get { return damage; }
    }
}
