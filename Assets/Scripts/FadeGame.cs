using UnityEngine;
using System.Collections;

public class FadeGame : MonoBehaviour 
{
	private static GameObject fadeSprite = null;
	private static bool shouldFadeIn = false;
	private static bool shouldFadeOut = false;
	private bool firstTick = true;

	void Start () 
	{
		if(fadeSprite == null)
		{
			fadeSprite = Instantiate(this.gameObject);
			GetFadeSprite().enabled = true;
			GetFadeSprite().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			DontDestroyOnLoad(fadeSprite);
			fadeSprite.transform.SetParent (this.transform);
			shouldFadeOut = true;
		}
	}

	UnityEngine.UI.Image GetFadeSprite()
	{
		return fadeSprite.GetComponent<UnityEngine.UI.Image> ();
	}

	public void FadeIn()
	{
		shouldFadeOut = false;
		shouldFadeIn = true;
	}

	public void FadeOut()
	{
		shouldFadeIn = false;
		shouldFadeOut = true;
	}

	public bool IsFullyFadedIn()
	{
		return GetFadeSprite().color.a >= 1.0f;
	}
	
	void Update () 
	{
		if(fadeSprite != null && (shouldFadeIn || shouldFadeOut) && !firstTick)
		{
			const float fadeSpeed = 0.75f;
			float alpha = GetFadeSprite().color.a;

			if(shouldFadeOut)
			{
				alpha -= Time.deltaTime * fadeSpeed;
				if(alpha <= 0.0f)
				{
					alpha = 0.0f;
					shouldFadeOut = false;
				}
			}

			if(shouldFadeIn)
			{
				alpha += Time.deltaTime * fadeSpeed;
				if(alpha >= 1.0f)
				{
					alpha = 1.0f;
					shouldFadeIn = false;
				} 
			}

			GetFadeSprite().color = new Color(1.0f, 1.0f, 1.0f, alpha);
		}

		firstTick = false;
	}
}
