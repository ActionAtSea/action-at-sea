////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FloatingHealth.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/// <summary>
/// NOTE: Instantiated by Photon Networking
/// Start() cannot include any code relying on the world/level as 
/// this object can be instantiated before the level is created
/// </summary>
public class FloatingHealth : MonoBehaviour 
{
    public GameObject player;
    public float height = 3.0f;

    /// <summary>
    /// Moves a floating health bar alongside the game object
    /// </summary>
    void Update () 
    {
        if(!Utilities.IsLevelLoaded())
        {
            return;
        }

        transform.localPosition = new Vector3 (
            player.transform.localPosition.x,
            height,
            player.transform.localPosition.z);
    }
}
