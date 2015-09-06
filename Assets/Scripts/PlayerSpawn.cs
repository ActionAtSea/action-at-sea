////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerSpawn.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    /// <summary> 
    /// Initialises the spawn point
    /// </summary>
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}