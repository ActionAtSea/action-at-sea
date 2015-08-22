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

    /**
    * Initialises the particle spray effect
    */
    void Start () 
    {
        m_particles = GetComponent<ParticleSystem> ();
        m_particles.GetComponent<Renderer>().sortingLayerName = "World";
        m_particles.GetComponent<Renderer>().sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder - 1;
    }

    /**
    * Updates the particle spray effect
    */
    void Update () 
    {
        m_particles.enableEmission = parent.GetComponent<Rigidbody2D>().velocity.magnitude >= minSpeedForEmission;
    }
}
