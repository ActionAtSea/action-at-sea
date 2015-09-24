using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour {



    private List<IslandDiscoveryTrigger> islandList = new List<IslandDiscoveryTrigger>(); // Holds all islands within a level.
    private IslandDiscoveryTrigger[] m_islandList;

	// Use this for initialization
	void Start () 
    {
        m_islandList = FindObjectsOfType<IslandDiscoveryTrigger>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
