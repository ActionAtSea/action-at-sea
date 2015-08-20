using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FillScoreBoard : MonoBehaviour 
{
	void Update () 
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("EnemyPlayer");
		GameObject mainPlayer = GameObject.FindGameObjectWithTag("Player");
		List<GameObject> allPlayers = new List<GameObject>();

		if(mainPlayer != null)
		{
			allPlayers.Add(mainPlayer);
		}

		if(players.Length > 0)
		{
			allPlayers.AddRange(players);
		}

		allPlayers = allPlayers.OrderByDescending(x => x.GetComponent<NetworkedPlayer>().PlayerScore).ToList();
		var text = GetComponent<UnityEngine.UI.Text>();
		text.text = "";

		foreach(GameObject player in allPlayers)
		{
			text.text += player.GetComponent<NetworkedPlayer>().PlayerScore.ToString() + ": " 
				+ player.GetComponent<NetworkedPlayer>().PlayerName + "\n";
		}
	}
}
