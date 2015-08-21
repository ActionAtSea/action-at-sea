////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FloatingHealth.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class FloatingHealth : MonoBehaviour 
{
    public float xOffset = 1.0f;
    public float yOffset = 1.0f;
    public GameObject enemyShip;

    /**
    * Moves a floating health bar alongside the game object
    */
    void Update () 
    {
        if(enemyShip != null)
        {
            transform.localPosition = new Vector3 (
                enemyShip.transform.position.x + xOffset,
                enemyShip.transform.position.y + yOffset, 0.0f);
        }
    }
}
