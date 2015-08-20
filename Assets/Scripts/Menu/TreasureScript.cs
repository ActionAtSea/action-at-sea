using UnityEngine;
using System.Collections;

public class TreasureScript : MonoBehaviour {

    private SharedSoundHandler menuMusicHandler;

    void Start()
    {
        menuMusicHandler = FindObjectOfType<SharedSoundHandler>();
        if (!menuMusicHandler)
        {
            Debug.Log("MenuMusicHandler could not be found in scene.");
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
	public void BackButton ()
	{
        menuMusicHandler.PlayButtonClick();
		Application.LoadLevel (5);
	}
	public void BackToMenuButton ()
	{
        menuMusicHandler.PlayButtonClick();
		Application.LoadLevel (0);
	}
}
