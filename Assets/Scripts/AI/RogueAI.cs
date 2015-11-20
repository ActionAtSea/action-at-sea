using UnityEngine;
using System.Collections;

public class RogueAI : MonoBehaviour 
{
  
    private NavMeshAgent navAgent = null;
	// Use this for initialization
	void Start () 
    {
        navAgent = GetComponent<NavMeshAgent>();
        if(navAgent != null)
        {
            navAgent.SetDestination(new Vector3(10, 1, 100));
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}
}
