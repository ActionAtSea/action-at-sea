////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerSpawn.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    private GameObject m_owner = null;

    /** 
    * Initialises the spawn point
    */
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
   
    /**
    * Gets/Sets the owner of the spawn
    */
    public GameObject Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }
}