////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameOverScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour 
{   
    public GameObject gameLostImage;
    public GameObject gameWonImage;
    public GameObject gameOverText;
    public GameObject disconnected;
    public bool forceLoseGame = false;

    private NetworkMatchmaker m_network = null;
    private Health m_playerHealth = null;    // Heath bar for the controllable player
    private bool m_isGameOver = false;       // Whether game over is active for the player
    private bool m_hasLostGame = false;      // whether the player has lost the game
    private bool m_toMenuRequest = false;
    private bool m_toPlayRequest = false;
    private bool m_hasFoundPlayer = false;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_network = NetworkMatchmaker.Get();
    }

    /// <summary>
    /// Updates the game over logic
    /// </summary>
    void Update () 
    {
        // Only update once the player has been created
        if(!m_hasFoundPlayer)
        {
            var player = PlayerManager.GetControllablePlayer();
            if(player != null)
            {
                m_playerHealth = player.GetComponent<Health>();
                m_hasFoundPlayer = true;
            }
            return;
        }

        // Check whether disconnected from the game
        if(!m_network.IsConnected() || !m_network.IsInRoom())
        {
            disconnected.SetActive(true);
            disconnected.GetComponentInChildren<UnityEngine.UI.Text>().text =
                "Disconnected from game:\n" + m_network.GetDisconnectCause() + 
                    "\nAttempting to reconnect...";
        }
        else
        {
            disconnected.SetActive(false);
        }

        if(m_toMenuRequest || m_toPlayRequest)
        {
            var gameFader = FadeGame.Get();
            if(gameFader.IsFadedIn())
            {
                gameFader.FadeOut();

                if(m_toMenuRequest)
                {
                    NetworkMatchmaker.Get().LeaveGameLevel();
                    Application.LoadLevel((int)SceneID.LOBBY);
                }
                else
                {
                    Application.LoadLevel(Application.loadedLevel);
                }
            }
        }
        else if (!m_isGameOver)
        {
            if(Input.GetKeyDown (KeyCode.Escape) || 
               forceLoseGame || 
               m_playerHealth == null || 
               !m_playerHealth.IsAlive)
            {
                m_isGameOver = true;
                m_hasLostGame = true;
            }

            if (m_isGameOver) 
            {
                var soundManager = SoundManager.Get();
                soundManager.StopMusic(SoundManager.MusicID.GAME_TRACK);
                soundManager.StopMusic(SoundManager.MusicID.GAME_AMBIENCE);
                soundManager.PlayMusic(SoundManager.MusicID.MENU_TRACK);

                if(m_playerHealth != null)
                {
                    m_playerHealth.SetHealthLevel(0.0f);
                }
                NetworkMatchmaker.Get().DestroyPlayer();

                gameOverText.SetActive(true);

                if(m_hasLostGame)
                {
                    gameLostImage.GetComponent<UnityEngine.UI.Image>().enabled = true;
                }
                else
                {
                    gameWonImage.GetComponent<UnityEngine.UI.Image>().enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// On Click replay Game when game over is active
    /// </summary>
    public void PlayGameButton()
    {
        if (m_isGameOver && !m_toPlayRequest)
        {
            var soundManager = SoundManager.Get();
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
            soundManager.StopMusic(SoundManager.MusicID.MENU_TRACK);
            soundManager.PlayMusic(SoundManager.MusicID.GAME_TRACK);
            soundManager.PlayMusic(SoundManager.MusicID.GAME_AMBIENCE);
            FadeGame.Get().FadeIn();
            m_toPlayRequest = true;
        }
    }
    
    /// <summary>
    /// On Click Go To Menu when game over is active
    /// </summary>
    public void GoToMenuButton()
    {
        if (m_isGameOver && !m_toMenuRequest)
        {
            var soundManager = SoundManager.Get();
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
            soundManager.PlayMusic(SoundManager.MusicID.MENU_TRACK);
            FadeGame.Get().FadeIn();
            Crosshair.Get().ShowCursor();
            m_toMenuRequest = true;
        }
    }

    /// <summary>
    /// Gets whether the game is over
    /// </summary>
    public bool IsGameOver()
    {
        return m_isGameOver;
    }

    /// <summary>
    /// Gets the game over script instance from the scene
    /// </summary>
    public static GameOverScript Get()
    {
        var gameover = FindObjectOfType<GameOverScript>();
        if(gameover == null)
        {
            Debug.LogError("Could not find Game Over Script in scene");
        }
        return gameover;
    }
}
