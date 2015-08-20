using UnityEngine;
using System.Collections;


public class Cannon : MonoBehaviour
{
	private Vector3 firePosition = new Vector3();
	private Quaternion fireRotation = new Quaternion();
	private bool hasFired = false;
	private bool shouldFire = false;

    public bool rightSideCannon = true;             //Determines which side the cannon is on the ship.
    private float swivelRangeDegrees = 45.0f;       //The range that the cannons can swivel.
    private float aimingRangeDegrees = 90.0f;       //The range within which which side's cannons can be fired is determined.
    private float cursorAngle = 0.0f;               //stores the angle the mouse cursor is at relative to the ship.
    private BulletFireScript fireScript;
    private CannonController controller;
	public PhotonView photonView = null;

    // Use this for initialization
    void Start()
    {
        fireScript = GetComponent<BulletFireScript>();
        controller = GetComponentInParent<CannonController>();

        swivelRangeDegrees = controller.SwivelRangeDegrees;
        aimingRangeDegrees = controller.AimingRangeDegrees;
    }

    // Update is called once per frame
    void Update()
    {
		if(GameInformation.IsPVP())
		{
			if(controller.controllable)
			{
				firePosition = fireScript.FirePosition();
				fireRotation = fireScript.FireRotation();
				UpdateRotation();
			}
			
			if(shouldFire)
			{
				FireGun();
				shouldFire = false;
			}
		}
		else
		{
			firePosition = fireScript.FirePosition();
			fireRotation = fireScript.FireRotation();
			UpdateRotation();
		}
    }

    /*TODO:
     * Set the cannon's firing angle to their limits closest to the mouse cursor
     * angle when the mouse cursor move from the one side of the ship to the other
     * and is outside of the cannon tracking range.
     */

    private void UpdateRotation()
    {
        cursorAngle = controller.MouseCursorAngle;

        if (rightSideCannon)
        {
            if (cursorAngle <= (0.0f + swivelRangeDegrees) && cursorAngle >= 0.0f)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, cursorAngle);
            }
            else if (cursorAngle >= (360.0f - swivelRangeDegrees))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, cursorAngle);
            }
        }
        else
        {
            if (cursorAngle >= (180.0f - swivelRangeDegrees) && cursorAngle <= (180.0f + swivelRangeDegrees))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, cursorAngle);
            }
        }
    }

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.localEulerAngles);
			stream.SendNext(hasFired);
			stream.SendNext(firePosition);
			stream.SendNext(fireRotation);
			hasFired = false;
		}
		else
		{
			var angles = (Vector3)stream.ReceiveNext();
			transform.localEulerAngles = new Vector3(angles.x, angles.y, angles.z);
			shouldFire = (bool)stream.ReceiveNext();
			firePosition = (Vector3)stream.ReceiveNext();
			fireRotation = (Quaternion)stream.ReceiveNext();
		}
	}

    public void FireGun()
    {
		if(GameInformation.IsPVP())
		{
			fireScript.Fire(transform.parent.transform.parent.GetComponent<NetworkedPlayer>().PlayerID, firePosition, fireRotation);
		}
		else
		{
			fireScript.Fire(transform.parent.transform.parent.tag, firePosition, fireRotation);
		}
		hasFired = controller.controllable;
    }
}