////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkedPlayer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour 
{    
    public PhotonView photonView = null;

    private bool m_addedToMap = false;
    private static int sm_playerIDCounter = 0;
    private string m_playerName = "unnamed";
    private string m_playerID = "";
    private int m_playerScore = 0;
    private Vector3 m_correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion m_correctPlayerRot = Quaternion.identity; // We lerp towards this
    private float m_healthLevel = -1.0f;
    private bool m_connected = false;

    /// <summary>
    /// On instantiate of the player by photon networking
    /// </summary>
    void Start()
    {
        if(!photonView.isMine)
        {
            sm_playerIDCounter++;
            m_playerID = "Enemy" + sm_playerIDCounter.ToString();
        }
        else
        {
            m_playerID = "Player";
            gameObject.tag = "Player";
            gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
            
            PlayerPlacer.Placement place = FindObjectOfType<PlayerPlacer>().GetNewPosition(gameObject);
            gameObject.transform.position = place.position;
            gameObject.transform.localEulerAngles = place.rotation;
            
            GameObject healthbar = transform.parent.FindChild("FloatingHealthBar").gameObject;
            healthbar.SetActive(false);
        }

        name = m_playerID;
        Debug.Log("Created ship: " + name);
        DontDestroyOnLoad(gameObject.transform.parent);
    }

    /// <summary>
    /// On destroy
    /// </summary>
    void OnDestroy()
    {
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
        if(!m_addedToMap)
        {
            // Minimap isn't initialised straight away for non-client players
            var minimap = GameObject.FindObjectOfType<Minimap>();
            if(minimap != null)
            {
                minimap.AddPlayer(gameObject, photonView.isMine);
                m_addedToMap = true;
            }
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
