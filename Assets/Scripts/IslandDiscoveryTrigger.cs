using UnityEngine;
using System.Collections;

public class IslandDiscoveryTrigger : MonoBehaviour
{
    public float scoreValue = 20.0f;
    

    private IslandDiscoveryNode[] nodes;
    private bool islandDiscovered = false;
    private bool scoreAwarded = false;
	private PlayerScore scoreController = null;
    private SpriteRenderer renderer;
	private SoundEffectHandler soundEffects;

    // Use this for initialization
    void Start()
    {
		soundEffects = FindObjectOfType<SoundEffectHandler>();
		if (!soundEffects)
		{
			Debug.Log("SoundEffectHandler could not be found in scene.");
		}

        nodes = GetComponentsInChildren<IslandDiscoveryNode>();
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
    }

    // Update is called once per frame
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
		return renderer.enabled;
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
            //Debug.Log("Island Discovered");
            renderer.enabled = true;
            scoreController.AddScore(scoreValue);
			soundEffects.PlayOnIslandFind();
            scoreAwarded = true;
        }
    }
}