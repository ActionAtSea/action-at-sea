////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - MainMenuScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScript : MonoBehaviour 
{
    public Canvas optionsMenu = null;

    void Start()
    {
        if(optionsMenu != null)
        {
            optionsMenu.enabled = false;
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
        Application.LoadLevel((int)SceneID.MOVE_AND_FIRE);
    }

    public void OptionsButton()
    {
        SoundManager.Get().PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        //TODO: Fix this. Atm it only works if the scene starts with the options menu enabled.
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