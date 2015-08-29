////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - LobbyScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class LobbyScript : MonoBehaviour 
{
    public UnityEngine.UI.Toggle isReady = null;
    public UnityEngine.UI.Text playerNameText = null;
    public GameObject selectedLevel = null;
    private bool m_playGameRequest = false;
    private int m_selectedLevel = 0;

    /**
    * Initialises the lobby
    */
    void Start () 
    {
        SelectNewLevel(selectedLevel);
    }

    /**
    * Selects the level
    */
    public void SelectLevel(GameObject level)
    {
        if(level != selectedLevel)
        {
            if(selectedLevel != null)
            {
                var oldBackground = selectedLevel.transform.FindChild("Background");
                oldBackground.GetComponent<UnityEngine.UI.Image>().color = 
                    new Color(1.0f, 1.0f, 1.0f, 0.88f);
            }
           
            SoundManager.Get().PlaySound(SoundManager.SoundID.BUTTON_CLICK);
            SelectNewLevel(level);
        }
    }

    /**
    * Selects the level
    */
    private void SelectNewLevel(GameObject level)
    {
        m_selectedLevel = int.Parse(level.name);
        selectedLevel = level;
        
        var newBackground = selectedLevel.transform.FindChild("Background");
        newBackground.GetComponent<UnityEngine.UI.Image>().color = 
            new Color(1.0f, 0.96f, 0.43f, 0.88f);
    }

    /**
    * Starts the selected game
    */
    public void PlayGameButton ()
    {
        if(!m_playGameRequest)
        {
            GameInformation.SetPlayerName(playerNameText.text);

            var soundManager = SoundManager.Get();
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
            soundManager.StopMusic(SoundManager.MusicID.MENU_TRACK);
            soundManager.PlayMusic(SoundManager.MusicID.GAME_TRACK);
            soundManager.PlayMusic(SoundManager.MusicID.GAME_AMBIENCE);

            FadeGame.Get().FadeIn();
            m_playGameRequest = true;
        }
    }

    /**
    * Goes back to the menu
    */
    public void BackToMenuButton()
    {
		SoundManager.Get().PlaySound(SoundManager.SoundID.BUTTON_CLICK);
		Application.LoadLevel((int)SceneID.MENU);
    }

    /**
    * Updates the play game request
    */
    void Update()
    {
        if(m_playGameRequest && FadeGame.Get().IsFadedIn())
        {
            m_playGameRequest = false;
            FadeGame.Get().FadeOut();

            switch(m_selectedLevel)
            {
            case 1:
			    Application.LoadLevel((int)SceneID.LEVEL1);
                break;
            case 2:
                Application.LoadLevel((int)SceneID.LEVEL2);
                break;
            }
        }
    }
}