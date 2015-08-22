////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkedPlayer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour 
{    
    public PhotonView photonView = null;

    private static int sm_playerIDCounter = 0;
    private string m_playerName = "unnamed";
    private string m_playerID = "";
    private int m_playerScore = 0;
    private Vector3 m_correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion m_correctPlayerRot = Quaternion.identity; // We lerp towards this
    private float m_healthLevel = -1.0f;
    private bool m_connected = false;
    private GameObject m_player = null;

    /**
    * Initialises the networked player
    */
    void Start()
    {
        if(m_playerID == "")
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
        }

        var score = GetComponentInChildren<PlayerScore>();
        if(score == null)
        {
            Debug.LogError("Could not find Player to network");
        }
        m_player = score.transform.gameObject;
    }

    /**
    * Updates the player from the networked data
    */
    void Update()
    {
        if (!photonView.isMine)
        {
            if(m_connected)
            {
                m_player.transform.position = Vector3.Lerp(
                    m_player.transform.position, m_correctPlayerPos, Time.deltaTime * 5);

                m_player.transform.rotation = Quaternion.Lerp(
                    m_player.transform.rotation, m_correctPlayerRot, Time.deltaTime * 5);

                if(m_healthLevel >= 0)
                {
                    m_player.GetComponent<Health>().SetHealthLevel(m_healthLevel);
                    if(!m_player.GetComponent<Health>().IsAlive)
                    {
                        AnimationGenerator.Get().PlayAnimation(
                            m_player.transform.position, AnimationGenerator.ID.EXPLOSION);

                        Destroy(gameObject);
                    }
                }
            }
        }
        else
        {
            m_playerScore = (int)(m_player.GetComponent<PlayerScore>().RoundedScore);
            m_playerName = GameInformation.GetPlayerName();
            m_healthLevel = m_player.GetComponent<Health>().HealthLevel;
        }
    }

    /**
    * Serialises player data to each player
    */
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            Debug.Log("Sending");

            // We own this player: send the others our data
            m_connected = true;
            stream.SendNext(m_connected);
            stream.SendNext(m_player.transform.position);
            stream.SendNext(m_player.transform.rotation);
            stream.SendNext(m_healthLevel);
            stream.SendNext(m_playerName);
            stream.SendNext(m_playerScore);
            stream.SendNext(m_playerID);
        }
        else
        {
            Debug.Log("Recieving");

            // Network player, receive data
            m_connected = (bool)stream.ReceiveNext();
            m_correctPlayerPos = (Vector3)stream.ReceiveNext();
            m_correctPlayerRot = (Quaternion)stream.ReceiveNext();
            m_healthLevel = (float)stream.ReceiveNext();
            m_playerName = (string)stream.ReceiveNext();
            m_playerScore = (int)stream.ReceiveNext();
            m_playerID = (string)stream.ReceiveNext();
        }
    }

    /**
    * Gets the player name
    */
    public string PlayerName
    {
        get { return m_playerName; }
    }

    /**
    * Gets the player score
    */
    public float PlayerScore
    {
        get { return m_playerScore; }
    }

    /**
    * Gets the player ID
    */
    public string PlayerID
    {
        get { return m_playerID; }
    }

    /**
    * Returns the player Name
    */
    static public string GetPlayerName(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedPlayer>().PlayerName;
    }

    /**
    * Returns the player Score
    */
    static public float GetPlayerScore(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedPlayer>().PlayerScore;
    }

    /**
    * Returns the player ID
    */
    static public string GetPlayerID(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedPlayer>().PlayerID;
    }

    /**
    * Returns whether the player can control this
    */
    static public bool IsControllable(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedPlayer>().IsControllable();
    }

    /**
    * Returns whether the player can control this
    */
    public bool IsControllable()
    {
        return photonView.isMine;
    }
}
