////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerAiming.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/// <summary>
/// NOTE: Instantiated by Photon Networking
/// Start() cannot include any code relying on the world/level as 
/// this object can be instantiated before the level is created
/// </summary>
public class PlayerAiming : MonoBehaviour
{
    private CannonController m_controller;

    /// <summary>
    /// Initialises the aiming script
    /// </summary>
    void Start()
    {
        m_controller = GetComponentInChildren<PlayerCannonController>();
    }

    /// <summary>
    /// Updates the player aiming
    /// </summary>
    void Update()
    {
        if(!Utilities.IsLevelLoaded())
        {
            return;
        }

        if (Utilities.IsPlayerControllable(gameObject))
        {
            Vector3 pos = Input.mousePosition;
            Ray mouseRay = Camera.main.ScreenPointToRay(pos);
            Plane playerPlane = new Plane(Vector3.up, transform.position);
            float rayDistance = 0.0f;
            playerPlane.Raycast(mouseRay, out rayDistance);
            Vector3 castPoint = mouseRay.GetPoint(rayDistance);
            Vector3 toCastPoint = castPoint - transform.position;

            pos.z = Camera.main.transform.position.z;
            toCastPoint.Normalize();

            m_controller.AimWeapon(castPoint);
            //Debug.Log(castPoint);

            if (Input.GetMouseButtonDown(0))
            {
                //Debug.DrawRay(mouseRay.origin, mouseRay.direction);
                Debug.DrawRay(castPoint, Vector3.up, Color.green, 1.0f);
                m_controller.FireWeapon();
            }
        }
    }
}