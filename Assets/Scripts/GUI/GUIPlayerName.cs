////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUIPlayerName.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GUIPlayerName : MonoBehaviour
{
    void Start () 
    {
        var name = GameInformation.GetPlayerName();
        if(name == GameInformation.GetDefaultName() && Application.loadedLevel == (int)SceneID.MENU)
        {
            name = "Enter Name...";
        }
        GetComponent<UnityEngine.UI.Text>().text = name;
    }
}