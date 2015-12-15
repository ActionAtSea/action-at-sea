////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ScoreTextUpdate.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GUICoinScore : MonoBehaviour 
{
    private UnityEngine.UI.Text m_text = null;
    private UnityEngine.UI.Text m_backtext = null;

    void Start()
    {
        m_backtext = transform.parent.gameObject.GetComponent<UnityEngine.UI.Text>();
        m_text = GetComponent<UnityEngine.UI.Text>();
        m_text.text = "0";
        m_backtext.text = m_text.text;
    }

    void Update () 
    {
        var player = PlayerManager.GetControllablePlayer();
        if(player != null)
        {
            m_text.text = player.GetComponent<PlayerScore>().RoundedScore.ToString();
            m_backtext.text = m_text.text;
        }
    }
}
