////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Cannon.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/// <summary>
/// NOTE: Instantiated by Photon Networking
/// Start() cannot include any code relying on the world/level as 
/// this object can be instantiated before the level is created
/// </summary>
public class Cannon : MonoBehaviour
{
    public bool rightSideCannon = true;  // Determines which side the cannon is on the ship.
    private float m_swivelRangeDegrees = 45.0f;
    private BulletFireScript m_fireScript = null;
    private CannonController m_controller = null;
    private bool m_shouldFire = false;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_fireScript = GetComponent<BulletFireScript>();

        //TODO: figure out if cannons need to know about their cannon controller.
        m_controller = GetComponentInParent<CannonController>();
        m_swivelRangeDegrees = m_controller.SwivelRangeDegrees;
    }

    /// <summary>
    /// Updates the cannon
    /// </summary>
    void Update()
    {
        if(Utilities.IsLevelLoaded())
        {
            UpdateRotation();

            if(m_shouldFire)
            {
                m_fireScript.Fire(Utilities.GetPlayerID(gameObject), transform.position, transform.rotation);
                m_shouldFire = false;
            }
        }
    }

    /// <summary>
    /// TODO:
    /// Set the cannon's firing angle to their limits closest to the mouse cursor
    /// angle when the mouse cursor move from the one side of the ship to the other
    /// and is outside of the cannon tracking range.
    /// </summary>
    private void UpdateRotation()
    {
        //transform.eulerAngles = transform.parent.eulerAngles;
        var cursorAngle = m_controller.MouseCursorAngle;

        if (rightSideCannon)
        {
            if (cursorAngle <= (0.0f + m_swivelRangeDegrees) && cursorAngle >= 0.0f)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, cursorAngle);
            }
            else if (cursorAngle >= (360.0f - m_swivelRangeDegrees))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, cursorAngle);
            }
        }
        else
        {
            if (cursorAngle >= (180.0f - m_swivelRangeDegrees) && cursorAngle <= (180.0f + m_swivelRangeDegrees))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, cursorAngle);
            }
        }
    }

    /// <summary>
    /// Fires the cannon
    /// </summary>
    public void FireGun()
    {
        m_shouldFire = true;
    }
}