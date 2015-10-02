////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ParticleFogOfWarTile.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ParticleFogOfWarBuilder : MonoBehaviour 
{   
    public GameObject m_particleTemplateEffect = null; /// Template to base the particles on
    public List<GameObject> m_activeEmitters = null;   /// All particle emitters that can be interacted with

    /// <summary>
    /// Instantiates all particle systems for the fog of war
    /// </summary>
    void Start () 
    {
        var gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        if(gameBoard == null)
        {
            throw new NullReferenceException(
                "Could not find Game Board in scene");
        }

        var boardBounds = gameBoard.GetComponent<SpriteRenderer>().bounds;
        var boardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x);
        var boardLength = Mathf.Abs(boardBounds.max.z - boardBounds.min.z);
        var position = m_particleTemplateEffect.transform.position;

        const int border = 2;
        const float size = 6.0f;
        int amountX = Mathf.CeilToInt(boardWidth / size);
        int amountZ = Mathf.CeilToInt(boardLength / size);
        amountX += border * 2;
        amountZ += border * 2;

        m_activeEmitters = new List<GameObject>();

        for(int x = 0; x < amountX; ++x)
        {
            for(int z = 0; z < amountZ; ++z)
            {
                var particle = Instantiate(m_particleTemplateEffect);
                var system = particle.GetComponent<ParticleSystem>();

                float randX = UnityEngine.Random.Range(-1.0f, 1.0f);
                float randY = UnityEngine.Random.Range(-1.0f, 1.0f);
                float randZ = UnityEngine.Random.Range(-1.0f, 1.0f);

                particle.transform.position = new Vector3(
                    (size * ((amountX - 1) * 0.5f)) - (x * size) + randX - position.x,
                    transform.position.y + randY,
                    (size * ((amountZ - 1) * 0.5f)) - (z * size) + randZ - position.z);

                particle.transform.parent = transform;
                particle.SetActive(true);
                particle.isStatic = true;

                system.randomSeed = (uint)(x + z);
                system.Play();
                system.Pause();

                // Border tiles cannot be interacted with
                if(x < border || z < border || x >= amountX-border || z >= amountZ-border)
                {
                    particle.GetComponent<ParticleFogOfWar>().IsStatic = true;
                }
                else
                {
                    m_activeEmitters.Add(particle);
                }
            }
        }

        m_particleTemplateEffect.SetActive(false);
    }

    /// <summary>
    /// Hides the fog of war by disabling all emitters the player can interact with
    /// </summary>
    public void HideFog()
    {
        foreach(var emitter in m_activeEmitters)
        {
            emitter.SetActive(false);
        }
    }
}
