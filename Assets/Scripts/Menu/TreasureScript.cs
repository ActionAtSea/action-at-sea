////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - TreasureScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class TreasureScript : MonoBehaviour 
{

    private SoundManager menuMusicHandler;

    void Start()
    {
        menuMusicHandler = FindObjectOfType<SoundManager>();
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
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel ((int)SceneID.ENEMIES);
    }
    public void BackToMenuButton ()
    {
        menuMusicHandler.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel ((int)SceneID.MENU);
    }
}
