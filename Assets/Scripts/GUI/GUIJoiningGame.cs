////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUIJoiningGame.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GUIJoiningGame : MonoBehaviour
{
    private float m_timer = 0.0f;
    private int m_dots = 0;
    private int m_maxDots = 4;
    private float m_dotSpeed = 0.25f;
    private NetworkMatchmaker m_network = null;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_network = NetworkMatchmaker.Get();
    }

    /// <summary>
    /// Get string dots
    /// </summary>
    private string GetDots()
    {
        m_timer += Time.deltaTime;
        if(m_timer >= m_dotSpeed)
        {
            m_timer = 0.0f;
            m_dots++;
            if(m_dots >= m_maxDots)
            {
                m_dots = 0;
            }
        }
        
        string dots = "";
        for(int i = 0; i < m_dots; ++i)
        {
            dots += ".";
        }
        return dots;
    }

    /// <summary>
    /// Updates the script
    /// </summary>
    void Update() 
    {
        if(!m_network.IsInRoom())
        {
            GetComponent<UnityEngine.UI.Text>().enabled = true;
            GetComponent<UnityEngine.UI.Text>().text = "Entering Game" + GetDots();
        }
        else
        {
            GetComponent<UnityEngine.UI.Text>().enabled = false;
        }
    }
}