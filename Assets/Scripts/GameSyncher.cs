////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameSyncher.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
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
    private IslandDiscoveryNode[] m_nodes;
    private float m_timePassed = 0.0f;
    private double m_previousServerTime = 0.0;
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
        m_nodes = FindObjectsOfType<IslandDiscoveryNode>();
        if(m_nodes == null || m_nodes.Length == 0)
        {
            Debug.LogError("Could not find island nodes for level");
        }
        
        m_network = NetworkMatchmaker.Get();
        m_gameManager = GameModeManager.Get();
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
            m_timePassed = Time.time;
        }
        m_gameManager.TrySetTimePassed(m_timePassed);

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
            for(int i = 0; i < m_nodes.Length; ++i)
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
            for(int i = 0; i < m_nodes.Length; ++i)
            {
                stream.SendNext(m_nodes[i].OwnerID);
                stream.SendNext(m_nodes[i].TimeStamp);
                stream.SendNext(m_timePassed);
            }
        }
        else
        {
            for(int i = 0; i < m_nodes.Length; ++i)
            {
                int ownerID = (int)stream.ReceiveNext();
                double timestamp = (double)stream.ReceiveNext();
                m_timePassed = (float)stream.ReceiveNext();
                m_nodes[i].TrySetOwner(ownerID, timestamp);
            }
        }
    }
}


