////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ParticleFogOfWarTile.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;

public class ParticleFogOfWarTile : MonoBehaviour 
{   
    public GameObject particleEffect = null;

    /// <summary>
    /// Initialises the particle effect
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

        var center = particleEffect.GetComponent<SphereCollider>().center;

        const float size = 7.5f;
        const int border = 2;
        int amountX = Mathf.CeilToInt(boardWidth / size);
        int amountZ = Mathf.CeilToInt(boardLength / size);
        amountX += border * 2;
        amountZ += border * 2;

        for(int x = 0; x < amountX; ++x)
        {
            for(int z = 0; z < amountZ; ++z)
            {
                var particle = Instantiate(particleEffect);
                particle.transform.parent = transform;

                particle.GetComponent<ParticleSystem>().randomSeed = 
                    (uint)UnityEngine.Random.Range(0, 1000);

                float randX = UnityEngine.Random.Range(-1.0f, 1.0f);
                float randY = UnityEngine.Random.Range(-1.0f, 1.0f);
                float randZ = UnityEngine.Random.Range(-1.0f, 1.0f);

                particle.transform.position = new Vector3(
                    (size * ((amountX - 1) * 0.5f)) - (x * size) + randX - center.x,
                    transform.position.y + randY,
                    (size * ((amountZ - 1) * 0.5f)) - (z * size) + randZ - center.z);

                particle.GetComponent<ParticleSystem>().Play();
                particle.GetComponent<ParticleSystem>().Pause();
                particle.isStatic = true;

                // Border tiles cannot be interacted with
                if(x < border || z < border || x >= amountX-border || z >= amountZ-border)
                {
                    particle.GetComponent<SphereCollider>().enabled = false;
                }
            }
        }

        particleEffect.SetActive(false);
    }
}
