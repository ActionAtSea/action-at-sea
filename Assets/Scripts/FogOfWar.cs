////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FogOfWar.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a series of editable tiles that the player can interact with
/// </summary>
public class FogOfWar : MonoBehaviour 
{
    public GameObject generatedFog = null;      /// Holds all generated fog in scene, set through inspector
    public bool useGeneratedFogTiles = false;

    private List<FogTile> m_editableTiles;      /// Tiles that can be interacted with
    private List<FogTile> m_borderTiles;        /// Tiles that cannot be interacted with
    private const float m_tileSpacing = 0.75f;  /// Spacing offset between adjacent fog tiles
    private const float m_tileSize = 20.0f;     /// Local Size of the fog tiles
    private int m_tileInsideX = -1;             /// Fog tile the player is currently inside along the X axis
    private int m_tileInsideZ = -1;             /// Fog tile the player is currently inside along the Z axis
    private float m_minRevealRadius = 10.0f;     /// Minimum radius around the player fog is revealed // 5 (minimap)
    private float m_maxRevealRadius = 20.0f;     /// Maximum radius around the player fog is revealed // 9 (mini map)
    private int m_tileAmountX = 0;              /// Number of tiles along the X axis
    private int m_tileAmountZ = 0;              /// Number of tiles along the Z axis
    private float m_worldScale = 0.0f;          /// Size of a tile in world space
    private float m_worldRadius = 0.0f;         /// Radius of a tile in world space for collision detection
    private int m_textureSize = 0;              /// Dimensions of the fog tile texture
    private FogTile m_minimapTile = null;       /// Single tile for the minimap
    private const int m_minimapSize = 128;      /// Dimensions of the minimap tile texture
    private float m_minimapMinReveal = 14.0f;    /// Minimum radius around the player fog is revealed on the minimap
    private float m_minimapMaxReveal = 16.0f;    /// Maximum radius around the player fog is revealed on the minimap
    private Vector2 m_minimapWorldScale;        /// Size of the minimap tile in world space

    /// <summary>
    /// Holds information about a single tile of the fog
    /// </summary>
    class FogTile
    {
        public GameObject Fog = null;
        public SpriteRenderer Renderer = null;
        public Color32[] Pixels = null;
    }

    /// <summary>
    /// Initialises the fog of war
    /// </summary>
    void Start()
    {
        if(generatedFog == null)
        {
            throw new NullReferenceException(
                "Generated fog not set through Unity Inspector");
        }

        if (m_minRevealRadius >= m_maxRevealRadius)
        {
            throw new ArgumentException(
                "Min reveal radius must be smaller than max");
        }

        var renderer = GetComponent<SpriteRenderer>();
        if (renderer.sprite.texture.width != renderer.sprite.texture.height) 
        {
            throw new ArgumentException(
                "Width and height of fog tile should equal");
        }

        m_textureSize = renderer.sprite.texture.width;
        m_worldScale = (float)m_textureSize / renderer.sprite.pixelsPerUnit;
        m_worldScale *= m_tileSize;
        m_worldRadius = Mathf.Sqrt((m_worldScale * m_worldScale) + 
           (m_worldScale * m_worldScale)) * 0.5f;

        if (m_maxRevealRadius >= m_worldScale) 
        {
            throw new ArgumentException(
                "Max reveal radius must be smaller than tile scale");
        }

        var gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        if(gameBoard == null)
        {
            throw new NullReferenceException(
                "Could not find Game Board in scene");
        }

        gameBoard.GetComponent<SpriteRenderer>().enabled = false;
        var boardBounds = gameBoard.GetComponent<SpriteRenderer>().bounds;
        var boardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x);
        var boardLength = Mathf.Abs(boardBounds.max.z - boardBounds.min.z);

        if(useGeneratedFogTiles)
        {
            CreateFog(boardWidth, boardLength);
        }
        CreateMinimapFog(boardWidth, boardLength);
    }

    /// <summary>
    /// Creates the fog tiles placed over the game board
    /// </summary>
    /// <param name="width">The width of the game board to span fog over</param>
    /// <param name="height">The length of the game board to span fog over</param>
    void CreateFog(float width, float length)
    {
        m_borderTiles = new List<FogTile>();
        m_editableTiles = new List<FogTile>();

        const int borderTiles = 2;
        const int extraTiles = borderTiles + 1;
        m_tileAmountX = (int)Mathf.Ceil(width / m_worldScale) + extraTiles;
        m_tileAmountZ = (int)Mathf.Ceil(length / m_worldScale) + extraTiles;

        // Create and place the tiles on the board
        int ID = 0;
        for(int x = 0; x < m_tileAmountX; ++x)
        {
            for(int z = 0; z < m_tileAmountZ; ++z)
            {
                bool isBorderTile = x == 0 || z == 0 ||
                    x == m_tileAmountX-1 || z == m_tileAmountZ-1;

                CreateNewTile(!isBorderTile, x, z, ID.ToString());

                ++ID;
            }
        }

        // Actual removable fog amount
        m_tileAmountX -= borderTiles;
        m_tileAmountZ -= borderTiles;
    }

    /// <summary>
    /// Creates the minimap fog tile with the same dimensions as the game board
    /// </summary>
    /// <param name="width">The width of the game board to span fog over</param>
    /// <param name="length">The height of the game board to span fog over</param>
    void CreateMinimapFog(float width, float length)
    {
        int minimapPixels = m_minimapSize * m_minimapSize;
        Color[] pixels = new Color[minimapPixels];
        for(int i = 0; i < minimapPixels; ++i)
        {
            pixels[i].r = 1.0f;
            pixels[i].g = 1.0f;
            pixels[i].b = 1.0f;
            pixels[i].a = 1.0f;
        }
        
        m_minimapTile = new FogTile();
        InitialiseTile(m_minimapTile, m_minimapSize, "Minimap", pixels);

        var renderer = GetComponent<SpriteRenderer>();
        m_minimapWorldScale = new Vector2(width, length);
        var minimapScale = (float)m_minimapSize / renderer.sprite.pixelsPerUnit;
        var minimapScaleX = m_minimapWorldScale.x / minimapScale;
        var minimapScaleZ = m_minimapWorldScale.y / minimapScale;

        m_minimapTile.Fog.transform.localScale = 
            new Vector3(minimapScaleX, minimapScaleZ, minimapScaleZ);

        m_minimapTile.Fog.transform.localRotation =
            Quaternion.identity;

        m_minimapTile.Renderer.enabled = false;
        m_minimapTile.Fog.tag = "MinimapFog";
    }

    /// <summary>
    /// Creates a new fog tile and adds it to the correct container for editing
    /// </summary>
    /// <param name="isEditable">Whether this tile is editable or not</param>
    /// <param name="x">The x position of the tile in the grid</param>
    /// <param name="z">The z position of the tile in the grid</param>
    /// <param name="name">The name of the tile to create</param>
    void CreateNewTile(bool isEditable, int x, int z, string name)
    {
        List<FogTile> container = isEditable 
            ? m_editableTiles : m_borderTiles;

        int index = container.Count;
        container.Add(new FogTile());

        var renderer = this.GetComponent<SpriteRenderer>();
        var pixels = renderer.sprite.texture.GetPixels();
        InitialiseTile(container[index], m_textureSize, name, pixels);

        var gridScale = m_worldScale * m_tileSpacing;

        container[index].Fog.transform.position = new Vector3(
            (gridScale * ((m_tileAmountX - 1) * 0.5f)) - (x * gridScale),
            transform.position.y,
            (gridScale * ((m_tileAmountZ - 1) * 0.5f)) - (z * gridScale));

        container[index].Fog.transform.localRotation =
            Quaternion.identity;
    }

    /// <summary>
    /// Initialises a new fog tile from the given pixels
    /// </summary>
    /// <param name="tile">The tile to initialise</param>
    /// <param name="size">The dimensions of the texture to create</param>
    /// <param name="name">The name of the tile to create</param>
    /// <param name="pixels">The pixels to fill in the texture with</param>
    void InitialiseTile(FogTile tile, int size, string name, Color[] pixels)
    {
        var renderer = GetComponent<SpriteRenderer>();
        Vector2 pivot = new Vector2 (0.5f, 0.5f);
        var rect = new Rect(0, 0, size, size);

        Texture2D texture = new Texture2D (size, size);
        texture.SetPixels(0, 0, size, size, pixels);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        texture.name = "FogTexture " + name;

        tile.Fog = new GameObject ();
        tile.Fog.name = "Fog" + name;
        tile.Fog.transform.parent = generatedFog.transform;
        tile.Fog.transform.localScale = new Vector3(m_tileSize, m_tileSize, m_tileSize);
        tile.Fog.AddComponent<SpriteRenderer>();

        tile.Renderer = tile.Fog.GetComponent<SpriteRenderer>();
        tile.Renderer.sprite = Sprite.Create(texture, rect, pivot, renderer.sprite.pixelsPerUnit);
        tile.Renderer.sprite.name = "FogSprite" + name;
        tile.Renderer.name = "FogRenderer" + name;
        tile.Renderer.sortingOrder = renderer.sortingOrder;
        tile.Renderer.sortingLayerName = renderer.sortingLayerName;
    }

    /// <summary>
    /// Retrieves an index for the fog from the rows/column of the grid
    /// </summary>
    /// <param name="x">The x coordinate of the tile in the grid</param>
    /// <param name="z">The z coordinate of the tile in the grid</param>
    /// <returns>the index in the editable fog container for a tile</returns>
    int GetIndex(int x, int z)
    {
        return x * m_tileAmountX + z;
    }

    /// <summary>
    /// Determines whether the position is inside the tile at x,y position in the grid
    /// </summary>
    /// <param name="position">The position of the player</param>
    /// <param name="x">The x coordinate of the tile in the grid</param>
    /// <param name="z">The z coordinate of the tile in the grid</param>
    bool IsInsideTile(Vector2 position, int x, int z)
    {
        int index = GetIndex(x, z);
        var tilePosition = m_editableTiles[index].Fog.transform.position;

        float halfScale = m_worldScale * 0.5f;
        return position.x < tilePosition.x + halfScale &&
               position.y < tilePosition.z + halfScale &&
               position.x >= tilePosition.x - halfScale &&
               position.y >= tilePosition.z - halfScale;
    }

    /// <summary>
    /// Change a value from one range to a new range
    /// </summary>
    /// <param name="value">The value to transform</param>
    /// <param name="currentInner">The current inner limit of the value</param>
    /// <param name="currentOuter">The current outer limit of the value</param>
    /// <param name="newInner">The new inner limit of the value</param>
    /// <param name="newOuter">The new outer limit of the value</param>
    /// <returns>The value transformed to its new range</returns>
    float ConvertRange(float value, 
                       float currentInner, 
                       float currentOuter, 
                       float newInner, 
                       float newOuter)
    {
        return ((value - currentInner) * ((newOuter - newInner)
            / (currentOuter - currentInner))) + newInner;
    }

    /// <summary>
    /// Removes transparency of pixels from a tile as the player interacts with it
    /// </summary>
    /// <param name="position">The position of the player</param>
    /// <param name="tile">The tile to remove fog from</param>
    /// <param name="scaleX">The world scale on the x axis of the tile</param>
    /// <param name="scaleZ">The world scale on the z axis of the tile</param>
    /// <param name="dimensions">The dimensions of the tile texture</param>
    /// <param name="minReveal">Minimum radius around the player fog is revealed</param>
    /// <param name="maxReveal">Maximum radius around the player fog is revealed</param>
    void RemoveFog(Vector2 position, 
                   FogTile tile, 
                   float scaleX, 
                   float scaleZ, 
                   int dimensions, 
                   float minReveal, 
                   float maxReveal)
    {
        if (tile.Pixels == null) 
        {
            tile.Pixels = tile.Renderer.sprite.texture.GetPixels32();;
        }
        
        float pixelSizeX = scaleX / (float)dimensions;
        float pixelSizeZ = scaleZ / (float)dimensions;
        float halfSizeX = scaleX / 2.0f;
        float halfSizeZ = scaleZ / 2.0f;

        // Pixels start at bottom left corner and move upwards
        Vector2 pixelPosition = new Vector2();
        Vector2 pixelStart = new Vector2(
            tile.Fog.transform.position.x - halfSizeX, 
            tile.Fog.transform.position.z - halfSizeZ);
        
        for(int r = 0; r < dimensions; ++r)
        {
            for(int c = 0; c < dimensions; ++c)
            {
                pixelPosition.x = pixelStart.x + (c * pixelSizeX);
                pixelPosition.y = pixelStart.y + (r * pixelSizeZ);
                float distance = Vector2.Distance(pixelPosition, position);
                
                if(distance <= maxReveal)
                {
                    byte alpha = (byte)Mathf.Clamp(ConvertRange(distance, 
                        minReveal, maxReveal, 0.0f, 255.0f), 0.0f, 255.0f);
                    
                    var pixelIndex = r * dimensions + c;
                    if(tile.Pixels[pixelIndex].a > alpha)
                    {
                        tile.Pixels[pixelIndex].a = alpha;
                    }
                }
            }
        }
        
        tile.Renderer.sprite.texture.SetPixels32(tile.Pixels);
        tile.Renderer.sprite.texture.Apply();
    }

    /// <summary>
    /// Bounds checks the tile before removing fog from a tile
    /// </summary>
    /// <param name="position">The position of the player</param>
    /// <param name="x">The x coordinate of the fog in the grid</param>
    /// <param name="z">The z coordinate of the fog in the grid</param>
    void RemoveFog(Vector2 position, int x, int z)
    {
        // Only remove fog if an editable tile
        var index = GetIndex(x, z);
        if (index >= 0 && index < m_editableTiles.Count) 
        {
            var tile = m_editableTiles[index];

            Vector2 tilePosition = new Vector2(
                tile.Fog.transform.position.x,
                tile.Fog.transform.position.z);

            // Sphere-sphere Bounds checking
            float distance = Vector2.Distance(tilePosition, position);
            if(distance <= m_worldRadius + m_maxRevealRadius)
            {
                RemoveFog(position, tile, m_worldScale, m_worldScale, 
                    m_textureSize, m_minRevealRadius, m_maxRevealRadius);
            }
        }
    }

    /// <summary>
    /// Finds which tile the player is inside
    /// </summary>
    /// <param name="position">The position of the player</param>
    /// <returns>Whether player is inside a tile</returns>
    bool IsInsideTile(Vector2 position)
    {
        // Determine which tile the player is currently inside
        if (m_tileInsideX == -1 || m_tileInsideZ == -1 ||
            !IsInsideTile(position, m_tileInsideX, m_tileInsideZ))
        {
            m_tileInsideX = -1;
            m_tileInsideZ = -1;
            
            for(int x = 0; x < m_tileAmountX; ++x)
            {
                for(int z = 0; z < m_tileAmountZ; ++z)
                {
                    if(IsInsideTile(position, x, z))
                    {
                        m_tileInsideX = x;
                        m_tileInsideZ = z;
                        return true;
                    }
                }
            }
        }

        return m_tileInsideX != -1 && 
            m_tileInsideZ != -1;
    }

    /// <summary>
    /// Hides the fog of war
    /// </summary>
    public void HideFog()
    {
        if(useGeneratedFogTiles)
        {
            Debug.LogError("Currently not supported");
        }
        else
        {
            var particleFogOfWar = FindObjectOfType<ParticleFogOfWarBuilder>();
            if(particleFogOfWar == null)
            {
                Debug.LogError("Could not find Fog of war");
            }
            particleFogOfWar.HideFog();
            GameObject.Destroy(m_minimapTile.Fog);
        }
    }

    /// <summary>
    /// Solves the fog of war for a smooth fog effect
    /// </summary>
    void Update()
    {
        // Only solve for fog when a player is active
        var player = PlayerManager.GetControllablePlayer();
        if(player != null && Utilities.IsPlayerInitialised(player))
        {
            Vector2 position = new Vector2(
                player.transform.position.x, 
                player.transform.position.z);

            if(useGeneratedFogTiles && IsInsideTile(position))
            {
                // Assumes tile scale is less than max reveal 
                // radius from initialisation check. Because of 
                // this only remove fog from tile inside and 
                // all 8 surrounding tiles

                if(useGeneratedFogTiles)
                {
                    RemoveFog(position, m_tileInsideX, m_tileInsideZ);
                    RemoveFog(position, m_tileInsideX, m_tileInsideZ + 1);
                    RemoveFog(position, m_tileInsideX, m_tileInsideZ - 1);
                    RemoveFog(position, m_tileInsideX + 1, m_tileInsideZ);
                    RemoveFog(position, m_tileInsideX - 1, m_tileInsideZ);
                    RemoveFog(position, m_tileInsideX + 1, m_tileInsideZ + 1);
                    RemoveFog(position, m_tileInsideX - 1, m_tileInsideZ - 1);
                    RemoveFog(position, m_tileInsideX + 1, m_tileInsideZ - 1);
                    RemoveFog(position, m_tileInsideX - 1, m_tileInsideZ + 1);
                }
            }

            if(m_minimapTile.Fog != null)
            {
                RemoveFog(position, 
                          m_minimapTile, 
                          m_minimapWorldScale.x,
                          m_minimapWorldScale.y,
                          m_minimapSize, 
                          m_minimapMinReveal,
                          m_minimapMaxReveal);
            }
        }
    }
}
