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
	public float shipMarkerSize = 12.0f;
	public GameObject gameBoard;
	public GameObject fog;
	public GameObject terrain;
	private GameObject player = null;
	public GameObject playerMarker;
	public GameObject enemyMarker;
	
	private bool isInitialised = false;
	private List<MapItem> staticItems = new List<MapItem>();
	private List<MapItem> dynamicItems = new List<MapItem>();

	/**
	* Adds a new item to the minimap
	* @param parent The object to copy from
	*/
	void AddItem(Transform parentTransform, 
	             SpriteRenderer parentRenderer, 
	             bool isStatic, 
	             float scale = 1.0f)
	{
		MapItem item = null;

		if(isStatic)
		{
			staticItems.Add (new MapItem ());
			item =  staticItems [staticItems.Count - 1];
		}
		else
		{
			dynamicItems.Add (new MapItem ());
			item =  dynamicItems [dynamicItems.Count - 1];
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
			for(int i = 0; i < staticItems.Count; ++i)
			{
				UpdateMapItem(staticItems[i]);
			}
		}
	}

	/**
	* Updates the minimap. Note initialisation needs to be done here as fog is generated in Start()
	*/
	void Update () 
	{
		if(!isInitialised)
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

			// Add enemies as a dot on the map
			var enemies = GameObject.FindGameObjectsWithTag("Enemy");
			for(int i = 0; i < enemies.Length; ++i)
			{
				AddItem(enemies[i].GetComponent<Transform>(), 
				   enemyMarker.GetComponent<SpriteRenderer>(), false, shipMarkerSize);
			}

			isInitialised = true;
		}

		if(player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			if(player != null)
			{
				AddItem(player.transform, playerMarker.GetComponent<SpriteRenderer>(), false, shipMarkerSize);
			}
		}

		for(int i = 0; i < dynamicItems.Count; ++i)
		{
			UpdateMapItem(dynamicItems[i]);
		}
	}

	void UpdateMapItem(MapItem item)
	{
		if(item.parentTransform == null)
		{
			// Item has been destroyed
			if(item.item.activeSelf)
			{
				item.item.SetActive(false);
			}
			return;
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
	}
}
