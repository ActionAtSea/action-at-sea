////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkedPlayer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player information instantiated by Photon Networking
/// </summary>
public class NetworkedPlayer : NetworkedEntity
{    
    /// <summary>
    /// Information required which is not networked
    /// </summary>
    #region infonotnetworked
    private PlayerScore m_score = null;
    private GameObject m_networkDiagnostics = null;
    private GameObject m_islandWithinRange = null;
    public bool fleetShipBought = false;
    #endregion

    /// <summary>
    /// Information networked peer-to-peer
    /// </summary>
    #region infonetworkedp2p
    private int m_playerScore = 0;
    #endregion

    /// <summary>
    /// Initilaises the networked player
    /// Code not relying on the world goes here
    /// </summary>
    void Start()
    {
        m_isAI = false;

        m_networkDiagnostics = transform.parent.FindChild("NetworkDiagnostics").gameObject;

        base.InitialiseAtStart();
    }

    /// <summary>
    /// Initilaises the networked player
    /// Code relying on the world goes here
    /// </summary>
    protected override void InitialiseAtWorld()
    {
        if(photonView.isMine)
        {
            gameObject.tag = "Player";

            var matchMaker = Utilities.GetNetworking();
            m_spawnIndex = matchMaker.GetPlayerIndex();

            m_name = Utilities.GetPlayerName();
            if(m_name.Length == 0)
            {
                // Name is used to determine when successful
                // data is recieved and cannot be null
                m_name = Utilities.GetPlayerDefaultName();
            }
        }
        else
        {
            gameObject.tag = "EnemyPlayer";
        }

        base.InitialiseAtWorld();

        m_floatingHealthBar.SetActive(!photonView.isMine);
    }

    /// <summary>
    /// Adds the player to the minimap and notifies the player manager of creation
    /// </summary>
    protected override void NotifyPlayerCreation()
    {
        gameObject.name = m_ID.ToString();
        transform.parent.name = m_name;

        PlayerManager.AddPlayer(gameObject);

        base.NotifyPlayerCreation();
    }

    /// <summary>
    /// Hides/shows the ship. Keep the networked player active 
    /// to allow connected players to still recieve information
    /// </summary>
    protected override void ShowShip(bool show)
    {
        base.ShowShip(show);

        GetComponent<PlayerAiming>().enabled = show;
        GetComponent<PlayerMovement>().enabled = show;
        m_floatingHealthBar.SetActive(show && !photonView.isMine);
    }

    /// <summary>
    /// On destroy called for both client and non-client controlled
    /// </summary>
    void OnDestroy()
    {
        PlayerManager.RemovePlayer(gameObject);
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

        if(photonView.isMine)
        {
            m_playerScore = (int)m_score.RoundedScore;
        }
    }

    /// <summary>
    /// Renders diagnostics for the player networking
    /// </summary>
    void LateUpdate()
    {
        m_networkDiagnostics.SetActive(
            Diagnostics.IsActive() && !photonView.isMine);

        if(Diagnostics.IsActive())
        {
            if(m_networkDiagnostics.activeSelf)
            {
                m_networkDiagnostics.GetComponent<SpriteRenderer>().color = m_colour;
                m_networkDiagnostics.transform.position = m_networkedPosition;
                m_networkDiagnostics.transform.rotation = m_networkedRotation;
            }

            string playerConnection;
            if(photonView.isMine)
            {
                Diagnostics.Add("Player Spawn Index", m_spawnIndex);
                playerConnection = "Client";
            }
            else
            {
                playerConnection = "Enemy [" + (m_recievedValidData ? "x" : " ") + "]";
            }

            Diagnostics.Add("Player" + m_ID, m_name + 
                "|" + m_playerScore + 
                "|" + m_health + 
                "|" + m_hue + 
                "|" + m_initialised + 
                "|" + playerConnection);
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

        if (stream.isWriting)
        {
            stream.SendNext(m_playerScore);
        }
        else
        {
            m_playerScore = (int)stream.ReceiveNext();
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
    /// Gets the player score
    /// </summary>
    public float PlayerScore
    {
        get { return m_playerScore; }
    }

    /// <summary>
    /// Returns the current island within range of 
    /// the player or null if there isn't one.
    /// </summary>
    public GameObject IslandWithinRange
    {
        get { return m_islandWithinRange; }
        set { m_islandWithinRange = value; }
    }

    /// <summary>
    /// Gets the entity components
    /// </summary>
    protected override void InitialiseEntityComponents()
    {
        base.InitialiseEntityComponents();

        m_cannonController = GetComponentInChildren<PlayerCannonController>();
        if (m_cannonController == null)
        {
            Debug.LogError("Could not find cannon controller");
        }

        m_healthBar = GetComponent<PlayerHealth>();
        if (m_healthBar == null)
        {
            Debug.LogError("Could not find health bar");
        }

        m_score = GetComponent<PlayerScore>();
        if (m_score == null)
        {
            Debug.LogError("Could not find score");
        }
    }
}
