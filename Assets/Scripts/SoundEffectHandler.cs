using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEffectHandler : MonoBehaviour 
{
	public int onHitInstanceCount = 10;
	public int onFireInstanceCount = 20;
	public int onExplodeInstanceCount = 5;
	public int onIslandFindCount = 2;
	public int onIslandNodeCount = 2;
	public int onRamInstanceCount = 1;
	public int onSplashInstanceCount = 10;

	public GameObject onIslandNodeSound;
	public GameObject onIslandFindSound;
	public GameObject onHitSound;
	public GameObject onFireSound;
	public GameObject onExplosionSound;
	public GameObject onSplashSound;
	public GameObject onRamSound;

	private bool onIslandFindPlayedThisTick = false;
	private bool onIslandNodePlayedThisTick = false;
	private bool onHitPlayedThisTick = false;
	private bool onFirePlayedThisTick = false;
	private bool onExplodePlayedThisTick = false;
	private bool onSplashPlayedThisTick = false;
	private bool onRamPlayedThisTick = false;

	private List<GameObject> onIslandFindSounds = new List<GameObject> ();
	private List<GameObject> onIslandNodeSounds = new List<GameObject> ();
	private List<GameObject> onHitSounds = new List<GameObject> ();
	private List<GameObject> onFireSounds = new List<GameObject> ();
	private List<GameObject> onExplosionSounds = new List<GameObject> ();
	private List<GameObject> onSplashSounds = new List<GameObject> ();
	private List<GameObject> onRamSounds = new List<GameObject> ();

	void Start () 
	{
		CreateInstances (onIslandNodeSounds, onIslandNodeSound, onIslandNodeCount);
		CreateInstances (onIslandFindSounds, onIslandFindSound, onIslandFindCount);
		CreateInstances (onExplosionSounds, onExplosionSound, onExplodeInstanceCount);
		CreateInstances (onFireSounds, onFireSound, onFireInstanceCount);
		CreateInstances (onHitSounds, onHitSound, onHitInstanceCount);
		CreateInstances (onRamSounds, onRamSound, onRamInstanceCount);
		CreateInstances (onSplashSounds, onSplashSound, onSplashInstanceCount);
	}

	public bool IsCloseToPlayer(Vector3 position)
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

	void CreateInstances(List<GameObject> soundEffects, GameObject baseSound, int instanceCount)
	{
		for(int i = 0; i < instanceCount; ++i)
		{
			if(i == 0)
			{
				soundEffects.Add(baseSound);
			}
			else
			{
				soundEffects.Add((GameObject)(Instantiate(baseSound)));
			}
			
			soundEffects[i].name = baseSound.name + i.ToString();
			soundEffects[i].transform.parent = this.transform;
			soundEffects[i].GetComponent<AudioSource>().Stop();
		}	
	}

	void PlaySoundEffect(List<GameObject> soundEffects)
	{
		for(int i = 0; i < soundEffects.Count; ++i)
		{
			if(!soundEffects[i].GetComponent<AudioSource>().isPlaying)
			{
				soundEffects[i].GetComponent<AudioSource>().Play();
				return;
			}
		}
	}

	void LateUpdate()
	{
		if(onIslandNodePlayedThisTick)
		{
			PlaySoundEffect (onIslandNodeSounds);
			onIslandNodePlayedThisTick = false;
		}

		if(onIslandFindPlayedThisTick)
		{
			PlaySoundEffect (onIslandFindSounds);
			onIslandFindPlayedThisTick = false;
		}

		if(onExplodePlayedThisTick)
		{
			PlaySoundEffect (onExplosionSounds);
			onExplodePlayedThisTick = false;
		}

		if(onFirePlayedThisTick)
		{
			PlaySoundEffect (onFireSounds);
			onFirePlayedThisTick = false;
		}

		if(onHitPlayedThisTick)
		{
			PlaySoundEffect (onHitSounds);
			onHitPlayedThisTick = false;
		}

		if(onSplashPlayedThisTick)
		{
			PlaySoundEffect (onSplashSounds);
			onSplashPlayedThisTick = false;
		}

		if(onRamPlayedThisTick)
		{
			PlaySoundEffect (onRamSounds);
			onRamPlayedThisTick = false;
		}
	}

	public void PlayOnRam()
	{
		onRamPlayedThisTick = true;
	}

	public void PlayOnSplash()
	{
		onSplashPlayedThisTick = true;
	}

	public void PlayOnIslandFind()
	{
		onIslandFindPlayedThisTick = true;
	}

	public void PlayOnIslandNode()
	{
		onIslandNodePlayedThisTick = true;
	}

	public void PlayOnExplode()
	{
		onExplodePlayedThisTick = true;
	}

	public void PlayOnFire()
	{
		onFirePlayedThisTick = true;
	}

	public void PlayOnHit()
	{
		onHitPlayedThisTick = true;
	}
}
