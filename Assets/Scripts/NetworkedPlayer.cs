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
    private string m_playerName = "unnamed";
    private int? m_playerID = null;
    private int m_playerScore = 0;
    private int m_playerIndex = -1; // Not sent over network
    private Vector3 m_correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion m_correctPlayerRot = Quaternion.identity; // We lerp towards this
    private float m_healthLevel = -1.0f;
    private bool m_initialised = false;
    private bool m_placedOnSpawn = false;

    /// <summary>
    /// Initilaises the networked player
    /// Code not relying on the world goes here
    /// </summary>
    void Start()
    {
        if(photonView.isMine)
        {
            m_playerIndex = NetworkMatchmaker.Get().GetPlayerIndex();
            PlayerManager.Placement place = PlayerManager.Get().GetNewPosition(m_playerIndex, gameObject);
            m_playerColor = place.color;
            gameObject.transform.position = place.position;
            gameObject.transform.localEulerAngles = place.rotation;
            gameObject.tag = "Player";
        }
        else
        {
            gameObject.tag = "EnemyPlayer";
        }
        DontDestroyOnLoad(transform.parent); // Photon networking controls this
    }

    /// <summary>
    /// Initilaises the networked player
    /// Code relying on the world goes here
    /// </summary>
    void Initialise()
    {
        if(photonView.isMine)
        {
            m_playerID = NetworkMatchmaker.Get().GetPlayerID();
            if(m_playerID != -1)
            {
                m_initialised = true;
                m_placedOnSpawn = true;

                m_playerName = Utilities.GetPlayerName();

                gameObject.name = m_playerID.ToString();
                transform.parent.name = m_playerName + " (Client)";

                NotifyPlayerCreation();
                Debug.Log("Created Player Ship");
            }
        }
        else
        {
            m_initialised = true;
            Debug.Log("Created Enemy Ship");
        }

        if(m_initialised)
        {
            var floatingHealth = transform.parent.FindChild("FloatingHealthBar").gameObject;
            floatingHealth.SetActive(!photonView.isMine);
        }
    }

    /// <summary>
    /// Adds the player to the minimap and notifies its created
    /// </summary>
    void NotifyPlayerCreation()
    {
        if(m_playerID == -1)
        {
            Debug.LogError("Player has not been created!");
        }

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
            if(m_playerID != null)
            {
                transform.position = Vector3.Lerp(
                    transform.position, m_correctPlayerPos, Time.deltaTime * 5);

                transform.rotation = Quaternion.Lerp(
                    transform.rotation, m_correctPlayerRot, Time.deltaTime * 5);

                if(m_healthLevel >= 0)
                {
                    GetComponent<Health>().SetHealthLevel(m_healthLevel);
                }
            }
        }
        else
        {
            m_playerScore = (int)(GetComponent<PlayerScore>().RoundedScore);
            m_healthLevel = GetComponent<Health>().HealthLevel;
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
                " [" + m_playerScore + "] [" + m_healthLevel + "] " +
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
            stream.SendNext(m_healthLevel);
            stream.SendNext(m_playerName);
            stream.SendNext(m_playerID == null ? -1 : (int)m_playerID);
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
            m_healthLevel = (float)stream.ReceiveNext();
            m_playerName = (string)stream.ReceiveNext();
            int playerID = (int)stream.ReceiveNext();
            m_playerScore = (int)stream.ReceiveNext();
            m_playerColor.r = (float)stream.ReceiveNext();
            m_playerColor.g = (float)stream.ReceiveNext();
            m_playerColor.b = (float)stream.ReceiveNext();

            // On first recieve valid data
            if(m_initialised && m_playerID == null && playerID != -1)
            {
                m_playerID = playerID;
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
        get { return m_playerID == null ? -1 : (int)m_playerID; }
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
        if(IsControllable())
        {
            return m_initialised && m_placedOnSpawn;
        }
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
