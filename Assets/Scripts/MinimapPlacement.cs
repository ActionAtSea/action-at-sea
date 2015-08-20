using UnityEngine;
using System.Collections;

/**
* Keeps the minimap placed in screen space
*/
public class MinimapPlacement : MonoBehaviour 
{
	public Camera mainCamera;
	public int screenTopLeftX = 200;
	public int screenTopLeftY = 200;
	public float screenScale = 0.8f;
	private float depth = 0.0f;
	private Vector3 position = new Vector3();

	void Start () 
	{
		depth = transform.localPosition.z;

		// All graphics are disabled to prevent cluttering
		var mapGraphics = this.GetComponentsInChildren<SpriteRenderer>();
		for(int i = 0; i < mapGraphics.Length; ++i)
		{
			mapGraphics[i].enabled = true;
		}
	}

	void Update () 
	{
		position.x = (float)screenTopLeftX;
		position.y = mainCamera.pixelHeight - (float)screenTopLeftY;
		position = mainCamera.ScreenToWorldPoint(position);
		position.z = depth;
		transform.position = position;

		transform.localScale = new Vector3 (screenScale, screenScale, 1.0f);
	}
}
