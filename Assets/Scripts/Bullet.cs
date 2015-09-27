////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Bullet.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
    private int m_owner = -1;               // Player that shot the bullet
    private float m_damage = 10.0f;         // Percentage of health removed when damage is inflicted.
    private Rigidbody body = null;

    /// <summary>
    /// Initialises the bullet
    /// </summary>
    void Start () 
    {
        body = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Adds movement to the bullet
    /// </summary>
    void Update() 
    {
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
    public int Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
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

            ResetBulletVelocity();
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

            ResetBulletVelocity();
        }
    }

    /* Resets the velocity and forces associated with the bullet's rigidbody.*/
    private void ResetBulletVelocity()
    {
        if (body != null)
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
        else
        {
            Debug.LogError("Bullet's rigidbody not found.");
        }
    }
}
