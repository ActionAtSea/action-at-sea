////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUIJoiningGame.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GUIJoiningGame : MonoBehaviour
{
    private float m_timer = 0.0f;
    private bool m_joined = false;
    private int m_dots = 0;
    private int m_maxDots = 4;
    private float m_dotSpeed = 0.25f;

    void Update() 
    {
        if(!m_joined)
        {
            m_joined = RandomMatchmaker.Get().IsConnected();

            if(m_joined)
            {
                GetComponent<UnityEngine.UI.Text>().enabled = false;
            }

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

            GetComponent<UnityEngine.UI.Text>().text = "Entering Game";
            for(int i = 0; i < m_dots; ++i)
            {
                GetComponent<UnityEngine.UI.Text>().text += ".";
            }
        }
    }
}