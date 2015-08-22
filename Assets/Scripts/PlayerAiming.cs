////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerAiming.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerAiming : MonoBehaviour
{
    public bool controllable = false;
    private Vector3 m_mousePos;
    private CannonController m_controller;

    /**
    * Initialises the aiming script
    */
    void Start()
    {
        m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_controller = GetComponentInChildren<CannonController>();
    }

    /**
    * Updates the player aiming
    */
    void Update()
    {
        if(controllable)
        {
            m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_controller.AimWeapon(m_mousePos);

            if (Input.GetMouseButtonDown(0))
            {
                m_controller.FireWeapon();
            }
        }
    }
}