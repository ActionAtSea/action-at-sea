using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    private IslandDiscoveryTrigger[] m_islandList;

    /// <summary>
    /// Initialises the game mode manager
    /// </summary>  
	void Start () 
    {
        m_islandList = FindObjectsOfType<IslandDiscoveryTrigger>();
	}

    /// <summary>
    /// Updates the game mode manager
    /// </summary>  
    void Update () 
    {
	
	}
}
