////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - UpgradingDock.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class UpgradingDock : MonoBehaviour 
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
        Application.LoadLevel ((int)SceneID.ENEMIES);
    }
    public void BackButton ()
    {
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel ((int)SceneID.DISCOVER_LAND);
    }
    public void BackToMenuButton ()
    {
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel ((int)SceneID.MENU);
    }
}