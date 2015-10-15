// Based on Code by Sebastian Lague
// https://www.youtube.com/watch?v=OuqThz4Zc9c
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceableObject : MonoBehaviour {

    [HideInInspector]
    public List<Collider> colliders = new List<Collider>();

    //TODO: retrieve this material from another class (object placement manager maybe).
    public Material itemNotPlaceableMat;

    protected Material originalMaterial;
    protected MeshRenderer meshRenderer;
    protected bool placed = false;
	void Start () 
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalMaterial = meshRenderer.material; 
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter(Collider c)
    {
        if(!placed)
        {
            if(c.tag == "PlaceableObject")
            {
                colliders.Add(c);
                meshRenderer.material = itemNotPlaceableMat;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if(!placed)
        {
            if(c.tag == "PlaceableObject")
            {
                colliders.Remove(c);
                meshRenderer.material = originalMaterial;
            }
        }
    }

    public bool Placed {
        get {
            return placed;
        }
        set {
            placed = value;
        }
    }
}
