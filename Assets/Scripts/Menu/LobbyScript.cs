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
    private bool m_joinGameRequest = false;
    private LevelID m_selectedLevel = LevelID.LEVEL1;
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
        m_selectedLevel = (LevelID)(int.Parse(level.name) - 1); // Levels start at 0
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

        m_network.StartLevel();

        var soundManager = SoundManager.Get();
        soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        soundManager.StopMusic(SoundManager.MusicID.MENU_TRACK);
        soundManager.PlayMusic(SoundManager.MusicID.GAME_TRACK);
        soundManager.PlayMusic(SoundManager.MusicID.GAME_AMBIENCE);
        
        FadeGame.Get().FadeIn();
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
    /// Get string dots
    /// </summary>
    private string GetDots()
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

        string dots = "";
        for(int i = 0; i < m_dots; ++i)
        {
            dots += ".";
        }
        return dots;
    }

    /// <summary>
    /// Updates the play game request
    /// </summary>
    void Update()
    {
        if(!m_playGameRequest)
        {
            if(!m_network.IsConnected())
            {
                // Wait until connected before entering a room
                m_joinGameRequest = false;
                lobbyStatus.text = "Connecting" + GetDots();
            }
            else
            {
                lobbyStatus.text = "Connected";
                if(isReady.isOn)
                {
                    if(!m_joinGameRequest)
                    {
                        m_joinGameRequest = true;
                        m_network.JoinGameLevel(m_selectedLevel);
                    }
                    else if(m_network.IsRoomReady())
                    {
                        StartGame();
                        m_playGameRequest = true;
                    }

                    if(Utilities.IsOpenLeveL(m_selectedLevel)  || !m_network.IsInRoom())
                    {
                        lobbyStatus.text = "Joining Level" + GetDots();
                    }
                    else
                    {
                        int maxSlots = Utilities.GetAcceptedPlayersForLevel(m_selectedLevel);
                        int players = m_network.GetRoomPlayerCount();
                        lobbyStatus.text = "Waiting for: " + 
                            (maxSlots - players).ToString() + " / " + maxSlots.ToString();
                    }
                }
                else if(m_joinGameRequest)
                {
                    m_network.LeaveGameLevel();
                    m_joinGameRequest = false;
                }
            }
        }
        else
        {
            // At this point if a disconnect happens let the level deal with it
            lobbyStatus.text = "Entering Game";
            if(FadeGame.Get().IsFadedIn())
            {
                FadeGame.Get().FadeOut();
                Application.LoadLevel((int)Utilities.GetSceneIDFromLevel(m_selectedLevel));
            }
        }
    }
}