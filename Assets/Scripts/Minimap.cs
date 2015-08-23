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
    public GameObject marker = null;

    private float m_shipMarkerSize = 2.0f;
    private bool m_isInitialised = false;
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

    /**
    * Adds a new item to the minimap
    */
    void AddStaticItem(Transform itemTransform, 
                       SpriteRenderer itemRenderer, 
                       Color colour)
    {
        AddItem(itemTransform, itemRenderer, colour, true, 0, 1.0f);
    }

    /**
    * Adds a new item to the minimap
    */
    void AddItem(Transform itemTransform, 
                 SpriteRenderer itemRenderer, 
                 Color colour,
                 bool isStatic,
                 int orderOffset,
                 float scale)
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
        item.renderer.sortingOrder = itemRenderer.sortingOrder + 
            GetComponent<SpriteRenderer>().sortingOrder + orderOffset;

        item.renderer.sprite = Sprite.Create(itemRenderer.sprite.texture, rect, 
                                             pivot, itemRenderer.sprite.pixelsPerUnit);

        item.renderer.color = new Color(colour.r, colour.g, colour.b, colour.a);

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
            AddStaticItem(m_gameBoard.transform, 
                    m_gameBoard.GetComponent<SpriteRenderer>(), 
                    m_gameBoard.GetComponent<SpriteRenderer>().color);

            GameObject[] terrain = GameObject.FindGameObjectsWithTag("Island");
            if(terrain == null)
            {
                Debug.LogError("Could not find any terrain");
            }

            for(int i = 0; i < terrain.Length; ++i)
            {
                AddStaticItem(terrain[i].transform, 
                        terrain[i].GetComponent<SpriteRenderer>(), 
                        terrain[i].GetComponent<SpriteRenderer>().color);
            }

            GameObject fog = GameObject.FindGameObjectWithTag("MinimapFog");
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
            
            m_isInitialised = true;
        }

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
    */
    public void AddPlayer(GameObject player, bool controlled)
    {
        var maxMapScale = Mathf.Max(transform.localScale.x, transform.localScale.y);

        var colour = controlled ?
            new Color(0.0f, 1.0f, 0.0f, 1.0f) :
            new Color(1.0f, 0.0f, 0.0f, 1.0f);

        AddItem(player.transform, 
                marker.GetComponent<SpriteRenderer>(), 
                colour, false, controlled ? 1 : 0,
                m_shipMarkerSize / maxMapScale);
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
