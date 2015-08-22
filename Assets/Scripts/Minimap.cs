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
    public GameObject gameBoard;
    public GameObject fog;
    public GameObject terrain;
    public GameObject playerMarker;
    public GameObject enemyMarker;

    private float m_shipMarkerSize = 40.0f;
    private bool m_isInitialised = false;
    private bool m_hasPlayer = false;
    private List<MapItem> m_staticItems = new List<MapItem>();
    private List<MapItem> m_dynamicItems = new List<MapItem>();

    /**
    * Adds a new item to the minimap
    */
    void AddItem(Transform parentTransform, 
                 SpriteRenderer parentRenderer, 
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

        item.parentTransform = parentTransform;
        item.parentRenderer = parentRenderer;
        item.scale = scale;

        item.item = new GameObject();
        item.item.AddComponent<SpriteRenderer>();
        item.item.transform.parent = this.transform;
        item.item.name = parentRenderer.sprite.name + "_mapitem";

        var width = parentRenderer.sprite.texture.width;
        var height = parentRenderer.sprite.texture.height;
        Vector2 pivot = new Vector2 (0.5f, 0.5f);
        var rect = new Rect (0, 0, width, height);

        item.renderer = item.item.GetComponent<SpriteRenderer>();
        item.renderer.sortingLayerName = parentRenderer.sortingLayerName;
        item.renderer.sortingOrder = parentRenderer.sortingOrder + 100;
        item.renderer.sprite = Sprite.Create(parentRenderer.sprite.texture, rect, 
                                             pivot, parentRenderer.sprite.pixelsPerUnit);

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
            AddItem(gameBoard.transform, gameBoard.GetComponent<SpriteRenderer>(), true);

            // Islands use same texture
            var islandTransforms = terrain.GetComponentsInChildren<Transform>();
            var islandRenderers = terrain.GetComponentsInChildren<SpriteRenderer>();
            for(int i = 0, j = 0; i < islandTransforms.Length; ++i)
            {
                if(!islandTransforms[i].Equals(terrain.transform))
                {
                    AddItem(islandTransforms[i], islandRenderers[j], true);
                    ++j;
                }
            }

            // Fog uses same texture, currently only working for smooth fog
            // Note to get tiled fog working with the minimap it needs to become
            // a dynamic item and item.renderer.color.a needs to be set in Update()
            var fogTransforms = fog.GetComponentsInChildren<Transform>();
            var fogRenderers = fog.GetComponentsInChildren<SpriteRenderer>();
            for(int i = 0, j = 0; i < fogTransforms.Length; ++i)
            {
                if(!fogTransforms[i].Equals(fog.transform))
                {
                    AddItem(fogTransforms[i], fogRenderers[j], true);
                    ++j;
                }
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
            item.parentTransform.localPosition.x,
            item.parentTransform.localPosition.y,
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
