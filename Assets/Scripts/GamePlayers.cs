////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GamePlayers.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GamePlayers : MonoBehaviour 
{    
    static private GameObject[] sm_enemies = null; // enemies
    static private GameObject sm_player = null;    // Controlled player

    /**
    * Fills in a list of current players
    */
    void Update()
    {
        if(sm_player == null)
        {
            sm_player = GameObject.FindGameObjectWithTag("Player");
        }

        sm_enemies = GameObject.FindGameObjectsWithTag("EnemyPlayer");
    }

    /**
    * Finds the other players (enemies) in the game
    */
    public static GameObject[] GetEnemies()
    {
        return sm_enemies;
    }

    /**
    * Finds the controllable player and returns
    * @note is possible to be null until the network has initialised
    */
    public static GameObject GetControllablePlayer()
    {
        return sm_player;
    }
}
