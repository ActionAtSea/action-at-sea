////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUITimer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;

public class GUITimer : MonoBehaviour 
{
    public Color m_thirtySecondColor = new Color(1.0f, 0.82f, 0.0f);
    public Color m_tenSecondColor = new Color(1.0f, 0.0f, 0.0f);
    private Color m_normalColor;
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
        m_frontText = transform.FindChild("GameTimeFront").GetComponent<UnityEngine.UI.Text>();
        m_normalColor = m_frontText.color;

        if(Utilities.IsOpenLeveL(Utilities.GetLoadedLevel()))
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Starts the countdown timer
    /// </summary>
    public void StartCountDown(float secondsToCount, Action onReachTarget)
    {
        m_countDown = true;
        m_onReachTarget = onReachTarget;
        m_targetTime = secondsToCount;
        m_startTime = m_manager.GetTimePassed();
        m_frontText.color = m_normalColor;
    }

    /// <summary>
    /// Updates the timer text
    /// </summary>
    void Update()
    {
        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("Counting", m_countDown);
            Diagnostics.Add("Start Time", m_startTime);
            Diagnostics.Add("Target Time", m_targetTime);
        }

        if(m_countDown)
        {
            float timePassed = m_manager.GetTimePassed();
            float timeDifference = Mathf.Abs(timePassed - m_startTime);
            m_startTime = timePassed;

            m_targetTime -= timeDifference;
            m_targetTime = Mathf.Max(0.0f, m_targetTime);

            int seconds = (int)m_targetTime;
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

            if(m_targetTime <= 10.0f)
            {
                m_frontText.color = m_tenSecondColor;
            }
            else if(m_targetTime <= 30.0f)
            {
                m_frontText.color = m_thirtySecondColor;
            }

            if(m_targetTime == 0.0f)
            {
                m_countDown = false;
                m_onReachTarget();
            }
        }
    }
}
