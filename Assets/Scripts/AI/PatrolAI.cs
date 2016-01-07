using UnityEngine;
using System.Collections;

public class PatrolAI : MonoBehaviour
{
    /// <summary>
    /// The distance from the current waypoint the AI must be for it
    /// to set the next waypoint as its destination.
    /// </summary>
    public float waypointTriggerDistance = 5.0f;

    private CannonController        cannonController = null;
    private NavMeshAgent            navAgent = null;
    private Rigidbody               body = null;
    private GameObject              player = null;
    private IslandDiscoveryNode[]   waypoints = null;
    private IslandDiscoveryNode     currentWaypoint = null;
    private int                     ownerPlayerID = -1;
    private bool                    purchased = false;
    private bool                    aiInitialised = false;


    public bool Purchased
    {
        get { return purchased; }
        set { purchased = value; }
    }

    public int OwnerPlayerID
    {
        get { return ownerPlayerID; }
        set
        {
            ownerPlayerID = value;
            GetComponent<NetworkedAI>().SetAssignedPlayer(ownerPlayerID);
        }
    }

    public void Initialise(IslandDiscoveryNode[] buoys, int owningPlayerID)
    {
        if (!aiInitialised)
        {
            OwnerPlayerID = owningPlayerID;
            waypoints = buoys;
            if (waypoints != null)
            {
                currentWaypoint = waypoints[0];
                Debug.Log(currentWaypoint);
                if (navAgent != null)
                {
                    Debug.Log(" Destination Set");
                    navAgent.SetDestination(currentWaypoint.transform.position);
                    aiInitialised = true;
                }
                else
                {
                    aiInitialised = false;
                }
                
            }
            else
            {
                Debug.LogError("Waypoints are null.");
            }
        }
        else
        {
            Debug.LogError("PatrolAI already initialised.");
        }
    }

    // Use this for initialization
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        body = GetComponent<Rigidbody>();
        cannonController = GetComponentInChildren<AICannonController>();
        if (navAgent != null)
        {
            Debug.Log("NavMeshAgent set.");
            if (navAgent.isOnNavMesh)
            {
                
                //navAgent.SetDestination(new Vector3(10, 1, 100));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (navAgent == null)
        {
            Debug.Log("nav agent is null");
        }
        else
        {
            //Debug.Log("Nav agent is not null");
            if (!aiInitialised)
            {
                    if (navAgent != null)
                    {
                        if (currentWaypoint != null)
                        {
                            //Debug.Log("CALLING FUNCTION!");
                            navAgent.SetDestination(currentWaypoint.transform.position);
                            aiInitialised = true;
                        }
                    }
            }
        }

        if (aiInitialised)
        {
           // Debug.Log("Patrol is being called.");
            Patrol();
        }
    }

    private void Patrol()
    {
        if (navAgent.isOnNavMesh)
        {
           // Debug.Log(Vector3.Distance(transform.position, currentWaypoint.transform.position));
            if (Vector3.Distance(transform.position, currentWaypoint.transform.position) <= waypointTriggerDistance)
            {
                for (int i = 0; i < waypoints.Length; ++i)
                {
                    //Finds the current waypoint in the array.
                    if (currentWaypoint == waypoints[i])
                    {
                        if (i + 1 < waypoints.Length)
                        {
                            currentWaypoint = waypoints[i + 1];
                        }
                        else
                        {
                            currentWaypoint = waypoints[0];
                        }
                        navAgent.SetDestination(currentWaypoint.transform.position);
                        break;
                    }
                }
            }

        }
    }
}