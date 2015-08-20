using UnityEngine;
using System.Collections;

public class MenuMusicHandler : MonoBehaviour 
{
	public AudioSource menuMusicClip;
	public AudioSource menuButtonClick;

	private static AudioSource sharedMenuMusic = null; // Used to share across all scenes
	private static AudioSource sharedMenuClick = null; // Used to share across all scenes

	void Start () 
	{
		if (sharedMenuMusic == null && menuMusicClip != null)
		{
			sharedMenuMusic = Instantiate(menuMusicClip);
			sharedMenuClick = Instantiate(menuButtonClick);
			sharedMenuMusic.name = "SharedMenuMusic";
			sharedMenuClick.name = "SharedMenuButton";
			DontDestroyOnLoad(sharedMenuMusic);
			DontDestroyOnLoad(sharedMenuClick);
			StartPlaying();
		}
	}

	public void PlayButtonClick()
	{
		sharedMenuClick.Stop ();
		sharedMenuClick.Play ();
	}

	public void StopPlaying()
	{
		sharedMenuMusic.Stop();
	}

	public void StartPlaying()
	{
		sharedMenuMusic.Play();
	}
}
