using UnityEngine;
using System.Collections;

public class PlayerHealth : Health 
{
    protected override void Initialise()
    {
        if(Utilities.IsPlayerControllable(gameObject))
        {
            m_healthBar = GameObject.FindWithTag("PlayerHealth");
            m_healthBar.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }

        base.Initialise(); 
    }

    /// <summary>
    /// Called on update
    /// </summary>
    protected override void OnUpdate () 
    {
	}
}
