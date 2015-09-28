////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Diagnostics.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Diagnostics : MonoBehaviour 
{
    private static bool sm_renderDiagnostics = false;
    private static string sm_diagnostics = "";
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
        sm_diagnostics += category + ": " + text + "\n";
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
        sm_rendererdText = string.Copy(sm_diagnostics);
        sm_diagnostics = "";
    }

    /// <summary>
    /// Displays diagnostic information about the network
    /// </summary>
    void OnGUI()
    {
        if(sm_renderDiagnostics)
        {
            GUILayout.Label(sm_rendererdText);
        }
    }
}
