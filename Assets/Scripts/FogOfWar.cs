////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FogOfWar.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a series of billboard tiles that the player can interact with and remove
/// Fog tile alpha is determined by the alpha value on the minimap
/// </summary>
public class FogOfWar : MonoBehaviour 
{
    public Vector2 m_fogSpeed = new Vector2(1.5f, 0.5f);
    private float m_fogRevealSpeed = 2.0f;
    private float m_fogSize = 3.5f;
    private Mesh m_fogTileMesh = null;                      /// Fog billboard mesh
    private Material m_fogTileMaterial = null;              /// Fog billboard shader material
    private List<FogOfWarTile> m_activeTiles = null;        /// Tiles that can be interacted with
    private List<FogOfWarTile> m_staticTiles = null;        /// Tiles around the game board border
    private float m_partitionSize = 0.0f;                   /// Width/height of the partitions
    private float m_halfBoardSize = 0.0f;
    private float[] m_fogAlpha = null;                      /// Pixels for the minimap 
    private Color32[] m_minimapPixels = null;               /// Pixels for the minimap 
    private GameObject m_minimapTile = null;                /// Single tile for the minimap
    private SpriteRenderer m_minimapRenderer = null;        /// Single tile for the minimap
    private const int m_minimapSize = 128;                  /// Dimensions of the minimap tile texture
    private float m_fogMinReveal = 8.0f;
    private float m_minimapMinReveal = 14.0f;               /// Minimum radius around the player fog is revealed
    private float m_minimapMaxReveal = 16.0f;               /// Maximum radius around the player fog is revealed
    private Vector2 m_minimapWorldScale;                    /// Size of the minimap tile in world space

    /// <summary>
    /// Information for rendering an individual tile
    /// </summary>
    public class FogOfWarTile
    {
        public MaterialPropertyBlock material;
        public Vector3 position;
        public float alpha = 0.0f;
    }

    /// <summary>
    /// Initialises the fog of war
    /// </summary>
    void Start()
    {
        m_staticTiles = new List<FogOfWarTile>();
        m_activeTiles = new List<FogOfWarTile>();
       
        var boardBounds = GameBoard.GetBounds();
        var boardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x);
        var boardLength = Mathf.Abs(boardBounds.max.z - boardBounds.min.z);

        float boardSize = Mathf.Max(boardWidth, boardLength);
        m_partitionSize = boardSize / (float)m_minimapSize;
        m_halfBoardSize = (boardSize - (m_partitionSize * 2.0f)) / 2.0f;

        CreateTemplateMesh();
        CreateMinimapFog(boardWidth, boardLength);
        CreateFogTiles(boardWidth, boardLength);
    }

    /// <summary>
    /// Creates the fog tile billboards
    /// <param name="width">The width of the game board</param>
    /// <param name="length">The height of the game board</param>
    /// </summary>
    void CreateFogTiles(float width, float length)
    {
        const int border = 4;
        const int borderOverlap = 2;
        int amountX = Mathf.CeilToInt(width / m_fogSize);
        int amountZ = Mathf.CeilToInt(length / m_fogSize);
        amountX += border * 2;
        amountZ += border * 2;
        
        for(int x = 0; x < amountX; ++x)
        {
            for(int z = 0; z < amountZ; ++z)
            {
                FogOfWarTile tile = new FogOfWarTile();
                tile.material = new MaterialPropertyBlock();
                
                tile.alpha = 1.0f;
                tile.material.SetFloat("_Alpha", tile.alpha);
                
                float randX = UnityEngine.Random.Range(-1.0f, 1.0f);
                float randZ = UnityEngine.Random.Range(-1.0f, 1.0f);
                
                tile.position = new Vector3(
                    (m_fogSize * ((amountX - 1) * 0.5f)) - (x * m_fogSize) + randX,
                    UnityEngine.Random.Range(5.0f, 15.0f),
                    (m_fogSize * ((amountZ - 1) * 0.5f)) - (z * m_fogSize) + randZ);
                
                // Border tiles cannot be interacted with
                if(x < border+borderOverlap || 
                   z < border+borderOverlap ||
                   x >= amountX-border-borderOverlap || 
                   z >= amountZ-border-borderOverlap)
                {
                    m_staticTiles.Add(tile);

                    FogOfWarTile activeTile = new FogOfWarTile();
                    activeTile.material = new MaterialPropertyBlock();
                    activeTile.alpha = 1.0f;
                    activeTile.material.SetFloat("_Alpha", tile.alpha);
                    activeTile.position = tile.position;
                    m_activeTiles.Add(activeTile);
                }
                else
                {
                    m_activeTiles.Add(tile);
                }
            }
        }
    }

    /// <summary>
    /// Creates the template mesh for the billboards
    /// </summary>
    void CreateTemplateMesh()
    {
        var template = transform.GetChild(0).gameObject;
        Mesh templateMesh = template.GetComponent<MeshFilter>().sharedMesh;
        m_fogTileMaterial = template.GetComponent<MeshRenderer>().material;

        int vertexCount = templateMesh.vertices.Length;
        Vector3[] vertices = new Vector3[vertexCount];
        for(int i = 0; i < vertexCount; ++i)
        {
            vertices[i].Set(
                templateMesh.vertices[i].x * template.transform.localScale.x,
                templateMesh.vertices[i].y * template.transform.localScale.y,
                templateMesh.vertices[i].z * template.transform.localScale.z);
        }

        int triangleCount = templateMesh.triangles.Length;
        int[] triangles = new int[triangleCount];
        for(int i = 0; i < triangleCount; ++i)
        {
            triangles[i] = templateMesh.triangles[i];
        }

        int uvCount = templateMesh.uv.Length;
        Vector2[] uvs = new Vector2[uvCount];
        for(int i = 0; i < uvCount; ++i)
        {
            uvs[i] = templateMesh.uv[i];
        }

        m_fogTileMesh = new Mesh();
        m_fogTileMesh.vertices = vertices;
        m_fogTileMesh.triangles = triangles;
        m_fogTileMesh.uv = uvs;
    }

    /// <summary>
    /// Creates the minimap fog tile with the same dimensions as the game board
    /// </summary>
    /// <param name="width">The width of the game board</param>
    /// <param name="length">The height of the game board</param>
    void CreateMinimapFog(float width, float length)
    {
        int pixelAmount = m_minimapSize * m_minimapSize;
        var pixels = new Color[pixelAmount];
        m_minimapPixels = new Color32[pixelAmount];
        m_fogAlpha = new float[pixelAmount];

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
            m_fogAlpha[i] = 1.0f;
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
        m_minimapTile.transform.parent = transform;

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
    /// Hides the fog of war
    /// </summary>
    public void HideFog()
    {
        GameObject.Destroy(m_minimapTile);
        m_activeTiles.Clear();
    }

    /// <summary>
    /// Renders a fog tile
    /// <param name="tile">The fog tile to render</param>
    /// </summary>
    void RenderTile(FogOfWarTile tile)
    {
        Graphics.DrawMesh(m_fogTileMesh, 
                          tile.position, 
                          Camera.main.transform.rotation,
                          m_fogTileMaterial, 0, null, 0,
                          tile.material, 
                          false, false);
    }

    /// <summary>
    /// Updates the fog of war
    /// </summary>
    void Update()
    {
        bool updatedMinimap = false;

        var player = PlayerManager.GetControllablePlayer();
        if (player != null && Utilities.IsPlayerInitialised(player))
        {
            updatedMinimap = true;
            UpdateMinimap(player);
        }

        var ai = PlayerManager.GetControllableAI();
        if (ai != null && Utilities.IsPlayerInitialised(ai))
        {
            updatedMinimap = true;
            UpdateMinimap(ai);
        }

        if (updatedMinimap)
        {
            m_minimapRenderer.sprite.texture.SetPixels32(m_minimapPixels);
            m_minimapRenderer.sprite.texture.Apply();
        }

        UpdateFog();
        RenderDiagnostics();
    }

    /// <summary>
    /// Updates and renders the fog of war
    /// </summary>
    void UpdateFog()
    {
        for(int i = 0; i < m_activeTiles.Count; ++i)
        {
            var tile = m_activeTiles[i];

            // Move the tile across the board
            bool hasReset = false;
            tile.position.x += Time.deltaTime * m_fogSpeed.x;
            tile.position.z += Time.deltaTime * m_fogSpeed.y;

            if(tile.position.x >= m_halfBoardSize - m_fogSize)
            {
                hasReset = true;
                tile.position.x = -m_halfBoardSize;
            }
            if(tile.position.z >= m_halfBoardSize - m_fogSize)
            {
                hasReset = true;
                tile.position.z = -m_halfBoardSize;
            }

            // Determine what minimap pixel the fog tile is currently mapping to
            int row = (int)((tile.position.x + m_halfBoardSize) / m_partitionSize);
            int column = (int)((tile.position.z + m_halfBoardSize) / m_partitionSize);
            row = Mathf.Max(0, Mathf.Min(m_minimapSize-1, row));
            column = Mathf.Max(0, Mathf.Min(m_minimapSize-1, column));
            float alpha = m_fogAlpha[row + (m_minimapSize * column)];

            tile.alpha = Mathf.Lerp(tile.alpha, alpha, 
                Time.deltaTime * m_fogRevealSpeed * (hasReset ? 10.0f : 1.0f));

            tile.material.SetFloat("_Alpha", tile.alpha);

            RenderTile(tile);
        }
        
        for(int i = 0; i < m_staticTiles.Count; ++i)
        {
            RenderTile(m_staticTiles[i]);
        }
    }

    /// <summary>
    /// Renders diagnostics for the fog of war
    /// </summary>
    void RenderDiagnostics()
    {
        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("Fog Tiles Static", m_staticTiles.Count);
            Diagnostics.Add("Fog Tiles Active", m_activeTiles.Count);
        }
    }

    /// <summary>
    /// Solves the minimap fog of war
    /// <param name="player">The human controllable player</param>
    /// </summary>
    void UpdateMinimap(GameObject player)
    {
        if(m_minimapTile != null)
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
                    var pixelIndex = r * m_minimapSize + c;

                    pixelPosition.x = pixelStart.x + (c * pixelSizeX);
                    pixelPosition.y = pixelStart.y + (r * pixelSizeZ);
                    float distance = Vector2.Distance(pixelPosition, position);
                    
                    if(distance <= m_minimapMaxReveal)
                    {
                        byte pixelValue = (byte)Mathf.Clamp(((distance - m_minimapMinReveal) * 255.0f) 
                            / (m_minimapMaxReveal - m_minimapMinReveal), 0.0f, 255.0f);

                        if(m_minimapPixels[pixelIndex].a > pixelValue)
                        {
                            m_minimapPixels[pixelIndex].a = pixelValue;
                        }

                        float fogValue = Mathf.Clamp((distance - m_fogMinReveal) 
                            / (m_minimapMaxReveal - m_fogMinReveal), 0.0f, 1.0f);

                        if(m_fogAlpha[pixelIndex] > fogValue)
                        {
                            m_fogAlpha[pixelIndex] = fogValue;
                        }
                    }
                }
            }
        }
    }
}
