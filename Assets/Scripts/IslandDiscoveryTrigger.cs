////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandDiscoveryTrigger.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandDiscoveryTrigger : MonoBehaviour
{
    public float scoreValue = 20.0f;
    public UnityEngine.UI.Image tickImage = null;
    public UnityEngine.UI.Text ownerText = null;

    private IslandDiscoveryNode[] m_nodes;
    private GameObject m_owner = null;
    private List<SpriteRenderer> m_islands = new List<SpriteRenderer>();

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_nodes = transform.parent.GetComponentsInChildren<IslandDiscoveryNode>();

        var islands = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        foreach(var island in islands)
        {
            if(island.CompareTag("Island"))
            {
                m_islands.Add(island);
            }
        }

        if(m_islands.Count == 0)
        {
            Debug.LogError("No associated island sprite");
        }
        if(m_nodes.Length == 0)
        {
            Debug.LogError("No associated nodes");
        }

        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// Checks whether the island has been discovered
    /// </summary>
    void Update()
    {
        GameObject owner = m_nodes[0].Owner;
        for(int i = 1; i < m_nodes.Length; ++i)
        {
            if(owner == null || 
               m_nodes[i].Owner == null ||
               owner.name != m_nodes[i].Owner.name)
            {
                if(GetComponent<Canvas>().enabled)
                {
                    SetCaptured(null);
                }
                return;
            }
        }

        if(owner == null)
        {
            SetCaptured(null);
        }
        else if(m_owner == null || m_owner.name != owner.name)
        {
            Debug.Log("Setting new owner of island: " + owner.name);
            SetCaptured(owner);
        }
    }

    /// <summary>
    /// Sets whether the island is captured
    /// </summary>
    void SetCaptured(GameObject owner)
    {
        GetComponent<Canvas>().enabled = owner != null;

        if(owner != null)
        {
            tickImage.color = NetworkedPlayer.GetPlayerColor(owner);
            ownerText.text = NetworkedPlayer.GetPlayerName(owner);

            SoundManager.Get().PlaySound(SoundManager.SoundID.ISLAND_FIND);

            var player = PlayerManager.GetControllablePlayer();
            if(player != null && player.name == owner.name)
            {
                owner.GetComponent<PlayerScore>().AddScore(scoreValue);
            }
        }
        else
        {
            tickImage.color = new Color(1.0f, 1.0f, 1.0f);
            ownerText.text = "";
        }

        foreach(var island in m_islands)
        {
            island.color = tickImage.color;
        }

        m_owner = owner;
    }

    /// <summary>
    /// Returns whether this island has been discovered
    /// </summary>
    public bool IsDiscovered()
    {
        return GetComponent<SpriteRenderer>().enabled;
    }
}