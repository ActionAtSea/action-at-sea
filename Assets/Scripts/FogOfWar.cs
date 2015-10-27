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
    public GameObject generatedFog = null;           /// Holds all generated fog in scene, set through inspector
    public GameObject fogTileTemplate = null;

    private List<GameObject> m_fogTiles = null;      /// All generated fog tiles
    private Color32[] m_minimapPixels = null;        /// Pixels for the minimap 
    private GameObject m_minimapTile = null;         /// Single tile for the minimap
    private SpriteRenderer m_minimapRenderer = null; /// Single tile for the minimap
    private const int m_minimapSize = 128;           /// Dimensions of the minimap tile texture
    private float m_minimapMinReveal = 14.0f;        /// Minimum radius around the player fog is revealed on the minimap
    private float m_minimapMaxReveal = 16.0f;        /// Maximum radius around the player fog is revealed on the minimap
    private Vector2 m_minimapWorldScale;             /// Size of the minimap tile in world space

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

        if(fogTileTemplate == null)
        {
            throw new NullReferenceException(
                "Fog tile not set through Unity Inspector");
        }
       
        var gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        if(gameBoard == null)
        {
            throw new NullReferenceException(
                "Could not find Game Board in scene");
        }

        m_fogTiles = new List<GameObject>();

        gameBoard.GetComponent<SpriteRenderer>().enabled = false;
        var boardBounds = gameBoard.GetComponent<SpriteRenderer>().bounds;
        var boardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x);
        var boardLength = Mathf.Abs(boardBounds.max.z - boardBounds.min.z);

        const int border = 2;
        float size = 2.0f;
        float scale = fogTileTemplate.transform.localScale.x;
        float height = fogTileTemplate.transform.position.y;
        int amountX = Mathf.CeilToInt(boardWidth / size);
        int amountZ = Mathf.CeilToInt(boardLength / size);
        amountX += border * 2;
        amountZ += border * 2;

        for(int x = 0; x < amountX; ++x)
        {
            for(int z = 0; z < amountZ; ++z)
            {
                var fogTile = Instantiate(fogTileTemplate);
                fogTile.transform.parent = generatedFog.transform;
                m_fogTiles.Add(fogTile);
                
                float randX = UnityEngine.Random.Range(-1.0f, 1.0f);
                float randY = UnityEngine.Random.Range(-2.0f, 2.0f);
                float randZ = UnityEngine.Random.Range(-1.0f, 1.0f);
                float randScale = scale * UnityEngine.Random.Range(0.75f, 1.25f);

                fogTile.transform.localScale = new Vector3(randScale, randScale, randScale);

                fogTile.transform.position = new Vector3(
                    (size * ((amountX - 1) * 0.5f)) - (x * size) + randX,
                    height + randY,
                    (size * ((amountZ - 1) * 0.5f)) - (z * size) + randZ);
               
                // Border tiles cannot be interacted with
                if(x < border || z < border || x >= amountX-border || z >= amountZ-border)
                {
                    fogTile.GetComponent<FogOfWarTile>().IsStatic = true;
                }
            }
        }

        fogTileTemplate.SetActive(false);

        CreateMinimapFog(boardWidth, boardLength);
    }

    /// <summary>
    /// Creates the minimap fog tile with the same dimensions as the game board
    /// </summary>
    /// <param name="width">The width of the game board to span fog over</param>
    /// <param name="length">The height of the game board to span fog over</param>
    void CreateMinimapFog(float width, float length)
    {
        int pixelAmount = m_minimapSize * m_minimapSize;
        var pixels = new Color[pixelAmount];
        m_minimapPixels = new Color32[pixelAmount];

        for(int i = 0; i < pixelAmount; ++i)
        {
            pixels[i].r = 1.0f;
            pixels[i].g = 1.0f;
            pixels[i].b = 1.0f;
            pixels[i].a = 1.0f;
            m_minimapPixels[i].r = 255;
            m_minimapPixels[i].g = 255;
            m_minimapPixels[i].b = 255;
            m_minimapPixels[i].a = 255;
        }
        
        Texture2D texture = new Texture2D (m_minimapSize, m_minimapSize);
        texture.SetPixels(0, 0, m_minimapSize, m_minimapSize, pixels);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        texture.name = "MinimapTexture";
        
        m_minimapTile = new GameObject ();
        m_minimapTile.name = "Fog" + name;
        m_minimapTile.tag = "MinimapFog";
        m_minimapTile.transform.parent = generatedFog.transform;
        //m_minimapTile.transform.localScale = new Vector3(m_tileSize, m_tileSize, m_tileSize);

        Vector2 pivot = new Vector2 (0.5f, 0.5f);
        var rect = new Rect(0, 0, m_minimapSize, m_minimapSize);
        var pixelsPerUnit = 100.0f;

        m_minimapRenderer = m_minimapTile.AddComponent<SpriteRenderer>();
        m_minimapRenderer.sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit);
        m_minimapRenderer.sprite.name = "MinimapSprite";
        m_minimapRenderer.name = "MinimapRenderer";
        m_minimapRenderer.sortingOrder = 10;
        m_minimapRenderer.sortingLayerName = "World";
        m_minimapRenderer.enabled = false;

        m_minimapWorldScale = new Vector2(width, length);
        var minimapScale = (float)m_minimapSize / pixelsPerUnit;
        var minimapScaleX = m_minimapWorldScale.x / minimapScale;
        var minimapScaleZ = m_minimapWorldScale.y / minimapScale;

        m_minimapTile.transform.localScale = 
            new Vector3(minimapScaleX, minimapScaleZ, minimapScaleZ);

        m_minimapTile.transform.localRotation =
            Quaternion.identity;
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
    /// Hides the fog of war
    /// </summary>
    public void HideFog()
    {
        GameObject.Destroy(m_minimapTile);
        for(int i = 0; i < m_fogTiles.Count; ++i)
        {
            m_fogTiles[i].SetActive(false);
        }
    }

    /// <summary>
    /// Solves the minimap fog of war
    /// </summary>
    void Update()
    {
        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("Fog Tiles", m_fogTiles.Count);
        }

        if(m_minimapTile != null)
        {
            var player = PlayerManager.GetControllablePlayer();
            if(player != null && Utilities.IsPlayerInitialised(player))
            {
                Vector2 position = new Vector2(
                    player.transform.position.x, 
                    player.transform.position.z);
               
                float pixelSizeX = m_minimapWorldScale.x / (float)m_minimapSize;
                float pixelSizeZ = m_minimapWorldScale.y / (float)m_minimapSize;
                float halfSizeX = m_minimapWorldScale.x / 2.0f;
                float halfSizeZ = m_minimapWorldScale.y / 2.0f;
                
                // Pixels start at bottom left corner and move upwards
                Vector2 pixelPosition = new Vector2();
                Vector2 pixelStart = new Vector2(
                    m_minimapTile.transform.position.x - halfSizeX, 
                    m_minimapTile.transform.position.z - halfSizeZ);
                
                for(int r = 0; r < m_minimapSize; ++r)
                {
                    for(int c = 0; c < m_minimapSize; ++c)
                    {
                        pixelPosition.x = pixelStart.x + (c * pixelSizeX);
                        pixelPosition.y = pixelStart.y + (r * pixelSizeZ);
                        float distance = Vector2.Distance(pixelPosition, position);
                        
                        if(distance <= m_minimapMaxReveal)
                        {
                            byte alpha = (byte)Mathf.Clamp(ConvertRange(distance, 
                                m_minimapMinReveal, m_minimapMaxReveal, 0.0f, 255.0f), 0.0f, 255.0f);
                            
                            var pixelIndex = r * m_minimapSize + c;
                            if(m_minimapPixels[pixelIndex].a > alpha)
                            {
                                m_minimapPixels[pixelIndex].a = alpha;
                            }
                        }
                    }
                }
                
                m_minimapRenderer.sprite.texture.SetPixels32(m_minimapPixels);
                m_minimapRenderer.sprite.texture.Apply();
            }
        }
    }
}
