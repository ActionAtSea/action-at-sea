////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkedPlayer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player information instantiated by Photon Networking
/// </summary>
public class NetworkedPlayer : MonoBehaviour 
{    
    public PhotonView photonView = null;
    public GameObject[] children = null;

    /// <summary>
    /// Information required which is not networked
    /// </summary>
    #region infonotnetworked
    public Color m_playerColor = new Color(1.0f, 1.0f, 1.0f);
    private int m_playerIndex = -1; //what is this?
    private bool m_initialised = false;
    private bool m_visible = true;
    private bool m_recievedValidData = false;
    private bool m_requiresPositionUpdate = false;
    private Health m_healthBar = null;
    private GameObject m_floatingHealthBar = null;
    private CannonController m_cannonController = null;
    private PlayerScore m_score = null;
    private Rigidbody m_rigidBody = null;
    private GameObject m_networkDiagnostics = null;
    private bool m_usePrediction = false;
    private PlayerPrediction m_playerPrediction = new PlayerPrediction();
    #endregion

    /// <summary>
    /// Information networked peer-to-peer
    /// </summary>
    #region infonetworkedp2p
    private int m_playerHue = 0;
    private string m_playerName = "";
    private int m_playerID = -1; //what is this?
    private int m_playerScore = 0;
    private float m_health = -1.0f;
    private float m_mouseCursorAngle = 0.0f;
    private bool m_firedCannonsLeft = false;
    private bool m_firedCannonsRight = false;
    private Vector3 m_networkedPosition;
    private Quaternion m_networkedRotation;
    #endregion

    /// <summary>
    /// Initilaises the networked player
    /// Code not relying on the world goes here
    /// </summary>
    void Start()
    {
        // Photon Networking will destroy the object
        var parent = transform.parent;
        DontDestroyOnLoad(parent);

        m_healthBar = GetComponent<PlayerHealth>();
        m_score = GetComponent<PlayerScore>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_cannonController = GetComponentInChildren<PlayerCannonController>();
        m_networkDiagnostics = parent.FindChild("NetworkDiagnostics").gameObject;
    }

    /// <summary>
    /// Initilaises the networked player
    /// Code relying on the world goes here
    /// </summary>
    void Initialise()
    {
        if(photonView.isMine)
        {
            gameObject.tag = "Player";

            var matchMaker = NetworkMatchmaker.Get();
            m_playerIndex = matchMaker.GetPlayerIndex();
            m_playerID = matchMaker.GetPlayerID();

            PlaceOnSpawn();

            m_playerName = Utilities.GetPlayerName();
            if(m_playerName.Length == 0)
            {
                // Name is used to determine when successful
                // data is recieved and cannot be null
                m_playerName = Utilities.GetPlayerDefaultName();
            }
           
            NotifyPlayerCreation();
        }
        else
        {
            gameObject.tag = "EnemyPlayer";
        }

        m_floatingHealthBar = transform.parent.FindChild("FloatingHealthBar").gameObject;
        m_floatingHealthBar.SetActive(!photonView.isMine);
         
        Debug.Log("Created " + gameObject.tag);
        m_initialised = true;
    }

    /// <summary>
    /// Adds the player to the minimap and notifies the player manager of creation
    /// </summary>
    void NotifyPlayerCreation()
    {
        gameObject.name = m_playerID.ToString();
        transform.parent.name = m_playerName;
        m_playerColor = Colour.HueToRGB(m_playerHue);

        var minimap = GameObject.FindObjectOfType<Minimap>();
        minimap.AddPlayer(gameObject, photonView.isMine, m_playerColor);

        PlayerManager.AddPlayer(gameObject);
    }

    /// <summary>
    /// Positions the ship on a spawn
    /// </summary>
    void PlaceOnSpawn()
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
    void ShowShip(bool show)
    {
        m_visible = show;
        GetComponent<PlayerMovement>().enabled = show;
        GetComponent<PlayerAiming>().enabled = show;
        GetComponent<CapsuleCollider>().enabled = show;
        m_floatingHealthBar.SetActive(show && !photonView.isMine);

        foreach(var child in children)
        {
            child.SetActive(show);
        }
    }

    /// <summary>
    /// Callback when game over is set
    /// </summary>
    public void SetVisible(bool isVisible, bool shouldExplode = false)
    {
        if(isVisible)
        {
            ShowShip(true);
            m_healthBar.SetAlive(true);
        }
        else
        {
            if(shouldExplode)
            {
                m_healthBar.SetAlive(false);
                Explode();
            }

            ShowShip(false);

            if(photonView.isMine)
            {
                PlaceOnSpawn();
            }
        }
    }

    /// <summary>
    /// Explodes the ship
    /// </summary>
    void Explode()
    {
        // Will be null if leaving game
        var animationGenerator = FindObjectOfType<AnimationGenerator>();
        if(animationGenerator != null)
        {
            animationGenerator.PlayAnimation(
                transform.position, AnimationGenerator.ID.EXPLOSION);
        }
    }

    /// <summary>
    /// On destroy called for both client and non-client controlled
    /// </summary>
    void OnDestroy()
    {
        PlayerManager.RemovePlayer(gameObject);
        Debug.Log("Destroying ship: " + name);
        Explode();
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
        
        if(!m_initialised)
        {
            Initialise();
        }

        if(photonView.isMine)
        {
            m_playerScore = (int)m_score.RoundedScore;
            m_health = m_healthBar.HealthLevel;
            m_mouseCursorAngle = m_cannonController.MouseCursorAngle;

            if(m_cannonController.CannonsFiredRight)
            {
                m_firedCannonsRight = true;
            }
            if(m_cannonController.CannonsFiredLeft)
            {
                m_firedCannonsLeft = true;
            }
        }
        else if(m_recievedValidData)
        {
            PositionNonClientPlayer();
            m_cannonController.MouseCursorAngle = m_mouseCursorAngle;

            if(m_firedCannonsLeft)
            {
                m_cannonController.FireWeaponLeft();
                m_firedCannonsLeft = false;
            }

            if(m_firedCannonsRight)
            {
                m_cannonController.FireWeaponRight();
                m_firedCannonsRight = false;
            }

            if(m_health >= 0)
            {
                // Only update health if networked version is lower
                // This can mean however that networked version thinks its higher
                // and the player can be running around seemingly empty
                // Because of this, initially set if lower but slowly increment if higher
                // This means if theres a difference it'll eventually correct itself 

                float health = m_healthBar.HealthLevel;
                if(m_health <= health)
                {
                    m_healthBar.SetHealthLevel(m_health);
                }
                else
                {
                    float difference = m_health - health;
                    float addSpeed = Time.deltaTime * 0.05f;
                    float incrementingHealth = health + (difference * addSpeed);
                    m_healthBar.SetHealthLevel(incrementingHealth);
                }
            }
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
                m_networkDiagnostics.GetComponent<SpriteRenderer>().color = m_playerColor;
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

            Diagnostics.Add("Player" + m_playerID, m_playerName + 
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
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(m_rigidBody.velocity);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(m_playerName);
            stream.SendNext(m_playerID);
            stream.SendNext(m_playerScore);
            stream.SendNext(m_playerHue);
            stream.SendNext(m_health);
            stream.SendNext(m_mouseCursorAngle);
            stream.SendNext(m_firedCannonsLeft);
            stream.SendNext(m_firedCannonsRight);
            stream.SendNext(m_visible);

            m_firedCannonsRight = false;
            m_firedCannonsLeft = false;
        }
        else
        {
            m_requiresPositionUpdate = true;

            var velocity = (Vector3)stream.ReceiveNext();
            m_networkedPosition = (Vector3)stream.ReceiveNext();
            m_networkedRotation = (Quaternion)stream.ReceiveNext();
            m_playerPrediction.OnNetworkUpdate(m_networkedPosition, m_networkedRotation, velocity);

            m_playerName = (string)stream.ReceiveNext();
            m_playerID = (int)stream.ReceiveNext();
            m_playerScore = (int)stream.ReceiveNext();
            m_playerHue = (int)stream.ReceiveNext();
            m_health = (float)stream.ReceiveNext();
            m_mouseCursorAngle = (float)stream.ReceiveNext();

            bool firedCannonsLeft = (bool)stream.ReceiveNext();
            if(firedCannonsLeft)
            {
                m_firedCannonsLeft = true;
            }

            bool firedCannonsRight = (bool)stream.ReceiveNext();
            if(firedCannonsRight)
            {
                m_firedCannonsRight = true;
            }

            bool isVisible = (bool)stream.ReceiveNext();
            if(isVisible != m_visible)
            {
                SetVisible(isVisible, !isVisible);
            }

            // On first recieve valid data
            if(m_initialised && !m_recievedValidData && m_playerName.Length > 0)
            {
                m_recievedValidData = true;
                name = m_playerID.ToString();
                m_rigidBody.velocity = Vector3.zero;
                transform.rotation = m_networkedRotation;
                transform.position = m_networkedPosition;
                NotifyPlayerCreation();
            }
        }
    }

    /// <summary>
    /// Attempts to predict where the non-client player would be to reduce network latency
    /// </summary>
    void PositionNonClientPlayer()
    {
        if(!m_usePrediction)
        {
            transform.position = Vector3.Lerp(
                transform.position, m_networkedPosition, Time.deltaTime * 5);
            
            transform.rotation = Quaternion.Lerp(
                transform.rotation, m_networkedRotation, Time.deltaTime * 5);
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
        get { return m_playerName; }
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

    /// <summary>
    /// Gets the player Color
    /// </summary>
    public Color PlayerColor
    {
        get { return m_playerColor; }
    }

    /// <summary>
    /// Returns whether the player can control this
    /// </summary>
    public bool IsControllable()
    {
        return photonView.isMine;
    }

    /// <summary>
    /// Whether the player is fully initialised
    /// </summary>
    public bool IsInitialised()
    {
        return IsControllable() ? m_initialised : 
            m_initialised && m_recievedValidData;
    }
}
