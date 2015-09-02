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

    /// <summary>
    /// Initialises the bullet
    /// </summary>
    void Start () 
    {
        GetComponent<Rigidbody2D>().velocity = transform.forward * m_initialVelocity; 
    }

    /// <summary>
    /// Adds movement to the bullet
    /// </summary>
    void Update() 
    {
        GetComponent<Rigidbody2D>().AddForce(transform.forward * m_initialVelocity);
    }

    /// <summary>
    /// Returns the amount of damange this bullet does
    /// </summary>
    public float Damage
    {
        get { return m_damage; }
    }

    /// <summary>
    /// Get/Set the player that shot the bullet
    /// </summary>
    public string Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }

    /// <summary>
    /// Called on bullet enable
    /// </summary>
    void OnEnable()
    {
        Invoke("DestroyOnSplash", 2f);
    }

    /// <summary>
    /// Called on bullet disable
    /// </summary>
    void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// Destroys the bullet on impact
    /// </summary>
    public void DestroyOnImpact()
    {
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);

            AnimationGenerator.Get().PlayAnimation(
                gameObject.transform.position, AnimationGenerator.ID.HIT);
        }
    }

    /// <summary>
    /// Destroys the bullet on splash
    /// </summary>
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
