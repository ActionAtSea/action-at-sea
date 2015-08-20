using UnityEngine;
using System.Collections;

public class ScoreTextUpdate : MonoBehaviour 
{
	PlayerScore scoreController = null;

	void Update () 
	{
		if(scoreController == null)
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			if(player != null)
			{
				scoreController = player.GetComponent<PlayerScore>();
			}
			return;
		}

		GetComponent<UnityEngine.UI.Text>().text = scoreController.RoundedScore.ToString();
	}
}
