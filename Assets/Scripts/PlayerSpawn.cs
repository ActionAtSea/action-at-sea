////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerSpawn.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    private GameObject m_owner = null;

    /// <summary> 
    /// Initialises the spawn point
    /// </summary>
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
   
    /// <summary>
    /// Gets/Sets the owner of the spawn
    /// </summary>
    public GameObject Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }
}