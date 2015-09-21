////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameSyncher.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSyncher : MonoBehaviour
{ 
    private List<IslandDiscoveryNode> m_nodes = null;
    public PhotonView photonView = null;

    /// <summary>
    /// Initialises the syncher
    /// </summary>
    void Start()
    {
        m_nodes = new List<IslandDiscoveryNode>();
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("IslandNode");

        for(int i = 0; i < nodes.Length; ++i)
        {
            m_nodes.Add(nodes[i].GetComponent<IslandDiscoveryNode>());
        }
    }
   
    /// <summary>
    /// Serialises data to each player
    /// </summary>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            for(int i = 0; i < m_nodes.Count; ++i)
            {
                stream.SendNext(m_nodes[i].OwnerID);
                stream.SendNext(m_nodes[i].TimeStamp);
            }
        }
        else
        {
            // Network player, receive data
            if(m_nodes != null)
            {
                for(int i = 0; i < m_nodes.Count; ++i)
                {
                    int ownerID = (int)stream.ReceiveNext();
                    double timestamp = (double)stream.ReceiveNext();
                    m_nodes[i].TrySetOwner(ownerID, timestamp);
                }
            }
        }
    }
}


