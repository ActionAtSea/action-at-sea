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
    public float scale = 1.0f;
};

public class Minimap : MonoBehaviour 
{
    public GameObject fog;
    public GameObject playerMarker;
    public GameObject enemyMarker;

    private float m_shipMarkerSize = 40.0f;
    private bool m_isInitialised = false;
    private bool m_hasPlayer = false;
    private List<MapItem> m_staticItems = new List<MapItem>();
    private List<MapItem> m_dynamicItems = new List<MapItem>();
    private GameObject m_gameBoard = null;

    /**
    * Initialises the script
    */
    void Start()
    {
        m_gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        if(m_gameBoard == null)
        {
            Debug.LogError("Could not find game board");
        }

        var boardBounds = m_gameBoard.GetComponent<SpriteRenderer>().bounds;
        var boardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x);
        var boardLength = Mathf.Abs(boardBounds.max.y - boardBounds.min.y);

        var mapBounds = GetComponent<SpriteRenderer>().bounds;
        var mapWidth = Mathf.Abs(mapBounds.max.x - mapBounds.min.x);
        var mapLength = Mathf.Abs(mapBounds.max.y - mapBounds.min.y);

        Debug.Log(mapWidth);
        Debug.Log(mapLength);

        var scaleX = mapWidth / boardWidth;
        var scaleY = mapLength / boardLength;

        GetComponent<SpriteRenderer>().enabled = false;
        transform.localScale = new Vector3(scaleX, scaleY, 0.0f);
    }

    /**
    * Adds a new item to the minimap
    */
    void AddItem(Transform itemTransform, 
                 SpriteRenderer itemRenderer, 
                 bool isStatic, 
                 float scale = 1.0f)
    {
        MapItem item = null;

        if(isStatic)
        {
            m_staticItems.Add (new MapItem ());
            item =  m_staticItems [m_staticItems.Count - 1];
        }
        else
        {
            m_dynamicItems.Add (new MapItem ());
            item = m_dynamicItems [m_dynamicItems.Count - 1];
        }

        item.parentTransform = itemTransform;
        item.parentRenderer = itemRenderer;
        item.scale = scale;

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
        item.renderer.sortingOrder = itemRenderer.sortingOrder + 100;
        item.renderer.sprite = Sprite.Create(itemRenderer.sprite.texture, rect, 
                                             pivot, itemRenderer.sprite.pixelsPerUnit);

        var colour = itemRenderer.color;
        item.renderer.color = new Color(colour.r, colour.g, colour.b, 1.0f);

        if(isStatic)
        {
            for(int i = 0; i < m_staticItems.Count; ++i)
            {
                UpdateMapItem(m_staticItems[i]);
            }
        }
    }

    /**
    * Updates the minimap. Note initialisation needs to be done here as fog is generated in Start()
    */
    void Update () 
    {
        if(!m_isInitialised)
        {
            AddItem(m_gameBoard.transform, m_gameBoard.GetComponent<SpriteRenderer>(), true);

            GameObject[] terrain = GameObject.FindGameObjectsWithTag("Island");
            if(terrain == null)
            {
                Debug.LogError("Could not find any terrain");
            }

            for(int i = 0; i < terrain.Length; ++i)
            {
                AddItem(terrain[i].transform, 
                        terrain[i].GetComponent<SpriteRenderer>(), 
                        true);
            }

            m_isInitialised = true;
        }

        AddPlayer();
        UpdateMap();
    }

    /**
    * Updates the minimap
    */
    void UpdateMap()
    {
        for(int i = 0; i < m_dynamicItems.Count; ++i)
        {
            if(!UpdateMapItem(m_dynamicItems[i]))
            {
                Destroy (m_dynamicItems[i].item);
                m_dynamicItems.Remove(m_dynamicItems[i]);
                --i;
            }
        }
    }

    /**
    * Adds the player to the minimap
    * The player can be created any time so continue to check until found
    */
    void AddPlayer()
    {
        if(!m_hasPlayer)
        {
            var player = PlayerManager.GetControllablePlayer();
            if(player != null)
            {
                m_hasPlayer = true;
                AddItem(player.transform, 
                        playerMarker.GetComponent<SpriteRenderer>(), 
                        false, 
                        m_shipMarkerSize);
            }
        }
    }

    /**
    * Updates a map item positions
    */
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

        item.item.transform.localPosition = new Vector3 (
            item.parentTransform.position.x,
            item.parentTransform.position.y,
            item.parentTransform.position.z);
        
        item.item.transform.localRotation = new Quaternion(
            item.parentTransform.localRotation.x,
            item.parentTransform.localRotation.y,
            item.parentTransform.localRotation.z,
            item.parentTransform.localRotation.w);
        
        item.item.transform.localScale = new Vector3 (
            item.parentTransform.localScale.x * item.scale,
            item.parentTransform.localScale.y * item.scale,
            item.parentTransform.localScale.z * item.scale);

        return true;
    }
}
