////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - MoveScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour 
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

    public void BackToMenuButton()
    {
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel ((int)SceneID.MENU);
    }
    public void NextButton ()
    {
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel ((int)SceneID.DISCOVER_LAND);
    }
}
