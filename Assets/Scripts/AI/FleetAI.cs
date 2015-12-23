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

        private Vector3 position = Vector3.zero;
        private bool assigned = false;
        private GameObject assignedGameObject = null;

        public FormationPosition AssignObject(GameObject objectToAssign)
        {
            if (!assigned)
            {
                assignedGameObject = objectToAssign;
                assigned = true;
                return this;
            }
            return null;
        }

        public void RemoveAssignedObject()
        {
            assigned = false;
            assignedGameObject = null;
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public bool Assigned
        {
            get { return assigned; }
        }

        public GameObject AssignedGameObject
        {
            get { return assignedGameObject; }
        }
    };

    //TODO: Figure out a way to make fleet ship positions relative to the direction the player is facing.
    public static readonly Vector3[] formationPositions = { new Vector3(-5.0f, 0, -5.0f), new Vector3(5.0f, 0, -5.0f)};  
    public static FormationPosition[] formationSlots = { 
                                                           new FormationPosition(formationPositions[0]), 
                                                           new FormationPosition(formationPositions[1])
                                                       }; 
    private CannonController cannonController = null;
    private NavMeshAgent navAgent = null;
    private Rigidbody body = null;
    private GameObject player = null;
    private int ownerPlayerID = -1;
    private bool gameInitialised = false;
    private bool purchased = false;
    private FormationPosition formationSlot = null;

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

    public bool AssignFormationPosition()
    {
        for (int i = 0; i < formationSlots.Length; ++i)
        {
            if (!formationSlots[i].Assigned)
            {
                formationSlot = formationSlots[i].AssignObject(gameObject);
                if (formationSlot != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void UnassignFormationPosition()
    {
        if (formationSlot != null)
        {
            formationSlot.RemoveAssignedObject();
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
            if (formationSlot != null)
            {
                if (formationSlot.Assigned)
                {
                    if (navAgent.isOnNavMesh)
                    {
                        Vector3 formationPos = player.transform.position;
                        formationPos -= formationSlot.Position;
                        navAgent.SetDestination(formationPos);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Player not set");
        }

    }
}