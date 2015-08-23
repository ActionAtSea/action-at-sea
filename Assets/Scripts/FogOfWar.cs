////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FogOfWar.cs
////////////////////////////////////////////////////////////////////////////////////////

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
    public float tileOffset = 0.75f;
    public float tileSize = 20.0f;             // Size multiplier of the tiles
    public float minRevealRadius = 5.0f;       // Distance the tiles are fully transparent from the player
    public float maxRevealRadius = 9.0f;       // Distance the tiles are fully opaque from the player
    public GameObject generatedFogParent;        

    private GameObject m_gameBoard = null;
    private int m_revealedCount = 0;
    private bool m_initialised = false;           // Whether the fog was correctly initialised
    private float m_worldScale;                   // Size of the texture in world space
    private int m_textureSize;                    // Dimensions of the texture
    private List<FogTile> m_tiles;                // List of all sprites 
    private List<FogTile> m_borderTiles;          // Row of tiles that cannot be edited 
    private FogTile m_minimapTile = null;         // Tile for the minimap
    private int m_minimapSize = 128;
    private Vector2 m_minimapWorldScale;
    private Vector2 m_tileInside;                 // Tile the player is currently inside
    private float m_worldRadius;
    private int m_tileAmountX = 0;                // Number of tiles along the X axis
    private int m_tileAmountY = 0;                // Number of tiles along the Y axis
    private float m_minimapMinReveal = 0.0f;
    private float m_minimapMaxReveal = 0.0f;

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
        if (maxRevealRadius >= m_worldScale) 
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
        m_gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        if(m_gameBoard == null)
        {
            Debug.LogError("Could not find game board");
        }

        var renderer = this.GetComponent<SpriteRenderer>();
        m_textureSize = renderer.sprite.texture.width;
        m_worldScale = (float)m_textureSize / renderer.sprite.pixelsPerUnit;
        m_worldScale *= tileSize;
        m_worldRadius = Mathf.Sqrt((m_worldScale * m_worldScale) + (m_worldScale * m_worldScale)) * 0.5f;
        m_borderTiles = new List<FogTile> ();
        m_tiles = new List<FogTile>();
        m_tileInside = new Vector2 ();
        m_tileInside.x = -1.0f;
        m_tileInside.y = -1.0f;

        if (!CanBeInitialised ()) 
        {
            return;
        }

        const int borderTiles = 2;
        const int extraTiles = 1 + borderTiles;
        var boardBounds = m_gameBoard.GetComponent<SpriteRenderer>().bounds;
        var boardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x);
        var boardLength = Mathf.Abs(boardBounds.max.y - boardBounds.min.y);
        m_tileAmountX = (int)Mathf.Ceil(boardWidth / m_worldScale) + extraTiles;
        m_tileAmountY = (int)Mathf.Ceil(boardLength / m_worldScale) + extraTiles;

        int ID = 0;
        for(int x = 0; x < m_tileAmountX; ++x)
        {
            for(int y = 0; y < m_tileAmountY; ++y)
            {
                CreateNewTile(x == 0 || y == 0 || x == m_tileAmountX-1 || y == m_tileAmountY-1, x, y, ID);
                ++ID;
            }
        }

        // Create the minimap fog
        Color[] pixels = new Color[m_minimapSize * m_minimapSize];
        for(int i = 0; i < m_minimapSize * m_minimapSize; ++i)
        {
            pixels[i].r = 1.0f;
            pixels[i].g = 1.0f;
            pixels[i].b = 1.0f;
            pixels[i].a = 1.0f;
        }

        m_minimapTile = new FogTile();
        InitialiseTile(m_minimapTile, m_minimapSize, "Minimap", pixels);

        m_minimapWorldScale = new Vector2(boardWidth, boardLength);
        var minimapScale = (float)m_minimapSize / renderer.sprite.pixelsPerUnit;
        var minimapScaleX = m_minimapWorldScale.x / minimapScale;
        var minimapScaleY = m_minimapWorldScale.y / minimapScale;

        m_minimapMaxReveal = maxRevealRadius;
        m_minimapMinReveal = (maxRevealRadius + minRevealRadius) / 2.0f;

        m_minimapTile.obj.transform.localScale = new Vector3 (minimapScaleX, minimapScaleY, 1.0f);
        m_minimapTile.renderer.enabled = false;
        m_minimapTile.obj.tag = "MinimapFog";

        // Actual removable fog amount
        m_tileAmountX -= borderTiles;
        m_tileAmountY -= borderTiles;
        m_initialised = true;
    }

    /**
    * Creates a new editable fog tile
    */
    void CreateNewTile(bool isBorder, int x, int y, int ID)
    {
        List<FogTile> tileContainer = isBorder ? m_borderTiles : m_tiles;
        tileContainer.Add(new FogTile());

        int i = tileContainer.Count - 1;
        InitialiseTile (tileContainer[i], m_textureSize, ID.ToString (),
                        this.GetComponent<SpriteRenderer>().sprite.texture.GetPixels());

        var gridScale = m_worldScale * tileOffset;
        Vector2 initialPosition = new Vector2 (
            gridScale * ((m_tileAmountX - 1) * 0.5f),
            gridScale * ((m_tileAmountY - 1) * 0.5f));

        tileContainer[i].obj.transform.position = new Vector3(
            initialPosition.x - (x * gridScale),
            initialPosition.y - (y * gridScale),
            transform.position.z);
    }

    /**
    * Initialises a new editable fog tile
    */
    void InitialiseTile(FogTile tile, int size, string name, Color[] pixels)
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
        tile.obj.transform.parent = generatedFogParent.transform;
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
        return x * m_tileAmountX + y;
    }

    /**
    * Determines whether the position is inside the tile at x,y in the grid
    */
    bool IsInsideTile(Vector3 position, int x, int y)
    {
        var spritePosition = m_tiles[GetIndex (x, y)].obj.transform.position;
        return IsInsideBounds (position.x, position.y, spritePosition.x, spritePosition.y, m_worldScale);
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
        if (!m_initialised)
        {
            return;
        }

        var player = PlayerManager.GetControllablePlayer();
        if(player != null && IsInsideFog(player))
        {
            SolveSmoothFog(player);
        }

        //Debug.Log (revealedCount);
        m_revealedCount = 0;
    }

    /**
    * Detemines which of the tiles the player is inside
    */
    bool IsInsideFog(GameObject player)
    {
        if (m_tileInside.x == -1.0f || m_tileInside.y == -1.0f || 
            !IsInsideTile (player.transform.position, (int)m_tileInside.x, (int)m_tileInside.y))
        {
            m_tileInside.x = -1.0f;
            m_tileInside.y = -1.0f;

            for(int x = 0; x < m_tileAmountX; ++x)
            {
                for(int y = 0; y < m_tileAmountY; ++y)
                {
                    if(IsInsideTile(player.transform.position, x, y))
                    {
                        m_tileInside.x = (float)x;
                        m_tileInside.y = (float)y;
                        break;
                    }
                }
            }
        }

        return true;
    }

    /**
    * Removes pixels from a tile
    */
    void RemoveFog(Vector2 playerPosition, FogTile tile, float scaleX, float scaleY, int dimensions, float minReveal, float maxReveal)
    {
        if (tile.pixels == null) 
        {
            tile.pixels = tile.texture.GetPixels32();;
        }
        
        float pixelSizeX = scaleX / (float)dimensions;
        float pixelSizeY = scaleY / (float)dimensions;
        float halfSizeX = scaleX / 2.0f;
        float halfSizeY = scaleY / 2.0f;
        
        Vector2 pixelPosition = new Vector2();
        
        // Pixels start at bottom left corner and move upwards
        Vector2 pixelStart = new Vector2(tile.obj.transform.position.x - halfSizeX, 
                                         tile.obj.transform.position.y - halfSizeY);
        
        for(int r = 0; r < dimensions; ++r)
        {
            for(int c = 0; c < dimensions; ++c)
            {
                pixelPosition.x = pixelStart.x + (c * pixelSizeX);
                pixelPosition.y = pixelStart.y + (r * pixelSizeY);
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

    /**
    * Bounds checks the tile before removing fog
    */
    void RemoveFogSmoothly(Vector2 playerPosition, int x, int y)
    {
        var index = GetIndex (x, y);
        if (index >= 0 && index < m_tiles.Count) 
        {
            var tile = m_tiles[index];

            // Sphere-sphere Bounds checking
            Vector2 position = new Vector2(tile.obj.transform.position.x, tile.obj.transform.position.y);
            if(Vector2.Distance(position, playerPosition) <= m_worldRadius + maxRevealRadius)
            {
                RemoveFog(playerPosition, tile, m_worldScale, m_worldScale, m_textureSize, minRevealRadius, maxRevealRadius);
                m_revealedCount++;
            }
        }
    }

    /**
    * Solves for a smooth fog effect
    */
    void SolveSmoothFog(GameObject player) 
    {
        // Assumes tile scale is less than max reveal radius from initialisation check
        // Because of this only remove fog from tile inside and all 8 surrounding tiles

        Vector2 playerPosition = new Vector2(player.transform.position.x, 
                                             player.transform.position.y);

        int x = (int)m_tileInside.x;
        int y = (int)m_tileInside.y;

        RemoveFogSmoothly(playerPosition, x, y);
        RemoveFogSmoothly(playerPosition, x, y+1);
        RemoveFogSmoothly(playerPosition, x, y-1);
        RemoveFogSmoothly(playerPosition, x+1, y);
        RemoveFogSmoothly(playerPosition, x-1, y);
        RemoveFogSmoothly(playerPosition, x+1, y+1);
        RemoveFogSmoothly(playerPosition, x-1, y-1);
        RemoveFogSmoothly(playerPosition, x+1, y-1);
        RemoveFogSmoothly(playerPosition, x-1, y+1);

        RemoveFog (playerPosition, 
                   m_minimapTile, 
                   m_minimapWorldScale.x,
                   m_minimapWorldScale.y,
                   m_minimapSize, 
                   m_minimapMinReveal,
                   m_minimapMaxReveal);
    }
}
