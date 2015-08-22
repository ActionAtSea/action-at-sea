////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayGame.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayGame : MonoBehaviour 
{
	private SoundManager menuMusicHandler;
    private FadeGame fadeGameHandler;
    private bool playGameRequest = false;
    
    void Start () 
    {
		menuMusicHandler = FindObjectOfType<SoundManager> ();
        if(!menuMusicHandler)
        {
            Debug.Log("MenuMusicHandler could not be found in scene.");
        }

        fadeGameHandler = FindObjectOfType<FadeGame> ();
        if(!fadeGameHandler)
        {
            Debug.Log("FadeGame could not be found in scene.");
        }
    }

    public void PlayGameButton ()
    {
        if(!playGameRequest)
        {
            GameInformation.SetPlayerName(GameObject.FindGameObjectWithTag(
                "PlayerNameText").GetComponent<UnityEngine.UI.Text>().text);

			menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
			menuMusicHandler.StopMusic(SoundManager.MusicID.MENU_TRACK);
			menuMusicHandler.PlayMusic(SoundManager.MusicID.GAME_TRACK);
			menuMusicHandler.PlayMusic(SoundManager.MusicID.GAME_AMBIENCE);

            fadeGameHandler.FadeIn();
            playGameRequest = true;
        }
    }

    public void HowToPlayButton ()
    {
		menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
		Application.LoadLevel((int)SceneID.MOVE_AND_FIRE);
    }

    public void QuitButton ()
    {
        Application.Quit();
    }

    void Update()
    {
        if(playGameRequest && fadeGameHandler.IsFadedIn())
        {
            playGameRequest = false;
            fadeGameHandler.FadeOut();
			Application.LoadLevel((int)SceneID.GAME);
        }
    }
}