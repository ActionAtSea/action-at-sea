////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameSyncher.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSyncher : MonoBehaviour
{ 
    public PhotonView photonView = null;
    private GameModeManager m_gameManager = null;
    private List<IslandDiscoveryNode> m_nodes = null;
    private float m_timePassed = 0.0f;

    /// <summary>
    /// Initialises the syncher
    /// </summary>
    void Start()
    {
        // This is required as ships are initially 
        // created in the lobby until enough players are found
        DontDestroyOnLoad(gameObject);

        m_nodes = new List<IslandDiscoveryNode>();
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("IslandNode");
        for(int i = 0; i < nodes.Length; ++i)
        {
            m_nodes.Add(nodes[i].GetComponent<IslandDiscoveryNode>());
        }

        m_gameManager = GameModeManager.Get();
    }
   
    /// <summary>
    /// Updates the syncher
    /// </summary>
    void Update()
    {
        if(!photonView.isMine)
        {
            m_gameManager.TrySetTimePassed(m_timePassed);
        }
    }

    /// <summary>
    /// Serialises data to each player
    /// </summary>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(m_nodes == null)
        {
            return;
        }

        if (stream.isWriting)
        {
            for(int i = 0; i < m_nodes.Count; ++i)
            {
                stream.SendNext(m_nodes[i].OwnerID);
                stream.SendNext(m_nodes[i].TimeStamp);
                stream.SendNext(Time.time);
            }
        }
        else
        {
            for(int i = 0; i < m_nodes.Count; ++i)
            {
                int ownerID = (int)stream.ReceiveNext();
                double timestamp = (double)stream.ReceiveNext();
                m_timePassed = (float)stream.ReceiveNext();
                m_nodes[i].TrySetOwner(ownerID, timestamp);
            }
        }
    }
}


