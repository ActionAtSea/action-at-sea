using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileCaustics : MonoBehaviour 
{
	public float animationSpeed = 0.5f;
	public int tileAmountX = 20;
	public int tileAmountY = 20;
	public GameObject causticFrames;
	public GameObject baseFrame;

	private float timePassed = 0.0f;
	private List<GameObject> tiles = new List<GameObject>();
	private List<Sprite> frames = new List<Sprite>();
	private int currentTexture = 0;
	
	void Start ()
	{
		var causticFrame = causticFrames.GetComponentsInChildren<SpriteRenderer>();
		for(int i = 0; i < causticFrame.Length; ++i)
		{
			causticFrame[i].enabled = false;
			frames.Add(causticFrame[i].sprite);
		}

		var totalTiles = tileAmountX * tileAmountY;
		for (int i = 0; i < totalTiles; ++i)
		{
			tiles.Add((GameObject)(Instantiate(baseFrame)));
			tiles[i].name = "Caustics" + i.ToString();
			tiles[i].transform.parent = this.transform;
			tiles[i].GetComponent<SpriteRenderer>().enabled = true;
			tiles[i].GetComponent<SpriteRenderer>().sprite = 
				baseFrame.GetComponent<SpriteRenderer>().sprite;
		}

		var renderer = baseFrame.GetComponent<SpriteRenderer>();
		var textureSize = renderer.sprite.texture.width;
		var worldScale = (float)textureSize / renderer.sprite.pixelsPerUnit;
		var scale = baseFrame.transform.localScale.x;
		worldScale *= scale;

		// Lay out the duplicates to form a grid, initial position centers it on 0,0
		Vector2 initialPosition = new Vector2 (
			worldScale * ((tileAmountX - 1) * 0.5f),
			worldScale * ((tileAmountY - 1) * 0.5f));

		for(int x = 0; x < tileAmountX; ++x)
		{
			for(int y = 0; y < tileAmountY; ++y)
			{
				var index = x * tileAmountX + y;
				tiles[index].transform.position = new Vector3(
					initialPosition.x - (x * worldScale),
					initialPosition.y - (y * worldScale),
					0.0f);

				tiles[index].transform.localScale = new Vector3(scale, scale, 0.0f);
			}
		}
	}

	void Update()
	{
		timePassed += Time.deltaTime;
		if(timePassed >= animationSpeed)
		{
			timePassed = 0.0f;

			++currentTexture;
			if(currentTexture >= frames.Count)
			{
				currentTexture = 0;
			}

			for (int i = 0; i < tiles.Count; ++i)
			{
				tiles[i].GetComponent<SpriteRenderer>().sprite = frames[currentTexture];
			}
		}
	}
}
