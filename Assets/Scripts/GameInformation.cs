////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameInformation.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GameInformation : MonoBehaviour 
{
    public string playerName = "";
    static string sm_playerName = GetDefaultName();

    /// <summary>
    /// Sets the player name if specified
    /// </summary>
    void Start()
    {
        if(playerName != "" && sm_playerName == GetDefaultName())
        {
            sm_playerName = playerName;
        }
    }

    /// <summary>
    /// Sets the player name
    /// </summary>
    static public void SetPlayerName(string name)
    {
        if(name != "")
        {
            sm_playerName = name;
        }
    }

    /// <summary>
    /// Gets the player name
    /// </summary>
    static public string GetPlayerName()
    {
        return sm_playerName;
    }

    /// <summary>
    /// Gets the default name
    /// </summary>
    static public string GetDefaultName()
    {
        return "Unnamed";
    }
}
