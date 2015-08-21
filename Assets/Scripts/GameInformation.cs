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
    
    void Start () 
    {
        if(!sm_isInitialised)
        {
            sm_isInitialised = true;
            sm_playerName = playerName;
        }
    }
    
    static public void SetPlayerName(string name)
    {
        sm_playerName = name;
    }

    static public string GetPlayerName()
    {
        return sm_playerName;
    }
}
