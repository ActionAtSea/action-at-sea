////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandDiscoveryNode.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class IslandDiscoveryNode : MonoBehaviour
{ 
    private GameObject m_owner = null;

    /// <summary>
    /// On collision with a player
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        SetOwner(other.gameObject);
    }

    /// <summary>
    /// Sets the owner of this node
    /// </summary>
    public void SetOwner(GameObject owner)
    {
        if(m_owner == null || m_owner.name != owner.name)
        {
            GetComponent<SpriteRenderer>().color = NetworkedPlayer.GetPlayerColor(owner);
            SoundManager.Get().PlaySound(SoundManager.SoundID.ISLAND_NODE);
            m_owner = owner;
        }
    }

    /// <summary>
    /// Gets the owner of this node
    /// </summary>
    public GameObject Owner
    {
        get { return m_owner; }
    }
}