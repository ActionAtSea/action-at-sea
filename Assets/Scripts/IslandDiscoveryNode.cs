////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandDiscoveryNode.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class IslandDiscoveryNode : MonoBehaviour
{
    public Sprite altSprite;

    private bool m_discovered = false;

    /// <summary>
    /// On collision with a player
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            GetComponent<SpriteRenderer>().sprite = altSprite;

            if(!m_discovered)
            {
                SoundManager.Get().PlaySound(SoundManager.SoundID.ISLAND_NODE);
            }
            m_discovered = true;
        }
    }

    /// <summary>
    /// Gets whether this node has been discovered
    /// </summary>
    public bool Discovered
    {
        get { return m_discovered; }
    }
}