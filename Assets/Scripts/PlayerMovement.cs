////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerMovement.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour 
{
    private float m_forwardSpeed = 8.0f;
    private float m_rotationSpeed = 1.0f;
    private Vector3 m_forwardForce = new Vector3();

    /// <summary>
    /// Updates the player movement
    /// </summary>
    void FixedUpdate() 
    {
        if(NetworkedPlayer.IsControllable(gameObject))
        {
            var rb = GetComponent<Rigidbody2D>();

            if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow))
            {
                m_forwardForce.x = transform.up.x * m_forwardSpeed;
                m_forwardForce.y = transform.up.y * m_forwardSpeed;
                rb.AddForce(m_forwardForce);
            }

            if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow))
            {
                rb.AddTorque(m_rotationSpeed);
            }

            if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow))
            {
                rb.AddTorque(-m_rotationSpeed);
            }
        }
    }

    /// <summary>
    /// On collision with another player
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "EnemyPlayer")
        {
            if(PlayerPlacer.IsCloseToPlayer(other.transform.position))
            {
                SoundManager.Get().PlaySound(SoundManager.SoundID.RAM);
            }
        }
    }
}
