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
/// </summary>
public class FogOfWar : MonoBehaviour 
{
    public Color m_tintColor;                                    /// Colour to tint the fog billboards 
    private Mesh m_fogTileMesh = null;                           /// Fog billboard mesh
    private Material m_fogTileMaterial = null;                   /// Fog billboard shader material
    private List<FogOfWarTile> m_activeTiles = null;             /// Tiles that can be interacted with
    private List<FogOfWarTile> m_staticTiles = null;             /// Tiles around the game board border
    private List<List<FogOfWarTile>> m_partitionedTiles = null;  /// Tiles in each partition of the board
    private List<Vector2> m_partitions = null;                   /// Four partitions of the game board
    private float m_partitionSize = 0.0f;                        /// Width/height of the partitions
    private int m_partitionIn = 0;                               /// Index for which partition the player is in
    private int m_updateIndex = 0;                               /// Current tile index doing collision checks
    private int m_updateIterations = 250;                        /// Number of tiles per tick to do collision checks
    private float m_radius = 10.0f;                              /// Collision radius of a single fog tile

    private Color32[] m_minimapPixels = null;                    /// Pixels for the minimap 
    private GameObject m_minimapTile = null;                     /// Single tile for the minimap
    private SpriteRenderer m_minimapRenderer = null;             /// Single tile for the minimap
    private const int m_minimapSize = 128;                       /// Dimensions of the minimap tile texture
    private float m_minimapMinReveal = 14.0f;                    /// Minimum radius around the player fog is revealed
    private float m_minimapMaxReveal = 16.0f;                    /// Maximum radius around the player fog is revealed
    private Vector2 m_minimapWorldScale;                         /// Size of the minimap tile in world space

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
       
        var boardBounds = GameBoard.GetBounds();
        var boardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x);
        var boardLength = Mathf.Abs(boardBounds.max.z - boardBounds.min.z);

        CreateTemplateMesh();
        CreateMinimapFog(boardWidth, boardLength);
        CreatePartitions(boardWidth, boardLength);
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
        float size = 3.0f;
        int amountX = Mathf.CeilToInt(width / size);
        int amountZ = Mathf.CeilToInt(length / size);
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
                        Debug.LogError(
                            "Could not find partition for fog tile " +
                            x.ToString() + ", " + z.ToString());
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Creates the four partitions of the game board
    /// <param name="width">The width of the game board</param>
    /// <param name="length">The height of the game board</param>
    /// </summary>
    void CreatePartitions(float width, float length)
    {
        m_partitionSize = Mathf.Max(width, length) / 2.0f;

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
    /// Determines if the given position is within the partition bounds
    /// <param name="position">The position to check</param>
    /// <param name="partition">ID of the partition to check whether inside</param>
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
    /// <param name="width">The width of the game board</param>
    /// <param name="length">The height of the game board</param>
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
    /// <param name="player">The human controllable player</param>
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
    /// <param name="tile">The fog tile to render</param>
    /// </summary>
    void RenderTile(FogOfWarTile tile)
    {
        if(!tile.active)
        {
            return;
        }

        if(!tile.isStatic && tile.fade)
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

        Graphics.DrawMesh(m_fogTileMesh, 
                          tile.position, 
                          Camera.main.transform.rotation,
                          m_fogTileMaterial, 0, null, 0,
                          tile.material, 
                          false, false);
    }

    /// <summary>
    /// Checks a fog tile for interaction with the player
    /// <param name="tile">The fog tile to check</param>
    /// </summary>
    void CheckCollision(FogOfWarTile tile)
    {
        if(tile.active && !tile.fade)
        {
            tile.fade = PlayerManager.IsCloseToPlayer(
                tile.position.x, tile.position.z, m_radius);
        }
    }

    /// <summary>
    /// Updates the fog of war
    /// </summary>
    void Update()
    {
        var player = PlayerManager.GetControllablePlayer();
        if(player != null && Utilities.IsPlayerInitialised(player))
        {
            FindPlayerPartition(player);
            UpdateMinimap(player);
            CheckFogCollisions();
        }

        RenderFog();
        RenderDiagnostics();
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
    /// Checks for collisions between the player and fog of war
    /// </summary>
    void CheckFogCollisions()
    {
        for(int i = 0; i < m_updateIterations; ++i)
        {
            m_updateIndex = m_updateIndex >= 
                m_partitionedTiles[m_partitionIn].Count-1 ? 0 : m_updateIndex + 1;
            
            CheckCollision(m_partitionedTiles[m_partitionIn][m_updateIndex]);
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
            Diagnostics.Add("Partition In", m_partitionIn);
            
            for(int i = 0; i < (int)Partition.MAX; ++i)
            {
                Diagnostics.Add(((Partition)i).ToString(), m_partitionedTiles[i].Count);
            }
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
                    pixelPosition.x = pixelStart.x + (c * pixelSizeX);
                    pixelPosition.y = pixelStart.y + (r * pixelSizeZ);
                    float distance = Vector2.Distance(pixelPosition, position);
                    
                    if(distance <= m_minimapMaxReveal)
                    {
                        float value = ((distance - m_minimapMinReveal) * 255.0f) 
                            / (m_minimapMaxReveal - m_minimapMinReveal);
                        byte alpha = (byte)Mathf.Clamp(value, 0.0f, 255.0f);
                        
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
