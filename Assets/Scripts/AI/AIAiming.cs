using UnityEngine;
using System.Collections;
/// <summary>
/// Controls both aiming and firing of an A.I. ship's cannons.
/// </summary>
public class AIAiming : MonoBehaviour 
{
    private Vector3 targetPosition = Vector3.zero;
    private CannonController cannonController = null;

	// Use this for initialization
	void Start () 
    {
        cannonController = GetComponentInChildren<AICannonController>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}


    //Currently targets just player. In the future will need to target player owned AI as well.
    //TODO: Either create base class for AI aiming or create enums for different AI types.
    void OnTriggerStay(Collider other)
    {
        //If another player is within range fire cannons in their direction.
        if(other.CompareTag("Player"))
        {
            targetPosition = other.transform.position;
            if(cannonController != null)
            {
                cannonController.AimWeapon(targetPosition);
                cannonController.FireWeapon();// Uncomment once cannonball firing by ai has been fixed.
            }
        }
    }
}
