////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerMovement.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
    private float m_forwardSpeed = 80.0f;
    private float m_rotationSpeed = 100.0f;
    private Vector3 m_forwardForce = new Vector3();
    private Rigidbody m_rigidBody = null;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Updates the player movement
    /// </summary>
    void FixedUpdate() 
    {
        if(NetworkedPlayer.IsControllable(gameObject))
        {
            if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow))
            {
                m_forwardForce.x = transform.up.x * m_forwardSpeed;
                m_forwardForce.z = transform.up.z * m_forwardSpeed;
                m_rigidBody.AddForce(m_forwardForce, ForceMode.Impulse);
            }

            if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow))
            {
                m_rigidBody.AddTorque(0.0f, -m_rotationSpeed, 0.0f, ForceMode.Impulse);
            }

            if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow))
            {
                m_rigidBody.AddTorque(0.0f, m_rotationSpeed, -0.0f, ForceMode.Impulse);
            }
        }
    }

    /// <summary>
    /// On collision with another player
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (PlayerManager.IsPlayer(other.gameObject))
        {
            if(PlayerManager.IsCloseToPlayer(other.transform.position, 30.0f))
            {
                SoundManager.Get().PlaySound(SoundManager.SoundID.RAM);
            }
        }
    }
}
