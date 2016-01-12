////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUIPlayerInput.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Used by the input field in the lobby 
/// </summary>
public class GUIPlayerInput : MonoBehaviour
{
    public UnityEngine.UI.Text placeholder = null;

    void Start()
    {
        string name = Utilities.GetPlayerName();
        
        if(Utilities.IsLevelLoaded())
        {
            GetComponent<UnityEngine.UI.Text>().text = name;
        }
        else if(name != Utilities.GetPlayerDefaultName())
        {
            GetComponent<UnityEngine.UI.Text>().text = name;
            if (placeholder != null)
            {
                placeholder.text = name;
            }
        }
    }

    public void SetName(string name)
    {
        Utilities.SetPlayerName(name);
    }

    void Update()
    {
        if (!Utilities.IsLevelLoaded())
        {
            string text = GetComponent<UnityEngine.UI.Text>().text;
            if(text.Length > 0)
            {
                Utilities.SetPlayerName(text);
            }
        }
    }
}