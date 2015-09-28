////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUITimer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GUITimer : MonoBehaviour 
{
    private GameModeManager m_manager = null;
    private UnityEngine.UI.Text m_backText = null;
    private UnityEngine.UI.Text m_frontText = null;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        // Don't show the time if an open level
        //if(Utilities.IsOpenLeveL(Utilities.GetLoadedLevel()))
        {
        //    gameObject.SetActive(false);
        }
        //else
        {
            m_manager = GameModeManager.Get();
            m_backText = GetComponent<UnityEngine.UI.Text>();
            m_frontText = transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        }
    }

    /// <summary>
    /// Updates the timer text
    /// </summary>
    void Update()
    {
        float timePassed = m_manager.GetTimePassed();
        int seconds = (int)timePassed;
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
    }
}
