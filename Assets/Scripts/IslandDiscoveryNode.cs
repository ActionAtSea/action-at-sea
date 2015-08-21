////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandDiscoveryNode.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class IslandDiscoveryNode : MonoBehaviour
{
    public Sprite altSprite;
    private bool discovered = false;
    private SoundManager soundEffects;
    
    void Start()
    {
        soundEffects = FindObjectOfType<SoundManager>();
        if (!soundEffects)
        {
            Debug.Log("SoundEffectHandler could not be found in scene.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            GetComponent<SpriteRenderer>().sprite = altSprite;

            if(!discovered)
            {
                soundEffects.PlaySound(SoundManager.SoundID.ISLAND_NODE);
            }
            discovered = true;
        }
    }

    public bool Discovered
    {
        get { return discovered; }
    }
}