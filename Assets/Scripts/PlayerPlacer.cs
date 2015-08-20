using UnityEngine;
using System.Collections;

public class PlayerPlacer : MonoBehaviour 
{	
	public GameObject gameboard;
	public GameObject terrain;
	private float gameboardOffset = 20.0f;
	private float playerRadious = 5.0f;

	public Vector2 GetNewPosition()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("EnemyPlayer");

		bool foundPosition = false;
		Vector2 position = new Vector3(0, 0);
		
		var boardBounds = gameboard.GetComponent<SpriteRenderer> ().bounds;
		while (!foundPosition) 
		{
			foundPosition = true;
			position.x = Random.Range (-boardBounds.extents.x + gameboardOffset, boardBounds.extents.x - gameboardOffset);
			position.y = Random.Range (-boardBounds.extents.y + gameboardOffset, boardBounds.extents.y - gameboardOffset);
			
			var islands = terrain.GetComponentsInChildren<SpriteRenderer>();
			for(int i = 0; i < islands.Length; ++i)
			{
				var islandBounds = islands[i].bounds;
				if(position.x > islandBounds.center.x - islandBounds.extents.x &&
				   position.x < islandBounds.center.x + islandBounds.extents.x &&
				   position.y > islandBounds.center.y - islandBounds.extents.y &&
				   position.y < islandBounds.center.y + islandBounds.extents.y)
				{
					foundPosition = false;
					break;
				}
			}

			if(foundPosition)
			{
				foreach(GameObject player in players)
				{
					Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
					Vector2 difference = position - playerPosition;
					if(difference.magnitude <= playerRadious)
				    {
						foundPosition = false;
						break;
					}
				}
			}
		}

		return position;
	}
}
