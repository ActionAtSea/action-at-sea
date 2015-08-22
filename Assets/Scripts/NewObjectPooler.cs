using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewObjectPooler : MonoBehaviour
{
    private static NewObjectPooler sm_objPooler = null;
    public GameObject pooledObject = null;
    private int m_pooledAmount = 20;
    private bool m_willGrow = true; // Determines if more objects can be added to the poolwhen needed.
    List<GameObject> m_pooledObjects = null;

    /**
    * Initialises the object pooler
    */
    void Start()
    {
        m_pooledObjects = new List<GameObject>();
        for (int i = 0; i < m_pooledAmount; ++i)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.SetActive(false);
            obj.transform.parent = transform;
            m_pooledObjects.Add(obj);
        }
    }

    /**
    * Gets a new object or adds to the pool if not enough available
    */
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < m_pooledObjects.Count; ++i)
        {
            if (!m_pooledObjects[i].activeInHierarchy)
            {
                return m_pooledObjects[i]; 
            }
        }

        if (m_willGrow)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            m_pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }

    /**
    * Gets the Object pooler from the scene
    */
    public static NewObjectPooler Get()
    {
        if(sm_objPooler == null)
        {
            sm_objPooler = FindObjectOfType<NewObjectPooler>();
            if(sm_objPooler == null)
            {
                Debug.LogError("Could not find NewObjectPooler");
            }
        }
        return sm_objPooler;
    }
}