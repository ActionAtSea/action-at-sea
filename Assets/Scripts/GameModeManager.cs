////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameModeManager.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    private IslandDiscoveryTrigger[] m_islandList;
    private float m_timePassed = 0; /// Time passed since starting level

    /// <summary>
    /// Initialises the game mode manager
    /// </summary>  
	void Start () 
    {
        m_islandList = FindObjectsOfType<IslandDiscoveryTrigger>();
        if(m_islandList == null || m_islandList.Length == 0)
        {
            Debug.LogError("Could not find island triggers for level");
        }
	}

    /// <summary>
    /// Set the time passed since starting the level
    /// </summary>  
    public void TrySetTimePassed(float timePassed)
    {
        m_timePassed = Mathf.Min(timePassed, m_timePassed);
    }

    /// <summary>
    /// Gets the time passed since starting the level
    /// </summary>  
    public float GetTimePassed()
    {
        return m_timePassed;
    }

    /// <summary>
    /// Updates the game mode manager
    /// </summary>  
    void Update() 
    {
        m_timePassed = Mathf.Max(m_timePassed, Time.time);
	}

    /// <summary>
    /// Gets the Game Mode Manager instance from the scene
    /// </summary>
    public static GameModeManager Get()
    {
        var gameManager = FindObjectOfType<GameModeManager>();
        if(gameManager == null)
        {
            Debug.LogError("Could not find GameModeManager in scene");
        }
        return gameManager;
    }
}
