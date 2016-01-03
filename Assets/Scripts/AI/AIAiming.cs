using UnityEngine;
using System.Collections;
/// <summary>
/// Controls both aiming and firing of an A.I. ship's cannons.
/// </summary>
public class AIAiming : MonoBehaviour 
{
    private CannonController cannonController = null;
    private NetworkedAI networkAI = null;
    private NetworkedAI.AIType aiType;

	// Use this for initialization
	void Start () 
    {
        cannonController = GetComponentInChildren<AICannonController>();
        networkAI = GetComponentInParent<NetworkedAI>();
        if (networkAI != null)
        {
            aiType = networkAI.aiType;
        }
        else
        {
            Debug.LogError("NetworkedAI was unable to be found in parent.");
        }
	}

    private void AimAndFire(Vector3 targetPosition)
    {
        if (cannonController != null)
        {
            cannonController.AimWeapon(targetPosition);
            cannonController.FireWeapon();
        }
    }


    //Currently targets just player. In the future will need to target player owned AI as well.
    //TODO: Either create base class for AI aiming or create enums for different AI types.
    void OnTriggerStay(Collider other)
    {
        switch (aiType)
        {
            case NetworkedAI.AIType.ROGUE:
                //Rogue AI will aim prioritise firing at player ships.
                if (other.CompareTag("Player") || other.CompareTag("EnemyPlayer"))
                {
                    AimAndFire(other.transform.position);
                }
                else if (other.CompareTag("AIShip"))
                {
                    if (other.GetComponent<NetworkedAI>().aiType != NetworkedAI.AIType.ROGUE)
                    {
                        AimAndFire(other.transform.position);
                    }
                }

                break;

            case NetworkedAI.AIType.FLEET:
                if (other.CompareTag("EnemyPlayer"))
                {
                    if (other.GetComponent<NetworkedPlayer>().PlayerID != GetComponentInParent<FleetAI>().OwnerPlayerID)
                    {
                        //Debug.Log("Other player targeted");
                        AimAndFire(other.transform.position);
                        break;
                    }
                }
                if (other.CompareTag("AIShip"))
                {
                    if (other.GetComponent<NetworkedAI>().aiType == NetworkedAI.AIType.ROGUE)
                    {
                        AimAndFire(other.transform.position);
                        break;
                    }

                    if (other.GetComponent<NetworkedAI>().aiType == NetworkedAI.AIType.PATROL)
                    {
                        if (other.GetComponentInParent<PatrolAI>().OwnerPlayerID != GetComponentInParent<FleetAI>().OwnerPlayerID)
                        {
                           // Debug.Log("Other AI targeted");
                            AimAndFire(other.transform.position);
                            break;
                        }
                    }
                }
                break;

            case NetworkedAI.AIType.PATROL:
                 if (other.CompareTag("EnemyPlayer"))
                {
                    if (other.GetComponent<NetworkedPlayer>().PlayerID != GetComponentInParent<PatrolAI>().OwnerPlayerID)
                    {
                        //Debug.Log("Other player targeted");
                        AimAndFire(other.transform.position);
                        break;
                    }
                }
                if (other.CompareTag("AIShip"))
                {
                    if (other.GetComponent<NetworkedAI>().aiType == NetworkedAI.AIType.ROGUE)
                    {
                        AimAndFire(other.transform.position);
                        break;
                    }

                    if (other.GetComponent<NetworkedAI>().aiType == NetworkedAI.AIType.FLEET)
                    {
                        if (other.GetComponentInParent<FleetAI>().OwnerPlayerID != GetComponentInParent<PatrolAI>().OwnerPlayerID)
                        {
                            //Debug.Log("Other AI targeted");
                            AimAndFire(other.transform.position);
                            break;
                        }
                    }
                }
                break;

            default:
                break;
        }
        //If another player is within range fire cannons in their direction.
       
    }
}
