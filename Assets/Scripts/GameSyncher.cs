////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameSyncher.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// NOTE: Instantiated by Photon Networking
/// Start() cannot include any code relying on the world/level as 
/// this object can be instantiated before the level is created
/// </summary>
public class GameSyncher : MonoBehaviour
{ 
    public PhotonView photonView = null;
    private GameModeManager m_gameManager = null;
    private NetworkMatchmaker m_network = null;
    private List<IslandDiscoveryNode> m_nodes;
    private float m_networkedTimePassed = 0.0f;
    private GameState m_networkedState = GameState.NONE;
    private double m_previousServerTime = 0.0;
    private float m_startUpTime = 0.0f;
    private bool m_initialised = false;

    /// <summary>
    /// Initilaises the game syncher
    /// Code relying not on the world goes here
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(gameObject); // Photon networking controls this
    }

    /// <summary>
    /// Initilaises the game syncher
    /// Code relying on the world goes here
    /// </summary>
    void Initialise()
    {
        m_nodes = Utilities.GetOrderedList<IslandDiscoveryNode>();
        if(m_nodes.Count == 0)
        {
            Debug.LogError("Could not find any island nodes");
        }

        m_network = NetworkMatchmaker.Get();
        m_gameManager = GameModeManager.Get();
        m_startUpTime = Time.time;
        m_initialised = true;
    }

    /// <summary>
    /// Updates the syncher
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
            m_networkedTimePassed = Time.time - m_startUpTime;
        }

        m_gameManager.TrySetTimePassed(m_networkedTimePassed);
        m_gameManager.TrySetState(m_networkedState);

        AdjustTimestamps();
    }

    /// <summary>
    /// Adjusts the timestamps if the server time overflows and reset to 9
    /// </summary>
    void AdjustTimestamps()
    {
        double serverTime = m_network.GetTime();
        if(serverTime < m_previousServerTime)
        {
            for(int i = 0; i < m_nodes.Count; ++i)
            {
                m_nodes[i].ResetTimestamp(serverTime);
            }
        }
        m_previousServerTime = serverTime;
    }

    /// <summary>
    /// Serialises data to each player
    /// Note not called if only player in the room
    /// </summary>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!m_initialised)
        {
            return;
        }

        if (stream.isWriting)
        {
            for(int i = 0; i < m_nodes.Count; ++i)
            {
                stream.SendNext(m_nodes[i].OwnerID);
                stream.SendNext(m_nodes[i].TimeStamp);
                stream.SendNext(m_networkedTimePassed);
                stream.SendNext((int)m_gameManager.GetState());
            }
        }
        else
        {
            for(int i = 0; i < m_nodes.Count; ++i)
            {
                int ownerID = (int)stream.ReceiveNext();
                double timestamp = (double)stream.ReceiveNext();
                m_networkedTimePassed = (float)stream.ReceiveNext();
                m_networkedState = (GameState)stream.ReceiveNext();
                m_nodes[i].TrySetOwner(ownerID, timestamp);
            }
        }
    }
}


