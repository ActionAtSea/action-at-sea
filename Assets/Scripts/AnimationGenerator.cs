using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationGenerator : MonoBehaviour 
{
	public GameObject explosionAnimation;
	public GameObject hitAnimation;
	public GameObject splashAnimation;

	public int explosionCount = 5;
	public int hitCount = 20;
	public int splashCount = 20;

	private List<GameObject> explosions = new List<GameObject> ();
	private List<GameObject> hits = new List<GameObject> ();
	private List<GameObject> splashes = new List<GameObject> ();
	private SoundEffectHandler soundEffects;

	bool IsCloseToPlayer(Vector3 position)
	{
		if(GameInformation.IsPVP())
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			if(player != null)
			{
				const float maxDistance = 30.0f;
				return (player.transform.position - position).magnitude <= maxDistance;
			}
		}
		return true;
	}
	
	public void PlaceExplosion(Vector3 position)
	{
		if(IsCloseToPlayer(position) && StartAnimation(position, explosions))
	    {
			soundEffects.PlayOnExplode();
		}
	}

	public void PlaceHit(Vector3 position)
	{
		if(IsCloseToPlayer(position) && StartAnimation(position, hits))
	    {
			soundEffects.PlayOnHit();
		}
	}

	public void PlaceSplash(Vector3 position)
	{
		if(IsCloseToPlayer(position) && StartAnimation(position, splashes))
	    {
			soundEffects.PlayOnSplash();
		}
	}

	bool StartAnimation(Vector3 position, List<GameObject> animations)
	{
		for(int i = 0; i < animations.Count; ++i)
		{
			if(!animations[i].activeSelf)
			{
				animations[i].SetActive(true);
				animations[i].transform.position = position;
				animations[i].GetComponent<Animator>().enabled = true;
				return true;
			}
		}
		return false;
	}
	
	void Start () 
	{
		soundEffects = FindObjectOfType<SoundEffectHandler>();
		if (!soundEffects)
		{
			Debug.Log("SoundEffectHandler could not be found in scene.");
		}

		CreateAnimations (explosionCount, explosions, explosionAnimation, "Explosion");
		CreateAnimations (hitCount, hits, hitAnimation, "Hit");
		CreateAnimations (splashCount, splashes, splashAnimation, "Splash");
	}

	void CreateAnimations(int count, List<GameObject> animations, GameObject animation, string name)
	{
		for(int i = 0; i < count; ++i)
		{
			if(i == 0)
			{
				animations.Add (animation);
			}
			else
			{
				animations.Add((GameObject)(Instantiate(animation)));
			}
			
			animations[i].name = name + i.ToString();
			animations[i].transform.parent = this.transform;
			
			animations[i].SetActive(false);
			animations[i].GetComponent<Animator>().enabled = false;
		}
	}

	void UpdateAnimations(List<GameObject> animations)
	{
		for(int i = 0; i < animations.Count; ++i)
		{
			if(animations[i].activeSelf)
			{
				if(animations[i].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Exit"))
				{
					animations[i].SetActive(false);
					animations[i].GetComponent<Animator>().CrossFade("Base Layer.Entry", 0.0f);
					animations[i].GetComponent<Animator>().enabled = false;
				}
			}
		}	
	}

	void Update () 
	{
		UpdateAnimations (explosions);
		UpdateAnimations (hits);
		UpdateAnimations (splashes);
	}
}
