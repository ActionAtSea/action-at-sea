using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCannonController : CannonController 
{
	// Use this for initialization
	void Start () 
    {
        StartUp();
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
