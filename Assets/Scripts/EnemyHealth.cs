using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour 
{
	public float xOffset = 1.0f;
	public float yOffset = 1.0f;
	public GameObject enemyShip;
	
	void Update () 
	{
		if(enemyShip != null)
		{
			transform.localPosition = new Vector3 (
				enemyShip.transform.position.x + xOffset,
				enemyShip.transform.position.y + yOffset, 0.0f);
		}
	}
}
