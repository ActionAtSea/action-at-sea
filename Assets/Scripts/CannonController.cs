////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - CannonController.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NOTE: Instantiated by Photon Networking
/// Start() cannot include any code relying on the world/level as 
/// this object can be instantiated before the level is created
/// </summary>
public class CannonController : MonoBehaviour
{
    public static int MAX_CANNONS = 8;

    private float m_mouseCursorAngle = 0.0f;          //The angle of the mouse cursor relative to the ship.
    private float m_swivelRangeDegrees = 45.0f;       //The range that the cannons can swivel.
    private float m_aimingRangeDegrees = 90.0f;       //The range within which which side's cannons can be fired is determined.
    private float m_reloadTime = 2.0f;                //The time in seconds that it takes to reload.
    private float m_currentReloadTimeRight = 0.0f;
    private float m_currentReloadTimeLeft = 0.0f;
    private Vector3 m_startPosition;                  //Contains the parent's position for use in aiming calculations.
    private Vector3 m_mousePositionWorld;             //Contains the mousePosition in world space.
    private List<Cannon> m_cannonList;                //Array of all the cannons on the ship.
    private List<Cannon> m_rightSideCannons;
    private List<Cannon> m_leftSideCannons;
    private bool m_fireGuns = false;                  //Determines whether the cannons will be fire in the current frame.
    private bool m_firedCannonRight = false;          //Whether the right cannons were recently fired
    private bool m_firedCannonLeft = false;          //Whether the left cannons were recently fired

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_currentReloadTimeRight = m_reloadTime;
        m_currentReloadTimeLeft = m_reloadTime;

        m_cannonList = Utilities.GetOrderedListInChildren<Cannon>(gameObject);
        m_rightSideCannons = new List<Cannon>(MAX_CANNONS/2);
        m_leftSideCannons = new List<Cannon>(MAX_CANNONS/2);

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
    /// Updates the cannons
    /// </summary>  
    void Update()
    {
        if(Utilities.IsLevelLoaded() && Utilities.IsPlayerControllable(gameObject))
        {
            m_firedCannonLeft = false;
            m_firedCannonRight = false;

            UpdateMouseCursorAngle();
            FireCannons();
            RenderDiagnostics();
        }
    }

    /// <summary>
    /// Renders the diagnostics
    /// </summary>  
    void RenderDiagnostics()
    {
        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("Mouse Cursor Angle", m_mouseCursorAngle);
            Diagnostics.Add("Fired Cannons Left", m_firedCannonLeft);
            Diagnostics.Add("Fired Cannons Right", m_firedCannonRight);
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

        //                  atan2(y, z) used to be this in 2d
        float angle = Mathf.Atan2(mouseDelta.z, mouseDelta.x) * Mathf.Rad2Deg;

        angle += transform.parent.rotation.eulerAngles.y;   // minus the rotation of the ship

        if (angle < 0.0f)
        {
            angle += 360.0f;
            if (angle < 0.0f)   //if the angle is still < 0.0f
            {
                angle += 360.0f;
            }
        }
        if (angle > 360.0f)
        {
            angle -= 360.0f;
        }
        MouseCursorAngle = angle;
    }

    /// <summary>
    /// Fires the cannons
    /// </summary>  
    public void FireWeaponLeft()
    {
        foreach (Cannon c in m_leftSideCannons)
        {
            c.FireGun();
        }
    }

    /// <summary>
    /// Fires the cannons
    /// </summary>  
    public void FireWeaponRight()
    {
        foreach (Cannon c in m_rightSideCannons)
        {
            c.FireGun();
        }
    }

    /// <summary>
    /// Fires the cannones
    /// </summary>  
    private void FireCannons()
    {
        m_currentReloadTimeRight += Time.deltaTime;
        m_currentReloadTimeLeft += Time.deltaTime;

        if (m_fireGuns)
        {
            //Left side Cannon reload time
            if (m_currentReloadTimeLeft >= m_reloadTime)
            {
                //If the mouse cursor is within the ship's left side firing range.
                if (m_mouseCursorAngle >= (180.0f - m_aimingRangeDegrees) && m_mouseCursorAngle <= (180.0f + m_aimingRangeDegrees))
                {
                    FireWeaponLeft();
                    m_currentReloadTimeLeft = 0.0f;
                    m_firedCannonLeft = true;
                }
            }
            //Right side Cannon reload time
            if (m_currentReloadTimeRight >= m_reloadTime)
            {
                //If the mouse cursor is within the ship's right side firing range.
                if (m_mouseCursorAngle <= (0.0f + m_aimingRangeDegrees) && m_mouseCursorAngle >= 0.0f)
                {
                    FireWeaponRight();
                    m_currentReloadTimeRight = 0.0f;
                    m_firedCannonRight = true;
                }
                else if (m_mouseCursorAngle >= (360.0f - m_aimingRangeDegrees))
                {
                    FireWeaponRight();
                    m_currentReloadTimeRight = 0.0f;
                    m_firedCannonRight = true;
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
        set { m_mouseCursorAngle = value; }
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
    /// Returns whether the right cannons were recently fired
    /// </summary>  
    public bool CannonsFiredRight
    {
        get { return m_firedCannonRight; }
    }

    /// <summary>
    /// Returns whether the left cannons were recently fired
    /// </summary>  
    public bool CannonsFiredLeft
    {
        get { return m_firedCannonLeft; }
    }

    /// <summary>
    /// Returns the cannon reload time
    /// </summary>  
    public float ReloadTime
    {
        get { return m_reloadTime; }
    }
}