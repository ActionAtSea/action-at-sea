////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerAiming.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class PlayerAiming : MonoBehaviour
{
    private Vector3 m_mousePos;
    private CannonController m_controller;

    /// <summary>
    /// Initialises the aiming script
    /// </summary>
    void Start()
    {
        m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_controller = GetComponentInChildren<CannonController>();
    }

    /// <summary>
    /// Updates the player aiming
    /// </summary>
    void Update()
    {
        if(NetworkedPlayer.IsControllable(gameObject))
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