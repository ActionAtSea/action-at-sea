using UnityEngine;
using System.Collections;
public class SeaMine : PlaceableObject 
{

	// Use this for initialization
	void Start () {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalMaterial = meshRenderer.material; 
        //TODO: Figure out a way for objects already placed in the scene to behave properly.
        if(!placed)
        {
            meshRenderer.material = itemIsPlacableMat;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
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
                //meshRenderer.material = originalMaterial;
                if(colliders.Count == 0)
                {
                    meshRenderer.material = itemIsPlacableMat;
                }
            }
        }
    }
}
