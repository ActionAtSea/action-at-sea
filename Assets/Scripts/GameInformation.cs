////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameInformation.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GameInformation : MonoBehaviour 
{
    static string sm_playerName = GetDefaultName();

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
