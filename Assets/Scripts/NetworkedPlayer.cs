////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkedPlayer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour 
{    
    public PhotonView photonView = null;

    private bool m_addedToMap = false;
    private string m_playerName = "unnamed";
    private int m_playerID = -1;
    private int m_playerScore = 0;
    private Vector3 m_correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion m_correctPlayerRot = Quaternion.identity; // We lerp towards this
    private float m_healthLevel = -1.0f;
    private bool m_connected = false;
    public Color m_playerColor = new Color(1.0f, 1.0f, 1.0f);

    /// <summary>
    /// On instantiate of the player by photon networking
    /// </summary>
    void Start()
    {
        if(photonView.isMine)
        {
            int index = NetworkMatchmaker.Get().GetPlayerIndex();
            m_playerID = NetworkMatchmaker.Get().GetPlayerID();
            m_playerName = GameInformation.GetPlayerName();
            Debug.Log("Created Player Ship: [" + m_playerID + "] [" + index + "]");

            PlayerManager.Placement place = PlayerManager.Get().GetNewPosition(index, gameObject);

            m_playerColor = place.color;
            gameObject.transform.position = place.position;
            gameObject.transform.localEulerAngles = place.rotation;
            gameObject.tag = "Player";
            gameObject.name = m_playerID.ToString() + "(Player)";

            transform.parent.FindChild("FloatingHealthBar").gameObject.SetActive(false);
           
        }
        else
        {
            Debug.Log("Created Enemy Ship");
        }

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
            stream.SendNext(m_playerID);
            stream.SendNext(m_playerScore);
            stream.SendNext(m_playerColor.r);
            stream.SendNext(m_playerColor.g);
            stream.SendNext(m_playerColor.b);
        }
        else
        {
            // Network player, receive data
            m_connected = (bool)stream.ReceiveNext();
            m_correctPlayerPos = (Vector3)stream.ReceiveNext();
            m_correctPlayerRot = (Quaternion)stream.ReceiveNext();
            m_healthLevel = (float)stream.ReceiveNext();
            m_playerName = (string)stream.ReceiveNext();
            m_playerID = (int)stream.ReceiveNext();
            m_playerScore = (int)stream.ReceiveNext();
            name = m_playerID.ToString();
            m_playerColor.r = (float)stream.ReceiveNext();
            m_playerColor.g = (float)stream.ReceiveNext();
            m_playerColor.b = (float)stream.ReceiveNext();
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
