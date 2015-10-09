////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GamePlayers.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class PlayerManager : MonoBehaviour 
{
    private static GameObject sm_player = null;
    private static Dictionary<int, GameObject> sm_playerIDs = null;
    private static List<GameObject> sm_allplayers = null;

    private GameObject m_gameboard = null;
    private float m_gameboardOffset = 20.0f;
    private float m_playerRadious = 5.0f;
    private List<GameObject> m_spawns = null;

    /// <summary> 
    /// Position/rotation/color information
    /// </summary>
    public class Placement
    {
        public Vector3 position = new Vector3();
        public Vector3 rotation = new Vector3();
        public Color color;
    }

    /// <summary>
    /// On start a level
    /// </summary>
    void Start()
    {
        sm_allplayers = new List<GameObject>();
        sm_playerIDs = new Dictionary<int, GameObject>();

        m_gameboard = GameObject.FindGameObjectWithTag("GameBoard");
        if(m_gameboard == null)
        {
            Debug.LogError("Could not find game board");
        }

        m_spawns = Utilities.GetOrderedList("Spawn");
        if(!Utilities.IsOpenLeveL(Utilities.GetLoadedLevel()) && m_spawns.Count == 0)
        {
            Debug.LogError("Could not find any spawns");
        }
    }

    /// <summary>
    /// Updates the manager
    /// </summary>
    void Update()
    {
        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("Player Manager Count", sm_allplayers.Count);
        }
    }

    /// <summary>
    /// On leave a level
    /// </summary>
    void OnDestroy()
    {
        sm_playerIDs.Clear();
        sm_allplayers.Clear();
        sm_player = null;
    }

    /// <summary>
    /// Gets all the players in the game
    /// </summary>
    public static List<GameObject> GetAllPlayers()
    {
        return sm_allplayers;
    }

    /// <summary>
    /// Gets all the players in the game ordered by score
    /// </summary>
    public static List<GameObject> GetAllPlayersByScore()
    {
        return GetAllPlayers().OrderByDescending(
            x => NetworkedPlayer.GetPlayerScore(x)).ToList();
    }

    /// <summary>
    /// Finds the controllable player and returns
    /// @note is possible to be null until the network has initialised
    /// </summary>
    public static GameObject GetControllablePlayer()
    {
        return sm_player;
    }

    /// <summary>
    /// Finds the player with the given ID
    /// @note is possible to be null until the network has initialised
    /// </summary>
    public static GameObject GetPlayerWithID(int ID)
    {
        if(!sm_playerIDs.ContainsKey(ID))
        {
            // This can sometimes be null if the player hasn't been networked yet
            Debug.Log("Could not find player with ID " + ID);
            return null;
        }
        return sm_playerIDs[ID];
    }

    /// <summary>
    /// Determines whether the given game object is a player
    /// </summary>
    public static bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") ||
            obj.CompareTag("EnemyPlayer");
    }

    /// <summary>
    /// Determines whether the given game object is a player
    /// </summary>
    public static bool IsControllablePlayer(GameObject obj)
    {
        return obj.CompareTag("Player");
    }

    /// <summary>
    /// Adds a new player to the manager
    /// </summary>
    public static void AddPlayer(GameObject player)
    {
        if(IsControllablePlayer(player))
        {
            sm_player = player;
        }

        if(sm_allplayers.Exists(x => x == player))
        {
            sm_allplayers.Remove(player);
        }
        sm_allplayers.Add(player);

        int id = player.GetComponent<NetworkedPlayer>().PlayerID;
        if(id == -1)
        {
            Debug.LogError("Adding ship with invalid ID");
        }

        if(sm_playerIDs.ContainsKey(id))
        {
            sm_playerIDs.Remove(id);
        }
        sm_playerIDs.Add(id, player);
        Debug.Log("Adding ship " + id + " to manager");
    }

    /// <summary>
    /// Removes a player to the manager
    /// </summary>
    public static void RemovePlayer(GameObject player)
    {
        if(player.CompareTag("Player"))
        {
            sm_player = null;
        }
        sm_allplayers.Remove(player);

        int id = player.GetComponent<NetworkedPlayer>().PlayerID;
        sm_playerIDs.Remove(id);
        Debug.Log("Removing ship " + id + " from manager");
    }

    /// <summary>
    /// Utility function to determine if the given position is roughly visible to the player
    /// </summary>
    static public bool IsFogCloseToPlayer(float x, float z, float distance)
    {
        var player = GetControllablePlayer();
        if(player != null)
        {
            var fogTrigger = PlayerMovement.IsMoving(player) ?
                player.transform.FindChild("FogTrigger").gameObject : player;

            float xToPlayer = x - fogTrigger.transform.position.x;
            float zToPlayer = z - fogTrigger.transform.position.z;
            return ((xToPlayer * xToPlayer) + (zToPlayer * zToPlayer)) <= distance * distance;
        }
        return false;
    }
    
    /// <summary>
    /// Utility function to determine if the given position is roughly visible to the player
    /// </summary>
    static public bool IsCloseToPlayer(Vector3 position, float distance)
    {
        var player = GetControllablePlayer();
        if(player != null)
        {
            return (player.transform.position - position).magnitude <= distance;
        }
        return false;
    }

    /// <summary>
    /// Utility function to determine if the given position is roughly visible to the player
    /// </summary>
    static public bool IsCloseToPlayer(float x, float z, float distance)
    {
        var player = GetControllablePlayer();
        if(player != null)
        {
            float xToPlayer = x - player.transform.position.x;
            float zToPlayer = z - player.transform.position.z;
            return ((xToPlayer * xToPlayer) + (zToPlayer * zToPlayer)) <= distance * distance;
        }
        return false;
    }
    
    /// <summary>
    /// Retrieves a new position on the map
    /// </summary>
    public Placement GetNewPosition(int index, GameObject player)
    {
        Placement place = null;
        
        if(m_spawns != null && m_spawns.Count > 0)
        {
            int playersAllowed = Utilities.GetAcceptedPlayersForLevel(Utilities.GetLoadedLevel());
            if(m_spawns.Count < playersAllowed)
            {
                Debug.LogError("Spawn amount does not equal number of accepted players for level");
            }

            Debug.Log("Using spawn: " + m_spawns[index].name);
            place = new Placement();
            place.position.x = m_spawns[index].transform.position.x;
            place.position.z = m_spawns[index].transform.position.z;
            place.rotation.x = 0.0f;
            place.rotation.y = 0.0f;
            place.rotation.z = -m_spawns[index].transform.localEulerAngles.y;
            place.color = m_spawns[index].GetComponent<SpriteRenderer>().color;
        }
        else
        {
            place = GetRandomPosition();
            place.color = Utilities.HSVToRGB((uint)Random.Range(0, 360), 1.0f, 1.0f);
        }
        
        // Flip the ship to be upwards
        place.rotation.x = 90.0f;
        return place;
    }
    
    /// <summary>
    /// Retrieves a new position on the map that doesn't collide
    /// </summary>
    public Placement GetRandomPosition()
    {
        List<GameObject> players = GetAllPlayers();
        
        Vector2 position = new Vector2();
        bool foundPosition = false;
        
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
                   position.y > islandBounds.center.z - islandBounds.extents.z &&
                   position.y < islandBounds.center.z + islandBounds.extents.z)
                {
                    foundPosition = false;
                    break;
                }
            }
            
            if(foundPosition && players != null)
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
        
        Placement place = new Placement();
        place.position.x = position.x;
        place.position.z = position.y;
        return place;
    }

    /// <summary>
    /// Gets the Player Manager instance from the scene
    /// </summary>
    public static PlayerManager Get()
    {
        var obj = FindObjectOfType<PlayerManager>();
        if(obj == null)
        {
            Debug.LogError("Could not find PlayerManager in scene");
        }
        return obj;
    }
}
