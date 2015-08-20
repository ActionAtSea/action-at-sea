using UnityEngine;
using System.Collections;

public class BulletDestoryScript : MonoBehaviour 
{
	void OnEnable()
	{
		Invoke("DestroyOnSplash", 2f);
	}
		
	public void DestroyOnImpact()
	{
		if(gameObject.activeSelf)
		{
			gameObject.SetActive(false);
			FindObjectOfType<AnimationGenerator> ().PlaceHit(gameObject.transform.position);
		}
	}

	public void DestroyOnSplash()
	{
		if(gameObject.activeSelf)
		{
			gameObject.SetActive(false);
			FindObjectOfType<AnimationGenerator> ().PlaceSplash(gameObject.transform.position);
		}
	}

	void OnDisable()
	{
		CancelInvoke();
	}
}
