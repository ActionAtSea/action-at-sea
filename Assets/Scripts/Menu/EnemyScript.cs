////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - EnemyScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour 
{
	private SoundManager menuMusicHandler;
    
    void Start () 
    {
		menuMusicHandler = FindObjectOfType<SoundManager> ();
        if(!menuMusicHandler)
        {
            Debug.Log("MenuMusicHandler could not be found in scene.");
        }        
    }

    public void NextButton ()
    {
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
		Application.LoadLevel ((int)SceneID.TREASURE);
    }
    public void BackButton ()
    {
		menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
		Application.LoadLevel ((int)SceneID.UPGRADING_DOCKS);
    }
    public void BackToMenu ()
    {
		menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
		Application.LoadLevel ((int)SceneID.MENU);
    }
}
