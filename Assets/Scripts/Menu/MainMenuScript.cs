////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - MainMenuScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScript : MonoBehaviour 
{
    public Canvas optionsMenu = null;
    public Canvas howToPlayMenu = null;
    private HowToPlayMenu howToPlay = null;

    void Start()
    {
        if(optionsMenu != null)
        {
            optionsMenu.enabled = false;
        }

        if (howToPlayMenu != null)
        {
            howToPlayMenu.enabled = false;
            howToPlay = howToPlayMenu.GetComponentInChildren<HowToPlayMenu>();
        }
    }

    public void PlayGameButton ()
    {
        SoundManager.Get().PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        Application.LoadLevel((int)SceneID.LOBBY);
    }

    public void HowToPlayButton ()
    {
        SoundManager.Get().PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        //Application.LoadLevel((int)SceneID.MOVE_AND_FIRE);
        if (howToPlayMenu != null)
        {
            if (!howToPlayMenu.enabled)
            {
                howToPlayMenu.enabled = true;
            }
            else
            {
                howToPlay.ResetImages();
                howToPlayMenu.enabled = false;
            }
        }
    }

    public void OptionsButton()
    {
        SoundManager.Get().PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        //NOTE: This only works if the options menu starts enabled.
        if(optionsMenu != null)
        {
            if(!optionsMenu.enabled)
            {
                optionsMenu.enabled = true;
            }
            else
            {
                optionsMenu.enabled = false;
            }
        }
    }

    public void QuitButton ()
    {
        Application.Quit();
    }
}