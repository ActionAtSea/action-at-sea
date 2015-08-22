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
    public bool forceLoseGame = false;

    private Health m_playerHealth = null;    // Heath bar for the controllable player
    private bool m_isGameOver = false;       // Whether game over is active for the player
    private bool m_hasLostGame = false;      // whether the player has lost the game
    private bool m_toMenuRequest = false;
    private bool m_toPlayRequest = false;

    /**
    * Updates the game over logic
    */
    void Update () 
    {
        if(!PlayerHasHealth())
        {
            return;
        }

        if(m_toMenuRequest || m_toPlayRequest)
        {
            var gameFader = FadeGame.Get();
            if(gameFader.IsFadedIn())
            {
                gameFader.FadeOut();

                PhotonNetwork.Disconnect();
                Application.LoadLevel(m_toMenuRequest ? (int)SceneID.MENU : (int)SceneID.GAME);
            }
        }
        else if (!m_isGameOver) 
        {
            if (Input.GetKeyDown (KeyCode.Escape) || forceLoseGame || !m_playerHealth.IsAlive) 
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

                m_playerHealth.SetHealthLevel(0.0f);

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

    /**
    * Returns if the player is found and has a health bar
    */
    bool PlayerHasHealth()
    {
        if(m_playerHealth == null)
        {
            var player = PlayerManager.GetControllablePlayer();
            if(player != null)
            {
                m_playerHealth = player.GetComponent<Health>();
                if(m_playerHealth == null)
                {
                    Debug.LogError("Player requires a health bar");
                }
            }
            return false;
        }
        return true;
    }

    /**
    * On Click replay Game when game over is active
    */
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
    
    /**
    * On Click Go To Menu when game over is active
    */
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
}
