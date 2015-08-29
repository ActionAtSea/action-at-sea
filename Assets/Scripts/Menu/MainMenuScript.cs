////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - MainMenuScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour 
{
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

    public void QuitButton ()
    {
        Application.Quit();
    }
}