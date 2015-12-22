////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - LobbyScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyScript : MonoBehaviour 
{
    public UnityEngine.UI.Text lobbyStatus = null;
    public UnityEngine.UI.Toggle isReady = null;
    public UnityEngine.UI.Slider slider = null;
    public GameObject selectedLevel = null;
    private bool m_playGameRequest = false;
    private bool m_joinGameRequest = false;
    public LevelID m_selectedLevel = LevelID.LEVEL1;
    private NetworkMatchmaker m_network = null;
    private float m_timer = 0.0f;
    private int m_dots = 0;
    private int m_maxDots = 4;
    private float m_dotSpeed = 0.25f;
    private bool m_initialised = false;
    private ConnectedPlayersList m_playersList = null;
    private bool m_firstSelection = true;
    public Color m_selectedColour;
    private Color m_unselectedColor;

    /// <summary>
    /// Initialises the lobby
    /// </summary>
    void Start () 
    {
        m_network = NetworkMatchmaker.Get();
        m_playersList = GameObject.FindObjectOfType<ConnectedPlayersList>();
    }

    /// <summary>
    /// Gets the background image of the selected level
    /// </summary>
    UnityEngine.UI.Image GetSelectedBackground()
    {
        return selectedLevel.transform.FindChild("Background").GetComponent<UnityEngine.UI.Image>();
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
                GetSelectedBackground().color = m_unselectedColor;
            }
           
            SoundManager.Get().PlaySound(SoundManager.SoundID.BUTTON_CLICK);
            SelectNewLevel(level);
        }
    }

    /// <summary>
    /// Sets the max slider enabled/disabled
    /// </summary>
    private void SetMaxPlayerSlider(bool enabled)
    {
        // only enable for non-open levels
        slider.GetComponentInParent<GUIMaxPlayers>().SetEnabled(
            enabled && !Utilities.IsOpenLeveL(m_selectedLevel));
    }

    /// <summary>
    /// Selects the level
    /// </summary>
    private void SelectNewLevel(GameObject level)
    {
        m_selectedLevel = (LevelID)(int.Parse(level.name) - 1); // Levels start at 0
        selectedLevel = level;

        SetMaxPlayerSlider(true);

        slider.GetComponentInParent<GUIMaxPlayers>().SetMaxPlayers(
            Utilities.GetMaxPlayersForLevel(m_selectedLevel));

        if (m_firstSelection)
        {
            m_unselectedColor = GetSelectedBackground().color;
            m_firstSelection = false;
        }
        GetSelectedBackground().color = m_selectedColour;

        isReady.isOn = false;
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    void StartGame()
    {
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
    /// Updates the list of connected players
    /// </summary>
    void UpdatePlayersList()
    {
        string text = "";
        if (m_network.IsConnected() && m_network.IsInRoom())
        {
            PhotonNetwork.player.name = Utilities.GetPlayerName();

            List<string> players = new List<string>();
            PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
            foreach (var player in photonPlayers)
            {
                players.Add(player.name + "\n");
            }
            players.Sort();

            foreach (string player in players)
            {
                text += player;
            }
        }
        m_playersList.SetText(text);
    }

    /// <summary>
    /// Updates the play game request
    /// </summary>
    void Update()
    {
        if(!m_initialised)
        {
            SelectNewLevel(selectedLevel);
            m_initialised = true;
        }

        UpdatePlayersList();

        if (Diagnostics.IsActive())
        {
            Diagnostics.Add("Max Players Chosen", Utilities.GetMaximumPlayers());
        }

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
                        SetMaxPlayerSlider(false);
                    }
                    else if(m_network.IsRoomReady())
                    {
                        StartGame();
                        m_playGameRequest = true;
                    }

                    if(Utilities.IsOpenLeveL(m_selectedLevel)  || 
                      !m_network.IsInRoom() || 
                       m_network.GetPlayerID() < 0)
                    {
                        lobbyStatus.text = "Joining Level" + GetDots();
                    }
                    else
                    {
                        int maxSlots = Utilities.GetMaximumPlayers();
                        int players = m_network.GetRoomPlayerCount();
                        lobbyStatus.text = "Waiting for: " + 
                            (maxSlots - players).ToString() + " / " + maxSlots.ToString();
                    }
                }
                else if(m_joinGameRequest)
                {
                    Debug.Log("Leaving game room from lobby");
                    m_network.LeaveGameLevel();
                    m_joinGameRequest = false;
                    SetMaxPlayerSlider(true);
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