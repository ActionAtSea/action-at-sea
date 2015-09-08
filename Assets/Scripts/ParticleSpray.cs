////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ParticleSpray.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class ParticleSpray : MonoBehaviour 
{    
    public float minSpeedForEmission = 1.0f;
    public GameObject parent = null;

    private ParticleSystem m_particles;

    /// <summary>
    /// Initialises the particle spray effect
    /// </summary>
    void Start () 
    {
        m_particles = GetComponent<ParticleSystem> ();
        m_particles.GetComponent<Renderer>().sortingLayerName = "World";
        m_particles.GetComponent<Renderer>().sortingOrder = 4;
    }

    /// <summary>
    /// Updates the particle spray effect
    /// </summary>
    void Update () 
    {
        m_particles.enableEmission = parent.GetComponent<Rigidbody>().velocity.magnitude >= minSpeedForEmission;
    }
}
