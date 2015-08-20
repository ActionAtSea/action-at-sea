using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
* Holds information about an instances of the fog sprite 
*/
class FogTile
{
	public Texture2D texture;
	public GameObject obj;
	public SpriteRenderer renderer;
	public Sprite sprite;
	public Color32[] pixels;
}

/**
* Main script class
*/
public class FogOfWar : MonoBehaviour 
{
	public int tileAmountX = 20;                // Number of tiles along the X axis
	public int tileAmountY = 20;                // Number of tiles along the Y axis
	public float tileOffset = 1.0f;
	public float tileSize = 1.0f;				// Size multiplier of the tiles
	public float minRevealRadius = 5.0f;        // Distance the tiles are fully transparent from the player
	public float maxRevealRadius = 10.0f;       // Distance the tiles are fully opaque from the player
	public GameObject player;                   // Player to remove the fog
	public GameObject generatedFogParent;		
	public GameObject minimapFogParent;
	public float minimapScale = 1.0f;
	public float minimapRevealRadius = 01.0f;

	private int revealedCount = 0;
	private FogTile minimapFog = new FogTile();
	private bool initialised = false;           // Whether the fog was correctly initialised
	private float worldScale;		            // Size of the texture in world space
	private float minimapWorldScale;			// Size of the minimap in world space
	private int textureSize;				    // Dimensions of the texture
	private int minimapSize = 128;				// Dimensions of the minimap texture
	private List<FogTile> tiles;                // List of all sprites 
	private List<FogTile> borderTiles;           // Row of tiles that cannot be edited 
	private Vector2 tileInside;                 // Tile the player is currently inside
	private Vector2 playerPosition;				// Currently position of the player
	private Vector3 previousPlayerPosition;     // Position the player was previously in
	private float worldRadius;

	/**
    * Determines whether the fog is set up correctly
	*/
	bool CanBeInitialised()
	{
		var renderer = this.GetComponent<SpriteRenderer>();

		if (renderer.sprite.texture.width != renderer.sprite.texture.height) 
		{
			Debug.LogError("Fog: Fog sprite texture scale needs to match");
			return false;
		}
		if (minRevealRadius >= maxRevealRadius)
		{
			Debug.LogError("Fog: Min reveal radius must be smaller than max");
			return false;
		}
		if (maxRevealRadius >= worldScale) 
		{
			Debug.LogError("Fog: Max reveal radius must be smaller than tile scale");
			return false;
		}

		return true;
	}

	/**
	* Initialises the fog sprites
	*/
	void Start ()
	{
		var renderer = this.GetComponent<SpriteRenderer>();
		textureSize = renderer.sprite.texture.width;
		worldScale = (float)textureSize / renderer.sprite.pixelsPerUnit;
		worldScale *= tileSize;
		worldRadius = Mathf.Sqrt((worldScale * worldScale) + (worldScale * worldScale)) * 0.5f;

		borderTiles = new List<FogTile> ();
		tiles = new List<FogTile>();
		previousPlayerPosition = new Vector3 ();
		
		tileInside = new Vector2 ();
		tileInside.x = -1.0f;
		tileInside.y = -1.0f;

		if (!CanBeInitialised ()) 
		{
			return;
		}

		int ID = 0;
		for(int x = 0; x < tileAmountX; ++x)
		{
			for(int y = 0; y < tileAmountY; ++y)
			{
				CreateNewTile(x == 0 || y == 0 || x == tileAmountX-1 || y == tileAmountY-1, x, y, ID);
				++ID;
			}
		}

		// Actual removable fog amount
		tileAmountX -= 2;
		tileAmountY -= 2;

		// Create the minimap fog
		Color[] pixels = new Color[minimapSize * minimapSize];
		for(int i = 0; i < minimapSize * minimapSize; ++i)
		{
			pixels[i].r = 159.0f/255.0f;
			pixels[i].g = 138.0f/255.0f;
			pixels[i].b = 90.0f/255.0f;
			pixels[i].a = 255.0f/255.0f;
		}

		InitialiseTile (minimapFog, minimapSize, "Minimap", pixels, minimapFogParent.transform);
		minimapFog.obj.transform.localScale = new Vector3 (minimapScale, minimapScale, 1.0f);
		minimapFog.renderer.enabled = false;
		minimapWorldScale = (float)minimapSize / renderer.sprite.pixelsPerUnit;
		minimapWorldScale *= minimapScale;

		initialised = true;
	}

	void CreateNewTile(bool isBorder, int x, int y, int ID)
	{
		List<FogTile> tileContainer = isBorder ? borderTiles : tiles;
		tileContainer.Add(new FogTile());

		int i = tileContainer.Count - 1;
		InitialiseTile (tileContainer [i], textureSize, ID.ToString (),
		                this.GetComponent<SpriteRenderer> ().sprite.texture.GetPixels (),
		                generatedFogParent.transform);

		var gridScale = worldScale * tileOffset;
		Vector2 initialPosition = new Vector2 (
			gridScale * ((tileAmountX - 1) * 0.5f),
			gridScale * ((tileAmountY - 1) * 0.5f));

		tileContainer[i].obj.transform.position = new Vector3(
			initialPosition.x - (x * gridScale),
			initialPosition.y - (y * gridScale),
			transform.position.z);
	}

	void InitialiseTile(FogTile tile, int size, string name, Color[] pixels, Transform parent)
	{
		var renderer = this.GetComponent<SpriteRenderer> ();
		Vector2 pivot = new Vector2 (0.5f, 0.5f);
		var rect = new Rect (0, 0, size, size);

		tile.texture = new Texture2D (size, size);
		tile.texture.SetPixels (0, 0, size, size, pixels);
		tile.texture.filterMode = FilterMode.Bilinear;
		tile.texture.wrapMode = TextureWrapMode.Clamp;
		tile.texture.Apply ();
		tile.texture.name = "Fog Texture " + name;
		tile.sprite = Sprite.Create (tile.texture, rect, pivot, renderer.sprite.pixelsPerUnit);
		tile.sprite.name = "Fog Sprite" + name;
		tile.obj = new GameObject ();
		tile.obj.name = "Fog" + name;
		tile.obj.transform.parent = parent;
		tile.obj.transform.localScale = new Vector3 (tileSize, tileSize, 0.0f);
		tile.obj.AddComponent<SpriteRenderer> ();
		tile.renderer = tile.obj.GetComponent<SpriteRenderer> ();
		tile.renderer.sprite = tile.sprite;
		tile.renderer.name = "Fog Renderer " + name;
		tile.renderer.sortingOrder = renderer.sortingOrder;
		tile.renderer.sortingLayerName = renderer.sortingLayerName;
	}

	/**
	* Retrieves an idex for the sprites list from the rows/column of the grid
	*/
	int GetIndex(int x, int y)
	{
		return x * tileAmountX + y;
	}

	/**
	* Determines whether the position is inside the tile at x,y in the grid
	*/
	bool IsInsideTile(Vector3 position, int x, int y)
	{
		var spritePosition = tiles[GetIndex (x, y)].obj.transform.position;
		return IsInsideBounds (position.x, position.y, spritePosition.x, spritePosition.y, worldScale);
	}

	/**
	* Determines if the position is inside the bounds
	*/
	bool IsInsideBounds(float x, float y, float boundsX, float boundsY, float boundsScale)
	{
		float halfScale = boundsScale * 0.5f;
		return x < boundsX + halfScale &&
               y < boundsY + halfScale &&
               x >= boundsX - halfScale &&
               y >= boundsY - halfScale;
	}

	/**
	* Change the range a value is between
	*/
	float ConvertRange(float value, float currentInner, float currentOuter, float newInner, float newOuter)
	{
		return ((value-currentInner)*((newOuter-newInner)/(currentOuter-currentInner)))+newInner;
	}

	/**
	* Updates the fog of war
	*/
	void Update () 
	{
		if (!initialised)
		{
			return;
		}

		if(player == null)
		{
			player = GameObject.FindWithTag("Player");
			return;
		}

		if(IsInsideFog())
		{
			SolveSmoothFog();
		}

		//Debug.Log (revealedCount);
		revealedCount = 0;
	}

	/**
    * Detemines which of the tiles the player is inside
	*/
	bool IsInsideFog()
	{
		if (tileInside.x == -1.0f || tileInside.y == -1.0f || 
		    !IsInsideTile (player.transform.position, (int)tileInside.x, (int)tileInside.y))
		{
			tileInside.x = -1.0f;
			tileInside.y = -1.0f;

			for(int x = 0; x < tileAmountX; ++x)
			{
				for(int y = 0; y < tileAmountY; ++y)
				{
					if(IsInsideTile(player.transform.position, x, y))
					{
						tileInside.x = (float)x;
						tileInside.y = (float)y;
						break;
					}
				}
			}
		}

		return true;
	}
	
	void RemoveFog(Vector2 playerPosition, FogTile tile, float scale, int dimensions, float minReveal, float maxReveal)
	{
		if (tile.pixels == null) 
		{
			tile.pixels = tile.texture.GetPixels32();;
		}
		
		float pixelSize = scale / (float)dimensions;
		float halfSize = scale / 2.0f;
		
		Vector2 pixelPosition = new Vector2();
		
		// Pixels start at bottom left corner and move upwards
		Vector2 pixelStart = new Vector2(tile.obj.transform.position.x - halfSize, 
		                                 tile.obj.transform.position.y - halfSize);
		
		for(int r = 0; r < dimensions; ++r)
		{
			for(int c = 0; c < dimensions; ++c)
			{
				pixelPosition.x = pixelStart.x + (c * pixelSize);
				pixelPosition.y = pixelStart.y + (r * pixelSize);
				float distance = Vector2.Distance(pixelPosition, playerPosition);
				
				if(distance <= maxRevealRadius)
				{
					byte alpha = (byte)Mathf.Clamp(ConvertRange(distance, 
						minReveal, maxReveal, 0.0f, 255.0f), 0.0f, 255.0f);
					
					var pixelIndex = r * dimensions + c;
					if(tile.pixels[pixelIndex].a > alpha)
					{
						tile.pixels[pixelIndex].a = alpha;
					}
				}
			}
		}
		
		tile.texture.SetPixels32(tile.pixels);
		tile.texture.Apply();
	}

	void RemoveMinimap(Vector2 playerPosition)
	{
		RemoveFog (playerPosition, minimapFog, minimapWorldScale, minimapSize, minimapRevealRadius, minimapRevealRadius);
	}
	
	void RemoveFogSmoothly(Vector2 playerPosition, int x, int y)
	{
		var index = GetIndex (x, y);
		if (index >= 0 && index < tiles.Count) 
		{
			var tile = tiles[index];

			// Sphere-sphere Bounds checking
			Vector2 position = new Vector2(tile.obj.transform.position.x, tile.obj.transform.position.y);
			if(Vector2.Distance(position, playerPosition) <= worldRadius + maxRevealRadius)
			{
				RemoveFog(playerPosition, tile, worldScale, textureSize, minRevealRadius, maxRevealRadius);
				revealedCount++;
			}
		}
	}

	/**
    * Solves for a smooth fog effect
	*/
	void SolveSmoothFog() 
	{
		// Only remove fog if player has moved
		// This is because smooth fog is expensive
		//const float minMovement = 0.01f;
		//if (isFirstTick || Vector2.Distance (previousPlayerPosition, player.transform.position) > minMovement) 
		{
			// Assumes tile scale is less than max reveal radius from initialisation check
			// Because of this only remove fog from tile inside and all 8 surrounding tiles

			Vector2 playerPosition = new Vector2(player.transform.position.x, 
			                                     player.transform.position.y);

			int x = (int)tileInside.x;
			int y = (int)tileInside.y;

			RemoveFogSmoothly(playerPosition, x, y);
			RemoveFogSmoothly(playerPosition, x, y+1);
			RemoveFogSmoothly(playerPosition, x, y-1);
			RemoveFogSmoothly(playerPosition, x+1, y);
			RemoveFogSmoothly(playerPosition, x-1, y);
			RemoveFogSmoothly(playerPosition, x+1, y+1);
			RemoveFogSmoothly(playerPosition, x-1, y-1);
			RemoveFogSmoothly(playerPosition, x+1, y-1);
			RemoveFogSmoothly(playerPosition, x-1, y+1);
			RemoveMinimap(playerPosition);

			previousPlayerPosition.x = player.transform.position.x;
			previousPlayerPosition.y = player.transform.position.y;
			previousPlayerPosition.z = player.transform.position.z;
		}
	}
}
