////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GamePlayers.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour 
{    
    static private GameObject[] sm_enemies = null; // enemies
    static private GameObject sm_player = null;    // Controlled player

    /// <summary>
    /// Fills in a list of current players
    /// </summary>
    void Update()
    {
        if(sm_player == null)
        {
            sm_player = GameObject.FindGameObjectWithTag("Player");
        }

        sm_enemies = GameObject.FindGameObjectsWithTag("EnemyPlayer");
    }

    /// <summary>
    /// Finds the other players (enemies) in the game
    /// </summary>
    public static GameObject[] GetEnemies()
    {
        return sm_enemies;
    }

    /// <summary>
    /// Finds the controllable player and returns
    /// @note is possible to be null until the network has initialised
    /// </summary>
    public static GameObject GetControllablePlayer()
    {
        return sm_player;
    }
}
