////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Common.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/**
* Fade state
*/
enum FadeState
{
	FADE_IN,
	FADE_OUT,
	NO_FADE
};

/**
* Scene IDS
*/
enum SceneID
{
	MENU = 0,
	MOVE_AND_FIRE = 1,
	DISCOVER_LAND = 2,
	UPGRADING_DOCKS = 3,
	ENEMIES = 4,
	TREASURE = 5,
	GAME = 6
}

/**
* Utility class
*/
class Utilities
{
	static public bool IsCloseToPlayer(Vector3 position)
	{
		var player = GameObject.FindGameObjectWithTag("Player");
		if(player != null)
		{
			const float maxDistance = 30.0f;
			return (player.transform.position - position).magnitude <= maxDistance;
		}
		return false;
	}
}