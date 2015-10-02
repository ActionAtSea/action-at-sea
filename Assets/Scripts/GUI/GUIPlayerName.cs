////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUIPlayerName.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/// <summary>
/// Used by the input field in the lobby 
/// and by the player name in game
/// </summary>
public class GUIPlayerName : MonoBehaviour
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
            placeholder.text = name;
        }
    }

    void Update()
    {
        if(!Utilities.IsLevelLoaded())
        {
            string text = GetComponent<UnityEngine.UI.Text>().text;
            if(text.Length > 0)
            {
                Utilities.SetPlayerName(text);
            }
        }
    }
}