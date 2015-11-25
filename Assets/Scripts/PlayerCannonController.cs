using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCannonController : CannonController 
{
	// Use this for initialization
	void Start () 
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
	
	// Update is called once per frame
	void Update () 
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

    protected override void RenderDiagnostics()
    {
        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("Mouse Cursor Angle", m_mouseCursorAngle);
            Diagnostics.Add("Fired Cannons Left", m_firedCannonLeft);
            Diagnostics.Add("Fired Cannons Right", m_firedCannonRight);
        }
    }
}
