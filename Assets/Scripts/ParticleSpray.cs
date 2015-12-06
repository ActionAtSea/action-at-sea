////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ParticleSpray.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/// <summary>
/// NOTE: Instantiated by Photon Networking
/// Start() cannot include any code relying on the world/level as 
/// this object can be instantiated before the level is created
/// </summary>
public class ParticleSpray : MonoBehaviour 
{    
    public float minSpeedForEmission = 1.0f;
    private Rigidbody m_rigidBody = null;
    private ParticleSystem m_particles;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start () 
    {
        m_particles = GetComponent<ParticleSystem> ();
        m_particles.GetComponent<Renderer>().sortingLayerName = "World";
        m_particles.GetComponent<Renderer>().sortingOrder = 4;
        m_rigidBody = transform.parent.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Updates the particle spray effect
    /// </summary>
    void Update () 
    {
        if(!Utilities.IsLevelLoaded())
        {
            return;
        }

        m_particles.enableEmission = m_rigidBody.velocity.magnitude >= minSpeedForEmission;
    }
}
