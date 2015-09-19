////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ParticleFogOfWar.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class ParticleFogOfWar : MonoBehaviour 
{    
    private ParticleSystem m_particles;
    private bool m_fade = false;
    public Color m_colour;
    private bool m_initialised = false;

    /// <summary>
    /// Initialises the particle effect
    /// </summary>
    void Start () 
    {
        m_particles = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// On collision with the player
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_fade = true;
        }
    }

    /// <summary>
    /// Enables/disables collision detection
    /// </summary>
    void Update()
    {
        m_particles.GetComponent<SphereCollider>().enabled = 
            PlayerPlacer.IsCloseToPlayer(m_particles.transform.position, 15.0f);

        if(m_fade || !m_initialised)
        {
            if(m_initialised)
            {
                m_colour.r -= Time.deltaTime * 0.3f;
                m_colour.g -= Time.deltaTime * 0.3f;
                m_colour.b -= Time.deltaTime * 0.3f;
                m_colour.a -= Time.deltaTime * 0.7f;
            }

            m_initialised = true;

            if(m_colour.a <= 0.0f)
            {
                m_particles.gameObject.SetActive(false);
            }
            else
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[m_particles.particleCount];
                m_particles.GetParticles(particles);

                for(int i = 0; i < particles.Length; ++i)
                {
                    particles[i].size -= Time.deltaTime * 2.0f;
                    particles[i].color = m_colour;
                }

                m_particles.SetParticles(particles, m_particles.particleCount);
            }
        }
    }
}
