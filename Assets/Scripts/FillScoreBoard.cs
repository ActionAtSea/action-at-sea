////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FillScoreBoard.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FillScoreBoard : MonoBehaviour 
{
    /// <summary>
    /// Compiles a score board from all game players
    /// </summary>
    void Update () 
    {
        List<GameObject> players = new List<GameObject>();

        var player = PlayerManager.GetControllablePlayer();
        if(player != null)
        {
            players.Add(player);
        }

        GameObject[] enemies = PlayerManager.GetEnemies();
        if(enemies != null && enemies.Length > 0)
        {
            players.AddRange(enemies);
        }

        players = players.OrderByDescending(x => NetworkedPlayer.GetPlayerScore(x)).ToList();
        var textUI = GetComponent<UnityEngine.UI.Text>();
        textUI.text = "";

        foreach(GameObject obj in players)
        {
            if(obj != null)
            {
                textUI.text += NetworkedPlayer.GetPlayerScore(obj).ToString() + ": " +
                    NetworkedPlayer.GetPlayerName(obj) + "\n";
            }
        }
    }
}
