////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ScoreTextUpdate.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GUICoinScore : MonoBehaviour 
{
    private UnityEngine.UI.Text m_text = null;

    void Start()
    {
        m_text = GetComponent<UnityEngine.UI.Text>();
        m_text.text = "0";
    }

    void Update () 
    {
        var player = PlayerManager.GetControllablePlayer();
        if(player != null)
        {
            m_text.text = player.GetComponent<PlayerScore>().RoundedScore.ToString();
        }
    }
}
