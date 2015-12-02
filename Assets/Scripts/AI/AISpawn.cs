using UnityEngine;
using System.Collections;

public class AISpawn : MonoBehaviour
{
    //Disables the sprite renderer for the spawn point.
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}