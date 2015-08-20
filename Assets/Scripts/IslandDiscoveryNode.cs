using UnityEngine;
using System.Collections;

public class IslandDiscoveryNode : MonoBehaviour
{
    public Sprite altSprite;
    private bool discovered = false;
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

        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            //Debug.Log("yes");
            renderer.sprite = altSprite;

			if(!discovered)
			{
				soundEffects.PlayOnIslandNode();
			}
			discovered = true;
        }
    }

    public bool Discovered
    {
        get { return discovered; }
    }
}