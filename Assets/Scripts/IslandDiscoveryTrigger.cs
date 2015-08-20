////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandDiscoveryTrigger.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class IslandDiscoveryTrigger : MonoBehaviour
{
    public float scoreValue = 20.0f;

    private IslandDiscoveryNode[] nodes;
    private bool islandDiscovered = false;
    private bool scoreAwarded = false;
    private PlayerScore scoreController = null;
	private SoundManager soundEffects;
	
    void Start()
    {
		soundEffects = FindObjectOfType<SoundManager>();
        if (!soundEffects)
        {
            Debug.Log("SoundEffectHandler could not be found in scene.");
        }

        nodes = GetComponentsInChildren<IslandDiscoveryNode>();
		GetComponent<SpriteRenderer>().enabled = false;
    }
	
    void Update()
    {
        if(scoreController == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            {
                scoreController = player.GetComponent<PlayerScore>();
            }
            return;
        }

        CheckIfDiscovered();
    }

    public bool IsDiscovered()
    {
		return GetComponent<SpriteRenderer>().enabled;
    }

    void CheckIfDiscovered()
    {
        if (!islandDiscovered)
        {
            foreach (IslandDiscoveryNode n in nodes)
            {
                if (n.Discovered != true)
                {
                    return;
                }
            }
            islandDiscovered = true;
        }
        else if(!scoreAwarded)
        {
			GetComponent<SpriteRenderer>().enabled = true;
            scoreController.AddScore(scoreValue);
			soundEffects.PlaySound(SoundManager.SoundID.ISLAND_FIND);
            scoreAwarded = true;
        }
    }
}