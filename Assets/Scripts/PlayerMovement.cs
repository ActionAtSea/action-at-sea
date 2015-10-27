////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerMovement.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/// <summary>
/// NOTE: Instantiated by Photon Networking
/// Start() cannot include any code relying on the world/level as 
/// this object can be instantiated before the level is created
/// </summary>
public class PlayerMovement : MonoBehaviour 
{
    public float m_forwardSpeed = 30.0f; // 80, 40
    private float m_rotationSpeed = 165.0f;
    private Vector3 m_forwardForce = new Vector3();
    private Rigidbody m_rigidBody = null;
    private float m_currentVelocity = 0.0f;
    private float m_boostTimer = 0.0f;
    public float boostTimerTrigger = 1.0f;  // Length of time boat must travel at a certain speed to trigger a boost. 

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
        if(!Utilities.IsLevelLoaded())
        {
            return;
        }

        if(Utilities.IsPlayerControllable(gameObject))
        {
            if(Diagnostics.IsActive())
            {
                Diagnostics.Add("Player Speed", m_rigidBody.velocity.magnitude);
            }
            
            if(m_rigidBody.velocity.magnitude > 7.0f)
            {
                m_boostTimer += Time.deltaTime;
            }
            else
            {
                m_boostTimer = 0.0f;
            }

            if(m_boostTimer >= boostTimerTrigger)
            {
                m_forwardSpeed = Mathf.Lerp(30f, 40f, m_rigidBody.velocity.magnitude*1.0f/8.0f);
                m_rotationSpeed = 125f;
            }
            else
            {
                m_forwardSpeed = 30f;
                m_rotationSpeed = 165f;
            }

            if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow))
            {
                m_forwardForce.x = transform.up.x * m_forwardSpeed;
                m_forwardForce.z = transform.up.z * m_forwardSpeed;
                m_rigidBody.AddForce(m_forwardForce, ForceMode.Impulse);

                

                //m_forwardSpeed = Mathf.Lerp(30f, 40f, m_rigidBody.velocity.magnitude*1.0f/8.0f);


               

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
    /// Whether the player is moving
    /// </summary>
    public bool IsMoving()
    {
        return m_rigidBody.velocity.magnitude > 1.0f;
    }

    /// <summary>
    /// Whether the player is moving
    /// </summary>
    static public bool IsMoving(GameObject obj)
    {
        return obj.GetComponentInParent<PlayerMovement>().IsMoving();
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
