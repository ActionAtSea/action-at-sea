////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - CannonController.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonController : MonoBehaviour
{
    private float m_mouseCursorAngle = 0.0f;          //The angle of the mouse cursor relative to the ship.
    private float m_swivelRangeDegrees = 45.0f;       //The range that the cannons can swivel.
    private float m_aimingRangeDegrees = 90.0f;       //The range within which which side's cannons can be fired is determined.
    private float m_reloadTime = 2.0f;                //The time in seconds that it takes to reload.
    private float m_currentReloadTimeRight = 0.0f;
    private float m_currentReloadTimeLeft = 0.0f;
    private Vector3 m_startPosition;                  //Contains the parent's position for use in aiming calculations.
    private Vector3 m_mousePositionWorld;             //Contains the mousePosition in world space.
    private Cannon[] m_cannonList;                    //Array of all the cannons on the ship.
    private List<Cannon> m_rightSideCannons;
    private List<Cannon> m_leftSideCannons;
    private bool m_fireGuns = false;                  //Determines whether the cannons will be fire in the current frame.

    /// <summary>
    /// Initialises the script
    /// </summary>    
    void Start()
    {
        m_currentReloadTimeRight = m_reloadTime;
        m_currentReloadTimeLeft = m_reloadTime;

        m_cannonList = GetComponentsInChildren<Cannon>();
        m_rightSideCannons = new List<Cannon>(4);
        m_leftSideCannons = new List<Cannon>(4);

        foreach (Cannon c in m_cannonList)
        {
            if (c.rightSideCannon)
            {
                m_rightSideCannons.Add(c);
            }
            else
            {
                m_leftSideCannons.Add(c);
            }
        }
    }

    /// <summary>
    /// Aims the cannons at the direction
    /// </summary>  
    public void AimWeapon(Vector3 fireDirection)
    {
        m_mousePositionWorld = fireDirection;
    }

    /// <summary>
    /// Fires the cannons
    /// </summary>  
    public void FireWeapon()
    {
        m_fireGuns = true;
    }

    /// <summary>
    /// Updates the cannones
    /// </summary>  
    void Update()
    {
        if(NetworkedPlayer.IsControllable(gameObject))
        {
            UpdateMouseCursorAngle();
            FireCannons();
        }
    }

    /// <summary>
    /// Updates the cannons according to the mouse position
    /// </summary>  
    private void UpdateMouseCursorAngle()
    {
        m_startPosition = transform.parent.position;

        Vector3 mouseDelta = m_mousePositionWorld - m_startPosition;

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
        m_mouseCursorAngle = angle;
    }

    /// <summary>
    /// Fires the cannones
    /// </summary>  
    private void FireCannons()
    {
        m_currentReloadTimeRight += Time.deltaTime;
        m_currentReloadTimeLeft += Time.deltaTime;

        if(m_fireGuns)
        {
            //Left side Cannon reload time
            if (m_currentReloadTimeLeft >= m_reloadTime)
            {
                //If the mouse cursor is within the ship's left side firing range.
                if (m_mouseCursorAngle >= (180.0f - m_aimingRangeDegrees) && m_mouseCursorAngle <= (180.0f + m_aimingRangeDegrees))
                {
                    foreach (Cannon c in m_leftSideCannons)
                    {
                        c.FireGun();
                    }
                    m_currentReloadTimeLeft = 0.0f;
                }
            }
            //Right side Cannon reload time
            if (m_currentReloadTimeRight >= m_reloadTime)
            {
                //If the mouse cursor is within the ship's right side firing range.
                if (m_mouseCursorAngle <= (0.0f + m_aimingRangeDegrees) && m_mouseCursorAngle >= 0.0f)
                {
                    foreach (Cannon c in m_rightSideCannons)
                    {
                        c.FireGun();
                    }
                    m_currentReloadTimeRight = 0.0f;
                }
                else if (m_mouseCursorAngle >= (360.0f - m_aimingRangeDegrees))
                {
                    foreach (Cannon c in m_rightSideCannons)
                    {
                        c.FireGun();
                    }
                    m_currentReloadTimeRight = 0.0f;
                }
            }
            //Resets the fireGuns condition
            m_fireGuns = false;
        }
    }

    /// <summary>
    /// Returns the mouse cursor angle
    /// </summary>  
    public float MouseCursorAngle
    {
        get { return m_mouseCursorAngle; }
    }

    /// <summary>
    /// Returns the range for swivel
    /// </summary>  
    public float SwivelRangeDegrees
    {
        get { return m_swivelRangeDegrees; }
    }

    /// <summary>
    /// Returns the range for aiming
    /// </summary>  
    public float AimingRangeDegrees
    {
        get { return m_aimingRangeDegrees; }
    }

    /// <summary>
    /// Returns the cannon reload time
    /// </summary>  
    public float ReloadTime
    {
        get { return m_reloadTime; }
    }
}