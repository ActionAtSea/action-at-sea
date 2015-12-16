using UnityEngine;
using System.Collections;

public class AIHealth : Health 
{
    public bool AssignedPlayerDead = false;

    protected override void Initialise()
    {
        m_healthMax = 50.0f;
        base.Initialise();
    }

    /// <summary>
    /// Called on update
    /// </summary>
    protected override void OnUpdate()
    {
	}
}
