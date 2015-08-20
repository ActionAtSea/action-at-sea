using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	private SharedSoundHandler menuMusicHandler;
	
	void Start () 
	{
		menuMusicHandler = FindObjectOfType<SharedSoundHandler> ();
		if(!menuMusicHandler)
		{
			Debug.Log("MenuMusicHandler could not be found in scene.");
		}		
	}

	public void NextButton ()
	{
		menuMusicHandler.PlayButtonClick();
		Application.LoadLevel (6);
	}
	public void BackButton ()
	{
		menuMusicHandler.PlayButtonClick();
		Application.LoadLevel (4);
	}
	public void BackToMenu ()
	{
		menuMusicHandler.PlayButtonClick();
		Application.LoadLevel (0);
	}
}
