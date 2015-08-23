////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameInformation.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GameInformation : MonoBehaviour 
{
    public string playerName = "";
    static string sm_playerName = GetDefaultName();

    /**
    * Sets the player name if specified
    */
    void Start()
    {
        if(playerName != "" && sm_playerName == GetDefaultName())
        {
            sm_playerName = playerName;
        }
    }

    /**
    * Sets the player name
    */
    static public void SetPlayerName(string name)
    {
        if(name != "")
        {
            sm_playerName = name;
        }
    }

    /**
    * Gets the player name
    */
    static public string GetPlayerName()
    {
        return sm_playerName;
    }

    /**
    * Gets the default name
    */
    static public string GetDefaultName()
    {
        return "Unnamed";
    }
}
