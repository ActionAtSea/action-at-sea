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
    private GameState m_networkedState = GameState.NONE;
    private GameState m_state = GameState.NONE;
    private GUITimer m_countdownTimer = null;
    private FogOfWar m_fogOfWar = null;
    private bool? m_stateInitiatedByNetwork = null;
    private bool m_startedStage1Timer = false;
    private float m_stage1Countdown = 5.0f * 60.0f; // 5 minutes
    private float m_stage2Countdown = 10.0f * 60.0f; // 10 minutes
    private Action m_stage1CountdownFinish = null;
    private Action m_stage2CountdownFinish = null;
    static GameModeManager m_gameManager = null;

    /// <summary>
    /// Initialises the game mode manager
    /// </summary>  
	void Start () 
    {
        if(!Utilities.IsOpenLeveL(Utilities.GetLoadedLevel()))
        {
            m_networkedState = GameState.STAGE_1;
            m_state = GameState.STAGE_1;

            m_countdownTimer = FindObjectOfType<GUITimer>();
            if (m_countdownTimer == null)
            {
                Debug.LogError("Could not find GUI Count down timer");
            }
        }

        m_islandList = GameObject.FindObjectsOfType<IslandDiscoveryTrigger>();
        if(m_islandList.Length == 0)
        {
            Debug.LogError("Could not find any islands");
        }

        m_fogOfWar = FindObjectOfType<FogOfWar>();
        if(m_fogOfWar == null)
        {
            Debug.LogError("Could not find Fog of war");
        }

        m_stage1CountdownFinish = () =>
        {
            SwitchToState(GameState.STAGE_2);
            m_stateInitiatedByNetwork = false;
        };

        m_stage2CountdownFinish = () =>
        {
            GameOverScript.Get().SetLevelComplete();
        };
    }

    /// <summary>
    /// Returns the game state
    /// </summary>  
    public GameState GetState()
    {
        return m_state;
    }

    /// <summary>
    /// Set by all clients connected to the room
    /// Only accepts states further along in gameplay
    /// </summary>  
    public void TrySetState(GameState state)
    {
        if((int)state > (int)m_networkedState)
        {
            m_networkedState = state;
        }
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
    /// Switches to a new state
    /// </summary>
    void SwitchToState(GameState state)
    {
        if((int)m_state >= (int)state)
        {
            return;
        }

        if(state == GameState.STAGE_2)
        {
            Debug.Log("Starting Stage 2");
            m_state = GameState.STAGE_2;
            m_countdownTimer.StartCountDown(m_stage2Countdown, m_stage2CountdownFinish);
            m_fogOfWar.HideFog();
        }
        else
        {
            Debug.LogError("Tried to set an unsupported state: " + state.ToString());
        }
    }

    /// <summary>
    /// Updates the script
    /// </summary>
    void Update()
    {
        if(m_state == GameState.STAGE_1)
        {
            if(!m_startedStage1Timer)
            {
                if(m_timePassed > 0.0f)
                {
                    m_startedStage1Timer = true;
                    m_countdownTimer.StartCountDown(
                        m_stage1Countdown, m_stage1CountdownFinish);
                }
            }
            else
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
                    SwitchToState(GameState.STAGE_2);
                }
            }
        }

        RenderDiagnostics();
    }

    /// <summary>
    /// Late update required as networked time passed/state is set during update()
    /// </summary>
    void LateUpdate()
    {
        // Player over the network has initiated the next state before the client
        if((int)m_networkedState > (int)m_state)
        {
            SwitchToState(m_networkedState);
            m_stateInitiatedByNetwork = true;
        }

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
        if(m_gameManager == null)
        {
            m_gameManager = FindObjectOfType<GameModeManager>();
            if(m_gameManager == null)
            {
                Debug.LogError("Could not find GameModeManager in scene");
            }
        }
        return m_gameManager;
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
            Diagnostics.Add("Max Players Chosen", Utilities.GetMaximumPlayers());
            Diagnostics.Add("Max Players For Level", Utilities.GetMaxPlayersForLevel(level));
            Diagnostics.Add("Is Open Level", Utilities.IsOpenLeveL(level));
            Diagnostics.Add("Game State", m_state);

            if(m_stateInitiatedByNetwork != null)
            {
                Diagnostics.Add("State2 Initiated By Network", m_stateInitiatedByNetwork);
            }
        }
    }
}
