using UnityEngine;
using System.Collections;

public class PlayerAiming : MonoBehaviour
{
	public bool controllable = false;
    private Vector3 mousePos;
    private CannonController controller;

    // Use this for initialization
    void Start()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        controller = GetComponentInChildren<CannonController>();
    }

    // Update is called once per frame
    void Update()
    {
		if(GameInformation.IsPVP() && !controllable)
		{
			return;
		}

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        controller.AimWeapon(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            controller.FireWeapon();
        }
    }
}