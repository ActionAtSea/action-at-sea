using UnityEngine;
using System.Collections;

public class FleetAI : MonoBehaviour
{

    public class FormationPosition
    {
        public FormationPosition()
        {
            position = Vector3.zero;
            assigned = false;
            assignedGameObject = null;
        }

        public FormationPosition(Vector3 position)
        {
            this.position = position;
            assigned = false;
            assignedGameObject = null;
        }

        Vector3 position = Vector3.zero;
        bool assigned = false;
        GameObject assignedGameObject = null;

        public void AssignObject(GameObject objectToAssign)
        {

        }
    };

    public const Vector3[] formationPositions = { new Vector3(-5.0f, 0, -5.0f), new Vector3(-10.0f, 0, -5.0f), new Vector3(-15.0f, 0, -5.0f) };  
    private CannonController cannonController = null;
    private NavMeshAgent navAgent = null;
    private Rigidbody body = null;
    private GameObject player = null;
    private int ownerPlayerID = -1;
    private bool gameInitialised = false;
    private bool purchased = false;

    /// <summary>
    /// Whether or not the fleet ship has been purchased yet.
    /// </summary>
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
                navAgent.SetDestination(new Vector3(10, 1, 100));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Initialise player following.
        if (!gameInitialised && Utilities.IsLevelLoaded() && !Utilities.IsGameOver() && PlayerManager.GetControllablePlayer() != null)
        {
            gameInitialised = true;
            player = PlayerManager.GetControllablePlayer();

            if (player != null)
            {
                ownerPlayerID = player.GetComponent<NetworkedPlayer>().PlayerID;

                if (navAgent.isOnNavMesh)
                {
                    navAgent.SetDestination(player.transform.position);
                }
            }
        }
        if (player != null)
        {
            if (navAgent.isOnNavMesh)
            {
                Vector3 formationPos = player.transform.position;
                formationPos -= formationPositions[0];
                navAgent.SetDestination(formationPos);
            }
        }
        else
        {
            Debug.Log("PLayer not set");
        }

    }
}