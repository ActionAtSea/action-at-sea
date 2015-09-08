////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class IslandScript : MonoBehaviour 
{

    private SoundManager menuMusicHandler;
    
    void Start () 
    {
        menuMusicHandler = FindObjectOfType<SoundManager>();
        if(!menuMusicHandler)
        {
            Debug.Log("MenuMusicHandler could not be found in scene.");
        }        
    }

    public void NextButton ()
    {
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel ((int)SceneID.UPGRADING_DOCKS);
    }
    public void BackButton ()
    {
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel ((int)SceneID.MOVE_AND_FIRE);
    }
    public void BackToMenuButton ()
    {
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel ((int)SceneID.MENU);
    }
}
