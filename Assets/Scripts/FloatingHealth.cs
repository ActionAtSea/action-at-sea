////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FloatingHealth.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class FloatingHealth : MonoBehaviour 
{
    public GameObject player;
    public float height = 3.0f;

    /// <summary>
    /// Moves a floating health bar alongside the game object
    /// </summary>
    void Update () 
    {
        transform.localPosition = new Vector3 (
            player.transform.localPosition.x,
            height,
            player.transform.localPosition.z);
    }
}
