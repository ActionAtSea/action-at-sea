////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandDiscoveryTrigger.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class IslandDiscoveryTrigger : MonoBehaviour
{
    public float scoreValue = 20.0f;

    private IslandDiscoveryNode[] m_nodes;
    private bool m_islandDiscovered = false;
    private bool m_scoreAwarded = false;

    /**
    * Initialises the script
    */
    void Start()
    {
        m_nodes = GetComponentsInChildren<IslandDiscoveryNode>();
        GetComponent<SpriteRenderer>().enabled = false;
    }

    /**
    * Checks whether the island has been discovered
    */
    void Update()
    {
        if (!m_islandDiscovered)
        {
            foreach (IslandDiscoveryNode n in m_nodes)
            {
                if (n.Discovered != true)
                {
                    return;
                }
            }
            m_islandDiscovered = true;
        }
        else if(!m_scoreAwarded)
        {
            var player = PlayerManager.GetControllablePlayer();
            if(player == null)
            {
                Debug.LogError("Could not find player");
            }
            else
            {
                GetComponent<SpriteRenderer>().enabled = true;
                player.GetComponent<PlayerScore>().AddScore(scoreValue);
                SoundManager.Get().PlaySound(SoundManager.SoundID.ISLAND_FIND);
                m_scoreAwarded = true;
            }
        }
    }

    /**
    * Returns whether this island has been discovered
    */
    public bool IsDiscovered()
    {
        return GetComponent<SpriteRenderer>().enabled;
    }
}