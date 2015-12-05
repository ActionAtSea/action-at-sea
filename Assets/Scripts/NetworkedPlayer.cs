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
    private int m_playerIndex = -1; // based on players in room. helps assign spawn spots. not networked.
    private PlayerScore m_score = null;
    private GameObject m_networkDiagnostics = null;
    private bool m_usePrediction = false;
    private PlayerPrediction m_playerPrediction = new PlayerPrediction();
    #endregion

    /// <summary>
    /// Information networked peer-to-peer
    /// </summary>
    #region infonetworkedp2p
    private int m_playerHue = 0;
    private int m_playerID = -1; // photon creates one to uniquely identify this. 
    private int m_playerScore = 0;
    #endregion

    /// <summary>
    /// Initilaises the networked player
    /// Code not relying on the world goes here
    /// </summary>
    void Start()
    {
        m_cannonController = GetComponentInChildren<PlayerCannonController>();
        m_healthBar = GetComponent<PlayerHealth>();
        m_score = GetComponent<PlayerScore>();
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

            var matchMaker = NetworkMatchmaker.Get();
            m_playerIndex = matchMaker.GetPlayerIndex();
            m_playerID = matchMaker.GetPlayerID();

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
    }

    /// <summary>
    /// Adds the player to the minimap and notifies the player manager of creation
    /// </summary>
    protected override void NotifyPlayerCreation()
    {
        gameObject.name = m_playerID.ToString();
        transform.parent.name = m_name;
        m_colour = Colour.HueToRGB(m_playerHue);

        AddToMinimap();

        PlayerManager.AddPlayer(gameObject);
    }

    /// <summary>
    /// Positions the ship on a spawn
    /// </summary>
    protected override void ResetPosition()
    {
        var playerManager = PlayerManager.Get();
        var place = playerManager.GetNewPosition(m_playerIndex, gameObject);
        m_playerHue = place.hue;
        
        m_rigidBody.velocity = Vector3.zero;
        gameObject.transform.position = place.position;
        gameObject.transform.localEulerAngles = place.rotation;
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
                Diagnostics.Add("Player Client Index", m_playerIndex);
                playerConnection = "Client";
            }
            else
            {
                playerConnection = "Enemy [" + (m_recievedValidData ? "x" : " ") + "]";
            }

            Diagnostics.Add("Player" + m_playerID, m_name + 
                "|" + m_playerScore + 
                "|" + m_health + 
                "|" + m_playerHue + 
                "|" + m_initialised + 
                "|" + playerConnection);
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
            stream.SendNext(m_playerID);
            stream.SendNext(m_playerScore);
            stream.SendNext(m_playerHue);
        }
        else
        {
            if (m_usePrediction)
            {
                m_playerPrediction.OnNetworkUpdate(
                    m_networkedPosition, m_networkedRotation, m_networkedVelocity);
            }

            m_playerID = (int)stream.ReceiveNext();
            m_playerScore = (int)stream.ReceiveNext();
            m_playerHue = (int)stream.ReceiveNext();
        }
    }

    /// <summary>
    /// Attempts to predict where the non-client player would be to reduce network latency
    /// </summary>
    protected override void PositionNonClientPlayer()
    {
        if(!m_usePrediction)
        {
            base.PositionNonClientPlayer();
        }
        else
        {
            m_playerPrediction.Update();
            transform.position = m_playerPrediction.GetPosition();
            transform.rotation = m_playerPrediction.GetRotation();
        }
    }

    /// <summary>
    /// Gets the player name
    /// </summary>
    public string PlayerName
    {
        get { return m_name; }
    }

    /// <summary>
    /// Gets the player score
    /// </summary>
    public float PlayerScore
    {
        get { return m_playerScore; }
    }

    /// <summary>
    /// Gets the player ID
    /// </summary>
    public int PlayerID
    {
        get { return m_playerID; }
    }
}
