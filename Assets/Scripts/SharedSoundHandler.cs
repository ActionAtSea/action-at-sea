using UnityEngine;
using System.Collections;

public class SharedSoundHandler : MonoBehaviour 
{
	public AudioSource menuMusicClip = null;
	public AudioSource menuButtonClick = null;
	public AudioSource gameMusic = null;
	public AudioSource gameAmbience = null;
	public bool playMenuOnStart = false;
	public bool playGameOnStart = false;

	private static bool startMenuMusic = false;
	private static bool stopMenuMusic = false;
	private static bool startGameMusic = false;
	private static bool stopGameMusic = false;
	private static bool startGameAmbience = false;
	private static bool stopGameAmbience = false;

	private static float maxGameMusic = 0.0f;
	private static float maxGameAmbience = 0.0f;
	private static float maxMenuMusic = 0.0f;

	private static bool hasInitialised = false;
	private static AudioSource sharedMenuMusic = null; // Used to share across all scenes
	private static AudioSource sharedMenuClick = null; // Used to share across all scenes
	private static AudioSource sharedGameMusic = null; // Used to share across all scenes
	private static AudioSource sharedGameAmbience = null; // Used to share across all scenes

	void Start () 
	{
		if (!hasInitialised)
		{
			hasInitialised = true;

			sharedMenuMusic = Instantiate(menuMusicClip);
			sharedMenuClick = Instantiate(menuButtonClick);
			sharedMenuMusic.name = "SharedMenuMusic";
			sharedMenuClick.name = "SharedMenuButton";
			maxMenuMusic = sharedMenuMusic.volume;
			sharedMenuMusic.volume = 0.0f;
			DontDestroyOnLoad(sharedMenuMusic);
			DontDestroyOnLoad(sharedMenuClick);

			sharedGameMusic = Instantiate(gameMusic);
			sharedGameAmbience = Instantiate(gameAmbience);
			sharedGameMusic.name = "SharedGameMusic";
			sharedGameAmbience.name = "SharedGameAmbience";
			maxGameMusic = sharedGameMusic.volume;
			maxGameAmbience = sharedGameAmbience.volume;
			sharedGameMusic.volume = 0.0f;
			sharedGameAmbience.volume = 0.0f;
			DontDestroyOnLoad(sharedGameMusic);
			DontDestroyOnLoad(sharedGameAmbience);
		}

		if (playGameOnStart) 
		{
			StartPlayingGame();
		}

		if(playMenuOnStart)
		{
			StartPlayingMenu();
		}
	}

	void Update()
	{
		const float fadeSpeed = 0.25f;

		if(stopGameMusic)
		{
			sharedGameMusic.volume -= Time.deltaTime * fadeSpeed;
			if(sharedGameMusic.volume <= 0.0f)
			{
				sharedGameMusic.volume = 0.0f;
				sharedGameMusic.Stop();
				stopGameMusic = false;
			}
		}

		if(stopGameAmbience)
		{
			sharedGameAmbience.volume -= Time.deltaTime * fadeSpeed;
			if(sharedGameAmbience.volume <= 0.0f)
			{
				sharedGameAmbience.volume = 0.0f;
				sharedGameAmbience.Stop();
				stopGameAmbience = false;
			}
		}

		if(stopMenuMusic)
		{
			sharedMenuMusic.volume -= Time.deltaTime * fadeSpeed;
			if(sharedMenuMusic.volume <= 0.0f)
			{
				sharedMenuMusic.volume = 0.0f;
				sharedMenuMusic.Stop();
				stopMenuMusic = false;
			}
		}
		
		if(startGameMusic)
		{
			sharedGameMusic.volume += Time.deltaTime * fadeSpeed;
			if(sharedGameMusic.volume >= maxGameMusic)
			{
				sharedGameMusic.volume = maxGameMusic;
				startGameMusic = false;
			}
		}

		if(startGameAmbience)
		{
			sharedGameAmbience.volume += Time.deltaTime * fadeSpeed;
			if(sharedGameAmbience.volume >= maxGameAmbience)
			{
				sharedGameAmbience.volume = maxGameAmbience;
				startGameAmbience = false;
			}
		}

		if(startMenuMusic)
		{
			sharedMenuMusic.volume += Time.deltaTime * fadeSpeed;
			if(sharedMenuMusic.volume >= maxMenuMusic)
			{
				sharedMenuMusic.volume = maxMenuMusic;
				startMenuMusic = false;
			}
		}
	}

	public void PlayButtonClick()
	{
		if(sharedMenuClick != null)
		{
			sharedMenuClick.Stop ();
			sharedMenuClick.Play ();
		}
	}

	public void StopPlayingGame()
	{
		if(sharedGameMusic != null && sharedGameAmbience != null)
		{
			startGameMusic = false;
			stopGameMusic = true;
			startGameAmbience = false;
			stopGameAmbience = true;
		}
	}


	public void StartPlayingGame()
	{
		if(sharedGameMusic != null && !sharedGameMusic.isPlaying)
		{
			stopGameMusic = false;
			startGameMusic = true;
			stopGameAmbience = false;
			startGameAmbience = true;
			sharedGameMusic.Play();
			sharedGameAmbience.Play();
		}
	}

	public void StopPlayingMenu()
	{
		if(sharedMenuMusic != null)
		{
			startMenuMusic = false;
			stopMenuMusic = true;
		}
	}

	public void StartPlayingMenu()
	{
		if(sharedMenuMusic != null && !sharedMenuMusic.isPlaying)
		{
			stopMenuMusic = false;
			startMenuMusic = true;
			sharedMenuMusic.Play();
		}
	}
}
