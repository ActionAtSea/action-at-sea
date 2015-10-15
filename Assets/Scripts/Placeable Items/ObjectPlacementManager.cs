// Based on Code by Sebastian Lague
// https://www.youtube.com/watch?v=OuqThz4Zc9c
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPlacementManager : MonoBehaviour {


    public GameObject[]             placeableItems;
    private ObjectPlacement         objPlacement;
    

	// Use this for initialization
	void Start () 
    {
        objPlacement = FindObjectOfType<ObjectPlacement>() as ObjectPlacement;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SpawnSeaMine()
    {
        objPlacement.SetItem(placeableItems[0]);
        //Debug.Log (placeableItems[0].name);
    }

    public void Spawn(string item)
    {
        /*
        TODO: Optimize search algorithm and find a way
        to make sure that there aren't objects with the 
        same name.

        Use pre-created object strings to pass into the spawn
        function from the GUI, rather than simply typing in the 
        names as a parameter.
        */
        for(int i = 0; i<placeableItems.Length; ++i)
        {
            if(placeableItems[i].name == item)
            {
                objPlacement.SetItem(placeableItems[i]);
            }
        }
    }
}
