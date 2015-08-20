using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Handles logic for the ship's cannons.
 */
public class CannonController : MonoBehaviour, IAimable
{
    //TODO: Actually implement reload times.
    //TODO: Implement independent reload times for both sides of the ship.

	public bool controllable = false;
    private float mouseCursorAngle = 0.0f;          //The angle of the mouse cursor relative to the ship.

    private float swivelRangeDegrees = 45.0f;       //The range that the cannons can swivel.
    private float aimingRangeDegrees = 90.0f;       //The range within which which side's cannons can be fired is determined.

    private float reloadTime = 2.0f;                //The time in seconds that it takes to reload.
    private float currentReloadTimeRight = 0.0f;
    private float currentReloadTimeLeft = 0.0f;


    private Vector3 startPosition;                  //Contains the parent's position for use in aiming calculations.
    private Vector3 mousePositionWorld;             //Contains the mousePosition in world space.
    private Cannon[] cannonList;                    //Array of all the cannons on the ship.
    private List<Cannon> rightSideCannons;
    private List<Cannon> leftSideCannons;

    private bool fireGuns = false;                  //Determines whether the cannons will be fire in the current frame.

    // Use this for initialization
    void Start()
    {
        currentReloadTimeRight = reloadTime;
        currentReloadTimeLeft = reloadTime;

        cannonList = GetComponentsInChildren<Cannon>();
        rightSideCannons = new List<Cannon>(4);
        leftSideCannons = new List<Cannon>(4);

        foreach (Cannon c in cannonList)
        {
            if (c.rightSideCannon)
            {
                rightSideCannons.Add(c);
            }
            else
            {
                leftSideCannons.Add(c);
            }
        }
        //Debug.Log(rightSideCannons.Count);
        //Debug.Log(leftSideCannons.Count);
    }

    public void AimWeapon(Vector3 fireDirection)
    {
        mousePositionWorld = fireDirection;
    }

    public void FireWeapon()
    {
        fireGuns = true;
    }

    // Update is called once per frame
    void Update()
    {
		if(!GameInformation.IsPVP() || (GameInformation.IsPVP() && controllable))
		{
        	UpdateMouseCursorAngle();
        	FireCannons();
		}
    }

    private void UpdateMouseCursorAngle()
    {
        startPosition = transform.parent.position;

        //TODO: remove old  mouse input
        //Vector3 mouseDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - startPosition;
        Vector3 mouseDelta = mousePositionWorld - startPosition;

        if (mouseDelta.sqrMagnitude < 0.1f)
        {
            return; // don't do tiny rotations.
        }

        float angle = Mathf.Atan2(mouseDelta.y, mouseDelta.x) * Mathf.Rad2Deg;

        angle -= transform.parent.rotation.eulerAngles.z;   // minus the rotation of the ship

        if (angle < 0.0f)
        {
            angle += 360.0f;
            if (angle < 0.0f)   //if the angle is still < 0.0f
            {
                angle += 360.0f;
            }
        }
        mouseCursorAngle = angle;
    }

    private void FireCannons()
    {
        currentReloadTimeRight += Time.deltaTime;
        currentReloadTimeLeft += Time.deltaTime;

        //Debug.Log(currentReloadTime);

        //TODO: remove this old if statement
        //if (Input.GetMouseButtonDown(0))
        if(fireGuns)
        {
            //Left side Cannon reload time
            if (currentReloadTimeLeft >= reloadTime)
            {
                //If the mouse cursor is within the ship's left side firing range.
                if (mouseCursorAngle >= (180.0f - aimingRangeDegrees) && mouseCursorAngle <= (180.0f + aimingRangeDegrees))
                {
                    foreach (Cannon c in leftSideCannons)
                    {
                        c.FireGun();
                    }
                    currentReloadTimeLeft = 0.0f;
                }
            }
            //Right side Cannon reload time
            if (currentReloadTimeRight >= reloadTime)
            {
                //If the mouse cursor is within the ship's right side firing range.
                if (mouseCursorAngle <= (0.0f + aimingRangeDegrees) && mouseCursorAngle >= 0.0f)
                {
                    foreach (Cannon c in rightSideCannons)
                    {
                        c.FireGun();
                    }
                    currentReloadTimeRight = 0.0f;
                }
                else if (mouseCursorAngle >= (360.0f - aimingRangeDegrees))
                {
                    foreach (Cannon c in rightSideCannons)
                    {
                        c.FireGun();
                    }
                    currentReloadTimeRight = 0.0f;
                }
            }
            //Resets the fireGuns condition
            fireGuns = false;
        }
    }

    public float MouseCursorAngle
    {
        get { return mouseCursorAngle; }
    }

    public float SwivelRangeDegrees
    {
        get { return swivelRangeDegrees; }
    }

    public float AimingRangeDegrees
    {
        get { return aimingRangeDegrees; }
    }

    public float ReloadTime
    {
        get { return reloadTime; }
    }
}