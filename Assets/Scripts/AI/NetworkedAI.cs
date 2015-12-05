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
    /// <summary>
    /// Information required which is not networked
    /// </summary>
    #region infonotnetworked
    #endregion

    /// <summary>
    /// Information networked peer-to-peer
    /// </summary>
    #region infonetworkedp2p
    private int m_aiID = 0; // ID of the player who owns this AI
    #endregion
    
    /// <summary>
    /// Initilaises the networked ai
    /// Code not relying on the world goes here
    /// </summary>
    void Start()
    {
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
        gameObject.tag = "AIShip";

        // Name is used to determine when successful
        // data is recieved and cannot be null
        m_name = "Rogue";

        base.InitialiseAtWorld();
    }

    /// <summary>
    /// Adds the player to the minimap and notifies the player manager of creation
    /// </summary>
    protected override void NotifyPlayerCreation()
    {
        transform.parent.name = m_name;
        name = m_name;

        base.AddToMinimap();

        PlayerManager.AddAI(gameObject);
    }

    /// <summary>
    /// Positions the ship on a reset
    /// </summary>
    protected override void ResetPosition()
    {
        // AI positioned by navmesh
    }

    /// <summary>
    /// Hides/shows the ship. Keep the networked player active 
    /// to allow connected players to still recieve information
    /// </summary>
    protected override void ShowShip(bool show)
    {
        base.ShowShip(show);

        GetComponent<AIAiming>().enabled = show;
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
        
        if (m_health <= 0)
        {
            //TODO: implement this properly.
            PhotonNetwork.Destroy(photonView);
            Debug.Log("AI is dead");
        }
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
    protected override void Serialise(PhotonStream stream)
    {
        if (stream.isWriting)
        {
            stream.SendNext(m_aiID);
        }
        else
        {
            m_aiID = (int)stream.ReceiveNext();
        }
    }

    /// <summary>
    /// Attempts to predict where the non-client player would be to reduce network latency
    /// </summary>
    protected override void PositionNonClientPlayer()
    {
        base.PositionNonClientPlayer();
    }

    /// <summary>
    /// Gets the ai ID
    /// </summary>
    public int AiID
    {
        get { return m_aiID; }
    }
}
