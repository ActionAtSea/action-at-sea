using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {

	private SharedSoundHandler menuMusicHandler;
	
	void Start () 
	{
		menuMusicHandler = FindObjectOfType<SharedSoundHandler> ();
		if(!menuMusicHandler)
		{
			Debug.Log("MenuMusicHandler could not be found in scene.");
		}		
	}

	public void BackToMenuButton()
	{
		menuMusicHandler.PlayButtonClick();
		Application.LoadLevel (0);
	}
	public void NextButton ()
	{
		menuMusicHandler.PlayButtonClick ();
		Application.LoadLevel (3);
	}
}
