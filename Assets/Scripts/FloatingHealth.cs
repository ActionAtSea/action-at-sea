////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FloatingHealth.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class FloatingHealth : MonoBehaviour 
{
    public GameObject player;
    private Vector3 offset = new Vector3();

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        offset.x = transform.localPosition.x;
        offset.y = transform.localPosition.y;
        offset.z = transform.localPosition.z;
    }

    /// <summary>
    /// Moves a floating health bar alongside the game object
    /// </summary>
    void Update () 
    {
        transform.localPosition = new Vector3 (
            offset.x + player.transform.localPosition.x,
            offset.y + player.transform.localPosition.y,
            offset.z);
    }
}
