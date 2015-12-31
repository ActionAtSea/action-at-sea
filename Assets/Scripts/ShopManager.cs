using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/// <summary>
/// Handles the purchasing of player owned AI ships and ship upgrades.
/// </summary>
public class ShopManager : MonoBehaviour
{

    /*
     * TODO: Add tool tips showing the hotkey and once 
     * each button does whilst the mouse is hovering
     * over them. Also disable cannon firing whilst mouse
     * is hovering over buttons.
     */

    public class FleetShip
    {
        public FleetShip()
        {
            networkedAI = null;
            aiShip = null;
        }

        public FleetShip(NetworkedAI netAI, FleetAI ai)
        {
            networkedAI = netAI;
            aiShip = ai;

        }
        public NetworkedAI networkedAI = null;
        public FleetAI aiShip = null;
    };

    public float fleetShipCost;
    public float patrolShipCost;
    public bool viewDebuggingInfo = false;

    [Range(0.0f, 1.0f)]
    public float buttonPressCooldown = 0.4f;

    public Button fleetButton;
    public Button patrolButton;
    public Button cannonButton;

    private SoundManager soundManager = null;
    private GameObject player = null;
    private NetworkedPlayer networkedPlayer = null;
    private IslandDiscoveryTrigger nearbyIsland = null;
    private PlayerScore playerScore = null;

    private GameObject[] fleetShips = null;
    private FleetShip fleetShipToSpawn = null;
    
    private float fleetButtonTimer = 0.0f;
    private float patrolButtonTimer = 0.0f;
    private float cannonButtonTimer = 0.0f;

    // Use this for initialization
    void Start()
    {
        fleetButton.interactable = false;
        patrolButton.interactable = false;
        cannonButton.interactable = false;
        soundManager = SoundManager.Get();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null & Utilities.IsLevelLoaded() && !Utilities.IsGameOver())
        {
            player = PlayerManager.GetControllablePlayer();
            if(player != null)
            {
                networkedPlayer = player.GetComponent<NetworkedPlayer>();
                playerScore = player.GetComponent<PlayerScore>();
            }

            fleetShips = PlayerManager.GetOwnedFleetAI();
            fleetShipToSpawn = GetFleetShipToSpawn();
        }

        if (networkedPlayer != null)
        {
            if (networkedPlayer.IslandWithinRange != null)
            {
                nearbyIsland = networkedPlayer.IslandWithinRange.GetComponent<IslandDiscoveryTrigger>();
                fleetButton.interactable = true;
                patrolButton.interactable = true;
                //TODO: Enable cannonButton once cannon upgrades have been implemented.
                //cannonButton.interactable = true;
            }
            else
            {
                nearbyIsland = null;
                fleetButton.interactable = false;
                patrolButton.interactable = false;
                cannonButton.interactable = false;
            }
        }

        
        if (fleetButton.IsInteractable() && Input.GetAxis("BuyFleetShip") == 1 && fleetButtonTimer >=buttonPressCooldown)
        {
            FleetButtonPress();
            fleetButtonTimer = 0.0f;
        }

        if (patrolButton.IsInteractable() && Input.GetAxis("BuyPatrolShip") == 1 && patrolButtonTimer >= buttonPressCooldown)
        {
            PatrolButtonPress();
            patrolButtonTimer = 0.0f;
        }

        //TODO: Add cannon upgrade button.
        if (fleetButtonTimer < buttonPressCooldown)
        {
            fleetButtonTimer += Time.deltaTime;
        }
        if (patrolButtonTimer < buttonPressCooldown)
        {
            patrolButtonTimer += Time.deltaTime;
        }
    }



    public void FleetButtonPress()
    {
        string failureMsg = string.Empty;
        bool success = false;
        fleetShipToSpawn = GetFleetShipToSpawn();

        if (fleetShipToSpawn != null)
        {
            if (!fleetShipToSpawn.aiShip.Purchased)
            {
                if ((playerScore.RoundedScore - fleetShipCost) >= 0.0f)
                {
                    if (fleetShipToSpawn.aiShip.AssignFormationPosition())
                    {
                        fleetShipToSpawn.networkedAI.SetVisible(true, false);
                        fleetShipToSpawn.aiShip.Purchased = true;
                        playerScore.MinusScore(fleetShipCost);
                        success = true;
                    }
                    else
                    {
                        success = false;
                        failureMsg = "No formationPositions available.";
                    }
                }
                else
                {
                    success = false;
                    failureMsg = "Player has insufficient coin to purchase a ship.";
                }
            }
            else
            {
                success = false;
                failureMsg = "Ship has already been purchased.";
            }
        }
        else
        {
            success = false;
            if (viewDebuggingInfo)
            {
                failureMsg = "The fleetShipToSpawn was null.";
            }
        }

        if (success)
        {
            //Play success noise / ship spawn noise.
        }
        else
        {
            Debug.Log(failureMsg);
            //Play failure noise.
        }
        
        //TODO: Remove once success and failure noises have been added.
        if (soundManager != null)
        {
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        }

        if (viewDebuggingInfo)
        {
            Debug.Log("FleetButtonPress.");
        }
    }

    public void PatrolButtonPress()
    {
        bool success = false;
        string failureMessage = string.Empty;

        if (nearbyIsland != null)
        {
            if (!nearbyIsland.AISpawned)
            {
                if (playerScore.RoundedScore - patrolShipCost >= 0)
                {
                    nearbyIsland.SpawnPatrolAI();
                    playerScore.MinusScore(patrolShipCost);
                    success = true;
                }
                else
                {
                    success = false;
                    failureMessage = "Player has insufficient coin to purchase a Patrol ship.";
                }
            }
            else
            {
                success = false;
                failureMessage = "PatrolAI already spawned.";
            }
        }
        else
        {
            failureMessage = "NearbyIsland was null.";
        }

        if (soundManager != null)
        {
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        }

        if (viewDebuggingInfo)
        {
            if (success)
            {
                Debug.Log("Patrol Button Pressed.");
            }
            else
            {
                Debug.Log(failureMessage);
            }
        }
    }

    public void CannonButtonPress()
    {
        if (soundManager != null)
        {
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        }

        if (viewDebuggingInfo)
        {
            Debug.Log("CannonButtonPress");
        }
    }

    /// <summary>
    /// Returns an unpurchased FleetShip owned by the client that is ready to be spawned.
    /// </summary>
    /// <returns>
    /// A FleetShip ready to be spawned or null if there aren't any.
    /// </returns>
    private FleetShip GetFleetShipToSpawn()
    {
        FleetShip temp = null;
        if (fleetShips != null)
        {
            for (int i = 0; i < fleetShips.Length; ++i)
            {
                NetworkedAI ai = fleetShips[i].GetComponentInChildren<NetworkedAI>();
                FleetAI fleet = fleetShips[i].GetComponentInChildren<FleetAI>();

                if (ai != null && fleet != null)
                {
                    if (!fleet.Purchased)
                    {
                        temp = new FleetShip(ai, fleet);
                        break;
                    }
                }
            }
        }
        return temp;
    }
}