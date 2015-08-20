using UnityEngine;
using System.Collections;

public class UpgradingDock : MonoBehaviour {
	
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
		Application.LoadLevel (5);
	}
	public void BackButton ()
	{
		menuMusicHandler.PlayButtonClick ();
		Application.LoadLevel (3);
	}
	public void BackToMenuButton ()
	{
		menuMusicHandler.PlayButtonClick ();
		Application.LoadLevel (0);
	}
}