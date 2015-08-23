////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerPlacer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerPlacer : MonoBehaviour 
{    
    private GameObject m_gameboard = null;
    private float m_gameboardOffset = 20.0f;
    private float m_playerRadious = 5.0f;

    /**
    * Initialises the script
    */
    void Start()
    {
        m_gameboard = GameObject.FindGameObjectWithTag("GameBoard");
        if(m_gameboard == null)
        {
            Debug.LogError("Could not find game board");
        }
    }

    /**
    * Utility function to determine if the given position is roughly visible to the player
    */
    static public bool IsCloseToPlayer(Vector3 position)
    {
        var player = PlayerManager.GetControllablePlayer();
        if(player != null)
        {
            const float maxDistance = 30.0f;
            return (player.transform.position - position).magnitude <= maxDistance;
        }
        return false;
    }

    /**
    * Retrieves a new position on the map
    */
    public Vector2 GetNewPosition()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
        if(spawnPoints == null || spawnPoints.Length == 0)
        {
            return GetRandomPosition();
        }
        else
        {
            // TO DO: Add spawn points
            return new Vector2();
        }
    }

    /**
    * Retrieves a new position on the map that doesn't collide
    */
    public Vector2 GetRandomPosition()
    {
        GameObject[] players = PlayerManager.GetEnemies();

        bool foundPosition = false;
        Vector2 position = new Vector3(0, 0);
        
        var boardBounds = m_gameboard.GetComponent<SpriteRenderer>().bounds;
        var halfBoardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x) / 2.0f;
        var halfBoardLength = Mathf.Abs(boardBounds.max.y - boardBounds.min.y) / 2.0f;

        while (!foundPosition) 
        {
            foundPosition = true;
            position.x = Random.Range(-halfBoardWidth + m_gameboardOffset, 
                                      halfBoardWidth - m_gameboardOffset);

            position.y = Random.Range(-halfBoardLength + m_gameboardOffset, 
                                      halfBoardLength - m_gameboardOffset);


            GameObject[] terrain = GameObject.FindGameObjectsWithTag("Island");
            if(terrain == null)
            {
                Debug.LogError("Could not find any terrain");
            }

            for(int i = 0; i < terrain.Length; ++i)
            {
                var islandBounds = terrain[i].GetComponent<SpriteRenderer>().bounds;
                if(position.x > islandBounds.center.x - islandBounds.extents.x &&
                   position.x < islandBounds.center.x + islandBounds.extents.x &&
                   position.y > islandBounds.center.y - islandBounds.extents.y &&
                   position.y < islandBounds.center.y + islandBounds.extents.y)
                {
                    foundPosition = false;
                    break;
                }
            }

            if(foundPosition)
            {
                foreach(GameObject player in players)
                {
                    if(player != null)
                    {
                        Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
                        Vector2 difference = position - playerPosition;
                        if(difference.magnitude <= m_playerRadious)
                        {
                            foundPosition = false;
                            break;
                        }
                    }
                }
            }
        }

        return position;
    }
}
