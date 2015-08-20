using UnityEngine;
using System.Collections;

public class PlayGame : MonoBehaviour 
{
	private SharedSoundHandler menuMusicHandler;
	private FadeGame fadeGameHandler;
	private bool playGameRequest = false;
	
	void Start () 
	{
		menuMusicHandler = FindObjectOfType<SharedSoundHandler> ();
		if(!menuMusicHandler)
		{
			Debug.Log("MenuMusicHandler could not be found in scene.");
		}

		fadeGameHandler = FindObjectOfType<FadeGame> ();
		if(!fadeGameHandler)
		{
			Debug.Log("FadeGame could not be found in scene.");
		}

		GameInformation.SetPVP(false);
		GameInformation.SetPlayerName("unnamed");
	}

	public void PlayGameButton ()
	{
		if(!playGameRequest)
		{
			GameInformation.SetPlayerName(GameObject.FindGameObjectWithTag(
				"PlayerNameText").GetComponent<UnityEngine.UI.Text>().text);

			menuMusicHandler.PlayButtonClick ();
			menuMusicHandler.StopPlayingMenu();
			menuMusicHandler.StartPlayingGame();
			fadeGameHandler.FadeIn();
			playGameRequest = true;
		}
	}

	public void HowToPlayButton ()
	{
		menuMusicHandler.PlayButtonClick();
		Application.LoadLevel(2);
	}

	public void QuitButton ()
	{
		Application.Quit();
	}

	void Update()
	{
		if(playGameRequest && fadeGameHandler.IsFullyFadedIn())
		{
			playGameRequest = false;
			fadeGameHandler.FadeOut();
			Application.LoadLevel(GameInformation.IsPVP() ? 6 : 1);
		}
	}
}