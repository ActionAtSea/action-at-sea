using UnityEngine;
using System.Collections;

public class PatrolAI : MonoBehaviour
{

    private CannonController cannonController = null;
    private NavMeshAgent navAgent = null;
    private Rigidbody body = null;
    private GameObject player = null;
    private int ownerPlayerID = -1;
    private bool gameInitialised = false;
    private bool purchased = false;

    public bool Purchased
    {
        get { return purchased; }
        set { purchased = value; }
    }

    public int OwnerPlayerID
    {
        get { return ownerPlayerID; }
    }

    // Use this for initialization
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        body = GetComponent<Rigidbody>();
        cannonController = GetComponentInChildren<AICannonController>();
        if (navAgent != null)
        {
            if (navAgent.isOnNavMesh)
            {

                //navAgent.SetDestination(new Vector3(10, 1, 100));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameInitialised && Utilities.IsLevelLoaded() && !Utilities.IsGameOver() && PlayerManager.GetControllablePlayer() != null)
        {
            gameInitialised = true;
           
        }
    }
}