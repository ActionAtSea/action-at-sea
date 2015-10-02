////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandDiscoveryNode.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandDiscoveryNode : MonoBehaviour
{ 
    private int m_ownerID = 0;
    private GameObject m_owner = null;
    private double m_timestamp = 0.0;
    private SpriteRenderer m_renderer = null;
    private Color m_unownedColor = new Color(1.0f, 1.0f, 1.0f);

    /// <summary>
    /// Initialises the node
    /// </summary>
    void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Checks whether the owner is still valid
    /// </summary>
    void Update()
    {
        if(m_owner == null)
        {
            m_renderer.color = m_unownedColor;
            m_ownerID = -1;
            m_timestamp = 0.0;
        }
    }

    /// <summary>
    /// On collision with a player
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        SetOwner(other.gameObject);
    }

    /// <summary>
    /// The server time can overflow to 0. Adjust the timestamp if needed
    /// Assumes the server time never goes negative
    /// </summary>
    public void ResetTimestamp(double time)
    {
        if(m_owner != null && m_timestamp > time)
        {
            m_timestamp -= double.MaxValue; 
        }
    }

    /// <summary>
    /// Sets the owner of this node only if the timestamp is greater
    /// </summary>
    public void TrySetOwner(int ownerID, double timestamp)
    {
        if(ownerID != -1 && (m_owner == null || timestamp > m_timestamp))
        {
            GameObject player = PlayerManager.GetPlayerWithID(ownerID);
            if(player != null)
            {
                SetOwner(player);
                m_timestamp = timestamp;
            }
        }
    }

    /// <summary>
    /// Sets the owner of this node
    /// </summary>
    private void SetOwner(GameObject owner)
    {
        // If a valid owner to set
        if(owner != null && PlayerManager.IsPlayer(owner) && NetworkedPlayer.IsInitialised(owner))
        {
            // If doesn't have an owner or is a different owner
            if(m_owner == null || m_owner.name != owner.name)
            {
                m_renderer.color = NetworkedPlayer.GetPlayerColor(owner);
                SoundManager.Get().PlaySound(SoundManager.SoundID.ISLAND_NODE);

                m_owner = owner;
                m_ownerID = NetworkedPlayer.GetPlayerID(owner);
                m_timestamp = GameModeManager.Get().GetTimePassed();

                Debug.Log(m_owner.name + " grabbed a new node");
            }
        }
    }
   
    /// <summary>
    /// Gets the owner of this node
    /// </summary>
    public GameObject Owner
    {
        get { return m_owner; }
    }

    /// <summary>
    /// Gets the timestep the node was owned at
    /// </summary>
    public double TimeStamp
    {
        get { return (double)m_timestamp; }
    }

    /// <summary>
    /// Gets the ID of the node
    /// </summary>
    public int OwnerID
    {
        get { return m_ownerID; }
    }
}