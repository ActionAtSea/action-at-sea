////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameInformation.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GameInformation : MonoBehaviour 
{
    public string playerName = "Unnamed";    
    static bool sm_isInitialised = false;
    static string sm_playerName = "Unnamed";

    /**
    * Initialises scene shared information about the game to play
    */
    void Start () 
    {
        if(!sm_isInitialised)
        {
            sm_isInitialised = true;
            sm_playerName = playerName;
        }
    }

    /**
    * Sets the player name
    */
    static public void SetPlayerName(string name)
    {
        sm_playerName = name;
    }

    /**
    * Gets the player name
    */
    static public string GetPlayerName()
    {
        return sm_playerName;
    }
}
