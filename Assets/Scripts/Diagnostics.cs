////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Diagnostics.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Diagnostics : MonoBehaviour 
{
    private static bool sm_renderDiagnostics = false;
    private static StringBuilder sm_diagnostics = new StringBuilder();
    private static string sm_rendererdText = "";

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
    }
}
