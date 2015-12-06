////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerSpawn.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    public bool isAISpawn = false;

    /// <summary> 
    /// Initialises the spawn point
    /// </summary>
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}