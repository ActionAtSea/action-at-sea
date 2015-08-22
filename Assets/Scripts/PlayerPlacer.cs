////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerPlacer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerPlacer : MonoBehaviour 
{    
    public GameObject gameboard;
    public GameObject terrain;
    private float m_gameboardOffset = 20.0f;
    private float m_playerRadious = 5.0f;

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
    * Retrieves a new position on the map that doesn't collide
    */
    public Vector2 GetNewPosition()
    {
        GameObject[] players = PlayerManager.GetEnemies();

        bool foundPosition = false;
        Vector2 position = new Vector3(0, 0);
        
        var boardBounds = gameboard.GetComponent<SpriteRenderer> ().bounds;
        while (!foundPosition) 
        {
            foundPosition = true;
            position.x = Random.Range(-boardBounds.extents.x + m_gameboardOffset, 
                                      boardBounds.extents.x - m_gameboardOffset);

            position.y = Random.Range(-boardBounds.extents.y + m_gameboardOffset, 
                                      boardBounds.extents.y - m_gameboardOffset);
            
            var islands = terrain.GetComponentsInChildren<SpriteRenderer>();
            for(int i = 0; i < islands.Length; ++i)
            {
                var islandBounds = islands[i].bounds;
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

        return position;
    }
}
