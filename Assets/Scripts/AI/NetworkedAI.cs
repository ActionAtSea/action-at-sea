////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkedPlayer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player information instantiated by Photon Networking
/// </summary>
public class NetworkedAI : NetworkedEntity
{

    public enum AIType
    {
        ROGUE,
        FLEET,
        PATROL
    };
    
    /// <summary>
    /// Information required which is not networked
    /// </summary>
    #region infonotnetworked
    public AIType m_aiType;

    /// <summary>
    /// Whether or not the AI has already been spawned on game start.
    /// Used to determine whether AI is spawning on game start
    /// or respawning after having died.
    /// </summary>
    private bool m_alreadySpawned = false;
    //NOTE: Is never set atm.
    GameObject m_assignedPlayer = null;
    #endregion

    /// <summary>
    /// Information networked peer-to-peer
    /// </summary>
    #region infonetworkedp2p
    #endregion
    
    /// <summary>
    /// Initilaises the networked ai
    /// Code not relying on the world goes here
    /// </summary>
    void Start()
    {
        m_isAI = true;

        m_cannonController = GetComponentInChildren<AICannonController>();
        m_healthBar = GetComponent<AIHealth>();

        base.InitialiseAtStart();
    }

    /// <summary>
    /// Initilaises the networked ai
    /// Code relying on the world goes here
    /// </summary>
    protected override void InitialiseAtWorld()
    {
        m_spawnIndex = 0;
        gameObject.tag = "AIShip";

        // Name is used to determine when successful
        // data is recieved and cannot be null
        switch (m_aiType)
        {
            case AIType.ROGUE:
                m_name = "Rogue";
                break;
            case AIType.FLEET:
                m_name = "Fleet";
                break;
            case AIType.PATROL:
                m_name = "Patrol";
                break;
            default:
                break;
        }
        

        base.InitialiseAtWorld();
    }

    /// <summary>
    /// Adds the player to the minimap and notifies the player manager of creation
    /// </summary>
    protected override void NotifyPlayerCreation()
    {
        transform.parent.name = m_name;
        name = m_name;

        PlayerManager.AddAI(gameObject);

        base.NotifyPlayerCreation();
    }

    /// <summary>
    /// Hides/shows the ship. Keep the networked player active 
    /// to allow connected players to still recieve information
    /// </summary>
    protected override void ShowShip(bool show)
    {
        base.ShowShip(show);


        NavMeshAgent temp = GetComponent<NavMeshAgent>();
        temp.enabled = show;
    }
    
    /// <summary>
    /// On destroy called for both client and non-client controlled
    /// </summary>
    void OnDestroy()
    {
        PlayerManager.RemoveAI(gameObject);
        base.Destroy();
    }
    
    /// <summary>
    /// Updates the player from the networked data
    /// </summary>
    void Update()
    {
        if(!Utilities.IsLevelLoaded())
        {
            return;
        }

        base.OnUpdate();
    }
    
    /// <summary>
    /// Renders diagnostics for the player networking
    /// </summary>
    void LateUpdate()
    {
        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("AI " + name, m_health);
        }
    }

    /// <summary>
    /// Serialises player data to each player
    /// Note not called if only player in the room
    /// Note not called every tick or at regular intervals
    /// </summary>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.Serialize(stream);
    }

    /// <summary>
    /// Attempts to predict where the non-client player would be to reduce network latency
    /// </summary>
    protected override void PositionNonClientPlayer()
    {
        base.PositionNonClientPlayer();
    }

    /// <summary>
    /// Gets the assigned player of the ai
    /// </summary>    
    public GameObject GetAssignedPlayer()
    {
        return m_assignedPlayer;
    }

    /// <summary>
    /// Gets the assigned player's health of the ai
    /// </summary>    
    public bool IsAssignedPlayerIsAlive()
    {
        return Utilities.IsPlayerAlive(m_assignedPlayer);
    }

    public AIType aiType
    {
        get { return m_aiType; }
    }

    /// <summary>
    /// Whether has already been spawned initially.
    /// Used to determine whether an AI is spawning for the first time
    /// or respawning after having been destroyed.
    /// </summary>
    public bool AlreadySpawned
    {
        get { return m_alreadySpawned; }
        set { m_alreadySpawned = value; }
    }
}

