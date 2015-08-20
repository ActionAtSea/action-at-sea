using UnityEngine;
using System.Collections;

public class IslandScript : MonoBehaviour {

	private SharedSoundHandler menuMusicHandler;
	
	void Start () 
	{
		menuMusicHandler = FindObjectOfType<SharedSoundHandler>();
		if(!menuMusicHandler)
		{
			Debug.Log("MenuMusicHandler could not be found in scene.");
		}		
	}

	public void NextButton ()
	{
		menuMusicHandler.PlayButtonClick ();
		Application.LoadLevel (4);
	}
	public void BackButton ()
	{
		menuMusicHandler.PlayButtonClick ();
		Application.LoadLevel (2);
	}
	public void BackToMenuButton ()
	{
		menuMusicHandler.PlayButtonClick ();
		Application.LoadLevel (0);
	}
}
