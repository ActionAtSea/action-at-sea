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
    public UnityEngine.UI.Text replayGameText;
    public UnityEngine.UI.Text toMenuText;
    public RectTransform replayGameButton;
    public RectTransform toMenuButton;
    public bool forceLoseGame = false;
    public Color mouseOverColor = new Color(1.0f, 1.0f, 1.0f);

    private Color m_textColour;
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

        if(replayGameText.color != toMenuText.color)
        {
            Debug.LogError("Colours do not match for game over text");
        }
        m_textColour = replayGameText.color;
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
        else if (m_network.IsConnectedToLevel())
        {
            if(!m_isGameOver)
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
            else
            {
                // Check if used has clicked on the buttons
                // This is done here as raycasting the canvas doesn't seem to work!
               
                if(IsOverImage(replayGameButton))
                {
                    replayGameText.color = mouseOverColor;
                    if(Input.GetMouseButtonDown(0))
                    {
                        Debug.Log("Clicked Play Game");
                        PlayGameButton();
                    }
                }
                else
                {
                    replayGameText.color = m_textColour;
                }

                if(IsOverImage(toMenuButton))
                {
                    toMenuText.color = mouseOverColor;
                    if(Input.GetMouseButtonDown(0))
                    {
                        Debug.Log("Clicked To Menu");
                        GoToMenuButton();
                    }
                }
                else
                {
                    toMenuText.color = m_textColour;
                }
            }
        }
    }

    /// <summary>
    /// Gets whether a mouse is over the image
    /// </summary>
    bool IsOverImage(RectTransform image)
    {
        var halfWidth = image.sizeDelta.x / 2;
        var halfHeight = image.sizeDelta.y / 2;
        var x = image.position.x;
        var y = image.position.y;

        return Input.mousePosition.x <= x + halfWidth &&
            Input.mousePosition.x >= x - halfWidth &&
            Input.mousePosition.y <= y + halfHeight &&
            Input.mousePosition.y >= y - halfHeight;
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
            replayGameText.enabled = false;
            toMenuText.enabled = false;
            gameLostImage.GetComponent<UnityEngine.UI.Image>().enabled = false;
            gameWonImage.GetComponent<UnityEngine.UI.Image>().enabled = false;
        }
        else
        {
            m_isGameOver = true;
            replayGameText.enabled = true;
            replayGameText.color = m_textColour;
            toMenuText.enabled = true;
            toMenuText.color = m_textColour;
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
