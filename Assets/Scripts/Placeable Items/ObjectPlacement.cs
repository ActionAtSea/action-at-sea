// Based on Code by Sebastian Lague
// https://www.youtube.com/watch?v=OuqThz4Zc9c
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPlacement : MonoBehaviour
{
    private PlaceableObject     placeableObject;
    private Camera              overheadCamera;                 
    private Transform           currentObject;                  // Transform for item to be placed.
    private bool                hasPlaced = false;              // Whether an newly spawn item has been placed.
    
    // Use this for initialization
    void Start()
    {
        overheadCamera = FindObjectOfType<Camera>() as Camera;

    }

    // Update is called once per frame
    void Update()
    {
        if(currentObject != null && overheadCamera != null)
        {
            if(!hasPlaced)
            {
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, overheadCamera.transform.position.y);
                Vector3 p = overheadCamera.ScreenToWorldPoint(mousePos);
                currentObject.position = new Vector3(p.x, 0, p.z);
            }


            //TODO: Replace with key/button bindable option.
            if(Input.GetMouseButtonDown(0))
            {
                if(IsLegalPosition())
                {
                    hasPlaced = true;
                    placeableObject.Placed = true;
                }
            }
        }
    }
    bool IsLegalPosition()
    {
        if(placeableObject != null)
        {
        if(placeableObject.colliders.Count > 0)
        {
            return false;
        }
        return true;
        }
        else
        {
            Debug.LogError("placeableObject is null.");
            return false;
        }
    }

    public void SetItem(GameObject item)
    {
        hasPlaced = false;
        currentObject = Instantiate(item).transform;
        placeableObject = currentObject.gameObject.GetComponent<PlaceableObject>();
    }
}