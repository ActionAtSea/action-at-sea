////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Minimap.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class MapItem
{
    public GameObject item;
    public SpriteRenderer renderer;
    public Transform parentTransform;
    public SpriteRenderer parentRenderer;
    public bool updatesColor = false;
};

public class Minimap : MonoBehaviour 
{
    public GameObject marker = null;

    private float m_shipMarkerSize = 1.5f;
    private bool m_isInitialised = false;
    private List<MapItem> m_world = new List<MapItem>();
    private List<MapItem> m_markers = new List<MapItem>();
    private GameObject m_gameBoard = null;
    private MapItem m_fog = null;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        if(m_gameBoard == null)
        {
            Debug.LogError("Could not find game board");
        }

        var boardRenderer = m_gameBoard.GetComponent<SpriteRenderer>();

        var boardWidth = ((float)boardRenderer.sprite.texture.width / 
            boardRenderer.sprite.pixelsPerUnit) * m_gameBoard.transform.localScale.x;

        var boardLength = ((float)boardRenderer.sprite.texture.height /
            boardRenderer.sprite.pixelsPerUnit) * m_gameBoard.transform.localScale.y;

        var mapRenderer = GetComponent<SpriteRenderer>();

        var mapWidth = ((float)mapRenderer.sprite.texture.width / 
            mapRenderer.sprite.pixelsPerUnit) * transform.localScale.x;
        
        var mapLength = ((float)mapRenderer.sprite.texture.height /
            mapRenderer.sprite.pixelsPerUnit) * transform.localScale.y;

        var scaleX = mapWidth / boardWidth;
        var scaleY = mapLength / boardLength;
        transform.localScale = new Vector3(scaleX, scaleY, 0.0f);

        mapRenderer.enabled = false;
    }

    /// <summary>
    /// Adds a new item to the minimap
    /// </summary>
    void AddStaticItem(Transform itemTransform, 
                       SpriteRenderer itemRenderer, 
                       Color colour,
                       bool updatesColor = false)
    {
        AddItem(itemTransform, itemRenderer, colour, false, 0, 1.0f, updatesColor);
    }

    /// <summary>
    /// Adds a new item to the minimap
    /// </summary>
    void AddItem(Transform itemTransform, 
                 SpriteRenderer itemRenderer, 
                 Color colour,
                 bool isMarker,
                 int orderOffset,
                 float scale, 
                 bool updatesColor)
    {
        MapItem item = null;

        if(!isMarker)
        {
            m_world.Add (new MapItem ());
            item =  m_world [m_world.Count - 1];
        }
        else
        {
            m_markers.Add (new MapItem ());
            item = m_markers [m_markers.Count - 1];
        }

        item.updatesColor = updatesColor;
        item.parentTransform = itemTransform;
        item.parentRenderer = itemRenderer;

        item.item = new GameObject();
        item.item.AddComponent<SpriteRenderer>();
        item.item.transform.parent = this.transform;
        item.item.name = itemRenderer.sprite.name + "_mapitem";

        var width = itemRenderer.sprite.texture.width;
        var height = itemRenderer.sprite.texture.height;
        Vector2 pivot = new Vector2 (0.5f, 0.5f);
        var rect = new Rect (0, 0, width, height);

        item.renderer = item.item.GetComponent<SpriteRenderer>();
        item.renderer.sortingLayerName = itemRenderer.sortingLayerName;
        item.renderer.sortingOrder = itemRenderer.sortingOrder + 
            GetComponent<SpriteRenderer>().sortingOrder + orderOffset;

        item.renderer.sprite = Sprite.Create(itemRenderer.sprite.texture, rect, 
                                             pivot, itemRenderer.sprite.pixelsPerUnit);

        item.renderer.color = new Color(colour.r, colour.g, colour.b, colour.a);

        item.item.transform.localPosition = new Vector3 (
            item.parentTransform.position.x,
            item.parentTransform.position.z,
            0.0f);

        if(!isMarker)
        {
            item.item.transform.localRotation = new Quaternion(
                0.0f, item.parentTransform.localRotation.y,
                0.0f, item.parentTransform.localRotation.w);
        }
        
        item.item.transform.localScale = new Vector3 (
            item.parentTransform.localScale.x * scale,
            item.parentTransform.localScale.y * scale,
            item.parentTransform.localScale.z * scale);
    }

    /// <summary>
    /// Updates the minimap. Note initialisation needs to be done here as fog is generated in Start()
    /// </summary>
    void Update () 
    {
        if(!m_isInitialised)
        {
            AddStaticItem(m_gameBoard.transform, 
                    m_gameBoard.GetComponent<SpriteRenderer>(), 
                    m_gameBoard.GetComponent<SpriteRenderer>().color);

            var boardItem = m_world[m_world.Count - 1];
            boardItem.item.transform.localRotation = Quaternion.identity;
            boardItem.item.transform.localScale = new Vector3(
                boardItem.item.transform.localScale.x,
                boardItem.item.transform.localScale.y,
                boardItem.item.transform.localScale.y);

            GameObject[] terrain = GameObject.FindGameObjectsWithTag("Island");
            if(terrain == null)
            {
                Debug.LogError("Could not find any terrain");
            }

            for(int i = 0; i < terrain.Length; ++i)
            {
                AddStaticItem(terrain[i].transform, 
                        terrain[i].GetComponent<SpriteRenderer>(), 
                        terrain[i].GetComponent<SpriteRenderer>().color,
                        true);
            }

            var fog = GameObject.FindGameObjectWithTag("MinimapFog");
            if(fog == null)
            {
                Debug.LogError("Could not find minimap fog");
            }
            else
            {
                AddStaticItem(fog.transform, 
                              fog.GetComponent<SpriteRenderer>(),
                              GetComponent<SpriteRenderer>().color);
            }
            m_fog = m_world[m_world.Count - 1];
            
            m_isInitialised = true;
        }

        UpdateMap();
    }

    /// <summary>
    /// Updates the minimap
    /// </summary>
    void UpdateMap()
    {
        if(m_fog.parentRenderer == null)
        {
            m_fog.item.SetActive(false);
        }

        for(int i = 0; i < m_markers.Count; ++i)
        {
            if(!UpdateMapItem(m_markers[i]))
            {
                Destroy (m_markers[i].item);
                m_markers.Remove(m_markers[i]);
                --i;
            }
        }

        foreach(var item in m_world)
        {
            if(item.updatesColor)
            {
                item.renderer.color = item.parentRenderer.color;
            }
        }
    }

    /// <summary>
    /// Adds the player to the minimap
    /// </summary>
    public void AddPlayer(GameObject player, bool controlled, Color color)
    {
        var maxMapScale = Mathf.Max(transform.localScale.x, transform.localScale.y);

        AddItem(player.transform, 
                marker.GetComponent<SpriteRenderer>(), 
                color, true, controlled ? 1 : 0,
                m_shipMarkerSize / maxMapScale, false);
    }

    /// <summary>
    /// Updates a map item positions
    /// </summary>
    bool UpdateMapItem(MapItem item)
    {
        if(item.parentTransform == null)
        {
            // Item has been destroyed
            if(item.item.activeSelf)
            {
                return false;
            }
        }

        item.item.transform.localRotation = Quaternion.identity;
        item.item.transform.localPosition = new Vector3 (
            item.parentTransform.position.x,
            item.parentTransform.position.z,
            0.0f);

        return true;
    }
}
