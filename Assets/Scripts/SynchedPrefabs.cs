////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkMatchmakerData.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchedPrefabs
{
    GameObject m_player = null;
    List<GameObject> m_playerFleetAIs = new List<GameObject>();
    GameObject m_syncher = null;
    bool m_initialised = false;

    /// <summary>
    /// Whether the prefabs have been initialised
    /// </summary>
    public bool IsInitialised()
    {
        return m_initialised;
    }

    /// <summary>
    /// Destroys the player on all connected clients
    /// </summary>
    public void Destroy()
    {
        if(m_player != null)
        {
            PhotonNetwork.Destroy(m_player);
            m_player = null;
        }

        if (m_playerFleetAIs.Count != 0)
        {
            for (int i = 0; i < m_playerFleetAIs.Count; ++i)
            {
                PhotonNetwork.Destroy(m_playerFleetAIs[i]);
            }
            m_playerFleetAIs.Clear();
        }

        if(m_syncher != null)
        {
            PhotonNetwork.Destroy(m_syncher);
            m_syncher = null;
        }

        m_initialised = false;
    }

    /// <summary>
    /// Creates a new player on all connected clients
    /// </summary>
    public void Create()
    {
        m_player = PhotonNetwork.Instantiate(
            "PlayerPVP", Vector3.zero, Quaternion.identity, 0);

        m_syncher = PhotonNetwork.Instantiate(
            "GameSyncher", Vector3.zero, Quaternion.identity, 0);
    
        if (!Utilities.IsOpenLeveL())
        {
            //TODO: Figure out how way to instantiate a patrol ship for each island within a level.
            //PhotonNetwork.InstantiateSceneObject("PatrolAIPhotonView", Vector3.zero, Quaternion.identity, 0, null);

            for (int i = 0; i < Utilities.sm_noOfFleetAIPerPlayer; ++i)
            {
                m_playerFleetAIs.Add(PhotonNetwork.Instantiate(
                    "FleetAIPhotonView", Vector3.zero, Quaternion.identity, 0));
            }

            int rogueAICount = PhotonNetwork.isMasterClient ? Utilities.GetAICount() : 0;
            for (int i = 0; i < rogueAICount; ++i)
            {
                PhotonNetwork.InstantiateSceneObject(
                    "RogueAIPhotonView", Vector3.zero, Quaternion.identity, 0, null);
            }
        }

        m_initialised = true;
    }
}