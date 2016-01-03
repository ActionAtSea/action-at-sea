////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandDiscoveryNode.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandDiscoveryNode : MonoBehaviour
{ 
    private string m_nodeID;
    private IslandDiscoveryTrigger m_trigger = null;
    private int m_ownerID = 0;
    private GameObject m_owner = null;
    private bool m_hasOwner = false;
    private double m_timestamp = 0.0;
    private SpriteRenderer m_renderer = null;
    private Color m_unownedColor = new Color(1.0f, 1.0f, 1.0f);

    /// <summary>
    /// Setst the associated trigger
    /// </summary>
    public void SetTrigger(IslandDiscoveryTrigger trigger)
    {
        m_trigger = trigger;   
    }

    /// <summary>
    /// Initialises the node
    /// </summary>
    void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_nodeID = name.Replace("IslandDiscoveryNode", "");
        name = transform.parent.name + "|" + name;
    }

    /// <summary>
    /// Checks whether the owner is still valid
    /// </summary>
    void Update()
    {
        if(m_hasOwner && m_owner == null)
        {
            m_renderer.color = m_unownedColor;
            m_ownerID = -1;
            m_timestamp = 0.0;
            m_hasOwner = false;
        }
    }

    /// <summary>
    /// Diagnostics for the node
    /// </summary>
    void OnGUI()
    {
        if(Diagnostics.IsActive())
        {
            Vector2 position = Camera.main.WorldToScreenPoint(transform.position);

            GUILayout.BeginArea(new Rect(
                (int)position.x, 
                Screen.height - (int)position.y,
                100, 100));

            GUILayout.TextArea("Node" + m_nodeID.ToString() + ": " +
                (m_owner == null ? "-" : m_owner.name) + 
                "\n" + m_timestamp.ToString());

            GUILayout.EndArea ();
        }
    }

    /// <summary>
    /// On collision with a player
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Prevent this player from taking nodes on an already discovered island
        // We don't try to prevent any node taking given from the network
        var state = Utilities.GetGameState();
        if((!m_trigger.IsDiscovered() && state == GameState.STAGE_1) || state != GameState.STAGE_1)
        {
            //Checks if object is a player.
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("EnemyPlayer"))
            {
                SetOwner(other.gameObject, Utilities.GetNetworking().GetTime());
            }
            else if(other.gameObject.CompareTag("AIShip"))
            {
                if (other.GetComponent<NetworkedAI>().aiType == NetworkedAI.AIType.PATROL)
                {
                    SetOwner(PlayerManager.GetPlayerWithID(other.GetComponent<PatrolAI>().OwnerPlayerID), Utilities.GetNetworking().GetTime());
                }
            }
        }
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
                SetOwner(player, timestamp);
            }
        }
    }

    /// <summary>
    /// Sets the owner of this node
    /// </summary>
    private void SetOwner(GameObject owner, double timestamp)
    {
        // If a valid owner to set
        if(owner != null && PlayerManager.IsPlayer(owner) && Utilities.IsPlayerInitialised(owner))
        {
            if(m_owner == null || m_owner.name != owner.name)
            {
                if(PlayerManager.IsCloseToPlayer(owner.transform.position, 30.0f))
                {
                    SoundManager.Get().PlaySound(SoundManager.SoundID.ISLAND_NODE);
                }

                m_hasOwner = true;

                m_owner = owner;
                m_renderer.color = Utilities.GetPlayerColor(owner);
                m_ownerID = Utilities.GetPlayerID(owner);
                m_timestamp = timestamp;

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