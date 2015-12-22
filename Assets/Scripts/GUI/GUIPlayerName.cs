////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUIPlayerName.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Used by the player name in game
/// </summary>
public class GUIPlayerName : MonoBehaviour
{
    private UnityEngine.UI.Text m_text = null;
    private UnityEngine.UI.Text m_backtext = null;
    private string m_name = "";
    private Color m_colour;

    void Start()
    {
        m_backtext = transform.parent.gameObject.GetComponent<UnityEngine.UI.Text>();
        m_text = GetComponent<UnityEngine.UI.Text>();
        m_text.text = "";
        m_backtext.text = m_text.text;
    }

    public void SetName(string name)
    {
        Utilities.SetPlayerName(name);
    }

    void Update()
    {
        var player = PlayerManager.GetControllablePlayer();
        if (player != null)
        {
            string name = Utilities.GetPlayerName();
            if (m_name != name)
            {
                m_name = name;
                m_text.text = m_name;
                m_backtext.text = m_name;
            }

            Color color = Utilities.GetPlayerColor(player);
            if (m_colour != color)
            {
                m_colour = color;
                m_text.color = color;
            }
        }
    }
}