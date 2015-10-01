////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameModeManager.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    private IslandDiscoveryTrigger[] m_islandList;
    private float m_networkedTimePassed = 0.0f;
    private float m_timePassed = 0;
    private GUITimer m_countdownTimer = null;
    private Action m_onCountDownFinish = null;
    private GameState m_state;

    /// <summary>
    /// States that non-open game levels can be in
    /// </summary>  
    enum GameState
    {
        OPEN_FIGHT,
        CAPTURE_ISLANDS,
        COUNTDOWN
    }

    /// <summary>
    /// Initialises the game mode manager
    /// </summary>  
	void Start () 
    {
        m_islandList = FindObjectsOfType<IslandDiscoveryTrigger>();
        if(m_islandList == null || m_islandList.Length == 0)
        {
            Debug.LogError("Could not find island triggers for level");
        }

        if(Utilities.IsOpenLeveL(Utilities.GetLoadedLevel()))
        {
            m_state = GameState.OPEN_FIGHT;
        }
        else
        {
            m_state = GameState.CAPTURE_ISLANDS;
        }

        m_countdownTimer = FindObjectOfType<GUITimer>();
        if(m_countdownTimer == null)
        {
            Debug.LogError("Could not find GUI Count down timer");
        }

        m_onCountDownFinish = () =>
        {
            GameOverScript.Get().SetLevelComplete();
        };
    }

    /// <summary>
    /// Set the time passed since starting the level
    /// Set by all clients connected to the room
    /// Use the smallest time passed a client has
    /// </summary>  
    public void TrySetTimePassed(float timePassed)
    {
        m_networkedTimePassed = Mathf.Max(
            timePassed, m_networkedTimePassed);
    }

    /// <summary>
    /// Updates the script
    /// </summary>
    void Update()
    {
        if(m_state == GameState.CAPTURE_ISLANDS)
        {
            bool allDiscovered = true;
            for(int i = 0; i < m_islandList.Length; ++i)
            {
                if(!m_islandList[i].IsDiscovered())
                {
                    allDiscovered = false;
                    break;
                }
            }

            if(allDiscovered)
            {
                const float countDownTime = 5.0f * 60.0f; // 5 minutes
                m_state = GameState.COUNTDOWN;
                m_countdownTimer.StartCountDown(countDownTime, m_onCountDownFinish);
            }
        }

        RenderDiagnostics();
    }

    /// <summary>
    /// Late update required as time passed is set during update()
    /// </summary>
    void LateUpdate()
    {
        m_timePassed = m_networkedTimePassed;
        m_networkedTimePassed = 0.0f;
    }

    /// <summary>
    /// Gets the time passed since starting the level
    /// </summary>  
    public float GetTimePassed()
    {
        return m_timePassed;
    }

    /// <summary>
    /// Gets the Game Mode Manager instance from the scene
    /// </summary>
    public static GameModeManager Get()
    {
        var gameManager = FindObjectOfType<GameModeManager>();
        if(gameManager == null)
        {
            Debug.LogError("Could not find GameModeManager in scene");
        }
        return gameManager;
    }

    /// <summary>
    /// Renders Diagnostics
    /// </summary>
    void RenderDiagnostics()
    {
        if(Diagnostics.IsActive())
        {
            var level = Utilities.GetLoadedLevel();
            Diagnostics.Add("Time", m_timePassed);
            Diagnostics.Add("Level", level);
            Diagnostics.Add("Max Players", Utilities.GetAcceptedPlayersForLevel(level));
            Diagnostics.Add("Is Open Level", Utilities.IsOpenLeveL(level));
        }
    }
}
