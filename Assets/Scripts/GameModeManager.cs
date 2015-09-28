////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameModeManager.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    private IslandDiscoveryTrigger[] m_islandList;
    private float m_networkedTimePassed = 0.0f;
    private float m_timePassed = 0;

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
    /// Set by all clients connected to the room
    /// Use the smallest time passed a client has
    /// </summary>  
    public void TrySetTimePassed(float timePassed)
    {
        m_networkedTimePassed = Mathf.Max(
            timePassed, m_networkedTimePassed);
    }

    /// <summary>
    /// Late update required as time passed is set during update()
    /// </summary>
    void LateUpdate()
    {
        m_timePassed = m_networkedTimePassed;
        m_networkedTimePassed = 0.0f;
    }

    /// <summary>
    /// Gets the time passed since starting the level
    /// </summary>  
    public float GetTimePassed()
    {
        return m_timePassed;
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
