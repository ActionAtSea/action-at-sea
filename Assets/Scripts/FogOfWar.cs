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
    /// <summary>
    /// Information for rendering an individual tile
    /// </summary>
    public class FogOfWarTile
    {
        public MaterialPropertyBlock material;
        public Vector3 position;
        public Color tintColor;
        public float alpha = 0.0f;
        public bool fade = false;
        public bool active = true;
        public bool isStatic = false;
    }

    public Color m_tintColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private Mesh m_fogTileMesh = null;
    private Material m_fogTileMaterial = null;
    private List<FogOfWarTile> m_activeTiles = null;
    private List<FogOfWarTile> m_staticTiles = null;
    private List<List<FogOfWarTile>> m_partitionedTiles = null;
    private List<Vector2> m_partitions = null;
    private float m_partitionSize = 0.0f;
    private int m_partitionIn = 0;
    private int m_updateIndex = 0;
    private int m_updateIterations = 250;
    private float m_radius = 10.0f;

    private Color32[] m_minimapPixels = null;        /// Pixels for the minimap 
    private GameObject m_minimapTile = null;         /// Single tile for the minimap
    private SpriteRenderer m_minimapRenderer = null; /// Single tile for the minimap
    private const int m_minimapSize = 128;           /// Dimensions of the minimap tile texture
    private float m_minimapMinReveal = 14.0f;        /// Minimum radius around the player fog is revealed on the minimap
    private float m_minimapMaxReveal = 16.0f;        /// Maximum radius around the player fog is revealed on the minimap
    private Vector2 m_minimapWorldScale;             /// Size of the minimap tile in world space

    /// <summary>
    /// Four partitions of the game board
    /// </summary>
    enum Partition
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOT_LEFT,
        BOT_RIGHT,
        MAX
    }

    /// <summary>
    /// Initialises the fog of war
    /// </summary>
    void Start()
    {
        m_staticTiles = new List<FogOfWarTile>();
        m_activeTiles = new List<FogOfWarTile>();
        m_partitions = new List<Vector2>();
        m_partitionedTiles = new List<List<FogOfWarTile>>();

        var fogTemplate = transform.GetChild(0).gameObject;
        CreateTemplateMesh(fogTemplate);
       
        var gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        if(gameBoard == null)
        {
            throw new NullReferenceException(
                "Could not find Game Board in scene");
        }

        var boardBounds = gameBoard.GetComponent<SpriteRenderer>().bounds;
        var boardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x);
        var boardLength = Mathf.Abs(boardBounds.max.z - boardBounds.min.z);
        m_partitionSize = Mathf.Max(boardWidth, boardLength) / 2.0f;

        CreateMinimapFog(boardWidth, boardLength);

        for (int i = 0; i < (int)Partition.MAX; ++i)
        {
            float partitionX = 0.0f;
            float partitionZ = 0.0f;
            switch(i)
            {
            case (int)Partition.TOP_LEFT:
                partitionX = -m_partitionSize;
                partitionZ = -m_partitionSize;
                break;
            case (int)Partition.TOP_RIGHT:
                partitionZ = -m_partitionSize;
                break;
            case (int)Partition.BOT_LEFT:
                partitionX = -m_partitionSize;
                break;
            }

            m_partitionedTiles.Add(new List<FogOfWarTile>());
            m_partitions.Add(new Vector2(partitionX, partitionZ));
        }

        
        const int border = 4;
        const int borderOverlap = 2;
        float size = 3.0f;
        int amountX = Mathf.CeilToInt(boardWidth / size);
        int amountZ = Mathf.CeilToInt(boardLength / size);
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
                    (size * ((amountX - 1) * 0.5f)) - (x * size) + randX,
                    UnityEngine.Random.Range(5.0f, 15.0f),
                    (size * ((amountZ - 1) * 0.5f)) - (z * size) + randZ);

                // Border tiles cannot be interacted with
                if(x < border+borderOverlap || 
                   z < border+borderOverlap ||
                   x >= amountX-border-borderOverlap || 
                   z >= amountZ-border-borderOverlap)
                {
                    tile.isStatic = true;
                    m_staticTiles.Add(tile);
                }
                else
                {
                    m_activeTiles.Add(tile);

                    // Determine which partitions the tile should be in
                    bool foundPartition = false;
                    for(int i = 0; i < (int)Partition.MAX; ++i)
                    {
                        if(IsInPartition(tile.position, i))
                        {
                            m_partitionedTiles[i].Add(tile);
                            foundPartition = true;
                        }
                    }
                    
                    if(!foundPartition)
                    {
                        Debug.LogError("Could not find partition for fog tile " +
                            x.ToString() + ", " + z.ToString());
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates the template mesh
    /// </summary>
    void CreateTemplateMesh(GameObject template)
    {
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
    /// Determines if the given position is within the partition bounds
    /// </summary>
    bool IsInPartition(Vector3 position, int partition)
    {
        Vector2 position2D = new Vector2(position.x, position.z);

        Vector2 partitionCenter = new Vector2(
            m_partitions[partition].x + (m_partitionSize / 2.0f),
            m_partitions[partition].y + (m_partitionSize / 2.0f));

        Vector2 tileToPartition = partitionCenter - position2D;
        float distance = Vector2.Distance(partitionCenter, position2D);

        if(distance > m_radius)
        {
            tileToPartition /= distance;
            tileToPartition *= m_radius;

            Vector2 closestPoint = position2D + tileToPartition;
            return closestPoint.x <= m_partitions[partition].x + m_partitionSize &&
                   closestPoint.x >= m_partitions[partition].x &&
                   closestPoint.y <= m_partitions[partition].y + m_partitionSize &&
                   closestPoint.y >= m_partitions[partition].y;
        }
        return true;
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
        m_activeTiles.Clear();
        m_partitionedTiles.Clear();
    }

    /// <summary>
    /// Determines which partition the player is currently in
    /// </summary>
    void FindPlayerPartition(GameObject player)
    {
        var x = player.transform.position.x;
        var z = player.transform.position.z;

        if(x < m_partitions[(int)Partition.TOP_RIGHT].x)
        {
            if(z < m_partitions[(int)Partition.BOT_RIGHT].y)
            {
                m_partitionIn = (int)Partition.TOP_LEFT;
            }
            else
            {
                m_partitionIn = (int)Partition.BOT_LEFT;
            }
        }
        else
        {
            if(z < m_partitions[(int)Partition.BOT_RIGHT].y)
            {
                m_partitionIn = (int)Partition.TOP_RIGHT;
            }
            else
            {
                m_partitionIn = (int)Partition.BOT_RIGHT;
            }
        }
    }

    /// <summary>
    /// Renders a fog tile
    /// </summary>
    void RenderTile(FogOfWarTile tile)
    {
        if(tile.active)
        {
            if(!tile.isStatic)
            {
                if(tile.fade)
                {
                    if(tile.alpha <= 0.0f)
                    {
                        tile.active = false;
                    }
                    else
                    {
                        tile.alpha -= Time.deltaTime * 1.0f;
                        tile.material.SetFloat("_Alpha", tile.alpha);
                    }
                }
            }

            Graphics.DrawMesh(m_fogTileMesh, 
                              tile.position, 
                              Camera.main.transform.rotation,
                              m_fogTileMaterial, 0, null, 0,
                              tile.material, 
                              false, false);
        }
    }

    /// <summary>
    /// Checks a fog tile for interaction with the player
    /// </summary>
    void CheckForCollision(FogOfWarTile tile)
    {
        if(tile.active && !tile.fade)
        {
            tile.fade = PlayerManager.IsCloseToPlayer(
                tile.position.x, tile.position.z, m_radius);
        }
    }

    /// <summary>
    /// Renders the fog of war
    /// </summary>
    void RenderFog()
    {
        for(int i = 0; i < m_activeTiles.Count; ++i)
        {
            RenderTile(m_activeTiles[i]);
        }
        for(int i = 0; i < m_staticTiles.Count; ++i)
        {
            RenderTile(m_staticTiles[i]);
        }
    }

    /// <summary>
    /// Solves the fog of war
    /// </summary>
    void Update()
    {
        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("Fog Tiles Static", m_staticTiles.Count);
            Diagnostics.Add("Fog Tiles Active", m_activeTiles.Count);
            Diagnostics.Add("Partition In", m_partitionIn);

            for(int i = 0; i < (int)Partition.MAX; ++i)
            {
                Diagnostics.Add(((Partition)i).ToString(), m_partitionedTiles[i].Count);
            }
        }

        var player = PlayerManager.GetControllablePlayer();
        if(player != null && Utilities.IsPlayerInitialised(player))
        {
            FindPlayerPartition(player);
            UpdateMinimap(player);

            for(int i = 0; i < m_updateIterations; ++i)
            {
                m_updateIndex = m_updateIndex >= 
                    m_partitionedTiles[m_partitionIn].Count-1 ? 0 : m_updateIndex + 1;

                CheckForCollision(m_partitionedTiles[m_partitionIn][m_updateIndex]);
            }
        }

        RenderFog();
    }

    /// <summary>
    /// Solves the minimap fog of war
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
