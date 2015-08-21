////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FillScoreBoard.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FillScoreBoard : MonoBehaviour 
{
    /**
    * Compiles a score board from all game players
    */
    void Update () 
    {
        List<GameObject> players = new List<GameObject>();

        if(GamePlayers.GetControllablePlayer() != null)
        {
            players.Add(GamePlayers.GetControllablePlayer());
        }

        GameObject[] enemies = GamePlayers.GetEnemies();
        if(enemies.Length > 0)
        {
            players.AddRange(enemies);
        }

        players = players.OrderByDescending(x => x.GetComponent<NetworkedPlayer>().PlayerScore).ToList();
        var textUI = GetComponent<UnityEngine.UI.Text>();
        textUI.text = "";

        foreach(GameObject player in players)
        {
            textUI.text += player.GetComponent<NetworkedPlayer>().PlayerScore.ToString() + ": " 
                + player.GetComponent<NetworkedPlayer>().PlayerName + "\n";
        }
    }
}
