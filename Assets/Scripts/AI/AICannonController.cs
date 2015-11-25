using UnityEngine;
using System.Collections;

public class AICannonController : CannonController 
{
	// Use this for initialization
	void Start () 
    {
        StartUp();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(Utilities.IsLevelLoaded())
        {
            m_firedCannonLeft = false;
            m_firedCannonRight = false;
            
            UpdateMouseCursorAngle();
            FireCannons();
            RenderDiagnostics();
        }
	}
}
