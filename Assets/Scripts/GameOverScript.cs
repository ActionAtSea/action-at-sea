////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameOverScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour 
{    
    private GameObject player = null;
    public GameObject gameLostImage;
    public GameObject gameWonImage;
    public bool forceWinGame = false;
    public bool forceLoseGame = false;
    private Health playerHealth;
    private bool isGameOver = false;
    private bool hasLostGame = false;
    private List<GameObject> enemies = new List<GameObject> ();
    private List<GameObject> islands = new List<GameObject>();
    private SharedSoundHandler sharedSoundHandler;
    private FadeGame fadeGameHandler;
    private bool toMenuRequest = false;
    private bool toPlayRequest = false;

    void Start () 
    {
        sharedSoundHandler = FindObjectOfType<SharedSoundHandler> ();
        if(sharedSoundHandler == null)
        {
            Debug.LogError("Could not find shared sound handler");
        }

        fadeGameHandler = FindObjectOfType<FadeGame> ();
        if(fadeGameHandler == null)
        {
            Debug.LogError("Could not find FadeGame");
        }

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag ("Enemy");
        for(int i = 0; i < allEnemies.Length; ++i)
        {
            enemies.Add(allEnemies[i]);
        }

        GameObject[] allIslands = GameObject.FindGameObjectsWithTag ("IslandTrigger");
        for(int i = 0; i < allIslands.Length; ++i)
        {
            islands.Add(allIslands[i]);
        }

        if (islands.Count == 0) 
        {
            Debug.LogError("Could not find any island triggers");
        }
    }

    public void PlayGameButton()
    {
        if (isGameOver && !toPlayRequest)
        {
            sharedSoundHandler.PlayButtonClick();
            sharedSoundHandler.StopPlayingMenu();
            sharedSoundHandler.StartPlayingGame();
            fadeGameHandler.FadeIn();
            toPlayRequest = true;
        }
    }

    public void GoToMenuButton()
    {
        if (isGameOver && !toMenuRequest)
        {
            FindObjectOfType<Crosshair>().ShowCursor();
            sharedSoundHandler.PlayButtonClick();
            sharedSoundHandler.StartPlayingMenu();
            fadeGameHandler.FadeIn();
            toMenuRequest = true;
        }
    }

    void Update () 
    {
        if(player == null)
        {
            player = GameObject.FindWithTag("Player");
            if(player != null)
            {
                playerHealth = player.GetComponent<Health> ();
                if(playerHealth == null)
                {
                    Debug.LogError("Player requires health bar");
                }
            }
            return;
        }

        if(toMenuRequest || toPlayRequest)
        {
            if(fadeGameHandler.IsFadedIn())
            {
                fadeGameHandler.FadeOut();

                if(!GameInformation.IsPVP())
                {
                    Application.LoadLevel (toMenuRequest ? 0 : 1);
                }
                else
                {
                    PhotonNetwork.Disconnect();
                    Application.LoadLevel (toMenuRequest ? 0 : 6);
                }
            }
        }
        else if (!isGameOver) 
        {
            if (Input.GetKeyDown (KeyCode.Escape) || forceLoseGame || !playerHealth.IsAlive) 
            {
                isGameOver = true;
                hasLostGame = true;
            }

            // Currently no way to win PVP!
            if(!GameInformation.IsPVP())
            {
                if(forceWinGame || (enemies.TrueForAll(enemy => enemy == null) &&
                   islands.TrueForAll(island => island.GetComponent<IslandDiscoveryTrigger>().IsDiscovered())))
                {
                    hasLostGame = false;
                    isGameOver = true;
                }
            }
        
            if (isGameOver) 
            {
                sharedSoundHandler.StopPlayingGame();
                sharedSoundHandler.StartPlayingMenu();

                if(!GameInformation.IsPVP())
                {
                    player.SetActive(false);
                }
                else
                {
                    player.GetComponent<Health>().SetHealthLevel(0.0f);
                }

                if(hasLostGame)
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
}
