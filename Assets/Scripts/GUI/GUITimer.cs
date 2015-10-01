////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUITimer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;

public class GUITimer : MonoBehaviour 
{
    private GameModeManager m_manager = null;
    private UnityEngine.UI.Text m_backText = null;
    private UnityEngine.UI.Text m_frontText = null;
    private float m_startTime = 0.0f;
    private float m_targetTime = 0.0f;
    private bool m_countDown = false;
    private Action m_onReachTarget = null;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_manager = GameModeManager.Get();
        m_backText = GetComponent<UnityEngine.UI.Text>();
        m_frontText = transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        m_backText.enabled = false;
        m_frontText.enabled = false;
    }

    /// <summary>
    /// Starts the countdown timer
    /// </summary>
    public void StartCountDown(float secondsToCount, Action onReachTarget)
    {
        if(!m_countDown)
        {
            m_backText.enabled = true;
            m_frontText.enabled = true;
            m_countDown = true;
            m_onReachTarget = onReachTarget;
            m_targetTime = secondsToCount;
            m_startTime = m_manager.GetTimePassed();
            Update();
        }
    }

    /// <summary>
    /// Updates the timer text
    /// </summary>
    void Update()
    {
        if(m_countDown)
        {
            float timePassed = m_manager.GetTimePassed();
            float timeDifference = timePassed - m_startTime;
            float time = Mathf.Max(0.0f, m_targetTime - timeDifference);

            int seconds = (int)time;
            int minutes = seconds / 60;
            seconds -= minutes * 60;

            string secondsStr = seconds.ToString();
            if(secondsStr.Length == 1)
            {
                secondsStr = "0" + secondsStr;
            }

            string minutesStr = minutes.ToString();
            if(minutesStr.Length == 1)
            {
                minutesStr = "0" + minutesStr;
            }

            string text = minutesStr + ":" + secondsStr;
            m_backText.text = text;
            m_frontText.text = text;

            if(time == 0.0f)
            {
                m_onReachTarget();
                m_countDown = false;
            }
        }
    }
}
