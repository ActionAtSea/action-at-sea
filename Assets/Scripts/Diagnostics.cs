////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Diagnostics.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// F1 Shows diagnostic information
/// F2 Shows messages from the console
/// Tick Tick should be ticked and then the application run
/// </summary>
public class Diagnostics : MonoBehaviour 
{
    public bool m_tickTest = false;
    private float m_timePassed = 0.0f;
    private static bool sm_tickTest = false;
    private static bool sm_registeredLogCallback = false;
    private static bool sm_renderDiagnostics = false;
    private static bool sm_renderLogging = false;
    private static StringBuilder sm_diagnostics = new StringBuilder();
    private static string sm_rendererdText = "";
    private static List<string> sm_logMessages = new List<string>();

    void Start()
    {
        if(!sm_registeredLogCallback)
        {
            UnityEngine.Application.logMessageReceived += Diagnostics.OnLogConsole;
            sm_registeredLogCallback = true;
        }

        if(!sm_tickTest && m_tickTest)
        {
            sm_tickTest = true;
            Application.LoadLevel(0);
        }
    }

    /// <summary>
    /// Callback when logging to the console
    /// </summary>
    static void OnLogConsole(string log, string stackTrace, LogType type)
    {
        sm_logMessages.Add(type.ToString() + ": " + log);

        const int maxLogEntries = 40;
        if(sm_logMessages.Count > maxLogEntries)
        {
            sm_logMessages.RemoveAt(0);
        }
    }

    /// <summary>
    /// Gets whether the diagnostics are rendering
    /// </summary>
    static public bool IsActive()
    {
        return sm_renderDiagnostics;
    }

    /// <summary>
    /// Adds a new line to the diagnostic text
    /// </summary>
    static public void Add(string category, string text)
    {
        sm_diagnostics.Append(" " + category + ": " + text + "\n");
    }

    /// <summary>
    /// Adds a new line to the diagnostic text
    /// </summary>
    static public void Add<T>(string category, T text)
    {
        Add(category, text.ToString());
    }

    /// <summary>
    /// Toggles whether the diagnostics are visible
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            sm_renderDiagnostics = !sm_renderDiagnostics;
            sm_renderLogging = false;
        }
        else if(Input.GetKeyDown(KeyCode.F2))
        {
            sm_renderLogging = !sm_renderLogging;
            sm_renderDiagnostics = false;
        }

        if(sm_tickTest)
        {
            Utilities.SetMaximumPlayers(1);

            m_timePassed += Time.deltaTime;

            float maxTime = Utilities.IsLevelLoaded() ? 15.0f : 1.0f;

            if(m_timePassed >= maxTime)
            {
                m_timePassed = 0.0f;
                int level = Application.loadedLevel + 1;
                if(level < (int)SceneID.MAX_SCENES)
                {
                    Application.LoadLevel(level);
                }
                else
                {
                    sm_tickTest = false;
                    Debug.Log("TICK TEST COMPLETE");
                }
            }
        }
    }

    /// <summary>
    /// Saves the diagnostic text
    /// </summary>
    void LateUpdate()
    {
        if(sm_renderDiagnostics && sm_diagnostics.Length > 0)
        {
            sm_diagnostics.Remove(sm_diagnostics.Length-1, 1);
            sm_rendererdText = sm_diagnostics.ToString();
            sm_diagnostics = new StringBuilder();
        }
    }

    /// <summary>
    /// Displays diagnostic information about the network
    /// </summary>
    void OnGUI()
    {
        if(sm_renderDiagnostics)
        {
            GUILayout.TextArea(sm_rendererdText);
        }
        else if(sm_renderLogging)
        {
            string text = "";
            foreach(var log in sm_logMessages)
            {
                text += log + "\n";
            }
            text.Remove(text.Length-1, 1);
            GUILayout.TextArea(text);
        }
    }
}
