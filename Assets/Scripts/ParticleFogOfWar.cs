////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ParticleFogOfWar.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class ParticleFogOfWar : MonoBehaviour 
{    
    private Color m_color = new Color(1.0f, 1.0f, 1.0f, 1.0f); /// Colour of the particles 
    private float m_radius = 10.0f;                             /// Bounding radius for collision
    private ParticleSystem m_particles = null;                 /// Particle System component
    private ParticleSystemRenderer m_renderer = null;          /// Controls the particle rendering
    private bool m_fade = false;                               /// Whether the system is fading out
    private bool m_static = false;                             /// Whether can interact with the player
    private bool m_initialised = false;                        /// Whether the particles have been initialised

    /// <summary>
    /// Initialises the particle fog of war script
    /// </summary>
    void Start () 
    {
        m_particles = GetComponent<ParticleSystem>();
        m_renderer = m_particles.GetComponent<ParticleSystemRenderer>();
    }

    /// <summary>
    /// Sets whether this particle system is static and cannot be interacted with
    /// </summary>
    public bool IsStatic
    {
        set { m_static = value; }
    }

    /// <summary>
    /// Determines if the player is within the radius of the particle system
    /// Will stop the system once all particles are fully transparent
    /// </summary>
    void Update()
    {
        if(m_fade || !m_initialised)
        {
            if(m_color.a <= 0.0f)
            {
                gameObject.SetActive(false);
            }
            else
            {
                ParticleSystem.Particle[] particles = 
                    new ParticleSystem.Particle[m_particles.particleCount];
                
                m_particles.GetParticles(particles);

                if(m_initialised)
                {
                    m_color.a -= Time.deltaTime * 0.7f;
                }

                float sizeReduceAmount = m_initialised ? 
                    Time.deltaTime * 2.0f : 1.0f;
                
                for(int i = 0; i < particles.Length; ++i)
                {
                    particles[i].size -= sizeReduceAmount;
                    particles[i].color = m_color;
                }
                
                m_particles.SetParticles(particles, m_particles.particleCount);
                m_initialised = true;
            }
        }
        else if(!m_static && m_renderer.isVisible)
        {
            var player = PlayerManager.GetControllablePlayer();
            if(player != null && Utilities.IsPlayerInitialised(player))
            {
                m_fade = PlayerManager.IsFogCloseToPlayer(
                    transform.position.x, transform.position.z, m_radius);
            }
        }
    }
}
