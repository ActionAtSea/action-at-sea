using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionGenerator : MonoBehaviour 
{
	public GameObject explosionAnimation;
	private List<GameObject> explosions = new List<GameObject> ();
	private SoundEffectHandler soundEffects;

	/**
	* Finds an unused explosion and activates it
	*/
	public void PlaceExplosion(Vector3 position)
	{
		for(int i = 0; i < explosions.Count; ++i)
		{
			if(!explosions[i].activeSelf)
			{
				position.z = explosionAnimation.transform.position.z;
				explosions[i].SetActive(true);
				explosions[i].transform.position = position;
				explosions[i].GetComponent<Animator>().enabled = true;
				soundEffects.PlayOnExplode();
				return;
			}
		}

		Debug.LogError ("Could not find inactive explosion sprite to use");
	}

	/**
	* Creates one explosion per explodable enemy
	*/
	void Start () 
	{
		soundEffects = FindObjectOfType<SoundEffectHandler>();
		if (!soundEffects)
		{
			Debug.Log("SoundEffectHandler could not be found in scene.");
		}

		var count = GameObject.FindGameObjectsWithTag ("Enemy").Length / 2;
		for(int i = 0; i < count; ++i)
		{
			if(i == 0)
			{
				explosions.Add (explosionAnimation);
			}
			else
			{
				explosions.Add((GameObject)(Instantiate(explosionAnimation)));
			}

			explosions[i].name = "Explosion" + i.ToString();
			explosions[i].transform.parent = this.transform;

			explosions[i].SetActive(false);
			explosions[i].GetComponent<Animator>().enabled = false;
		}
	}

	/**
	* Updates the sprite animation for the explosion
	*/
	void Update () 
	{
		for(int i = 0; i < explosions.Count; ++i)
		{
			if(explosions[i].activeSelf)
			{
				if(explosions[i].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Exit"))
				{
					explosions[i].SetActive(false);
					explosions[i].GetComponent<Animator>().CrossFade("Base Layer.Entry", 0.0f);
					explosions[i].GetComponent<Animator>().enabled = false;
				}
			}
		}	
	}
}
