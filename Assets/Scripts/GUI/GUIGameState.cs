////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUIGameState.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;

public class GUIGameState : MonoBehaviour 
{
    private UnityEngine.UI.Text m_backText = null;
    private UnityEngine.UI.Text m_frontText = null;
    private GameState m_previousState = GameState.NONE;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_backText = GetComponent<UnityEngine.UI.Text>();
        m_frontText = transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        m_backText.text = "";
        m_frontText.text = "";
    }

    /// <summary>
    /// Updates the text
    /// </summary>
    void Update()
    {
        GameState state = Utilities.GetGameState();
        if(state != m_previousState)
        {
            switch (state)
            {
            case GameState.STAGE_1:
                m_backText.text = "CAPTURE ALL ISLANDS";
                m_frontText.text = "CAPTURE ALL ISLANDS";
                break;
            case GameState.STAGE_2:
                m_backText.text = "RE-TAKE ENEMY ISLANDS";
                m_frontText.text = "RE-TAKE ENEMY ISLANDS";
                break;
            default:
                m_backText.text = "";
                m_frontText.text = "";
                break;
            }
            m_previousState = state;
        }
    }
}
