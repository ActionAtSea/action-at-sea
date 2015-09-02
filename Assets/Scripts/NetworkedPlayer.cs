////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkedPlayer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour 
{    
    public PhotonView photonView = null;

    private bool m_initialised = false;
    private static int sm_playerIDCounter = 0;
    private string m_playerName = "unnamed";
    private string m_playerID = "";
    private int m_playerScore = 0;
    private Vector3 m_correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion m_correctPlayerRot = Quaternion.identity; // We lerp towards this
    private float m_healthLevel = -1.0f;
    private bool m_connected = false;
    private GameObject m_playerPrefab = null;

    /// <summary>
    /// Updates the player from the networked data
    /// </summary>
    void Update()
    {
        if(!m_initialised)
        {
            if(!photonView.isMine)
            {
                sm_playerIDCounter++;
                m_playerID = "Enemy" + sm_playerIDCounter.ToString();
            }
            else
            {
                m_playerID = "Player";
            }

            name = m_playerID;
            Debug.Log("Created player: " + name);

            var minimap = GameObject.FindObjectOfType<Minimap>();
            if(minimap == null)
            {
                Debug.LogError("Could not find minimap");
            }
            minimap.AddPlayer(gameObject, photonView.isMine);

            m_initialised = true;
            m_playerPrefab = transform.parent.gameObject;
        }

        if (!photonView.isMine)
        {
            if(m_connected)
            {
                transform.position = Vector3.Lerp(
                    transform.position, m_correctPlayerPos, Time.deltaTime * 5);

                transform.rotation = Quaternion.Lerp(
                    transform.rotation, m_correctPlayerRot, Time.deltaTime * 5);

                if(m_healthLevel >= 0)
                {
                    GetComponent<Health>().SetHealthLevel(m_healthLevel);
                    if(!GetComponent<Health>().IsAlive)
                    {
                        AnimationGenerator.Get().PlayAnimation(
                            transform.position, AnimationGenerator.ID.EXPLOSION);

                        Destroy(m_playerPrefab);
                    }
                }
            }
        }
        else
        {
            m_playerScore = (int)(GetComponent<PlayerScore>().RoundedScore);
            m_playerName = GameInformation.GetPlayerName();
            m_healthLevel = GetComponent<Health>().HealthLevel;
        }
    }

    /// <summary>
    /// Serialises player data to each player
    /// </summary>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            m_connected = true;
            stream.SendNext(m_connected);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(m_healthLevel);
            stream.SendNext(m_playerName);
            stream.SendNext(m_playerScore);
        }
        else
        {
            // Network player, receive data
            m_connected = (bool)stream.ReceiveNext();
            m_correctPlayerPos = (Vector3)stream.ReceiveNext();
            m_correctPlayerRot = (Quaternion)stream.ReceiveNext();
            m_healthLevel = (float)stream.ReceiveNext();
            m_playerName = (string)stream.ReceiveNext();
            m_playerScore = (int)stream.ReceiveNext();
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
    public string PlayerID
    {
        get { return m_playerID; }
    }

    /// <summary>
    /// Returns the player Name
    /// </summary>
    static public string GetPlayerName(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedPlayer>().PlayerName;
    }

    /// <summary>
    /// Returns the player Score
    /// </summary>
    static public float GetPlayerScore(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedPlayer>().PlayerScore;
    }

    /// <summary>
    /// Returns the player ID
    /// </summary>
    static public string GetPlayerID(GameObject obj)
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

    /// <summary>
    /// Returns whether the player can control this
    /// </summary>
    public bool IsControllable()
    {
        return photonView.isMine;
    }
}
