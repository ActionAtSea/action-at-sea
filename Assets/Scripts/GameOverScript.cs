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
    public bool forceLoseGame = false;

    private NetworkMatchmaker m_network = null;
    private bool m_isGameOver = false;       // Whether game over is active for the player
    private bool m_hasLostGame = false;      // whether the player has lost the game
    private bool m_toMenuRequest = false;
    private bool m_toPlayRequest = false;

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
                    SetGameOver(false);
                }
            }
        }
        else if (m_network.IsConnectedToLevel() && !m_isGameOver)
        {
            var player = PlayerManager.GetControllablePlayer();
            bool isGameOver = false;

            if(Input.GetKeyDown (KeyCode.Escape) || forceLoseGame || 
               (player != null && !player.GetComponent<Health>().IsAlive))
            {
                m_hasLostGame = true;
                isGameOver = true;
            }

            if (isGameOver) 
            {
                var soundManager = SoundManager.Get();
                soundManager.StopMusic(SoundManager.MusicID.GAME_TRACK);
                soundManager.StopMusic(SoundManager.MusicID.GAME_AMBIENCE);
                soundManager.PlayMusic(SoundManager.MusicID.MENU_TRACK);

                if(player != null)
                {
                    player.GetComponent<Health>().SetHealthLevel(0.0f);
                }

                NetworkMatchmaker.Get().DestroyPlayer();
                SetGameOver(true);
            }
        }
    }

    /// <summary>
    /// Sets whether game over or not
    /// </summary>
    void SetGameOver(bool gameover)
    {
        m_toMenuRequest = false;
        m_toPlayRequest = false;
        
        if(!gameover)
        {
            m_isGameOver = false;
            m_hasLostGame = false;
            gameOverText.SetActive(false);
            gameLostImage.GetComponent<UnityEngine.UI.Image>().enabled = false;
            gameWonImage.GetComponent<UnityEngine.UI.Image>().enabled = false;
        }
        else
        {
            m_isGameOver = true;
            gameOverText.SetActive(true);
            gameLostImage.GetComponent<UnityEngine.UI.Image>().enabled = m_hasLostGame;
            gameWonImage.GetComponent<UnityEngine.UI.Image>().enabled = !m_hasLostGame;
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
