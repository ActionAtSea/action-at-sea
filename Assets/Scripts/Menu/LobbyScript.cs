////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - LobbyScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;

public class LobbyScript : MonoBehaviour 
{
    public UnityEngine.UI.Text lobbyStatus = null;
    public UnityEngine.UI.Toggle isReady = null;
    public UnityEngine.UI.Text playerNameText = null;
    public GameObject selectedLevel = null;
    private bool m_playGameRequest = false;
    private int m_selectedLevel = 0;
    private NetworkMatchmaker m_network = null;
    private float m_timer = 0.0f;
    private int m_dots = 0;
    private int m_maxDots = 4;
    private float m_dotSpeed = 0.25f;

    /// <summary>
    /// Initialises the lobby
    /// </summary>
    void Start () 
    {
        m_network = NetworkMatchmaker.Get();
        SelectNewLevel(selectedLevel);
    }

    /// <summary>
    /// Selects the level
    /// </summary>
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

    /// <summary>
    /// Selects the level
    /// </summary>
    private void SelectNewLevel(GameObject level)
    {
        m_selectedLevel = int.Parse(level.name);
        selectedLevel = level;
        
        var newBackground = selectedLevel.transform.FindChild("Background");
        newBackground.GetComponent<UnityEngine.UI.Image>().color = 
            new Color(1.0f, 0.96f, 0.43f, 0.88f);
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    void StartGame()
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

    /// <summary>
    /// Goes back to the menu
    /// </summary>
    public void BackToMenuButton()
    {
		SoundManager.Get().PlaySound(SoundManager.SoundID.BUTTON_CLICK);
		Application.LoadLevel((int)SceneID.MENU);
    }

    /// <summary>
    /// Updates the play game request
    /// </summary>
    void Update()
    {
        if(!m_network.IsConnected())
        {
            m_timer += Time.deltaTime;
            if(m_timer >= m_dotSpeed)
            {
                m_timer = 0.0f;
                m_dots++;
                if(m_dots >= m_maxDots)
                {
                    m_dots = 0;
                }
            }

            lobbyStatus.text = "Connecting";
            for(int i = 0; i < m_dots; ++i)
            {
                lobbyStatus.text += ".";
            }

            return;
        }

        lobbyStatus.text = "Connected";

        if(!m_playGameRequest)
        {
            if(isReady.isOn)
            {
                m_network.JoinGameRoom(m_selectedLevel);
                StartGame();
            }
        }
        else
        {
            lobbyStatus.text = "Entering Game";

            if(FadeGame.Get().IsFadedIn())
            {
                FadeGame.Get().FadeOut();
                Application.LoadLevel((int)Utilities.GetSceneIDFromLevel(m_selectedLevel));
            }
        }
    }
}