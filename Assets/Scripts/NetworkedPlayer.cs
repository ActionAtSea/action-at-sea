////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkedPlayer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NOTE: Instantiated by Photon Networking
/// Start() cannot include any code relying on the world/level as 
/// this object can be instantiated before the level is created
/// </summary>
public class NetworkedPlayer : MonoBehaviour 
{    
    public PhotonView photonView = null;
    public Color m_playerColor = new Color(1.0f, 1.0f, 1.0f);
    private string m_playerName = "";
    private int m_playerID = -1;
    private int m_playerScore = 0;
    private int m_playerIndex = -1; // Not sent over network
    private Vector3 m_correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion m_correctPlayerRot = Quaternion.identity; // We lerp towards this
    private float m_networkedHealth = -1.0f;
    private bool m_initialised = false;
    private bool m_recievedValidData = false;
    private Health m_healthBar = null;

    /// <summary>
    /// Initilaises the networked player
    /// Code not relying on the world goes here
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(transform.parent); // Photon networking controls this

        m_healthBar = GetComponent<Health>();
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
            Debug.Log("Created Player Ship");

            m_playerIndex = NetworkMatchmaker.Get().GetPlayerIndex();
            PlayerManager.Placement place = PlayerManager.Get().GetNewPosition(m_playerIndex, gameObject);
            m_playerColor = place.color;

            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.transform.position = place.position;
            gameObject.transform.localEulerAngles = place.rotation;

            m_playerID = NetworkMatchmaker.Get().GetPlayerID();
            m_playerName = Utilities.GetPlayerName();
            if(m_playerName.Length == 0)
            {
                // Name is used to determine when successful data is recieved and cannot be null
                m_playerName = Utilities.GetPlayerDefaultName();
            }

            gameObject.name = m_playerID.ToString();
            transform.parent.name = m_playerName + " (Client)";

            NotifyPlayerCreation();
        }
        else
        {
            gameObject.tag = "EnemyPlayer";
            Debug.Log("Created Enemy Ship");
        }

        var floatingHealth = transform.parent.FindChild("FloatingHealthBar");
        floatingHealth.gameObject.SetActive(!photonView.isMine);
            
        m_initialised = true;
    }

    /// <summary>
    /// Adds the player to the minimap and notifies its created
    /// </summary>
    void NotifyPlayerCreation()
    {
        var minimap = GameObject.FindObjectOfType<Minimap>();
        minimap.AddPlayer(gameObject, photonView.isMine, m_playerColor);
        PlayerManager.AddPlayer(gameObject);
    }

    /// <summary>
    /// On destroy called for both client and non-client controlled
    /// </summary>
    void OnDestroy()
    {
        PlayerManager.RemovePlayer(gameObject);
        Debug.Log("Destroying ship: " + name);

        // Will be null if leaving game
        var animationGenerator = FindObjectOfType<AnimationGenerator>();
        if(animationGenerator != null)
        {
            animationGenerator.PlayAnimation(
                transform.position, AnimationGenerator.ID.EXPLOSION);
        }
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

        if (!photonView.isMine)
        {
            if(m_recievedValidData)
            {
                transform.position = Vector3.Lerp(
                    transform.position, m_correctPlayerPos, Time.deltaTime * 5);

                transform.rotation = Quaternion.Lerp(
                    transform.rotation, m_correctPlayerRot, Time.deltaTime * 5);

                if(m_networkedHealth >= 0)
                {
                    // Only update health if networked version is lower
                    // This can mean however that networked version thinks its higher
                    // and the player can be running around seemingly empty
                    // Because of this, initially set if lower but slowly increment if higher
                    // This means if theres a difference it'll eventually correct itself 

                    float health = m_healthBar.HealthLevel;
                    if(m_networkedHealth <= health)
                    {
                        m_healthBar.SetHealthLevel(m_networkedHealth);
                    }
                    else
                    {
                        float difference = m_networkedHealth - health;
                        float addSpeed = Time.deltaTime * 0.05f;
                        float incrementingHealth = health + (difference * addSpeed);
                        m_healthBar.SetHealthLevel(incrementingHealth);
                    }
                }
            }
        }
        else
        {
            m_playerScore = (int)(GetComponent<PlayerScore>().RoundedScore);
            m_networkedHealth = m_healthBar.HealthLevel;
        }

        RenderDiagnostics();
    }

    /// <summary>
    /// Renders diagnostics
    /// </summary>
    void RenderDiagnostics()
    {
        if(Diagnostics.IsActive())
        {
            if(photonView.isMine)
            {
                Diagnostics.Add("Player Client Index", m_playerIndex);
            }

            Diagnostics.Add("Player" + m_playerID, m_playerName + 
                " [" + m_playerScore + "] [" + m_networkedHealth + "] " +
                (photonView.isMine ? "[Client] " : "[Enemy] "));
        }
    }

    /// <summary>
    /// Serialises player data to each player
    /// Note not called if only player in the room
    /// </summary>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(m_networkedHealth);
            stream.SendNext(m_playerName);
            stream.SendNext(m_playerID);
            stream.SendNext(m_playerScore);
            stream.SendNext(m_playerColor.r);
            stream.SendNext(m_playerColor.g);
            stream.SendNext(m_playerColor.b);
        }
        else
        {
            // Network player, receive data
            m_correctPlayerPos = (Vector3)stream.ReceiveNext();
            m_correctPlayerRot = (Quaternion)stream.ReceiveNext();
            m_networkedHealth = (float)stream.ReceiveNext();
            m_playerName = (string)stream.ReceiveNext();
            m_playerID = (int)stream.ReceiveNext();
            m_playerScore = (int)stream.ReceiveNext();
            m_playerColor.r = (float)stream.ReceiveNext();
            m_playerColor.g = (float)stream.ReceiveNext();
            m_playerColor.b = (float)stream.ReceiveNext();

            // On first recieve valid data
            if(!m_recievedValidData && m_playerName.Length > 0)
            {
                m_recievedValidData = true;
                name = m_playerID.ToString();

                transform.rotation = m_correctPlayerRot;
                transform.position = m_correctPlayerPos;

                NotifyPlayerCreation();
            }
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
        return m_initialised;
    }

    /// <summary>
    /// Whether the player is fully initialised
    /// </summary>
    static public bool IsInitialised(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedPlayer>().IsInitialised();
    }

    /// <summary>
    /// Returns the player Name
    /// </summary>
    static public string GetPlayerName(GameObject obj)
    {
        return obj != null ? obj.GetComponentInParent<NetworkedPlayer>().PlayerName : "";
    }

    /// <summary>
    /// Returns the player Score
    /// </summary>
    static public float GetPlayerScore(GameObject obj)
    {
        return obj != null ? obj.GetComponentInParent<NetworkedPlayer>().PlayerScore : 0.0f;
    }

    /// <summary>
    /// Returns the player color
    /// </summary>
    static public Color GetPlayerColor(GameObject obj)
    {
        return obj != null ? obj.GetComponentInParent<NetworkedPlayer>().PlayerColor : new Color();
    }

    /// <summary>
    /// Returns the player ID
    /// </summary>
    static public int GetPlayerID(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedPlayer>().PlayerID;
    }

    /// <summary>
    /// Returns whether the player can control this
    /// </summary>
    static public bool IsControllable(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedPlayer>().IsControllable();
    }
}
