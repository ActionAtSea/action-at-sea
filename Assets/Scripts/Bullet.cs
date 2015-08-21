////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Bullet.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour 
{
    private float m_initialVelocity = 30.0f;
    private string m_owner = "";            // Player that shot the bullet
    private float m_damage = 10.0f;         // Percentage of health removed when damage is inflicted.

    /**
    * Initialises the bullet
    */
    void Start () 
    {
        GetComponent<Rigidbody2D>().velocity = transform.forward * m_initialVelocity; 
    }

    /**
    * Adds movement to the bullet
    */
    void Update() 
    {
        GetComponent<Rigidbody2D>().AddForce(transform.forward * m_initialVelocity);
    }

    /**
    * Returns the amount of damange this bullet does
    */
    public float Damage
    {
        get { return m_damage; }
    }

    /**
    * Get/Set the player that shot the bullet
    */
    public string Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }

    /**
    * Called on bullet enable
    */
    void OnEnable()
    {
        Invoke("DestroyOnSplash", 2f);
    }

    /**
    * Called on bullet disable
    */
    void OnDisable()
    {
        CancelInvoke();
    }

    /**
    * Destroys the bullet on impact
    */
    public void DestroyOnImpact()
    {
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);

            AnimationGenerator.Get().PlayAnimation(
                gameObject.transform.position, AnimationGenerator.ID.HIT);
        }
    }

    /**
    * Destroys the bullet on splash
    */
    public void DestroyOnSplash()
    {
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);

            AnimationGenerator.Get().PlayAnimation(
                gameObject.transform.position, AnimationGenerator.ID.SPLASH);
        }
    }
}
