////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ScoreTextUpdate.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GUICoinScore : MonoBehaviour 
{
    void Update () 
    {
        var player = PlayerManager.GetControllablePlayer();
        if(player != null)
        {
            GetComponent<UnityEngine.UI.Text>().text = 
                player.GetComponent<PlayerScore>().RoundedScore.ToString();
        }
    }
}
